/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 15 июля 2026 12:14:10
 * Version: 1.0.109
 */

namespace LizeriumLogging.Services.AppLoggingService;

/// <summary>
/// Интерфейс логирования
/// </summary>
public interface ILoggingService
{
    /// <summary>
    /// Метод инициализирует сервис логирования
    /// </summary>
    /// <param name="nameProject">Название проекта</param>
    void InitializeLogging(string nameProject);

    /// <summary>
    /// Метод деинициализирует сервис логирования
    /// </summary>
    void DeinitializeLogging();

    /// <summary>
    /// Метод логирует сообщения
    /// </summary>
    /// <param name="textMessage">Текст сообщения</param>
    void LogMessageAsync(string textMessage);

    /// <summary>
    /// Метод логирует исключения
    /// </summary>
    /// <param name="exception">Исключение</param>
    /// <param name="notice">Дополнительная метка для исключения</param>
    void LogExceptionAsync(Exception exception, string notice = null);
}
