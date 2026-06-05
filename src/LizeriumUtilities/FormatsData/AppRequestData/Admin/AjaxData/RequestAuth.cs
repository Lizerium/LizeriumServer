/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 05 июня 2026 07:12:29
 * Version: 1.0.70
 */

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LizeriumUtilities.FormatsData.AppRequestData.Admin.AjaxData;

/// <summary>
/// Объект запроса авторизации
/// </summary>
public class RequestAuth
{
    /// <summary>
    /// Секретный ключ авторизации
    /// </summary>
    [Required]
    [JsonPropertyName("secretKey")]
    public string SecretKey { get; set; }
}