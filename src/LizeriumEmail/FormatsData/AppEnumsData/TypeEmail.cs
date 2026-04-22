/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 22 апреля 2026 18:58:18
 * Version: 1.0.27
 */

namespace LizeriumEmail.FormatsData.AppEnumsData;

/// <summary>
/// Перечисление типов Email
/// </summary>
public enum TypeEmail
{
    LogToAdmin = 1,                 //сообщение лога для администратора
    ExceptionToAdmin = 2,           //исключение для администратора
    ConfitmationCodeAdminAuth = 3,  //код подтверждения на авторизацию администратора
    ConfirmRegistrationByEmail = 4, //подтверждение регистрации по Email
    RecoverPassword = 5,            //восстановление пароля пользователя в личный кабинет
    ConfirmEmail = 6,               //подтверждение Email
    NewTicket = 7,                  //новый тикет
    UserNotify = 8                  //уведомление пользователя
}