/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 11 июня 2026 13:47:38
 * Version: 1.0.76
 */

namespace LizeriumUtilities.FormatsData.DataBase.Requests;

/// <summary>
/// Создание команды
/// </summary>
public class CreateCommandViewRequest
{
    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Категория например MARK
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// Список комманд категории
    /// </summary>
    public string CommandNames { get; set; }

    /// <summary>
    /// Пример ввода
    /// </summary>
    public string ExampleInput { get; set; }

    /// <summary>
    /// Описание команды
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// GIF источник примера ввода команды
    /// </summary>
    public string UrlGif { get; set; }

    /// <summary>
    /// Количество откликов
    /// </summary>
    public int CountLike { get; set; }

    /// <summary>
    /// Статус команды на сервере
    /// 1 - Активна
    /// 2 - Выключена
    /// 3 - В разработке
    /// </summary>
    public int Status { get; set; }
}