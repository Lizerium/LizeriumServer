/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 29 мая 2026 14:15:42
 * Version: 1.0.63
 */

using System;
using System.Data.SQLite;

using Api.LizeriumServer.Accessories.AuthExtensions;
using Api.LizeriumServer.FormatsData.AppAdminData;
using Api.LizeriumServer.FormatsData.Stats;
using Api.LizeriumServer.Models;

using LizeriumDatabase.Accessories.DataBaseAccessories;
using LizeriumDatabase.Services.AppDataBaseService;

using LizeriumLogging.Accessories.LoggingAccessories;

using LizeriumNetSecurity.Services.SecurityService;

using LizeriumUtilities.Accessories.SessionAccessories;
using LizeriumUtilities.FormatsData.DataBase.Response;

using Microsoft.AspNetCore.Mvc;

namespace Api.LizeriumServer.Controllers;

/// <summary>
/// Дефолтный контроллер MVC
/// </summary>
[Route("[action]")]
public class HomeController : Controller
{
    private IDataBaseService appDb { get; set; }
    private IAppSecurityService securityService { get; set; }

    public HomeController(IDataBaseService dataBaseService, IAppSecurityService appSecurityService)
    {
        appDb = dataBaseService;
        securityService = appSecurityService;
    }

    /// <summary>
    /// Главная страница
    /// </summary>
    /// <returns>Результат действия</returns>
    [HttpGet]
    [Route("/")]
    public async Task<IActionResult> Index()
    {
        //проверяем блокировку
        var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
        if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
            return StatusCode(403);

        //получаем объект сессии администратора
        var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");
        //если объект сессии администратора не задан или не авторизован, отдаем страницу авторизации
        if (adminSession is not { IsAuth: true }) 
            return View(new MainModel(null, null) { ShowLeftSide = false });

        return RedirectPermanent("~/cabinet");
    }

    /// <summary>
    /// Страница подтверждения авторизации
    /// </summary>
    /// <returns>Результат действия</returns>
    [HttpGet]
    public async Task<IActionResult> Confirmation()
    {
        try
        {
            //проверяем блокировку
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
                return StatusCode(403);

            //получаем объект сессии администратора
            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");

            //если объект сессии администратора не задан или разовый код не выслан 
            if (adminSession is not { SentOnceCode: true })
            {
                //редиректим на главную страницу
                return RedirectPermanent("~/");
            }

            //если сессия задана, но администратор не авторизован
            if (!adminSession.IsAuth)
            {
                //отдаем страницу подтверждения авторизации разовым кодом
                return View(new MainModel(null, null) { ShowLeftSide = false });
            }

            //редиректим на страницу кабинета администратора
            return RedirectPermanent("~/cabinet");
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем 404 ошибку
            return StatusCode(404);
        }
    }

    [HttpGet]
    [Route("/block/{IP}")]
    public async Task<IActionResult> Block(string IP)
    {
        //проверяем блокировку
        var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
        if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
            return StatusCode(403);

        await securityService.AddIpAsync(IP);
        return Redirect($"~/cabinet");
    }

    [HttpGet]
    [Route("/unblock/{IP}")]
    public async Task<IActionResult> UnBlock(string IP)
    {
        //проверяем блокировку
        var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
        if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
            return StatusCode(403);

        await securityService.RemoveIpAsync(IP);
        return Redirect($"~/cabinet");
    }

