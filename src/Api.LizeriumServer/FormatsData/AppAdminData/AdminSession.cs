/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 25 октября 2026 09:09:46
 * Version: 1.0.216
 */

using System.Text.Json.Serialization;

namespace Api.LizeriumServer.FormatsData.AppAdminData;

/// <summary>
/// Объект данных сессии администратора
/// </summary>
public class AdminSession
{
    /// <summary>
    /// Email администратора
    /// </summary>
    [JsonPropertyName("emailAdmin")]
    public string EmailAdmin { get; init; }

    /// <summary>
    /// Флаг что отправлен разовый код подтверждения
    /// </summary>
    [JsonPropertyName("sentOnceCode")]
    public bool SentOnceCode { get; init; }

    /// <summary>
    /// Разовый код авторизации
    /// </summary>
    [JsonPropertyName("onceCode")]
    public int OnceCode { get; init; }

    /// <summary>
    /// Флаг что разовый код подтвержден
    /// </summary>
    [JsonPropertyName("isConfirmed")]
    public bool IsConfirmed { get; set; }

    /// <summary>
    /// Флаг что администратор авторизован
    /// </summary>
    public bool IsAuth => !string.IsNullOrEmpty(EmailAdmin) && SentOnceCode && OnceCode > 0 && IsConfirmed;
}