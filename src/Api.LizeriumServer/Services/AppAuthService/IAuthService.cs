/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 03 мая 2026 07:12:11
 * Version: 1.0.38
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
