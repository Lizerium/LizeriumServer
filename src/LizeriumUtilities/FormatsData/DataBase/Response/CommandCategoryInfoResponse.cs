/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 04 мая 2026 07:13:32
 * Version: 1.0.39
 */

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LizeriumUtilities.FormatsData.DataBase.Response;

public class CommandCategoryInfoResponse
{
    /// <summary>
    /// Указание первичного ключа
    /// </summary>
    [Key]
    [JsonPropertyName("Id")]
    public int Id { get; set; }
    /// <summary>
    /// Уникальный идентификатор категории
    /// </summary>
    [JsonPropertyName("Key")]
    public string Key { get; set; } = null!;
    /// <summary>
    /// Английское название (может быть null)
    /// </summary>
    [JsonPropertyName("NameEn")]
    public string? NameEn { get; set; }           
    /// <summary>
    /// Русское название (может быть null)
    /// </summary>
    [JsonPropertyName("NameRu")]
    public string? NameRu { get; set; }
    /// <summary>
    /// Версия плагина
    /// </summary>
    [JsonPropertyName("Version")]
    public string Version { get; set; }
    /// <summary>
    /// Адрес до исходного кода
    /// </summary>
    [JsonPropertyName("Repository")]
    public string Repository { get; set; }
}