/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 10 мая 2026 08:03:40
 * Version: 1.0.45
 */

using Microsoft.AspNetCore.Antiforgery;
using LizeriumLogging.Accessories.LoggingAccessories;
using Microsoft.EntityFrameworkCore;
using LizeriumDatabase.Accessories.DataBaseAccessories;

namespace LizeriumUtilities.Middleware;

/// <summary>
/// Обработчик службы против подделки запросов
/// </summary>
public class MonitorMiddleware
{
    /// <summary>
    /// Делегат на передачу действия следующему в роутере
    /// </summary>
    private RequestDelegate Next { get; }

    /// <summary>
    /// API для настройки функций против подделки
    /// </summary>
    private IAntiforgery Antiforgery { get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="next">Делегат на передачу действия следующему в роутере</param>
    /// <param name="antiforgery">API для настройки функций против подделки</param>
    public MonitorMiddleware(RequestDelegate next, IAntiforgery antiforgery)
    {
        Next = next;
        Antiforgery = antiforgery;
    }

    /// <summary>
    /// Асинхронный обработчик запроса
    /// </summary>
    /// <param name="context">Контекст запроса</param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            //редиректим на страницу ошибки можно еще код ошибки отправлять get параметром
            var ip = context?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
            var agent = context?.Request?.Headers["User-Agent"].ToString();
            var url = context.Request.Path.ToString();
            var lang = context.Request.Headers["Accept-Language"].ToString();
            var date = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            
            var dataSecretRecords = DatabaseExtensions.Configuration.GetValue<string>("private_path"); ;
            // 1. Создание соединения (не регистрируя контекст)
            var path = dataSecretRecords;

            using (var db = new DbContext(new DbContextOptionsBuilder()
                .UseSqlite($"Data Source={path}").Options))
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Database.ExecuteSqlInterpolated($"INSERT INTO monitor (DateT, IP, LANG, AGENT, PATH) VALUES ({date}, {ip}, {lang}, {agent}, {url});");
                        transaction.Commit(); // Фиксация изменений
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback(); // Отмена изменений в случае ошибки
                    }
                }
            }
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();
        }
        finally
        {
            //передаем управление на следующий метод в роутере что бы был вывод/перенаправление ошибки 
            await Next.Invoke(context);
        }
    }
}

/*
CREATE TABLE monitor (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    DateT TEXT,                        
    IP TEXT,
    LANG TEXT,
    AGENT TEXT,
    PATH TEXT);

    INSERT INTO monitor (DateT, IP, LANG, AGENT) VALUES ('2023-10-26T15:32:00Z', '123.45.67.89', 'en-US', 'Mozilla/5.0');
 */