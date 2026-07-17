/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 17 июля 2026 11:21:19
 * Version: 1.0.111
 */

using LizeriumLogging.Services.AppLoggingService;
using LizeriumLogging.Services.AppLoggingService.Implements;
using System.Reflection;

namespace LizeriumLogging.Accessories.LoggingAccessories;

/// <summary>
/// Вспомогательные методы логирования
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Полный путь к рабочей директории приложения
    /// </summary>
    private static string _appDir;

    /// <summary>
    /// Обертка полного пути к рабочей директории приложения
    /// </summary>
    public static string AppDir => _appDir ??= Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

    /// <summary>
    /// Интерфейс логирования
    /// </summary>
    private static ILoggingService _logging;

    /// <summary>
    /// Обертка интерфейса логирования
    /// </summary>
    public static ILoggingService Logging => _logging ??= new LoggingService();

    /// <summary>
    /// Метод - расширение логирует исключение
    /// </summary>
    /// <param name="exception">Исключение</param>
    /// <param name="notice">Дополнительная метка для исключения</param>
    public static void LogException(this Exception exception, string notice = null)
    {
        try
        {
            //логируем исключение
            Logging?.LogExceptionAsync(exception, notice);
        }
        catch
        {
            //Ignore
        }
    }

    /// <summary>
    /// Метод - расширение логирует сообщение
    /// </summary>
    /// <param name="textMessage">Текст сообщения</param>
    public static void LogMessage(this string textMessage)
    {
        try
        {
            //логируем сообщение
            Logging?.LogMessageAsync(textMessage);
        }
        catch
        {
            //Ignore
        }
    }
}
