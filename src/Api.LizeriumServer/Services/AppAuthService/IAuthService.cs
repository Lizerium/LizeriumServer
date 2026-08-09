/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 09 августа 2026 15:52:37
 * Version: 1.0.135
 */

namespace Api.LizeriumServer.Services.AppAuthService;

/// <summary>
/// Интерфейс авторизации
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Метод проверяет валидность секретного ключа авторизации
    /// </summary>
    /// <param name="secretKey">Секретный ключ авторизации</param>
    /// <returns>Результат проверки</returns>
    public bool IsValidSecretKey(string secretKey);

    /// <summary>
    /// Метод отдаем Email администратора
    /// </summary>
    /// <param name="secretKey">Секретный ключ авторизации</param>
    /// <returns>Email администратора</returns>
    public string GetEmailAdmin(string secretKey);
}
