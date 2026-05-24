/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 24 мая 2026 07:12:31
 * Version: 1.0.58
 */

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using LizeriumCrypt.Accessories;

namespace LizeriumUtilities.FormatsData.AppRequestData.Admin.AjaxData;

/// <summary>
/// Объект запроса подтверждения авторизации
/// </summary>
public class RequestConfirm
{
    /// <summary>
    /// Запись подтверждения авторизации
    /// </summary>
    [Required]
    [JsonPropertyName("confirmRecord")]
    public string ConfirmRecord { get; set; }

    /// <summary>
    /// Разовый код авторизации в числовом виде
    /// </summary>
    [JsonIgnore]
    public int OnceCode => ConfirmRecord.ParseInt32();
}