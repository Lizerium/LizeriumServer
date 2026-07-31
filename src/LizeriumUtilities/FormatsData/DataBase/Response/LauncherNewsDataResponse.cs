/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 31 июля 2026 16:48:21
 * Version: 1.0.127
 */

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LizeriumUtilities.FormatsData.DataBase.Response;

/// <summary>
/// РќРѕРІРѕСЃС‚РЅРѕР№ РїРѕСЃС‚ РґР»СЏ СЃС‚СЂР°РЅРёС†С‹ Lizerium Launcher.
/// </summary>
public class LauncherNewsDataResponse
{
    /// <summary>
    /// РРґРµРЅС‚РёС„РёРєР°С‚РѕСЂ РЅРѕРІРѕСЃС‚Рё.
    /// </summary>
    [Key]
    [JsonPropertyName("Id")]
    public int Id { get; set; }

    /// <summary>
    /// Р СѓСЃСЃРєРёР№ Р·Р°РіРѕР»РѕРІРѕРє.
    /// </summary>
    [JsonPropertyName("TitleRu")]
    public string TitleRu { get; set; }

    /// <summary>
    /// РђРЅРіР»РёР№СЃРєРёР№ Р·Р°РіРѕР»РѕРІРѕРє.
    /// </summary>
    [JsonPropertyName("TitleEn")]
    public string TitleEn { get; set; }

    /// <summary>
    /// Р СѓСЃСЃРєРёР№ Markdown-С‚РµРєСЃС‚.
    /// </summary>
    [JsonPropertyName("MarkdownRu")]
    public string MarkdownRu { get; set; }

    /// <summary>
    /// РђРЅРіР»РёР№СЃРєРёР№ Markdown-С‚РµРєСЃС‚.
    /// </summary>
    [JsonPropertyName("MarkdownEn")]
    public string MarkdownEn { get; set; }

    /// <summary>
    /// РЎСЃС‹Р»РєР° РЅР° YouTube-РІРёРґРµРѕ.
    /// </summary>
    [JsonPropertyName("YoutubeUrl")]
    public string YoutubeUrl { get; set; }

    /// <summary>
    /// РЎСЃС‹Р»РєР° РЅР° Rutube-РІРёРґРµРѕ.
    /// </summary>
    [JsonPropertyName("RutubeUrl")]
    public string RutubeUrl { get; set; }

    /// <summary>
    /// VK video URL.
    /// </summary>
    [JsonPropertyName("VkVideoUrl")]
    public string VkVideoUrl { get; set; }

    /// <summary>
    /// РЎСЃС‹Р»РєР° РЅР° РёР·РѕР±СЂР°Р¶РµРЅРёРµ.
    /// </summary>
    [JsonPropertyName("ImageUrl")]
    public string ImageUrl { get; set; }

    /// <summary>
    /// JSON array with additional news image URLs.
    /// </summary>
    [JsonPropertyName("ImageGalleryJson")]
    public string ImageGalleryJson { get; set; }

    /// <summary>
    /// News type label shown in the full reader.
    /// </summary>
    [JsonPropertyName("NewsType")]
    public string NewsType { get; set; }

    /// <summary>
    /// Russian news type label shown in the full reader.
    /// </summary>
    [JsonPropertyName("NewsTypeRu")]
    public string NewsTypeRu { get; set; }

    /// <summary>
    /// English news type label shown in the full reader.
    /// </summary>
    [JsonPropertyName("NewsTypeEn")]
    public string NewsTypeEn { get; set; }

    /// <summary>
    /// Optional product icon URL for this news item.
    /// </summary>
    [JsonPropertyName("IconUrl")]
    public string IconUrl { get; set; }

    /// <summary>
    /// Lightweight public like counter.
    /// </summary>
    [JsonPropertyName("LikeCount")]
    public int LikeCount { get; set; }

    /// <summary>
    /// РЎСЃС‹Р»РєР° РЅР° GitHub-РїСЂРѕРµРєС‚, СЃРІСЏР·Р°РЅРЅС‹Р№ СЃ РЅРѕРІРѕСЃС‚СЊСЋ.
    /// </summary>
    [JsonPropertyName("GithubUrl")]
    public string GithubUrl { get; set; }

    /// <summary>
    /// РћС‚РѕР±СЂР°Р¶Р°РµРјРѕРµ РЅР°Р·РІР°РЅРёРµ GitHub-РїСЂРѕРµРєС‚Р°.
    /// </summary>
    [JsonPropertyName("GithubProjectName")]
    public string GithubProjectName { get; set; }

    /// <summary>
    /// РћРїСѓР±Р»РёРєРѕРІР°РЅР° Р»Рё РЅРѕРІРѕСЃС‚СЊ.
    /// </summary>
    [JsonPropertyName("IsPublished")]
    public bool IsPublished { get; set; }

    /// <summary>
    /// РџРѕСЂСЏРґРѕРє РІС‹РІРѕРґР°. Р§РµРј РјРµРЅСЊС€Рµ С‡РёСЃР»Рѕ, С‚РµРј РІС‹С€Рµ РєР°СЂС‚РѕС‡РєР°.
    /// </summary>
    [JsonPropertyName("SortOrder")]
    public int SortOrder { get; set; }

    /// <summary>
    /// Unix-РІСЂРµРјСЏ РїСѓР±Р»РёРєР°С†РёРё.
    /// </summary>
    [JsonPropertyName("PublishedAtUnix")]
    public long PublishedAtUnix { get; set; }
}
