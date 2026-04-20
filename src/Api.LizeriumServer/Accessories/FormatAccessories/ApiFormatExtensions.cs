/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 20 апреля 2026 16:37:49
 * Version: 1.0.25
 */

using LizeriumCrypt.Accessories;

using LizeriumLogging.Accessories.LoggingAccessories;
using LizeriumUtilities.FormatsData.AppUserData;

namespace Api.LizeriumServer.Accessories.FormatAccessories;

/// <summary>
/// Класс вспомогательных методов для форматирования данных
/// </summary>
public static class ApiFormatExtensions
{
    /// <summary>
    /// Метод - расширение получает данные о пользователе и ключе API 
    /// </summary>
    /// <param name="apiKey">Ключ API</param>
    /// <returns>Объект данных о ключе API пользователя</returns>
    public static UserApiKeyData GetUserApiKey(this string apiKey)
    {
        try
        {
            //проверяем входящие данные
            if (string.IsNullOrEmpty(apiKey)) return null;

            //разбиваем ключ API на части
            var parts = apiKey.Split(':');

            //если не 2 части, отдаем null
            if (parts.Length != 2) return null;

            //генерируем и отдаем объект данных о ключе API пользователя
            return new UserApiKeyData
            {
                IdUser = parts[0].ParseInt64(),
                ApiKey = parts[1]
            };
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
