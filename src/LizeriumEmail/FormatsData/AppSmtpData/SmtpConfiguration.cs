/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 29 июня 2026 07:16:51
 * Version: 1.0.94
 */

namespace LizeriumEmail.FormatsData.AppSmtpData;

/// <summary>
/// Объект данных SMTP настроек для Email 
/// </summary>
internal class SmtpConfiguration
{
    /// <summary>
    /// SMTP хост
    /// </summary>
    public string SmtpHost { get; init; }

    /// <summary>
    /// SMTP порт
    /// </summary>
    public int SmtpPort { get; init; }

    /// <summary>
    /// SMTP логин
    /// </summary>
    public string SmtpLogin { get; init; }

    /// <summary>
    /// SMTP пароль
    /// </summary>
    public string SmtpPassword { get; init; }
}
