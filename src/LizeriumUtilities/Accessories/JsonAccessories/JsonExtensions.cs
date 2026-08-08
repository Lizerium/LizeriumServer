/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 08 августа 2026 07:13:54
 * Version: 1.0.134
 */

using System.Text.Json;
using System.Text.Json.Serialization;
using LizeriumLogging.Accessories.LoggingAccessories;
using LizeriumUtilities.FormatsData.AppEnumsData;
using LizeriumUtilities.FormatsData.AppResponseData.BadResponses;
using Microsoft.AspNetCore.Mvc;

namespace LizeriumUtilities.Accessories.JsonAccessories;

/// <summary>
/// Класс вспомогательных методов для работы с JSON
/// </summary>
public static class JsonExtensions
{
    /// <summary>
    /// Метод - расширение десериализует JSON строку в объект заданного типа
    /// </summary>
    /// <typeparam name="T">Тип данных</typeparam>
    /// <param name="value">JSON строка</param>
    /// <param name="jsonConverter">Кастомный конвертер</param>
    /// <returns></returns>
    public static T DeserializeTo<T>(this string value, JsonConverter jsonConverter = null)
    {
        try
        {
            //если входящая строка пуста, отдаем значение типа по умолчанию
            if (string.IsNullOrEmpty(value)) return default;

            //создаем объект настроек сериализации
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false)
                }
            };

            //если задан кастомный конвертер
            if (jsonConverter != null)
            {
                //добавляем кастомный конвертер
                jsonSerializerOptions.Converters.Add(jsonConverter);
            }

            //десериализуем JSON строку в объект заданного типа
            return JsonSerializer.Deserialize<T>(value, jsonSerializerOptions);
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //логируем строку JSON которую не удалось десериализовать
            $"JSON: {value}".LogMessage();

            //отдаем значение типа по умолчанию
            return default;
        }
    }

    /// <summary>
    /// Метод - расширение формата объекта в JSON строку
    /// </summary>
    /// <param name="data">Входящий объект</param>
    /// <param name="jsonConverter">Кастомный конвертер</param>
    /// <returns>Строка JSON</returns>
    public static string SerializeToJson(this object data, JsonConverter jsonConverter = null)
    {
        try
        {
            //проверяем входящие данные
            if (data == null) return string.Empty;

            //создаем объект настроек сериализации
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false)
                }
            };

            //если задан кастомный конвертер
            if (jsonConverter != null)
            {
                //добавляем кастомный конвертер
                jsonSerializerOptions.Converters.Add(jsonConverter);
            }

            //сериализуем объект в строку JSON и отдаем ее
            return JsonSerializer.Serialize(data, jsonSerializerOptions);
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем пустую строку
            return string.Empty;
        }
    }

    /// <summary>
    /// Метод - расширение форматирует объект в ответ с JSON строкой в теле
    /// </summary>
    /// <param name="data">Входящий объект</param>
    /// <param name="jsonConverter">Кастомный конвертер</param>
    /// <returns>Результат ответа</returns>
    public static ContentResult SuccessResponse(this object data, JsonConverter jsonConverter = null)
    {
        try
        {
            //создаем контент ответа
            return new ContentResult
            {
                Content = data.SerializeToJson(jsonConverter),
                ContentType = "application/json",
                StatusCode = 200
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

    /// <summary>
    /// Метод - расширение отдает ответ о неудачном запросе
    /// </summary>
    /// <param name="reason">Причина неудачного запроса</param>
    /// <param name="sideError">На чьей стороне возникла ошибка</param>
    /// <returns>Результат ответа</returns>
    public static ContentResult FailedResponse(this string reason, SideError sideError)
    {
        try
        {
            //проверяем входящие данные
            if (reason == null) return null;

            //HTTP код статуса ответа
            int statusCode;

            //смотрим на чьей стороне возникла ошибка
            switch (sideError)
            {
                case SideError.UserSide: //ошибка на стороне пользователя
                    statusCode = 400;
                    break;
                case SideError.ServerSide: //ошибка на стороне сервера
                    statusCode = 500;
                    break;
                default:
                    return null;
            }

            //создаем контент ответа
            return new ContentResult
            {
                Content = new FailedRequestReason { Reason = reason }.SerializeToJson(),
                ContentType = "application/json",
                StatusCode = statusCode
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