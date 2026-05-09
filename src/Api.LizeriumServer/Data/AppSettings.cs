/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 09 мая 2026 08:13:02
 * Version: 1.0.44
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
