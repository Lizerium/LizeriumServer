/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 23 сентября 2026 09:46:26
 * Version: 1.0.185
 */

namespace LizeriumServer.Models;

/// <summary>
/// Модель с ошибкой
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Идентификатор запроса
    /// </summary>
    public string RequestId { get; set; }

    /// <summary>
    /// Статус показа идентификатора запроса
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
