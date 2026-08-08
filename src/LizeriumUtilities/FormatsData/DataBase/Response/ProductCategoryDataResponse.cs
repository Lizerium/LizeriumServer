/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 08 августа 2026 07:13:54
 * Version: 1.0.134
 */

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LizeriumUtilities.FormatsData.DataBase.Response;

/// <summary>
/// РљР°С‚РµРіРѕСЂРёСЏ РїСЂРѕРґСѓРєС‚Р° РґР»СЏ РІРёС‚СЂРёРЅС‹ РёРіСЂ Рё Р·Р°РіСЂСѓР·РѕРє.
/// </summary>
public class ProductCategoryDataResponse
{
    [Key]
    [JsonPropertyName("Id")]
    public int Id { get; set; }

    [JsonPropertyName("Key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("NameRu")]
    public string NameRu { get; set; } = string.Empty;

    [JsonPropertyName("NameEn")]
    public string NameEn { get; set; } = string.Empty;

    [JsonPropertyName("DescriptionRu")]
    public string DescriptionRu { get; set; } = string.Empty;

    [JsonPropertyName("DescriptionEn")]
    public string DescriptionEn { get; set; } = string.Empty;

    [JsonPropertyName("IconUrl")]
    public string IconUrl { get; set; } = string.Empty;

    [JsonPropertyName("BackgroundUrl")]
    public string BackgroundUrl { get; set; } = string.Empty;

    [JsonPropertyName("SortOrder")]
    public int SortOrder { get; set; }

    [JsonPropertyName("IsActive")]
    public bool IsActive { get; set; } = true;

    [JsonPropertyName("Products")]
    public List<ProductDataResponse> Products { get; set; } = new();
}
