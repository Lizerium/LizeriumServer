/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 15 августа 2026 07:14:28
 * Version: 1.0.146
 */

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LizeriumUtilities.FormatsData.DataBase.Response;

/// <summary>
/// Новостной пост для страницы Lizerium Steam.
/// </summary>
public class LauncherNewsDataResponse
{
    /// <summary>
    /// Идентификатор новости.
    /// </summary>
    [Key]
    [JsonPropertyName("Id")]
    public int Id { get; set; }

    /// <summary>
    /// Русский заголовок.
    /// </summary>
    [JsonPropertyName("TitleRu")]
    public string TitleRu { get; set; }

    /// <summary>
    /// Английский заголовок.
    /// </summary>
    [JsonPropertyName("TitleEn")]
    public string TitleEn { get; set; }

    /// <summary>
    /// Русский Markdown-текст.
    /// </summary>
    [JsonPropertyName("MarkdownRu")]
    public string MarkdownRu { get; set; }

    /// <summary>
    /// Английский Markdown-текст.
    /// </summary>
    [JsonPropertyName("MarkdownEn")]
    public string MarkdownEn { get; set; }

    /// <summary>
    /// Ссылка на YouTube-видео.
    /// </summary>
    [JsonPropertyName("YoutubeUrl")]
    public string YoutubeUrl { get; set; }

    /// <summary>
    /// Ссылка на Rutube-видео.
    /// </summary>
    [JsonPropertyName("RutubeUrl")]
    public string RutubeUrl { get; set; }

    /// <summary>
    /// VK video URL.
    /// </summary>
    [JsonPropertyName("VkVideoUrl")]
    public string VkVideoUrl { get; set; }

    /// <summary>
    /// Ссылка на изображение.
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
    /// Ссылка на GitHub-проект, связанный с новостью.
    /// </summary>
    [JsonPropertyName("GithubUrl")]
    public string GithubUrl { get; set; }

    /// <summary>
    /// Отображаемое название GitHub-проекта.
    /// </summary>
    [JsonPropertyName("GithubProjectName")]
    public string GithubProjectName { get; set; }

    /// <summary>
    /// Опубликована ли новость.
    /// </summary>
    [JsonPropertyName("IsPublished")]
    public bool IsPublished { get; set; }

    /// <summary>
    /// Порядок вывода. Чем меньше число, тем выше карточка.
    /// </summary>
    [JsonPropertyName("SortOrder")]
    public int SortOrder { get; set; }

    /// <summary>
    /// Unix-время публикации.
    /// </summary>
    [JsonPropertyName("PublishedAtUnix")]
    public long PublishedAtUnix { get; set; }
}
