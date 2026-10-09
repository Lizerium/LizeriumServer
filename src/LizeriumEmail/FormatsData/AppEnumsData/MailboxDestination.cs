/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 09 октября 2026 11:34:15
 * Version: 1.0.201
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
