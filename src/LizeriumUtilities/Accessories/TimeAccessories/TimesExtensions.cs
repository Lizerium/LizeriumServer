/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 14 июля 2026 07:17:23
 * Version: 1.0.108
 */

using LizeriumLogging.Accessories.LoggingAccessories;

namespace LizeriumUtilities.Accessories.TimeAccessories;

/// <summary>
/// Класс вспомогательных методов для работы со временем и датой
/// </summary>
public static class TimesExtensions
{
    /// <summary>
    /// Метод - расширение отдает текущий год как строку
    /// </summary>
    /// <returns>Текущий год как строка</returns>
    public static string GetCurrentYearAsString()
    {
        try
        {
            //отдаем год на сейчас как строку
            return DateTime.Now.ToString("yyyy");
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
    /// Метод отдает текущее время в формате UNIX
    /// </summary>
    /// <returns>Текущее время в UNIX формате</returns>
    public static long UnixTime()
    {
        try
        {
            //отдаем UnixTime
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем ноль
            return 0;
        }
    }

    /// <summary>
    /// Метод - расширение конвертирует UnixTime в обычное
    /// </summary>
    /// <param name="unixTime">Время в UNIX формате</param>
    /// <returns>Время в DateTime формате</returns>
    public static DateTime ConvertUnixTime(this long unixTime)
    {
        try
        {
            //создаем дату от 1 января 1970 года в московское время
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(unixTime).AddHours(3);
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем минимальное значение даты
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// Метод - расширение форматирует дату в строку
    /// </summary>
    /// <param name="dateTime">Дата</param>
    /// <returns>Строчное значение даты</returns>
    public static string TimeToString(this DateTime dateTime)
    {
        try
        {
            //форматируем дату в строку
            return dateTime.ToString("dd.MM.yyyy HH:mm");
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
    /// Метод - расширение форматирует дату в строку
    /// </summary>
    /// <param name="dateTime">Дата</param>
    /// <returns>Строчное значение даты</returns>
    public static string FullTimeToString(this DateTime dateTime)
    {
        try
        {
            //форматируем дату в строку
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
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
    /// Метод отдает текущее московское время
    /// </summary>
    /// <returns>Текущее московское время</returns>
    public static DateTime MoscowTime()
    {
        try
        {
            //рассчитываем московское время по UTC с добавленными тремя часами (Москва - Лондон три часа разницы)
            var moscowTime = DateTime.UtcNow.AddHours(3);

            //отдаем московское время
            return new DateTime(moscowTime.Year, moscowTime.Month, moscowTime.Day, moscowTime.Hour, moscowTime.Minute, moscowTime.Second);
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем минимальное время
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// Метод - расширение получает дату в строковом значении из Дата
    /// </summary>
    /// <param name="dateTime">Дата</param>
    /// <returns>Дата в строковом значении</returns>
    public static string GetDateAsString(this DateTime dateTime)
    {
        try
        {
            //форматируем дату в строковом значении
            return dateTime.ToString("dd.MM.yyyy");
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