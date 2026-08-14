/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 14 августа 2026 08:37:41
 * Version: 1.0.145
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
