/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 13 июня 2026 14:19:57
 * Version: 1.0.78
 */

using Microsoft.AspNetCore.Http;
using LizeriumLogging.Accessories.LoggingAccessories;

namespace LizeriumUtilities.Middleware;

/// <summary>
/// Мой обработчик ошибок протокола
/// </summary>
public class ErrorHandlingMiddleware
{
    /// <summary>
    /// Делегат на передачу действия следующему в роутере
    /// </summary>
    private RequestDelegate Next { get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="next">Делегат на передачу действия следующему в роутере</param>
    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        Next = next;
    }

    /// <summary>
    /// Асинхронный обработчик запроса
    /// </summary>
    /// <param name="context">Контекст запроса</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            //вначале передаем управление сразу на следующий метод в роутере
            await Next(context);

            //проверяем код ошибки
            if (context.Response.StatusCode == 404)
            {
                //редиректим на страницу ошибки можно еще код ошибки отправлять get параметром
                context.Response.Redirect("/Home/Error", true);
            }
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();
            //редиректим на страницу ошибки можно еще код ошибки отправлять get параметром
            context.Response.Redirect("/Home/Error", true);
        }
    }
}