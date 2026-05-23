/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 23 мая 2026 11:33:14
 * Version: 1.0.57
 */

using LizeriumEmail.Accessories.EmailAccessories;
using LizeriumEmail.FormatsData.AppEmailData;
using LizeriumEmail.FormatsData.AppEnumsData;
using LizeriumLogging.Accessories.LoggingAccessories;
using LizeriumUtilities.Accessories.ConfigurationAccessories;
using LizeriumUtilities.Accessories.TimeAccessories;

namespace LizeriumEmail.Services.AppEmailTemplatesService.Implements;

/// <summary>
/// Реализация интерфейса создания шаблонов Email сообщений
/// </summary>
internal class EmailTemplatesService : IEmailTemplatesService
{
    /// <summary>
    /// Метод возвращает HTML содержимое письма
    /// </summary>
    /// <param name="emailData"></param>
    /// <returns></returns>
    public string GetEmailHtml(EmailData emailData)
    {
        try
        {
            //смотрим тип письма
            switch (emailData.EmailType)
            {
                case TypeEmail.LogToAdmin: //сообщение лога для администратора
                case TypeEmail.ExceptionToAdmin: //исключение для администратора
                case TypeEmail.ConfitmationCodeAdminAuth: //код подтверждения на авторизацию администратора
                    {
                        //отдаем HTML письма
                        return AdminEmailTemplate(emailData.SubjectEmail, emailData.Message);
                    }
                case TypeEmail.ConfirmRegistrationByEmail: //подтверждение регистрации по Email
                    {
                        //отдаем HTML письма
                        return TransactionalEmailTemplate(
                            "Активация аккаунта Lizerium",
                            @"Поздравляем с успешной регистрацией на Lizerium. " +
                            "Для активации вашего аккаунта нажмите на кнопку \"Подтвердить Email\"",
                            @"Подтвердить Email",
                            emailData.BtnLinkUrl, "Спасибо за выбор проектов Dvurechensky!");
                    }
                case TypeEmail.RecoverPassword: //восстановление пароля пользователя в личный кабинет
                    {
                        //отдаем HTML письма
                        return TransactionalEmailTemplate(
                            @"Восстановление пароля Lizerium",
                            "Для восстановления пароля аккаунта Lizerium нажмите на кнопку " +
                            "\"Восстановить\", и на открывшейся странице придумайте новый пароль",
                            "Восстановить",
                            emailData.BtnLinkUrl, "Спасибо за выбор проектов Dvurechensky!");
                    }
                case TypeEmail.ConfirmEmail: //подтверждение Email
                    {
                        //отдаем HTML письма
                        return TransactionalEmailTemplate(
                            "Подтверждение Email аккаунта Lizerium",
                            @"Для подтверждения Email нажмите на кнопку ""Подтвердить Email""",
                            "Подтвердить Email",
                            emailData.BtnLinkUrl, "Спасибо за выбор проектов Dvurechensky!");
                    }
                case TypeEmail.NewTicket: //новый тикет
                    {
                        //отдаем HTML письма
                        return TransactionalEmailTemplate(emailData.SubjectEmail, emailData.Message, "Ответить",
                            emailData.BtnLinkUrl, "");
                    }
                case TypeEmail.UserNotify: //уведомление пользователя
                    {
                        //отдаем HTML письма
                        return TransactionalEmailTemplate(emailData.SubjectEmail, emailData.Message, "Перейти",
                            emailData.BtnLinkUrl, "Спасибо за выбор проектов Dvurechensky!");
                    }
                default:
                    return null;
            }
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем null
            return null;
        }
    }

