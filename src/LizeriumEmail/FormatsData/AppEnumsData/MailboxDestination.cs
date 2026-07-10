/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 10 июля 2026 12:16:48
 * Version: 1.0.104
 */

namespace LizeriumEmail.FormatsData.AppEnumsData;

/// <summary>
/// Перечисление предназначений почтового ящика
/// </summary>
internal enum MailboxDestination
{
    Transactional = 1,  //важные обязательные письма
    Administrator = 2,  //для отправки сообщений от имени администратора
    Notification = 3    //для рассылки уведомлений
}
