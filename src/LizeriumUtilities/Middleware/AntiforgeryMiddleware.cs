/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 15 августа 2026 07:14:28
 * Version: 1.0.146
 */

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using LizeriumLogging.Accessories.LoggingAccessories;

namespace LizeriumUtilities.Middleware;

/// <summary>
/// Обработчик службы против подделки запросов
/// </summary>
public class AntiforgeryMiddleware
{
    /// <summary>
    /// Делегат на передачу действия следующему в роутере
    /// </summary>
    private RequestDelegate Next { get; }

    /// <summary>
    /// API для настройки функций против подделки
    /// </summary>
    private IAntiforgery Antiforgery { get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="next">Делегат на передачу действия следующему в роутере</param>
    /// <param name="antiforgery">API для настройки функций против подделки</param>
    public AntiforgeryMiddleware(RequestDelegate next, IAntiforgery antiforgery)
    {
        Next = next;
        Antiforgery = antiforgery;
    }

    /// <summary>
    /// Асинхронный обработчик запроса
    /// </summary>
    /// <param name="context">Контекст запроса</param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            if (HttpMethods.IsGet(context.Request.Method) || HttpMethods.IsHead(context.Request.Method))
            {
                //генерируем токен против подделки запросов
                var tokens = Antiforgery.GetAndStoreTokens(context);

                //устанавливаем токен в куку
                context.Response.Cookies.Append("CSRF-TOKEN", tokens.RequestToken ?? string.Empty, new CookieOptions
                {
                    HttpOnly = false,
                    SameSite = SameSiteMode.Lax,
                    Secure = context.Request.IsHttps
                });
            }
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();
        }
        finally
        {
            //передаем управление на следующий метод в роутере что бы был вывод/перенаправление ошибки 
            await Next.Invoke(context);
        }
    }
}