    /// <summary>
    /// Шаблон письма администратору только тема и сообщение
    /// </summary>
    /// <param name="subject">Тема письма</param>
    /// <param name="message">Сообщение Email</param>
    /// <returns>HTML письма</returns>
    private static string AdminEmailTemplate(string subject, string message)
    {
        try
        {
            //отдаем тему письма администратору
            return $@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
                        <html xmlns=""http://www.w3.org/1999/xhtml"">
                        <head>
                            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                            <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
                            <title>{subject}</title>
                            <style type=""text/css"">

                                * {{
                                    margin: 0;
                                    font-family: ""Helvetica Neue"", Helvetica, Arial, sans-serif;
                                    box-sizing: border-box;
                                    font-size: 14px;
                                }}

                                body {{
                                    -webkit-font-smoothing: antialiased;
                                    -webkit-text-size-adjust: none;
                                    width: 100% !important;
                                    height: 100%;
                                    line-height: 1.6em;
                                }}

                                table td {{
                                    vertical-align: top;
                                }}

                                body {{
                                    background-color: #f6f6f6;
                                }}

                                .body-wrap {{
                                    background-color: #f6f6f6;
                                    width: 100%;
                                }}

                                .container {{
                                    display: block !important;
                                    max-width: 600px !important;
                                    margin: 0 auto !important;
                                    clear: both !important;
                                }}

                                .content {{
                                    max-width: 600px;
                                    margin: 0 auto;
                                    display: block;
                                    padding: 20px;
                                }}

                                .main {{
                                    background-color: #fff;
                                    border: 1px solid #e9e9e9;
                                    border-radius: 3px;
                                }}

                                .content-wrap {{
                                    padding: 20px;
                                }}

                                .content-block {{
                                    padding: 0 0 20px;
                                }}

                                    .content-block.desc {{
                                        padding: 0 !important;
                                        color: #404e67;
                                        font-size: 10px;
                                    }}

                                .footer {{
                                    width: 100%;
                                    clear: both;
                                    color: #999;
                                    padding: 10px;
                                }}

                                    .footer p, .footer a, .footer td {{
                                        color: #999;
                                        font-size: 12px;
                                    }}

                                    .footer .content-block {{
                                        padding: 0 0 5px !important;
                                    }}

                                .reg, .reg a {{
                                    color: #0e1621 !important;
                                    font-weight: bold;
                                    text-decoration: none;
                                    font-size: 14px;
                                }}

                                .btn-primary {{
                                    text-decoration: none;
                                    color: #ffffff !important;
                                    background-color: #0267b2;
                                    border: solid #0267b2;
                                    border-width: 10px 20px;
                                    line-height: 2em;
                                    font-weight: bold;
                                    text-align: center;
                                    cursor: pointer;
                                    display: inline-block;
                                    border-radius: 5px;
                                }}

                                .aligncenter {{
                                    text-align: center;
                                }}

                                .header {{
                                    padding: 20px;
                                    text-align: center;
                                    border-radius: 3px 3px 0 0;
                                    background-color: #0e1621;
                                }}

                                    .header h1 {{
                                        font-size: 32px;
                                        color: #ffd700;
                                        font-weight: bold;
                                        letter-spacing: 1px;
                                        margin: 0 !important;
                                    }}

                                @media only screen and (max-width: 640px) {{
                                    body {{
                                        padding: 0 !important;
                                    }}

                                    .container {{
                                        padding: 0 !important;
                                        width: 100% !important;
                                    }}

                                    .content {{
                                        padding: 0 !important;
                                    }}
                                }}
                            </style>
                        </head>
                        <body>
                            <table class=""body-wrap"">
                                <tr>
                                    <td></td>
                                    <td class=""container"" width=""600"">
                                        <div class=""content"">
                                            <!-- ReSharper disable Html.Obsolete -->
                                            <table class=""main"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                                                <tr style=""height: 64px;"">
                                                    <td class=""header""><h1>{EmailExtensions.DisplayedName}</h1></td>
                                                </tr>
                                                <tr>
                                                    <td class=""content-wrap"">
                                                        <!-- ReSharper disable Html.Obsolete -->
                                                        <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
                                                            <tr>
                                                                <td class=""content-block""><strong>{subject}.</strong></td>
                                                            </tr>
                                                            <tr>
                                                                <td class=""content-block"">{message}.</td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                            <div class=""footer"">
                                                <table width=""100%"">
                                                    <tr>
                                                        <td class=""aligncenter content-block reg""><a href=""https://{CommonConfigurationExtensions.MainDomain}"">{TimesExtensions.GetCurrentYearAsString()}&nbsp;&reg;&nbsp;{CommonConfigurationExtensions.MainDomain}</a></td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>
                                    </td>
                                    <td></td>
                                </tr>
                            </table>
                        </body>
                        </html>";
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем null
            return null;
        }
    }

