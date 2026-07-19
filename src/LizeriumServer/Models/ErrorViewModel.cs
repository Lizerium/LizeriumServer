/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 19 июля 2026 10:21:30
 * Version: 1.0.113
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
