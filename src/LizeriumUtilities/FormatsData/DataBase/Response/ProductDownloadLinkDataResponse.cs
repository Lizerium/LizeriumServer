/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 13 августа 2026 07:11:18
 * Version: 1.0.144
 */

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LizeriumUtilities.FormatsData.DataBase.Response;

/// <summary>
/// Источник скачивания или внешняя площадка продукта.
/// </summary>
public class ProductDownloadLinkDataResponse
{
    [Key]
    [JsonPropertyName("Id")]
    public int Id { get; set; }

    [JsonPropertyName("ProductId")]
    public int ProductId { get; set; }

    [JsonIgnore]
    public ProductDataResponse Product { get; set; }

    [JsonPropertyName("NameRu")]
    public string NameRu { get; set; } = string.Empty;

    [JsonPropertyName("NameEn")]
    public string NameEn { get; set; } = string.Empty;

    [JsonPropertyName("Url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("IconUrl")]
    public string IconUrl { get; set; } = string.Empty;

    [JsonPropertyName("SortOrder")]
    public int SortOrder { get; set; }

    [JsonPropertyName("IsActive")]
    public bool IsActive { get; set; } = true;
}
