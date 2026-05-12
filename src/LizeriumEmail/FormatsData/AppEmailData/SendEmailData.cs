/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 12 мая 2026 12:22:50
 * Version: 1.0.47
 */

using LizeriumEmail.FormatsData.AppEnumsData;

namespace LizeriumEmail.FormatsData.AppEmailData;

/// <summary>
/// Объект данных об отправляемом Email
/// </summary>
internal class SendEmailData
{
    /// <summary>
    /// С какого почтового ящика отправить
    /// </summary>
    internal MailboxDestination SendFrom { get; init; }

    /// <summary>
    /// Email получателя
    /// </summary>
    internal string EmailRecipient { get; init; }

    /// <summary>
    /// Тема письма
    /// </summary>
    internal string SubjectEmail { get; init; }

    /// <summary>
    /// Содержание письма
    /// </summary>
    internal string ContentEmail { get; init; }

    /// <summary>
    /// Флаг валидности данных
    /// </summary>
    internal bool ValidData => SendFrom != 0 && !string.IsNullOrEmpty(EmailRecipient) &&
                               !string.IsNullOrEmpty(SubjectEmail) && !string.IsNullOrEmpty(ContentEmail);
}