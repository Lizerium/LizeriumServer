/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 25 мая 2026 11:33:09
 * Version: 1.0.59
 */

using LizeriumEmail.FormatsData.AppEnumsData;
using LizeriumLogging.Accessories.LoggingAccessories;

namespace LizeriumEmail.FormatsData.AppEmailData;

/// <summary>
/// Объект данных об отправляемом Email
/// </summary>
public class EmailData
{
    /// <summary>
    /// Тип Email
    /// </summary>
    public TypeEmail EmailType { get; init; }

    /// <summary>
    /// Email адрес получателя
    /// </summary>
    public string Recipient { get; set; }

    /// <summary>
    /// Тема письма
    /// </summary>
    public string SubjectEmail { get; init; }

    /// <summary>
    /// Сообщение Email
    /// </summary>
    public string Message { get; init; }

    /// <summary>
    /// Ссылка для кнопки
    /// </summary>
    public string BtnLinkUrl { get; init; }

    /// <summary>
    /// Метод проверят валидность данных о Email
    /// </summary>
    /// <returns>Флаг валидности данных</returns>
    public bool ValidData()
    {
        try
        {
            //смотрим тип Email
            switch (EmailType)
            {
                case TypeEmail.LogToAdmin: //сообщение лога для администратора
                case TypeEmail.ExceptionToAdmin: //исключение для администратора
                case TypeEmail.ConfitmationCodeAdminAuth: //код подтверждения на авторизацию администратора
                    {
                        //отдаем проверку Email получателя, темы письма и текста сообщения
                        return !string.IsNullOrEmpty(Recipient) && !string.IsNullOrEmpty(SubjectEmail) && !string.IsNullOrEmpty(Message);
                    }
                case TypeEmail.ConfirmRegistrationByEmail: //подтверждение регистрации по Email
                case TypeEmail.RecoverPassword: //восстановление пароля пользователя в личный кабинет
                case TypeEmail.ConfirmEmail: //подтверждение Email
                    {
                        //отдаем проверку Email получателя, ссылки для кнопки и локализация
                        return !string.IsNullOrEmpty(Recipient) && !string.IsNullOrEmpty(BtnLinkUrl);
                    }
                case TypeEmail.NewTicket: //новый тикет
                    {
                        //отдаем проверку сообщения и ссылки для кнопки
                        return !string.IsNullOrEmpty(SubjectEmail) && !string.IsNullOrEmpty(Message) && !string.IsNullOrEmpty(BtnLinkUrl);
                    }
                case TypeEmail.UserNotify: //уведомление пользователя
                    {
                        //отдаем проверку Email получателя, темы письма, текста сообщения, ссылки для кнопки и локализация
                        return !string.IsNullOrEmpty(Recipient) && !string.IsNullOrEmpty(SubjectEmail) && !string.IsNullOrEmpty(Message) && !string.IsNullOrEmpty(BtnLinkUrl);
                    }
                default:
                    return false;
            }
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем данные Email не валидны
            return false;
        }
    }
}