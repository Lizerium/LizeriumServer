/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 31 мая 2026 15:10:59
 * Version: 1.0.65
 */

using LizeriumLogging.Accessories.LoggingAccessories;
using System.Text;
using Timer = System.Timers.Timer;

namespace LizeriumLogging.Services.AppLoggingService.Implements;

/// <summary>
/// Реализация интерфейса логирования
/// </summary>
public class LoggingService : ILoggingService
{
    /// <summary>
    /// Имя директории хранения логов
    /// </summary>
    private static string NameLogDir => "Logging";

    /// <summary>
    /// Блокиратор
    /// </summary>
    private SemaphoreSlim Locker { get; }

    /// <summary>
    /// TextWriter логирования
    /// </summary>
    private TextWriter LoggingWriter { get; set; }

    /// <summary>
    /// Таймер изменения текущих файлов лога
    /// </summary>
    private Timer ChangeLogFilesTimer { get; set; }

    /// <summary>
    /// Конструктор
    /// </summary>
    public LoggingService()
    {
        Locker = new SemaphoreSlim(1);
    }

    /// <inheritdoc />
    /// <summary>
    /// Метод инициализирует сервис логирования
    /// </summary>
    /// <param name="nameProject">Название проекта</param>
    public void InitializeLogging(string nameProject)
    {
        try
        {
            //генерируем путь к директории логирования
            var logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Path.Combine(LoggingExtensions.AppDir, NameLogDir));

            //получаем информацию о директории логирования
            var logDirectoryInfo = new DirectoryInfo(logDirectory);

            //проверяем наличие директории
            if (!logDirectoryInfo.Exists)
            {
                //создаем директорию если ее нет
                logDirectoryInfo.Create();
            }

            //обходим все файлы в директории
            foreach (var fileInfo in logDirectoryInfo.GetFiles())
            {
                //если файлу менее трех дней, продолжаем цикл
                if ((DateTime.Now - fileInfo.CreationTime).TotalDays <= 3) continue;

                try
                {
                    //удаляем файл
                    fileInfo.Delete();
                }
                catch
                {
                    //Ignored
                }
            }

            //генерируем полный путь к файлу лога
            var fullPathLogExceptionFile = Path.GetFullPath($"{logDirectory}/log_{DateTime.Now:dd.MM.yy}.log");

            //проверяем наличие файлов логов
            if (!File.Exists(fullPathLogExceptionFile))
            {
                //создаем и закрываем файл логов
                File.Create(fullPathLogExceptionFile).Close();
            }

            //инициализируем TextWriter логирования
            LoggingWriter = TextWriter.Synchronized(new StreamWriter(fullPathLogExceptionFile, true, new UTF8Encoding(false)));

            //пишем сообщение о начале логирования в файл лога исключений
            LogMessageAsync($"START LOGGING {nameProject}...");

            //останавливаем таймер изменения текущих файлов лога
            ChangeLogFilesTimer?.Stop();

            //разрушаем таймер изменения текущих файлов лога
            ChangeLogFilesTimer?.Dispose();

            //запускаем новый таймер изменения текущих файлов лога
            ChangeLogFilesTimer = new Timer
            {
                Interval = 24 * 60 * 60 * 1000, //1 сутки
                AutoReset = true,
                Enabled = true
            };

            //действие по таймеру
            ChangeLogFilesTimer.Elapsed += (_, _) =>
            {
                //пишем строку об окончании логирования в текущем файле
                LogMessageAsync("STOP LOGGING...\n");

                //закрываем TextWriter логирования
                LoggingWriter?.Close();

                //уничтожаем TextWriter логирования
                LoggingWriter?.Dispose();

                //присваиваем null TextWriter логирования
                LoggingWriter = null;

                //заново инициализируем логирование
                InitializeLogging(nameProject);
            };
        }
        catch
        {
            //Ignore
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Метод деинициализирует сервис логирования
    /// </summary>
    public void DeinitializeLogging()
    {
        try
        {
            //пишем сообщение об окончании логирования в файл лога исключений
            LogMessageAsync("STOP LOGGING...\n");

            //останавливаем таймер изменения текущих файлов лога
            ChangeLogFilesTimer?.Stop();

            //разрушаем таймер изменения текущих файлов лога
            ChangeLogFilesTimer?.Dispose();

            //закрываем TextWriter логирования
            LoggingWriter?.Close();

            //уничтожаем TextWriter логирования
            LoggingWriter?.Dispose();

            //присваиваем null TextWriter логирования
            LoggingWriter = null;

            //уничтожаем блокиратор
            Locker?.Dispose();
        }
        catch
        {
            //Ignore
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Метод логирует сообщения
    /// </summary>
    /// <param name="textMessage">Текст сообщения</param>
    public async void LogMessageAsync(string textMessage)
    {
        //проверяем инициализацию логирования
        if (LoggingWriter == null) return;

        //блокируем поток
        await Locker.WaitAsync();

        try
        {
            //генерируем текст лога
            var textLog = $"{DateTime.Now:dd.MM.yy HH:mm:ss}: {textMessage}";

            //пишем строку лога
            await LoggingWriter.WriteLineAsync(textLog);

            //из памяти
            await LoggingWriter.FlushAsync();
        }
        finally
        {
            //освобождаем блокировку потока
            Locker.Release();
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Метод логирует исключения
    /// </summary>
    /// <param name="exception">Исключение</param>
    /// <param name="notice">Дополнительная метка для исключения</param>
    public async void LogExceptionAsync(Exception exception, string notice = null)
    {
        //проверяем инициализацию логирования и исключение
        if (LoggingWriter == null || exception == null) return;

        //блокируем поток
        await Locker.WaitAsync();

        try
        {
            //генерируем текст исключения
            var textException = $"{DateTime.Now:dd.MM.yy HH:mm:ss}: {notice ?? ""} {exception.Message}, ({exception.StackTrace})";

            //пишем строку лога
            await LoggingWriter.WriteLineAsync(textException);

            //из памяти
            await LoggingWriter.FlushAsync();
        }
        finally
        {
            //освобождаем блокировку потока
            Locker.Release();
        }
    }
}
