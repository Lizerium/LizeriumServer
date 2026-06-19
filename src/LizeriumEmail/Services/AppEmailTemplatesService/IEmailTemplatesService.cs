/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 19 июня 2026 07:10:55
 * Version: 1.0.84
 */

using LizeriumEmail.FormatsData.AppEmailData;

namespace LizeriumEmail.Services.AppEmailTemplatesService;

/// <summary>
/// Интерфейс создания шаблонов Email сообщений
/// </summary>
internal interface IEmailTemplatesService
{
    /// <summary>
    /// Метод возвращает HTML содержимое письма
    /// </summary>
    /// <param name="emailData"></param>
    /// <returns></returns>
    string GetEmailHtml(EmailData emailData);
}