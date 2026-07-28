/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 28 июля 2026 10:29:56
 * Version: 1.0.122
 */

using System;
using System.Data.SQLite;

using Api.LizeriumServer.Accessories.AuthExtensions;
using Api.LizeriumServer.FormatsData.AppAdminData;
using Api.LizeriumServer.FormatsData.Stats;
using Api.LizeriumServer.Models;
using Api.LizeriumServer.Services.AdminAccess;
using Api.LizeriumServer.Services.BotDetection;

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
    private const int DashboardRowsLimit = 100;
    private const int DashboardRecentRowsScanLimit = 5000;

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
        if (!AdminAccessGuard.IsAllowed(HttpContext))
            return View("AccessClosed", new MainModel(null, null) { ShowLeftSide = false });

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
            if (!AdminAccessGuard.IsAllowed(HttpContext))
                return View("AccessClosed", new MainModel(null, null) { ShowLeftSide = false });

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
            if (!AdminAccessGuard.IsAllowed(HttpContext))
                return View("AccessClosed", new MainModel(null, null) { ShowLeftSide = false });

            //проверяем блокировку
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
                return StatusCode(403);

            //получаем объект сессии администратора
            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");

            //если сессии нет или администратор не авторизован редиректим на главную страницу
            if (adminSession is not { IsAuth: true }) return RedirectPermanent("~/");

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
                                IsBot = BotDetectionService.IsBot(Agent),
                                Count = Count
                            });
                        }
                    }
                }
            }
            catch
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
    public async Task<IActionResult> Cabinet(int page = 1)
    {
        try
        {
            if (!AdminAccessGuard.IsAllowed(HttpContext))
                return View("AccessClosed", new MainModel(null, null) { ShowLeftSide = false });

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

            page = Math.Max(1, page);
            var monitorDataList = new List<MonitorData>();
            var hourlyDataList = new List<MonitorHourlyData>();
            int countPerDay = 0;
            int visitsPerDay = 0;
            int humanUsersPerDay = 0;
            int botUsersPerDay = 0;
            int botVisitsPerDay = 0;
            int totalMonitorRows = 0;

            var databasePath = DatabaseExtensions.Configuration.GetValue<string>("private_path");

            if (string.IsNullOrWhiteSpace(databasePath) || !System.IO.File.Exists(databasePath))
            {
                $"Файл базы данных не найден или путь пустой. private_path: {databasePath}".LogMessage();

                return View(new MainModel(null, null)
                {
                    ShowLeftSide = true,
                    MonitorData = monitorDataList,
                    MonitorHourlyData = hourlyDataList,
                    AllUsersPerDay = countPerDay,
                    AllVisitsPerDay = visitsPerDay,
                    CurrentPage = page,
                    PageSize = DashboardRowsLimit,
                    TotalPages = 1,
                    TotalMonitorRows = totalMonitorRows
                });
            }

            var connectionString = $"Data Source={databasePath};Version=3;";

            using var connection = new SQLiteConnection(connectionString);
            await connection.OpenAsync();

            const string normalizedMonitorDateSql = @"
                datetime(
                    substr(replace(DateT, '.', '/'), 7, 4) || '-' ||
                    CASE
                        WHEN CAST(substr(replace(DateT, '.', '/'), 1, 2) AS INTEGER) > 12
                            THEN substr(replace(DateT, '.', '/'), 4, 2)
                        ELSE substr(replace(DateT, '.', '/'), 1, 2)
                    END || '-' ||
                    CASE
                        WHEN CAST(substr(replace(DateT, '.', '/'), 1, 2) AS INTEGER) > 12
                            THEN substr(replace(DateT, '.', '/'), 1, 2)
                        ELSE substr(replace(DateT, '.', '/'), 4, 2)
                    END ||
                    substr(replace(DateT, '.', '/'), 11)
                )";

            const string totalLatestIpsQuery = @"
                WITH recent AS (
                    SELECT IP
                    FROM monitor
                    ORDER BY Id DESC
                    LIMIT @RecentRowsLimit
                )
                SELECT COUNT(DISTINCT IP)
                FROM recent
                WHERE IP IS NOT NULL AND IP <> '';";

            using (var totalCommand = new SQLiteCommand(totalLatestIpsQuery, connection))
            {
                totalCommand.Parameters.AddWithValue("@RecentRowsLimit", DashboardRecentRowsScanLimit);
                var result = await totalCommand.ExecuteScalarAsync();
                totalMonitorRows = result != null && result != DBNull.Value
                    ? Convert.ToInt32(result)
                    : 0;
            }

            const string latestIpsQuery = @"
                WITH recent AS (
                    SELECT Id, DateT, IP, LANG, AGENT, PATH
                    FROM monitor
                    ORDER BY Id DESC
                    LIMIT @RecentRowsLimit
                ),
                stats AS (
                    SELECT IP, MAX(Id) AS LatestId, COUNT(*) AS TotalCount
                    FROM recent
                    WHERE IP IS NOT NULL AND IP <> ''
                    GROUP BY IP
                )
                SELECT
                    recent.Id,
                    recent.DateT,
                    recent.IP,
                    recent.LANG,
                    recent.AGENT,
                    recent.PATH,
                    stats.TotalCount
                FROM recent
                INNER JOIN stats ON recent.Id = stats.LatestId
                ORDER BY recent.Id DESC
                LIMIT @DashboardRowsLimit OFFSET @Offset;";

            using (var command = new SQLiteCommand(latestIpsQuery, connection))
            {
                command.Parameters.AddWithValue("@RecentRowsLimit", DashboardRecentRowsScanLimit);
                command.Parameters.AddWithValue("@DashboardRowsLimit", DashboardRowsLimit);
                command.Parameters.AddWithValue("@Offset", (page - 1) * DashboardRowsLimit);

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
                            IsBot = BotDetectionService.IsBot(agent),
                            Count = count
                        });
                    }
                }
            }

            var yesterday = DateTime.Now.AddHours(-24);

            var hourlyPoints = new Dictionary<string, MonitorHourlyData>();
            var hourlyUniqueIps = new Dictionary<string, HashSet<string>>();
            var hourlyHumanIps = new Dictionary<string, HashSet<string>>();
            var hourlyBotIps = new Dictionary<string, HashSet<string>>();
            var allUniqueIps = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var humanUniqueIps = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var botUniqueIps = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var hourlyQuery = $@"
                WITH recent AS (
                    SELECT DateT, IP, AGENT
                    FROM monitor
                    ORDER BY Id DESC
                    LIMIT @RecentRowsLimit
                )
                SELECT
                    strftime('%Y-%m-%d %H', {normalizedMonitorDateSql}) AS HourKey,
                    strftime('%H:00', {normalizedMonitorDateSql}) AS HourLabel,
                    IP,
                    AGENT
                FROM recent
                WHERE {normalizedMonitorDateSql} >= datetime(@Yesterday)
                    AND IP IS NOT NULL
                    AND IP <> ''
                ORDER BY strftime('%Y-%m-%d %H', {normalizedMonitorDateSql});";

            using (var hourlyCommand = new SQLiteCommand(hourlyQuery, connection))
            {
                hourlyCommand.Parameters.AddWithValue("@Yesterday", yesterday.ToString("yyyy-MM-dd HH:mm:ss"));
                hourlyCommand.Parameters.AddWithValue("@RecentRowsLimit", DashboardRecentRowsScanLimit);

                using var reader = await hourlyCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var hourKey = reader["HourKey"]?.ToString() ?? string.Empty;
                    var hourLabel = reader["HourLabel"]?.ToString() ?? string.Empty;
                    var ip = reader["IP"]?.ToString() ?? string.Empty;
                    var agent = reader["AGENT"]?.ToString() ?? string.Empty;
                    var isBot = BotDetectionService.IsBot(agent);

                    if (string.IsNullOrWhiteSpace(hourKey) || string.IsNullOrWhiteSpace(ip))
                        continue;

                    if (!hourlyPoints.TryGetValue(hourKey, out var point))
                    {
                        point = new MonitorHourlyData { Label = hourLabel };
                        hourlyPoints[hourKey] = point;
                        hourlyUniqueIps[hourKey] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        hourlyHumanIps[hourKey] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        hourlyBotIps[hourKey] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    }

                    visitsPerDay++;
                    point.Visits++;
                    allUniqueIps.Add(ip);
                    hourlyUniqueIps[hourKey].Add(ip);

                    if (isBot)
                    {
                        botVisitsPerDay++;
                        point.BotVisits++;
                        botUniqueIps.Add(ip);
                        hourlyBotIps[hourKey].Add(ip);
                    }
                    else
                    {
                        point.HumanVisits++;
                        humanUniqueIps.Add(ip);
                        hourlyHumanIps[hourKey].Add(ip);
                    }
                }
            }

            foreach (var item in hourlyPoints)
            {
                item.Value.UniqueIps = hourlyUniqueIps[item.Key].Count;
                item.Value.HumanUniqueIps = hourlyHumanIps[item.Key].Count;
                item.Value.BotUniqueIps = hourlyBotIps[item.Key].Count;
                hourlyDataList.Add(item.Value);
            }

            countPerDay = allUniqueIps.Count;
            humanUsersPerDay = humanUniqueIps.Count;
            botUsersPerDay = botUniqueIps.Count;

            var totalPages = Math.Max(1, (int)Math.Ceiling(totalMonitorRows / (double)DashboardRowsLimit));

            // Отдаём страницу панели администратора
            return View(new MainModel(null, null)
            {
                ShowLeftSide = true,
                MonitorData = monitorDataList,
                MonitorHourlyData = hourlyDataList,
                AllUsersPerDay = countPerDay,
                AllVisitsPerDay = visitsPerDay,
                HumanUsersPerDay = humanUsersPerDay,
                BotUsersPerDay = botUsersPerDay,
                BotVisitsPerDay = botVisitsPerDay,
                CurrentPage = page,
                PageSize = DashboardRowsLimit,
                TotalPages = totalPages,
                TotalMonitorRows = totalMonitorRows
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
            if (!AdminAccessGuard.IsAllowed(HttpContext))
                return View("AccessClosed", new MainModel(null, null) { ShowLeftSide = false });

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
    public async Task<IActionResult> Commands(int status = 1, string category = "all")
    {
        try
        {
            if (!AdminAccessGuard.IsAllowed(HttpContext))
                return View("AccessClosed", new MainModel(null, null) { ShowLeftSide = false });

            //проверяем блокировку
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(HttpContext?.Connection?.RemoteIpAddress?.ToString()))
                return StatusCode(403);

            //получаем объект сессии администратора
            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");

            //если сессии нет или администратор не авторизован редиректим на главную страницу
            if (adminSession is not { IsAuth: true }) return RedirectPermanent("~/");

            var commands = await appDb.GetAllAdminCommandsAsync() ?? new();
            var categories = commands
                .Select(command => command.Category)
                .Where(commandCategory => !string.IsNullOrWhiteSpace(commandCategory))
                .Distinct()
                .OrderBy(commandCategory => commandCategory)
                .ToList();

            if (status > 0)
                commands = commands.Where(command => command.Status == status).ToList();

            if (!string.IsNullOrWhiteSpace(category) && !string.Equals(category, "all", StringComparison.OrdinalIgnoreCase))
                commands = commands.Where(command => command.Category == category).ToList();

            //отдаем страницу управления пользователями
            return View(new MainModel(null, commands)
            {
                ShowLeftSide = true,
                SelectedCommandStatus = status,
                SelectedCommandCategory = category,
                CommandCategories = categories
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
            if (!AdminAccessGuard.IsAllowed(HttpContext))
                return View("AccessClosed", new MainModel(null, null) { ShowLeftSide = false });

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