    /// <summary>
    /// Шаблон transactional письма с кнопкой (без отписки)
    /// </summary>
    /// <param name="subjectEmail">Тема письма</param>
    /// <param name="textEmail">Текст письма</param>
    /// <param name="nameButton">Название кнопки</param>
    /// <param name="linkButton">Ссылка кнопки</param>
    /// <param name="thanksPhrase">Фраза благодарности</param>
    /// <returns>HTML письма с кнопкой</returns>
    private static string TransactionalEmailTemplate(string subjectEmail, string textEmail, string nameButton, string linkButton, string thanksPhrase)
    {
        try
        {
            //отдаем тему письма transactional
            return $@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
                        <html xmlns=""http://www.w3.org/1999/xhtml"">
                        <head>
                            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                            <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
                            <title>{subjectEmail}</title>
                            <style type=""text/css"">

                                * {{
                                    margin: 0;
                                    font-family: ""Helvetica Neue"", Helvetica, Arial, sans-serif;
                                    box-sizing: border-box;
                                    font-size: 14px;
                                }}

                                body {{
                                    -webkit-font-smoothing: antialiased;
                                    -webkit-text-size-adjust: none;
                                    width: 100% !important;
                                    height: 100%;
                                    line-height: 1.6em;
                                }}

                                table td {{
                                    vertical-align: top;
                                }}

                                body {{
                                    background-color: #f6f6f6;
                                }}

                                .body-wrap {{
                                    background-color: #f6f6f6;
                                    width: 100%;
                                }}

                                .container {{
                                    display: block !important;
                                    max-width: 600px !important;
                                    margin: 0 auto !important;
                                    clear: both !important;
                                }}

                                .content {{
                                    max-width: 600px;
                                    margin: 0 auto;
                                    display: block;
                                    padding: 20px;
                                }}

                                .main {{
                                    background-color: #fff;
                                    border: 1px solid #e9e9e9;
                                    border-radius: 3px;
                                }}

                                .content-wrap {{
                                    padding: 20px;
                                }}

                                .content-block {{
                                    padding: 0 0 20px;
                                }}

                                    .content-block.desc {{
                                        padding: 0 !important;
                                        color: #404e67;
                                        font-size: 10px;
                                    }}

                                .footer {{
                                    width: 100%;
                                    clear: both;
                                    color: #999;
                                    padding: 10px;
                                }}

                                    .footer p, .footer a, .footer td {{
                                        color: #999;
                                        font-size: 12px;
                                    }}

                                    .footer .content-block {{
                                        padding: 0 0 5px !important;
                                    }}

                                .reg, .reg a {{
                                    color: #0e1621 !important;
                                    font-weight: bold;
                                    text-decoration: none;
                                    font-size: 14px;
                                }}

                                .btn-primary {{
                                    text-decoration: none;
                                    color: #ffffff !important;
                                    background-color: #0267b2;
                                    border: solid #0267b2;
                                    border-width: 10px 20px;
                                    line-height: 2em;
                                    font-weight: bold;
                                    text-align: center;
                                    cursor: pointer;
                                    display: inline-block;
                                    border-radius: 5px;
                                }}

                                .aligncenter {{
                                    text-align: center;
                                }}

                                .header {{
                                    padding: 20px;
                                    text-align: center;
                                    border-radius: 3px 3px 0 0;
                                    background-color: #0e1621;
                                }}

                                    .header h1 {{
                                        font-size: 32px;
                                        color: #ffd700;
                                        font-weight: bold;
                                        letter-spacing: 1px;
                                        margin: 0 !important;
                                    }}

                                @media only screen and (max-width: 640px) {{
                                    body {{
                                        padding: 0 !important;
                                    }}

                                    .container {{
                                        padding: 0 !important;
                                        width: 100% !important;
                                    }}

                                    .content {{
                                        padding: 0 !important;
                                    }}
                                }}
                            </style>
                        </head>
                        <body>
                            <table class=""body-wrap"">
                                <tr>
                                    <td></td>
                                    <td class=""container"" width=""600"">
                                        <div class=""content"">
                                            <!-- ReSharper disable Html.Obsolete -->
                                            <table class=""main"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                                                <tr style=""height: 64px;"">
                                                    <td class=""header""><h1>{EmailExtensions.DisplayedName}</h1></td>
                                                </tr>
                                                <tr>
                                                    <td class=""content-wrap"">
                                                        <!-- ReSharper disable Html.Obsolete -->
                                                        <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
                                                            <tr>
                                                                <td class=""content-block""><strong>{subjectEmail}.</strong></td>
                                                            </tr>
                                                            <tr>
                                                                <td class=""content-block"">{textEmail}.</td>
                                                            </tr>
                                                            <tr>
                                                                <td class=""content-block""><a href=""{linkButton}"" class=""btn-primary"">{nameButton}</a></td>
                                                            </tr>
                                                            <tr>
                                                                <td class=""content-block"">{thanksPhrase}</td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                            <div class=""footer"">
                                                <table width=""100%"">
                                                    <tr>
                                                        <td class=""aligncenter content-block reg""><a href=""https://{CommonConfigurationExtensions.MainDomain}"">{TimesExtensions.GetCurrentYearAsString()}&nbsp;&reg;&nbsp;{CommonConfigurationExtensions.MainDomain}</a></td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>
                                    </td>
                                    <td></td>
                                </tr>
                            </table>
                        </body>
                        </html>";
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем null
            return null;
        }
    }
}
