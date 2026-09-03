/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 03 сентября 2026 07:38:14
 * Version: 1.0.165
 */

using System.Text.Json.Serialization;

namespace LizeriumUtilities.FormatsData.AppResponseData.BadResponses;

/// <summary>
/// Объект данных о причине неудачного запроса
/// </summary>
public class FailedRequestReason
{
    /// <summary>
    /// Причина неудачного запроса
    /// </summary>
    [JsonPropertyName("reason")]
    public string Reason { get; init; }
}