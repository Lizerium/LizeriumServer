/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 13 июня 2026 14:19:57
 * Version: 1.0.78
 */

using LizeriumLogging.Accessories.LoggingAccessories;
using Microsoft.Extensions.Configuration;

namespace LizeriumUtilities.Accessories.ConfigurationAccessories;

/// <summary>
/// Класс вспомогательных методов общей конфигурации
/// </summary>
public static class CommonConfigurationExtensions
{
    /// <summary>
    /// Данные общей конфигурации
    /// </summary>
    private static IConfigurationRoot _commonConfiguration;

    /// <summary>
    /// Обертка общих данных конфигурации
    /// </summary>
    public static IConfigurationRoot CommonConfiguration => _commonConfiguration ??= GenerateConfiguration();

    /// <summary>
    /// Режим запуска приложения
    /// </summary>
    private static bool? _isRelease;

    /// <summary>
    /// Обертка режима запуска приложения
    /// </summary>
    public static bool IsRelease
    {
        get
        {
            if (_isRelease == null)
            {
#if DEBUG
                _isRelease = false;
#else
                    _isRelease = true;
#endif
            }

            return _isRelease.Value;
        }
    }

    /// <summary>
    /// Главный домен приложения
    /// </summary>
    private static string _mainDomain;

    /// <summary>
    /// Обертка главного домена приложения
    /// </summary>
    public static string MainDomain => _mainDomain ??= CommonConfiguration["appSettings:mainDomain"];

    /// <summary>
    /// Метод генерирует данные общей конфигурации
    /// </summary>
    /// <returns>Данные общей конфигурации</returns>
    private static IConfigurationRoot GenerateConfiguration()
    {
        try
        {
            //инициализируем строителя конфигурации
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"{LoggingExtensions.AppDir}/common_configuration.json");

            //строим и отдаем параметры конфигурации
            return builder.Build();
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем null
            return null;
        }
    }

    /// <summary>
    /// Метод отдает коллекцию известных прокси
    /// </summary>
    /// <returns>Коллекция известных прокси</returns>
    public static IEnumerable<string> GetKnownProxies()
    {
        try
        {
            //получаем из файла конфигурации хосты прокси
            var knownProxies = CommonConfiguration
                .GetSection("knownProxies")
                .GetChildren()
                .Select(children => children.Value);

            //отдаем коллекцию известных прокси
            return knownProxies;
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем null
            return null;
        }
    }
}
