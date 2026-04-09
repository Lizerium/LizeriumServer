/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 09 апреля 2026 11:13:36
 * Version: 1.0.11
 */

using Api.LizeriumServer;
using Data;
using LizeriumLogging.Accessories.LoggingAccessories;
using LizeriumUtilities.Accessories.ConfigurationAccessories;

/// <summary>
/// РЎС‚Р°СЂС‚РѕРІС‹Р№ РєР»Р°СЃСЃ РїСЂРёР»РѕР¶РµРЅРёСЏ
/// </summary>
public static class Program
{
    /// <summary>
    /// РћР±СЉРµРєС‚ РґР°РЅРЅС‹С… Рѕ РЅР°СЃС‚СЂРѕР№РєРµ РєРѕРЅС„РёРіСѓСЂР°С†РёРё РїСЂРёР»РѕР¶РµРЅРёСЏ
    /// </summary>
    public static AppSettings SettingsApp { get; private set; }

    /// <summary>
    /// РќР°СЃС‚СЂРѕР№РєРё РєРѕРЅС„РёРіСѓСЂР°С†РёРё РїСЂРёР»РѕР¶РµРЅРёСЏ
    /// </summary>
    public static IConfigurationRoot Configuration { get; private set; }

    /// <summary>
    /// РЎС‚Р°СЂС‚РѕРІС‹Р№ РјРµС‚РѕРґ РїСЂРёР»РѕР¶РµРЅРёСЏ
    /// </summary>
    public static void Main()
    {
        //РёРЅРёС†РёР°Р»РёР·РёСЂСѓРµРј РѕР±СЉРµРєС‚ РґР°РЅРЅС‹С… Рѕ РЅР°СЃС‚СЂРѕР№РєРµ РєРѕРЅС„РёРіСѓСЂР°С†РёРё РїСЂРёР»РѕР¶РµРЅРёСЏ
        SettingsApp = new AppSettings();

        //РёРЅРёС†РёР°Р»РёР·РёСЂСѓРµРј СЃС‚СЂРѕРёС‚РµР»СЏ РєРѕРЅС„РёРіСѓСЂР°С†РёРё
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"{LoggingExtensions.AppDir}/app_configuration.json");

        //РїРѕР»СѓС‡Р°РµРј РїР°СЂР°РјРµС‚СЂС‹ РєРѕРЅС„РёРіСѓСЂР°С†РёРё
        Configuration = builder.Build();

        //РїРёС€РµРј СЂРµР¶РёРј Р·Р°РїСѓСЃРєР° РїСЂРёР»РѕР¶РµРЅРёСЏ
        SettingsApp.IsRelease = CommonConfigurationExtensions.IsRelease;

        //РїРёС€РµРј Р»РѕРєР°Р»СЊРЅС‹Р№ С…РѕСЃС‚ РїСЂРёР»РѕР¶РµРЅРёСЏ
        SettingsApp.AppHost = Configuration["appSettings:appHost"];

        //РёРЅРёС†РёР°Р»РёР·РёСЂСѓРµРј С…РѕСЃС‚ РїСЂРёР»РѕР¶РµРЅРёСЏ
        var host = new WebHostBuilder()

            //РёСЃРїРѕР»СЊР·СѓРµРј Kestrel
            .UseKestrel(options =>
            {
                //РІС‹СЃС‚Р°РІР»СЏРµРј РЅР°СЃС‚СЂРѕР№РєСѓ РјР°РєСЃРёРјР°Р»СЊРЅРѕРіРѕ РєРѕР»РёС‡РµСЃС‚РІР° РѕРґРЅРѕРІСЂРµРјРµРЅРЅС‹С… СЃРѕРµРґРёРЅРµРЅРёР№
                options.Limits.MaxConcurrentConnections = null;

                //РІС‹СЃС‚Р°РІР»СЏРµРј РЅР°СЃС‚СЂРѕР№РєСѓ РјР°РєСЃРёРјР°Р»СЊРЅРѕРіРѕ РєРѕР»РёС‡РµСЃС‚РІР° РѕРґРЅРѕРІСЂРµРјРµРЅРЅС‹С… СЃРѕРµРґРёРЅРµРЅРёР№
                options.Limits.MaxConcurrentUpgradedConnections = null;

                //С‚Р°Р№РјР°СѓС‚ РЅР° РїРѕР»СѓС‡РµРЅРёРµ Р·Р°РіРѕР»РѕРІРєРѕРІ СЃС‚Р°РІРёРј 30 СЃРµРєСѓРЅРґ
                options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);

                //С‚Р°Р№РјР°СѓС‚ РЅР° РїРѕР»СѓС‡РµРЅРёРµ С‚РµР»Р° РѕС‚РІРµС‚Р° СЃС‚Р°РІРёРј 2 РјРёРЅСѓС‚С‹
                options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);

                //СѓР±РёСЂР°РµРј Р»РёРјРёС‚ РЅР° Р·Р°РіСЂСѓР·РєСѓ С„Р°Р№Р»РѕРІ
                options.Limits.MaxRequestBodySize = null;
            })

            //С‚РµРєСѓС‰Р°СЏ РґРёСЂРµРєС‚РѕСЂРёСЏ РїСЂРёР»РѕР¶РµРЅРёСЏ
            .UseContentRoot(Directory.GetCurrentDirectory())

            //СЃС‚Р°СЂС‚РѕРІС‹Р№ РєР»Р°СЃСЃ РїСЂРёР»РѕР¶РµРЅРёСЏ
            .UseStartup<Startup>()

            //РєР°РєРѕР№ URL РёСЃРїРѕР»СЊР·РѕРІР°С‚СЊ РґР»СЏ РїСЂРёР»РѕР¶РµРЅРёСЏ
            .UseUrls($"http://{SettingsApp.AppHost}")

            //СЃС‚СЂРѕРёРј host
            .Build();

        //Р·Р°РїСѓСЃРєР°РµРј host
        host.Run();
    }
}