/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 06 июля 2026 13:55:13
 * Version: 1.0.100
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
