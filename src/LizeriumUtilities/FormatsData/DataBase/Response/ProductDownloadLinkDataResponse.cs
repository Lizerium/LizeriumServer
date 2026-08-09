/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 09 августа 2026 15:52:37
 * Version: 1.0.135
 */

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LizeriumUtilities.FormatsData.DataBase.Response;

/// <summary>
/// РСЃС‚РѕС‡РЅРёРє СЃРєР°С‡РёРІР°РЅРёСЏ РёР»Рё РІРЅРµС€РЅСЏСЏ РїР»РѕС‰Р°РґРєР° РїСЂРѕРґСѓРєС‚Р°.
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
