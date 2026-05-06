/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 06 мая 2026 10:50:31
 * Version: 1.0.41
 */

using LizeriumLogging.Accessories.LoggingAccessories;
using Microsoft.Extensions.Configuration;

namespace LizeriumDatabase.Accessories.DataBaseAccessories;

/// <summary>
/// Класс вспомогательных методов для работы с Email
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Данные конфигурации
    /// </summary>
    private static IConfigurationRoot _configuration;

    /// <summary>
    /// Обертка данных конфигурации
    /// </summary>
    public static IConfigurationRoot Configuration => _configuration ??= GenerateConfiguration();

    /// <summary>
    /// Метод генерирует данные конфигурации
    /// </summary>
    /// <returns>Данные конфигурации</returns>
    private static IConfigurationRoot GenerateConfiguration()
    {
        try
        {
            var path = $"{LoggingExtensions.AppDir}/database_configuration.json";

            //инициализируем строителя конфигурации
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path);

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
}
