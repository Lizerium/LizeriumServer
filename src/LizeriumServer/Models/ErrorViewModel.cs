/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 21 мая 2026 11:58:33
 * Version: 1.0.55
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
