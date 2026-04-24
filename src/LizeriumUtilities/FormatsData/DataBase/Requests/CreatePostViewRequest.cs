/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 24 апреля 2026 07:11:28
 * Version: 1.0.29
 */

namespace LizeriumUtilities.FormatsData.DataBase.Requests;

/// <summary>
/// Модель создания поста на сервере
/// </summary>
public class CreatePostViewRequest
{
    /// <summary>
    /// Дата
    /// </summary>
    public long DateTimeUnix => (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    /// <summary>
    /// Автор
    /// </summary>
    public string Autor { get; set; }
    /// <summary>
    /// Сообщение пользователя
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// Статус рассмотрения
    /// Новое 1
    /// Прочитано 2
    /// В работе 3
    /// Отказано 4
    /// Выполнено 5 
    /// </summary>
    public int Status { get; set; }
}