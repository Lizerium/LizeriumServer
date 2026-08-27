/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 27 августа 2026 08:52:27
 * Version: 1.0.159
 */

using Api.LizeriumServer;
using Data;
using LizeriumLogging.Accessories.LoggingAccessories;
using LizeriumUtilities.Accessories.ConfigurationAccessories;

/// <summary>
/// Application entry point.
/// </summary>
public static class Program
{
    /// <summary>
    /// Application settings.
    /// </summary>
    public static AppSettings SettingsApp { get; private set; }

    /// <summary>
    /// Application configuration.
    /// </summary>
    public static IConfigurationRoot Configuration { get; private set; }

    /// <summary>
    /// Starts the application.
    /// </summary>
    public static void Main()
    {
        SettingsApp = new AppSettings();

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"{LoggingExtensions.AppDir}/app_configuration.json");

        Configuration = builder.Build();

        SettingsApp.IsRelease = CommonConfigurationExtensions.IsRelease;

        SettingsApp.AppHost = Configuration["appSettings:appHost"];

        var host = new WebHostBuilder()

            // Local debug should not depend on Windows EventLog permissions.
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
            })

            .UseKestrel(options =>
            {
                options.Limits.MaxConcurrentConnections = null;
                options.Limits.MaxConcurrentUpgradedConnections = null;
                options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
                options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
                options.Limits.MaxRequestBodySize = null;
            })

            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseStartup<Startup>()
            .UseUrls($"http://{SettingsApp.AppHost}")
            .Build();

        host.Run();
    }
}
