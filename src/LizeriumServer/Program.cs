/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 25 апреля 2026 08:31:24
 * Version: 1.0.30
 */

using System.Globalization;
using System.Reflection;

using AspNetCore.ReCaptcha;

using LizeriumDatabase.Services.AppDataBaseService;
using LizeriumDatabase.Services.AppDataBaseService.Implements;

using LizeriumNetSecurity.Middleware;
using LizeriumNetSecurity.Services.SecurityService;
using LizeriumNetSecurity.Services.SecurityService.Implements;

using LizeriumServer.Accessories.AuthAccessories;
using LizeriumServer.Middleware;
using LizeriumServer.Options;
using LizeriumServer.Services.Breadcrumb;
using LizeriumServer.Services.Breadcrumb.Implements;
using LizeriumServer.Services.Hosted;

using LizeriumUtilities.Middleware;
using LizeriumUtilities.Services.DevelopService;
using LizeriumUtilities.Services.DownloadLinksService;
using LizeriumUtilities.Services.SecurityService.Implements;

using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<StoragePathsOptions>(
    builder.Configuration.GetSection("StoragePaths"));

//конфигурация политики кук
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = _ => false;            //не требуется согласие, а то даже сессионные куки не устанавливает
    options.MinimumSameSitePolicy = SameSiteMode.None;  //требуется ли согласие Пользователя на несущественные файлы cookie для данного запроса
    options.Secure = CookieSecurePolicy.Always;         //
});

//добавляем кэширование сервисов в памяти
builder.Services.AddDistributedMemoryCache();

//конфигурируем параметры сессии
builder.Services.AddSession(options =>
{
    options.Cookie.Name = AuthExtensions.NameSessionCookie; //ставим название куку сессии
    options.IdleTimeout = TimeSpan.FromMinutes(15);         //время хранения сессии при бездействии
    options.Cookie.HttpOnly = true;                         //доступ только с сервера
    options.Cookie.IsEssential = true;                      //указывает, действительно ли этот файл cookie необходим для правильной работы приложения. Если true, то проверки политики согласия могут быть обойдены
});

//настройки службы против подделки запросов
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

// Добавляем локализацию
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en", "ru" }
        .Select(c => new CultureInfo(c)).ToList();

    options.DefaultRequestCulture = new RequestCulture("ru");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Добавьте сервисы в контейнер.
builder.Services.AddControllersWithViews().AddViewLocalization()
    .AddDataAnnotationsLocalization().AddRazorRuntimeCompilation(); ; //компиляцию View при изменениях;

//добавляем политику CORS
builder.Services.AddCors();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Lizerium API",
        Version = "1.0.1",
        Description = "Управление сервисом загрузчика модов игр",
        Contact = new OpenApiContact
        {
            Name = "Dvurechensky",
            Email = "dvurechensky_pro@mail.ru"
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.Configure<ReCaptchaSettings>(builder.Configuration.GetSection("GoogleReCaptcha"));

// Настройка базы данных
var baseDir = AppContext.BaseDirectory;
var dbPath = Path.Combine(baseDir, "application.db");
builder.Services.AddDbContext<IDataBaseService, DataBaseService>(options =>
{
    options.UseSqlite($"Data Source={dbPath}")
           .EnableSensitiveDataLogging()
           .LogTo(message =>
           {
               // можно писать и в Debug Output
               System.Diagnostics.Debug.WriteLine(message);
               Console.WriteLine(message); // на случай запуска из терминала
           }, LogLevel.Information);
});


builder.Services.AddSingleton<IBreadcrumbService, BreadcrumbService>();
// HostedService, который при старте вызывает BuildSiteMapAsync
builder.Services.AddHostedService<SitemapHostedService>();
builder.Services.AddSingleton<IAppSecurityService, AppSecurityService>();
builder.Services.AddHostedService<BackgroundBlacklistWriter>();
builder.Services.AddSingleton(sp => new DevModeService("dev_mode.json"));
// сервис который мониторит актуальные ссылки на скачивание
builder.Services.AddSingleton(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var logger = sp.GetRequiredService<ILogger<DownloadLinksService>>();
    var filePath = Path.Combine(env.ContentRootPath, "downloads.json");
    return new DownloadLinksService(filePath, logger);
});

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All,
    RequireHeaderSymmetry = false,
    ForwardLimit = null
});

// Настройте конвейер HTTP-запросов.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Документация Lizerium Server");
        c.RoutePrefix = "docs";
    });
    app.UseDeveloperExceptionPage(); //используем страницу исключений
}
//app.UseHttpsRedirection();
app.UseStaticFiles();

//использовать политику куки
app.UseCookiePolicy();

//использовать сессии
app.UseSession();

app.UseRouting();

//устанавливаем настройки языковых параметров
var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);

//сервис проверки разрешений входа для IP
app.UseMiddleware<IpBlockMiddleware>();

//ручной CDN для отражения DoS атак (Mini Anti DoS (10.000-50.000 RPS)
app.UseMiddleware<DoSDetectionMiddleware>();

//обработчик службы отслеживания статистики
app.UseMiddleware<MonitorMiddleware>();

//обработчик службы против подделки запросов
app.UseMiddleware<AntiforgeryMiddleware>();

//обработчик ошибок
app.UseMiddleware<ErrorHandlingMiddleware>();

//обработчик службы отслеживания режима работы сервера
app.UseMiddleware<DevModeMiddleware>();

//используем CORS с любых хостов
app.UseCors(options =>
{
    options.WithOrigins("https://lizup.ru", "https://localhost:7176", "https://0.0.0.0:7176")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program { }