/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 12 апреля 2026 14:31:14
 * Version: 1.0.14
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