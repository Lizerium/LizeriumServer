/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 27 апреля 2026 10:01:53
 * Version: 1.0.32
 */

using LizeriumLogging.Accessories.LoggingAccessories;

namespace LizeriumServer.Accessories.AuthAccessories;

/// <summary>
/// Класс вспомогательных методов для авторизации
/// </summary>
public static class AuthExtensions
{
    /// <summary>
    /// Название куки сессии пользователя
    /// </summary>
    public const string NameSessionCookie = ".Aws.Session";

    /// <summary>
    /// Метод - расширение разрушает сессию пользователя
    /// </summary>
    /// <param name="httpContext">HttpContext запроса</param>
    public static void DestroyUserSession(this HttpContext httpContext)
    {
        try
        {
            //очищаем сессию пользователя
            httpContext.Session.Clear();

            //удаляем куку сессии
            httpContext.Response.Cookies.Delete(NameSessionCookie);
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();
        }
    }
}