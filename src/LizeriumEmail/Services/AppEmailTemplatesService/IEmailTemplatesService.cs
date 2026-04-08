/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 08 апреля 2026 14:43:45
 * Version: 1.0.10
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