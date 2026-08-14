/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 14 августа 2026 08:37:41
 * Version: 1.0.145
 */

using LizeriumEmail.FormatsData.AppEmailData;
using LizeriumEmail.FormatsData.AppEnumsData;
using LizeriumEmail.FormatsData.AppSmtpData;
using LizeriumEmail.Services.AppEmailService;
using LizeriumEmail.Services.AppEmailService.Implements;
using LizeriumLogging.Accessories.LoggingAccessories;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;

namespace LizeriumEmail.Accessories.EmailAccessories;

/// <summary>
/// Класс вспомогательных методов для работы с Email
/// </summary>
public static class EmailExtensions
{
    /// <summary>
    /// Коллекция SMTP настроек системных Email
    /// </summary>
    private static ConcurrentDictionary<MailboxDestination, SmtpConfiguration> _systemEmailsData;

    /// <summary>
    /// Обертка коллекции SMTP настроек системных Email
    /// </summary>
    internal static ConcurrentDictionary<MailboxDestination, SmtpConfiguration> SystemEmailsData => _systemEmailsData ??= GetSystemEmailsConfiguration();

    /// <summary>
    /// Отображаемое имя в письме
    /// </summary>
    private static string _displaedName;

    /// <summary>
    /// Отображаемое имя в письме
    /// </summary>
    internal static string DisplayedName => _displaedName ??= Configuration["displayedName"];

    /// <summary>
    /// Данные конфигурации
    /// </summary>
    private static IConfigurationRoot _configuration;

    /// <summary>
    /// Обертка данных конфигурации
    /// </summary>
    internal static IConfigurationRoot Configuration => _configuration ??= GenerateConfiguration();

    /// <summary>
    /// Метод генерирует данные конфигурации
    /// </summary>
    /// <returns>Данные конфигурации</returns>
    private static IConfigurationRoot GenerateConfiguration()
    {
        try
        {
            //инициализируем строителя конфигурации
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"{LoggingExtensions.AppDir}/email_configuration.json");

            //строим и отдаем параметры конфигурации
            return builder.Build();
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
    /// Метод - расширение отправляет Email с логом администратору
    /// </summary>
    /// <param name="content">Содержимое письма</param>
    /// <returns></returns>
    public static async Task SendLogToAdminByEmailAsync(this string content)
    {
        try
        {
            //проверяем входящие данные
            if (string.IsNullOrEmpty(content)) return;

            //создаем экземпляр интерфейса отправки Email
            IEmailService emailService = new EmailService();

            //отправляем письмо администратору
            await emailService.SendEmailAsync(new EmailData
            {
                EmailType = TypeEmail.LogToAdmin,
                Recipient = Configuration["emailAdmin"],
                SubjectEmail = "Лог на lizerium",
                Message = content
            });
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();
        }
    }

    /// <summary>
    /// Метод генерирует SMTP настройки системных Email
    /// </summary>
    /// <returns>Коллекция SMTP настроек системных Email</returns>
    private static ConcurrentDictionary<MailboxDestination, SmtpConfiguration> GetSystemEmailsConfiguration()
    {
        try
        {
            //инициализируем коллекцию SMTP настроек системных Email
            var systemEmailsData = new ConcurrentDictionary<MailboxDestination, SmtpConfiguration>();

            //получаем секцию с настройками SMTP
            var smtpConfigurationSection = Configuration.GetSection("smtpSettings");

            //обходим дочерние объекты
            foreach (var section in smtpConfigurationSection.GetChildren())
            {
                //получаем предназначение почтового ящика
                var type = section.GetValue<string>("type");

                //создаем объект данных об SMTP настройках
                var smtpConfiguration = new SmtpConfiguration
                {
                    SmtpHost = section.GetValue<string>("smtpHost"),
                    SmtpPort = section.GetValue<int>("smtpPort"),
                    SmtpLogin = section.GetValue<string>("email"),
                    SmtpPassword = section.GetValue<string>("password")
                };

                //смотрим предназначение почтового ящика и кладем в коллекцию
                switch (type)
                {
                    case "transactional":
                        systemEmailsData.TryAdd(MailboxDestination.Transactional, smtpConfiguration);
                        break;
                    case "administrator":
                        systemEmailsData.TryAdd(MailboxDestination.Administrator, smtpConfiguration);
                        break;
                    case "notification":
                        systemEmailsData.TryAdd(MailboxDestination.Notification, smtpConfiguration);
                        break;
                    default:
                        continue;
                }
            }
            //отдаем коллекцию SMTP настроек системных Email
            return systemEmailsData;
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
