/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 27 мая 2026 13:44:09
 * Version: 1.0.61
 */

namespace Data;

/// <summary>
/// Объект данных о настройке конфигурации приложения
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Флаг режима запуска приложения
    /// </summary>
    public bool IsRelease { get; set; }

    /// <summary>
    /// Хост приложения
    /// </summary>
    public string AppHost { get; set; }

    /// <summary>
    /// Хост приложения для Release
    /// </summary>
    public string ReleaseHost { get; set; }

    /// <summary>
    /// Максимальный размер загружаемого файла в МБ
    /// </summary>
    public long MaxRequestBodySizeMb { get; set; }
}
