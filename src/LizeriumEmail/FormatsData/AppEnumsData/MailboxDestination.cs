/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 06 июня 2026 09:06:57
 * Version: 1.0.71
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
