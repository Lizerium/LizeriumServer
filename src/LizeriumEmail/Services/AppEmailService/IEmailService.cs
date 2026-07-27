/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 27 июля 2026 15:17:11
 * Version: 1.0.121
 */

using LizeriumEmail.FormatsData.AppEmailData;

namespace LizeriumEmail.Services.AppEmailService;

/// <summary>
/// Интерфейс отправки Email
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Метод отправляет Email
    /// </summary>
    /// <param name="emailData">Объект данных об отправляемом Email</param>
    Task SendEmailAsync(EmailData emailData);
}