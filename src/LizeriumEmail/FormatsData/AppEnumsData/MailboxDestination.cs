/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 11 октября 2026 11:57:09
 * Version: 1.0.203
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
