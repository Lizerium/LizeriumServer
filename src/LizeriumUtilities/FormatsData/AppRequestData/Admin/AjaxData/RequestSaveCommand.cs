/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 22 июня 2026 07:13:51
 * Version: 1.0.87
 */

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LizeriumUtilities.FormatsData.AppRequestData.Admin.AjaxData;

/// <summary>
/// Объект данных для создания команды
/// </summary>
public class RequestSaveCommand
{
    /// <summary>
    /// ID
    /// </summary>
    [JsonPropertyName("Id")]
    public int Id { get; set; }

    /// <summary>
    /// Категория например MARK
    /// </summary>
    [JsonPropertyName("newCategory")]
    public string Category { get; set; }

    /// <summary>
    /// Список комманд категории
    /// </summary>
    [NotMapped]
    [JsonPropertyName("newName")]
    public string CommandNames { get; set; }

    /// <summary>
    /// Пример ввода
    /// </summary>
    [JsonPropertyName("newExampleInput")]
    public string ExampleInput { get; set; }

    /// <summary>
    /// Описание команды
    /// </summary>
    [JsonPropertyName("newDescription")]
    public string Description { get; set; }

    /// <summary>
    /// GIF источник примера ввода команды
    /// </summary>
    [JsonPropertyName("newGif")]
    public string UrlGif { get; set; }

    /// <summary>
    /// Количество откликов
    /// </summary>
    [JsonPropertyName("newLikes")]
    public int CountLike { get; set; }

    /// <summary>
    /// Статус команды на сервере
    /// 1 - Активна
    /// 2 - Выключена
    /// 3 - В разработке
    /// </summary>
    [JsonPropertyName("newStatus")]
    public int Status { get; set; }
}
