/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 23 июня 2026 16:15:16
 * Version: 1.0.88
 */

using LizeriumLogging.Accessories.LoggingAccessories;
using LizeriumEmail.FormatsData.AppEmailData;
using LizeriumEmail.FormatsData.AppEnumsData;
using System.Net.Mail;
using System.Net;
using LizeriumEmail.Services.AppEmailTemplatesService.Implements;
using LizeriumEmail.Services.AppEmailTemplatesService;
using LizeriumEmail.Accessories.EmailAccessories;

namespace LizeriumEmail.Services.AppEmailService.Implements;

/// <summary>
/// Реализация интерфейс отправки Email
/// </summary>
public class EmailService : IEmailService
{
    /// <summary>
    /// Интерфейс создания шаблонов Email сообщений
    /// </summary>
    private IEmailTemplatesService EmailTemplates { get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="localizator">Интерфейс локализации</param>
    public EmailService()
    {
        EmailTemplates = new EmailTemplatesService();
    }

    /// <inheritdoc />
    /// <summary>
    /// Метод отправляет Email
    /// </summary>
    /// <param name="emailData">Объект данных об отправляемом Email</param>
    public async Task SendEmailAsync(EmailData emailData)
    {
        try
        {
            //проверяем входящие данные
            if (emailData == null || !emailData.ValidData()) return;

            //смотрим тип Email
            switch (emailData.EmailType)
            {
                case TypeEmail.LogToAdmin: //сообщение лога для администратора
                case TypeEmail.ExceptionToAdmin: //исключение для администратора
                case TypeEmail.ConfitmationCodeAdminAuth: //код подтверждения на авторизацию администратора

                    //отправляем Email
                    await SendEmailBySmtpAsync(new SendEmailData
                    {
                        SendFrom = MailboxDestination.Administrator,
                        EmailRecipient = emailData.Recipient,
                        SubjectEmail = emailData.SubjectEmail,
                        ContentEmail = EmailTemplates.GetEmailHtml(emailData)
                    });

                    break;
                case TypeEmail.ConfirmRegistrationByEmail: //подтверждение регистрации по Email

                    //отправляем Email
                    await SendEmailBySmtpAsync(new SendEmailData
                    {
                        SendFrom = MailboxDestination.Transactional,
                        EmailRecipient = emailData.Recipient,
                        SubjectEmail = "Активация аккаунта Lizerium",
                        ContentEmail = EmailTemplates.GetEmailHtml(emailData)
                    });

                    break;
                case TypeEmail.RecoverPassword: //восстановление пароля пользователя в личный кабинет

                    //отправляем Email
                    await SendEmailBySmtpAsync(new SendEmailData
                    {
                        SendFrom = MailboxDestination.Transactional,
                        EmailRecipient = emailData.Recipient,
                        SubjectEmail = "Восстановление пароля Lizerium",
                        ContentEmail = EmailTemplates.GetEmailHtml(emailData)
                    });

                    break;
                case TypeEmail.ConfirmEmail: //подтверждение Email
                    {
                        //отправляем Email
                        await SendEmailBySmtpAsync(new SendEmailData
                        {
                            SendFrom = MailboxDestination.Transactional,
                            EmailRecipient = emailData.Recipient,
                            SubjectEmail = "Подтверждение Email аккаунта Lizerium",
                            ContentEmail = EmailTemplates.GetEmailHtml(emailData)
                        });
                    }
                    break;
                case TypeEmail.NewTicket: //новый тикет
                    {
                        //пишем Email получателя
                        emailData.Recipient = EmailExtensions.Configuration["emailSupport"];

                        //отправляем Email
                        await SendEmailBySmtpAsync(new SendEmailData
                        {
                            SendFrom = MailboxDestination.Administrator,
                            EmailRecipient = emailData.Recipient,
                            SubjectEmail = emailData.SubjectEmail,
                            ContentEmail = EmailTemplates.GetEmailHtml(emailData)
                        });
                    }
                    break;
                case TypeEmail.UserNotify: //уведомление пользователя
                    {
                        //отправляем Email
                        await SendEmailBySmtpAsync(new SendEmailData
                        {
                            SendFrom = MailboxDestination.Notification,
                            EmailRecipient = emailData.Recipient,
                            SubjectEmail = emailData.SubjectEmail,
                            ContentEmail = EmailTemplates.GetEmailHtml(emailData)
                        });
                    }
                    break;
                default:
                    return;
            }
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();
        }
    }

    /// <summary>
    /// Метод отправляет Email через API MailGun
    /// </summary>
    /// <param name="sendEmailData">Объект данных об отправляемом Email</param>
    /// <returns></returns>
    private static async Task SendEmailBySmtpAsync(SendEmailData sendEmailData)
    {
        try
        {
            //проверяем входящие параметры
            if (sendEmailData is not { ValidData: true }) return;

            //создаем SMTP клиент
            var smtpClient = new SmtpClient(EmailExtensions.SystemEmailsData[sendEmailData.SendFrom].SmtpHost, EmailExtensions.SystemEmailsData[sendEmailData.SendFrom].SmtpPort)
            {
                Credentials = new NetworkCredential(EmailExtensions.SystemEmailsData[sendEmailData.SendFrom].SmtpLogin, EmailExtensions.SystemEmailsData[sendEmailData.SendFrom].SmtpPassword),
                EnableSsl = true  //используем SSL
            };

            //используем SMTP клиент
            using (smtpClient)
            {
                //создаем объект сообщения
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(
                        EmailExtensions.SystemEmailsData[sendEmailData.SendFrom].SmtpLogin, 
                        EmailExtensions.DisplayedName),
                    Subject = sendEmailData.SubjectEmail,
                    Body = sendEmailData.ContentEmail,
                    IsBodyHtml = true,
                    To =
                    {
                        sendEmailData.EmailRecipient
                    }
                };

                //используем объект сообщения
                using (mailMessage)
                {
                    //отправляем письмо
                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();
        }
    }
}
