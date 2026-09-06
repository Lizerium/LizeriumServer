/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 06 сентября 2026 11:13:26
 * Version: 1.0.168
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
    private const int MaxLogLines = 100_000;
    private const int TrimmedLogLines = 50_000;

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
    /// Путь к текущему файлу логирования
    /// </summary>
    private string CurrentLogFilePath { get; set; }

    /// <summary>
    /// Количество строк в текущем файле логирования
    /// </summary>
    private int CurrentLogLineCount { get; set; }

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
            var logDirectory = Path.Combine(LoggingExtensions.AppDir, NameLogDir);

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
            CurrentLogFilePath = fullPathLogExceptionFile;

            //проверяем наличие файлов логов
            if (!File.Exists(fullPathLogExceptionFile))
            {
                //создаем и закрываем файл логов
                File.Create(fullPathLogExceptionFile).Close();
            }

            TrimLogFileIfNeeded(fullPathLogExceptionFile);
            CurrentLogLineCount = File.ReadLines(fullPathLogExceptionFile).Count();

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
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Logging initialization failed: {exception}");
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
            CurrentLogFilePath = null;
            CurrentLogLineCount = 0;

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
            CurrentLogLineCount++;

            //из памяти
            await LoggingWriter.FlushAsync();

            TrimCurrentLogFileIfNeeded();
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
            CurrentLogLineCount++;

            //из памяти
            await LoggingWriter.FlushAsync();

            TrimCurrentLogFileIfNeeded();
        }
        finally
        {
            //освобождаем блокировку потока
            Locker.Release();
        }
    }

    private void TrimCurrentLogFileIfNeeded()
    {
        if (CurrentLogLineCount <= MaxLogLines || string.IsNullOrEmpty(CurrentLogFilePath))
            return;

        LoggingWriter?.Close();
        LoggingWriter?.Dispose();
        LoggingWriter = null;

        TrimLogFileIfNeeded(CurrentLogFilePath);
        CurrentLogLineCount = File.ReadLines(CurrentLogFilePath).Count();
        LoggingWriter = TextWriter.Synchronized(new StreamWriter(CurrentLogFilePath, true, new UTF8Encoding(false)));
    }

    private static void TrimLogFileIfNeeded(string filePath)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return;

            var lines = File.ReadLines(filePath).ToList();
            if (lines.Count <= MaxLogLines)
                return;

            var trimmedLines = lines.Skip(Math.Max(0, lines.Count - TrimmedLogLines)).ToList();
            trimmedLines.Insert(0, $"{DateTime.Now:dd.MM.yy HH:mm:ss}: LOG FILE TRIMMED. Kept last {TrimmedLogLines} lines from {lines.Count}.");
            File.WriteAllLines(filePath, trimmedLines, new UTF8Encoding(false));
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Log trimming failed: {exception}");
        }
    }
}
