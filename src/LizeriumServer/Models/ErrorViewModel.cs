/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 11 июня 2026 13:47:38
 * Version: 1.0.76
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
