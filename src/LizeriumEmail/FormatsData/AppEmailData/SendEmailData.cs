/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 15 апреля 2026 07:04:51
 * Version: 1.0.20
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