/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 26 мая 2026 12:03:42
 * Version: 1.0.60
 */

using LizeriumUtilities.FormatsData.DataBase.Response;
using System.Text.Json.Serialization;

namespace LizeriumUtilities.FormatsData.AppUserData;

/// <summary>
/// Объект данных о пользователях
/// </summary>
public class DataPosts
{
    /// <summary>
    /// Коллекция данных о пользователях
    /// </summary>
    [JsonPropertyName("posts")]
    public List<PostDataResponse> Posts { get; set; }

    /// <summary>
    /// Крайний идентификатор пользователя
    /// </summary>
    [JsonPropertyName("lastUserId")]
    public long LastUserId { get; set; }

    /// <summary>
    /// Максимальный идентификатор пользователя
    /// </summary>
    [JsonPropertyName("maxUserId")]
    public long MaxUserId { get; set; }
}
