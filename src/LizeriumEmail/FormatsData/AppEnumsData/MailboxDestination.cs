/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 04 мая 2026 07:13:32
 * Version: 1.0.39
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
