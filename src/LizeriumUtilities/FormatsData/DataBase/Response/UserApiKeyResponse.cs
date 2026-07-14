/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 14 июля 2026 07:17:23
 * Version: 1.0.108
 */

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LizeriumUtilities.FormatsData.DataBase.Response;

public class UserApiKeyResponse
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    [Key]
    public long IdUser { get; init; }

    /// <summary>
    /// Ключ API пользователя
    /// </summary>
    [JsonPropertyName("ApiKey")]
    public string ApiKey { get; init; }
}