    [HttpGet]
    [Route("/stats/{IP}/info")]
    public async Task<IActionResult> Stats(string IP)
    {
        try
        {
            //проверяем блокировку
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
                return StatusCode(403);

            //получаем объект сессии администратора
            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");

            //если сессии нет или администратор не авторизован редиректим на главную страницу
            if (adminSession is not { IsAuth: true }) return RedirectPermanent("~/");

            int countPerDay = 0;
            List<MonitorData> MonitorD = new List<MonitorData>();
            var dataSecretRecords = DatabaseExtensions.Configuration.GetValue<string>("private_path");
            try
            {
                // Строка  соединения  с  базой  данных  SQLite
                string connectionString = $"Data Source={dataSecretRecords}";
                // Создать  соединение
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    // Открыть  соединение
                    connection.Open();

                    // Создать  запрос
                    SQLiteCommand command = new SQLiteCommand("SELECT Id, MAX(DateT) AS LatestDate, IP, LANG, AGENT, PATH, COUNT(*) FROM monitor WHERE DateT >= @Yesterday AND IP == @ip GROUP BY PATH ORDER BY LatestDate DESC LIMIT 1000", connection);
                    // Текущее время
                    DateTime now = DateTime.Now;
                    // Время 24 часа назад
                    DateTime yesterday = now.AddHours(-24);
                    // Добавить параметр для времени 24 часа назад
                    command.Parameters.AddWithValue("@Yesterday", yesterday.ToString("MM/dd/yyyy HH:mm:ss"));
                    command.Parameters.AddWithValue("@ip", IP);

                    // Выполнить  запрос
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        // Прочитать  данные  по  строкам
                        while (reader.Read())
                        {
                            // Извлечь  значения  из  строки
                            int id = reader.GetInt32(0);
                            string date = reader.GetString(1);
                            string IPU = reader.GetString(2);
                            string Lang = reader.GetString(3);
                            string Agent = reader.GetString(4);
                            string Path = reader.GetString(5);
                            int Count = reader.GetInt32(6);

                            // Создать  объект  MonitorData  и  добавить  его  в  список
                            MonitorD.Add(new MonitorData
                            {
                                Id = id,
                                IP = IPU,
                                Date = date,
                                Lang = Lang,
                                Agent = Agent,
                                Path = Path,
                                Count = Count
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            //отдаем страницу панели администратора
            return View(new MainModel(null, null)
            {
                ShowLeftSide = true,
                UserDataStats = MonitorD
            });
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем 404 ошибку
            return StatusCode(404);
        }
    }

    /// <summary>
    /// Страница кабинета администратора
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Cabinet()
    {
        try
        {
            // IP текущего клиента
            var currentIp = HttpContext?.Connection?.RemoteIpAddress?.ToString();

            // Проверяем блокировку IP
            if (!string.IsNullOrWhiteSpace(currentIp) && await securityService.IsBlocked(currentIp))
                return StatusCode(403);

            // Получаем объект сессии администратора
            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");

            // Если сессии нет или администратор не авторизован — редирект на главную
            if (adminSession is not { IsAuth: true })
                return Redirect("~/");

            var monitorDataList = new List<MonitorData>();
            int countPerDay = 0;

            var databasePath = DatabaseExtensions.Configuration.GetValue<string>("private_path");

            if (string.IsNullOrWhiteSpace(databasePath) || !System.IO.File.Exists(databasePath))
            {
                $"Файл базы данных не найден или путь пустой. private_path: {databasePath}".LogMessage();

                return View(new MainModel(null, null)
                {
                    ShowLeftSide = true,
                    MonitorData = monitorDataList,
                    AllUsersPerDay = countPerDay
                });
            }

            var connectionString = $"Data Source={databasePath};Version=3;";

            using var connection = new SQLiteConnection(connectionString);
            await connection.OpenAsync();

            // ----------------------------
            // 1. Получаем последние 100 IP
            // ----------------------------
            const string latestIpsQuery = @"
                SELECT 
                    m.Id,
                    m.DateT,
                    m.IP,
                    m.LANG,
                    m.AGENT,
                    m.PATH,
                    stats.TotalCount
                FROM monitor m
                INNER JOIN (
                    SELECT IP, MAX(DateT) AS LatestDate, COUNT(*) AS TotalCount
                    FROM monitor
                    GROUP BY IP
                ) stats
                    ON m.IP = stats.IP AND m.DateT = stats.LatestDate
                ORDER BY m.DateT DESC
                LIMIT 100;";

            using (var command = new SQLiteCommand(latestIpsQuery, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0;
                    var date = reader["DateT"]?.ToString() ?? string.Empty;
                    var ip = reader["IP"]?.ToString() ?? string.Empty;
                    var lang = reader["LANG"]?.ToString() ?? string.Empty;
                    var agent = reader["AGENT"]?.ToString() ?? string.Empty;
                    var path = reader["PATH"]?.ToString() ?? string.Empty;
                    var count = reader["TotalCount"] != DBNull.Value ? Convert.ToInt32(reader["TotalCount"]) : 0;

                    monitorDataList.Add(new MonitorData
                    {
                        Id = id,
                        IP = ip,
                        Date = date,
                        Lang = lang,
                        Agent = agent,
                        Path = path,
                        Banned = !string.IsNullOrWhiteSpace(ip) && await securityService.IsBlocked(ip),
                        Count = count
                    });
                }
            }

            // ---------------------------------------
            // 2. Считаем уникальные IP за последние 24 часа
            // ---------------------------------------
            var yesterday = DateTime.UtcNow.AddHours(-24);

            const string countPerDayQuery = @"
                SELECT COUNT(DISTINCT IP)
                FROM monitor
                WHERE DateT >= @Yesterday;";

            using (var commandPerDay = new SQLiteCommand(countPerDayQuery, connection))
            {
                // Лучше хранить дату в ISO формате
                commandPerDay.Parameters.AddWithValue("@Yesterday", yesterday.ToString("yyyy-MM-dd HH:mm:ss"));

                var result = await commandPerDay.ExecuteScalarAsync();
                countPerDay = result != null && result != DBNull.Value
                    ? Convert.ToInt32(result)
                    : 0;
            }

            // Отдаём страницу панели администратора
            return View(new MainModel(null, null)
            {
                ShowLeftSide = true,
                MonitorData = monitorDataList,
                AllUsersPerDay = countPerDay
            });
        }
        catch (Exception ex)
        {
            ex.LogException("Ошибка при загрузке кабинета администратора");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Страница управления пользователями
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Posts()
    {
        try
        {
            //проверяем блокировку
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
                return StatusCode(403);

            //получаем объект сессии администратора
            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");

            //если сессии нет или администратор не авторизован редиректим на главную страницу
            if (adminSession is not { IsAuth: true }) return RedirectPermanent("~/");

            //используем базу приложения
            var posts = await appDb.GetAllAdminPostsAsync();

            //отдаем страницу управления пользователями
            return View(new MainModel(posts, null)
            {
                ShowLeftSide = true
            });
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем 404 ошибку
            return StatusCode(404);
        }
    }

    /// <summary>
    /// Страница управления пользователями
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Commands()
    {
        try
        {
            //проверяем блокировку
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
                return StatusCode(403);

            //получаем объект сессии администратора
            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");

            //если сессии нет или администратор не авторизован редиректим на главную страницу
            if (adminSession is not { IsAuth: true }) return RedirectPermanent("~/");

            //используем базу приложения
            var commands = await appDb.GetAllAdminCommandsAsync();

            //отдаем страницу управления пользователями
            return View(new MainModel(null, commands)
            {
                ShowLeftSide = true
            });
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем 404 ошибку
            return StatusCode(404);
        }
    }

    /// <summary>
    /// Страница управления пользователями
    /// </summary>
    [HttpGet]
    [Route("/command-translates")]
    public async Task<IActionResult> CommandTranslates()
    {
        try
        {
            //проверяем блокировку
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
                return StatusCode(403);

            //получаем объект сессии администратора
            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");

            //если сессии нет или администратор не авторизован редиректим на главную страницу
            if (adminSession is not { IsAuth: true }) return RedirectPermanent("~/");

            //используем базу приложения
            var translations = await appDb.GetAllAdminCommandTranslatesAsync();

            //отдаем страницу управления пользователями
            return View(new MainModel(null, null, translations)
            {
                ShowLeftSide = true
            });
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем 404 ошибку
            return StatusCode(404);
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveCommandTranslation(int commandId, List<CommandTranslation> Translations)
    {
        //проверяем блокировку
        var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
        if (await securityService.IsBlocked(ip))
            return StatusCode(403);

        var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");
        if (adminSession is not { IsAuth: true }) return RedirectPermanent("~/");

        var command = new AdminCommandWithTranslations()
        {
            CommandId = commandId,
            Translations = Translations
        };

        await appDb.SaveCommandTranslationsAsync(command);

        return Redirect("/command-translates");
    }


    /// <summary>
    /// Метод выхода из системы
    /// </summary>
    /// <returns>Результат действия</returns>
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        try
        {
            //проверяем блокировку
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
                return StatusCode(403);

            //получаем объект сессии администратора
            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");

            //если сессии нет или администратор не авторизован редиректим на главную страницу
            if (adminSession is not { IsAuth: true }) return RedirectPermanent("~/");

            //разрушаем сессию пользователя
            HttpContext.DestroyUserSession();

            //редиректим на главную
            return RedirectPermanent("~/");
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем 404 ошибку
            return StatusCode(404);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> ErrorAsync()
    {
        try
        {
            //проверяем блокировку
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
                return StatusCode(403);

            //отдаем страницу ошибки
            return View(new MainModel(null, null)
                { ShowLeftSide = false });
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем 404 ошибку
            return StatusCode(404);
        }
    }
}
