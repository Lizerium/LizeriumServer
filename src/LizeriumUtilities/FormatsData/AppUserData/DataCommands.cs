/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 17 апреля 2026 07:02:31
 * Version: 1.0.22
 */

using LizeriumUtilities.FormatsData.DataBase.Response;
using System.Text.Json.Serialization;

namespace LizeriumUtilities.FormatsData.AppUserData;

/// <summary>
/// Команды
/// </summary>
public class DataCommands
{
    /// <summary>
    /// Коллекция данных
    /// </summary>
    [JsonPropertyName("commands")]
    public List<CommandDataResponse> Commands { get; set; }

    /// <summary>
    /// Коллекция данных
    /// </summary>
    [JsonPropertyName("commandTranslations")]
    public List<AdminCommandWithTranslations> CommandTranslations { get; set; } = new List<AdminCommandWithTranslations>();

    /// <summary>
    /// Коллекция категорий
    /// </summary>
    [JsonPropertyName("categories")]
    public List<string> Categories => GetCategories();

    public List<string> GetCategories()
    {
        var unique = Commands.Select(p => p.Category);
        var dist = unique.Distinct().ToList();

        return dist;
    }

    /// <summary>
    /// Крайний идентификатор
    /// </summary>
    [JsonPropertyName("lastId")]
    public long LastId { get; set; }

    /// <summary>
    /// Максимальный идентификатор
    /// </summary>
    [JsonPropertyName("maxId")]
    public long MaxId { get; set; }
}
