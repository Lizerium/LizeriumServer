using Api.LizeriumServer;
using Data;
using LizeriumLogging.Accessories.LoggingAccessories;
using LizeriumUtilities.Accessories.ConfigurationAccessories;

/// <summary>
/// Стартовый класс приложения
/// </summary>
public static class Program
{
    /// <summary>
    /// Объект данных о настройке конфигурации приложения
    /// </summary>
    public static AppSettings SettingsApp { get; private set; }

    /// <summary>
    /// Настройки конфигурации приложения
    /// </summary>
    public static IConfigurationRoot Configuration { get; private set; }

    /// <summary>
    /// Стартовый метод приложения
    /// </summary>
    public static void Main()
    {
        //инициализируем объект данных о настройке конфигурации приложения
        SettingsApp = new AppSettings();

        //инициализируем строителя конфигурации
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"{LoggingExtensions.AppDir}/app_configuration.json");

        //получаем параметры конфигурации
        Configuration = builder.Build();

        //пишем режим запуска приложения
        SettingsApp.IsRelease = CommonConfigurationExtensions.IsRelease;

        //пишем локальный хост приложения
        SettingsApp.AppHost = Configuration["appSettings:appHost"];

        //инициализируем хост приложения
        var host = new WebHostBuilder()

            //используем Kestrel
            .UseKestrel(options =>
            {
                //выставляем настройку максимального количества одновременных соединений
                options.Limits.MaxConcurrentConnections = null;

                //выставляем настройку максимального количества одновременных соединений
                options.Limits.MaxConcurrentUpgradedConnections = null;

                //таймаут на получение заголовков ставим 30 секунд
                options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);

                //таймаут на получение тела ответа ставим 2 минуты
                options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);

                //убираем лимит на загрузку файлов
                options.Limits.MaxRequestBodySize = null;
            })

            //текущая директория приложения
            .UseContentRoot(Directory.GetCurrentDirectory())

            //стартовый класс приложения
            .UseStartup<Startup>()

            //какой URL использовать для приложения
            .UseUrls($"http://{SettingsApp.AppHost}")

            //строим host
            .Build();

        //запускаем host
        host.Run();
    }
}