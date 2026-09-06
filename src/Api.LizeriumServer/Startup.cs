/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 06 сентября 2026 11:13:26
 * Version: 1.0.168
 */

using System.Net;

using Api.LizeriumServer.Accessories.AuthExtensions;

using AspNetCore.ReCaptcha;

using LizeriumDatabase.Services.AppDataBaseService;
using LizeriumDatabase.Services.AppDataBaseService.Implements;

using LizeriumLogging.Accessories.LoggingAccessories;

using LizeriumNetSecurity.Services.SecurityService;

using LizeriumUtilities.Accessories.ConfigurationAccessories;
using LizeriumUtilities.Middleware;
using LizeriumUtilities.Services.SecurityService.Implements;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;

using TranslationService;
using TranslationService.Libre;

namespace Api.LizeriumServer;

/// <summary>
/// Класс конфигурации приложения
/// </summary>
public class Startup
{
    /// <summary>
    /// Конфигурация используемых сервисов в приложении
    /// </summary>
    /// <param name="services">Интерфейс коллекции используемых сервисов</param>
    public void ConfigureServices(IServiceCollection services)
    {
        //конфигурация политики кук
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = _ => false; //не требуется согласие, а то даже сессионные куки не устанавливает
            options.MinimumSameSitePolicy = SameSiteMode.Lax;
            options.Secure = CookieSecurePolicy.SameAsRequest;
        });

        // Получаем конфигурацию из файла appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Используйте AddJsonFile для чтения конфигурации из appsettings.json
            .Build();

        services.AddReCaptcha(configuration.GetSection("GoogleReCaptcha"));
        //добавляем кэширование сервисов в памяти
        services.AddDistributedMemoryCache();

        //конфигурируем параметры сессии
        services.AddSession(options =>
        {
            options.Cookie.Name = AuthExtensions.NameSessionCookie; //ставим название куку сессии
            options.IdleTimeout = TimeSpan.FromHours(8);            //время хранения сессии при бездействии
            options.Cookie.MaxAge = TimeSpan.FromHours(8);          //админка не должна просить вход заново во время долгого редактирования
            options.Cookie.HttpOnly = true;                         //доступ только с сервера
            options.Cookie.IsEssential = true;                      //указывает, действительно ли этот файл cookie необходим для правильной работы приложения. Если true, то проверки политики согласия могут быть обойдены
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        });

        services.AddDataProtection()
            .SetApplicationName("Api.LizeriumServer")
            .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "DataProtectionKeys")));

        //настройки службы против подделки запросов
        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = options.DefaultPolicy;
        });

        //добавляем использование контроллеров
        services.AddControllers();

        //добавляем
        services
            .AddControllersWithViews() //использование контроллеров с View 
            .AddRazorRuntimeCompilation(); //компиляцию View при изменениях

        //добавляем политику CORS
        services.AddCors();

        // Настройка базы данных
        services.AddDbContext<IDataBaseService, DataBaseService>();

        //добавляем сервис базы данных
        services.AddSingleton<IDataBaseService, DataBaseService>();
        services.AddSingleton<IAppSecurityService, AppSecurityService>();

        //добавляем сервис переводчика (локального)
        var libreConfig = configuration.GetSection("LibreTranslate");
        services.AddHttpClient<ITranslationService, LibreTranslate>(client =>
        {
            client.BaseAddress = new Uri(libreConfig["BaseUrl"]);
            client.Timeout = TimeSpan.FromSeconds(5);
        });
    }

    /// <summary>
    /// Конфигурация роутинга запросов приложения и действий при старте и остановке
    /// </summary>
    /// <param name="app">Интерфейс конфигурации роутинга запросов приложения</param>
    /// <param name="env">Интерфейс web хостинга приложения</param>
    /// <param name="appLifetime">Интерфейс управлением запуска и отключения приложения</param>
    public void Configure(IApplicationBuilder app, 
        IDataBaseService dataBase, 
        IWebHostEnvironment env, 
        IHostApplicationLifetime appLifetime)
    {
        //получаем известные прокси
        var knownProxies = CommonConfigurationExtensions.GetKnownProxies();

        //инициализируем опции перенаправления заголовков запросов
        var forwardedHeadersOptions = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All,
            RequireHeaderSymmetry = false,
            ForwardLimit = null
        };

        //добавляем хосты известных прокси
        foreach (var knownProxy in knownProxies)
        {
            //проверяем прокси на пустоту
            if (string.IsNullOrEmpty(knownProxy)) continue;

            //добавляем IP адрес известных прокси
            forwardedHeadersOptions.KnownProxies.Add(IPAddress.Parse(knownProxy));
        }

        //конфигурируем получение оригинальных заголовков
        app.UseForwardedHeaders(forwardedHeadersOptions);

        //старт работы приложения
        appLifetime.ApplicationStarted.Register(async () =>
        {
            //инициализируем сервис логирования
            LoggingExtensions.Logging.InitializeLogging("API Lizerium");
        });

        //остановка работы приложения
        appLifetime.ApplicationStopping.Register(() =>
        {
            //деинициализируем сервис логирования
            LoggingExtensions.Logging.DeinitializeLogging();
        });

        //если среда разработки
        if (env.IsDevelopment())
        {
            //используем страницу исключений
            app.UseDeveloperExceptionPage();
        }
        else
        {
            //используем страницу ошибок на случай исключений
            app.UseExceptionHandler("/Home/Error");
        }

        //используем статичные файлы
        app.UseStaticFiles();

        //использовать политику куки
        app.UseCookiePolicy();

        //использовать сессии
        app.UseSession();

        //используем роутинг
        app.UseRouting();

        //обработчик службы против подделки запросов
        app.UseMiddleware<AntiforgeryMiddleware>();

        //обработчик ошибок
        app.UseMiddleware<ErrorHandlingMiddleware>();

        //используем CORS с любых хостов
        app.UseCors(options =>
        {
            options.SetIsOriginAllowed(_ => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });

        //маршрутизация
        app.UseEndpoints(endpoints =>
        {
            //дефолтный роутер
            endpoints.MapDefaultControllerRoute();

            //для маршрутизации если используются атрибуты
            endpoints.MapControllers();
        });

        // добавляем роутинг по умолчанию
        app.Run(async context =>
        {
            //возвращаем 404 ошибку
            await Task.Run(() => context.Response.StatusCode = 404);
        });
    }
}
