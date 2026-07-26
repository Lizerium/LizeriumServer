/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 26 июля 2026 15:32:52
 * Version: 1.0.120
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
