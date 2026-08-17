/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 17 августа 2026 07:12:05
 * Version: 1.0.148
 */

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LizeriumUtilities.FormatsData.DataBase.Response;

/// <summary>
/// Продукт, доступный в игровом разделе портала.
/// </summary>
public class ProductDataResponse
{
    [Key]
    [JsonPropertyName("Id")]
    public int Id { get; set; }

    [JsonPropertyName("ProductCategoryId")]
    public int ProductCategoryId { get; set; }

    [JsonIgnore]
    public ProductCategoryDataResponse ProductCategory { get; set; }

    [JsonPropertyName("TitleRu")]
    public string TitleRu { get; set; } = string.Empty;

    [JsonPropertyName("TitleEn")]
    public string TitleEn { get; set; } = string.Empty;

    [JsonPropertyName("DescriptionRu")]
    public string DescriptionRu { get; set; } = string.Empty;

    [JsonPropertyName("DescriptionEn")]
    public string DescriptionEn { get; set; } = string.Empty;

    [JsonPropertyName("IconUrl")]
    public string IconUrl { get; set; } = string.Empty;

    [JsonPropertyName("SortOrder")]
    public int SortOrder { get; set; }

    [JsonPropertyName("IsActive")]
    public bool IsActive { get; set; } = true;

    [JsonPropertyName("DownloadLinks")]
    public List<ProductDownloadLinkDataResponse> DownloadLinks { get; set; } = new();
}
