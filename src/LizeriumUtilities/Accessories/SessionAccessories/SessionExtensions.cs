/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 13 июля 2026 07:11:25
 * Version: 1.0.107
 */

using Microsoft.AspNetCore.Http;
using LizeriumLogging.Accessories.LoggingAccessories;
using LizeriumUtilities.Accessories.JsonAccessories;

namespace LizeriumUtilities.Accessories.SessionAccessories;

/// <summary>
/// Класс вспомогательных методов для роботы с сессиями
/// </summary>
public static class SessionExtensions
{
    /// <summary>
    /// Метод - расширение преобразует объект в строку JSON и ставит ее в данные сессии
    /// </summary>
    /// <typeparam name="T">Тип данных объекта</typeparam>
    /// <param name="session">Интерфейс сессии</param>
    /// <param name="key">Ключ сессии</param>
    /// <param name="value">Объект данных для установки в сессию</param>
    public static void SetSession<T>(this ISession session, string key, T value)
    {
        try
        {
            //устанавливаем в сессию сериализованный в JSON строку объект сессии
            session.SetString(key, value.SerializeToJson());
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();
        }
    }

    /// <summary>
    /// Метод получает данные сессии и конвертирует их в объект данных
    /// </summary>
    /// <typeparam name="T">Тип данных объекта</typeparam>
    /// <param name="session">Интерфейс сессии</param>
    /// <param name="key">Ключ сессии</param>
    /// <returns>Десериализованный объект данных</returns>
    public static T GetSession<T>(this ISession session, string key)
    {
        try
        {
            //получаем из сессии строку JSON
            var value = session.GetString(key);

            //проверяем строку и отдаем десериализованный объект
            return value == null ? default : value.DeserializeTo<T>();
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем дефолтное значение типа
            return default;
        }
    }
}