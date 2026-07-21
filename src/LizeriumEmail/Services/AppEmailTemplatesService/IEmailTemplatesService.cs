/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 21 июля 2026 13:52:02
 * Version: 1.0.115
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