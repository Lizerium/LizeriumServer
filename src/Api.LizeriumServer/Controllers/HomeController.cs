/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 01 сентября 2026 08:53:24
 * Version: 1.0.163
 */

using System;
using System.Data.SQLite;
using System.Text.Json;

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
    private const string SitemapTokenHeader = "X-Lizerium-Sitemap-Token";
    private static readonly HttpClient SitemapRebuildHttpClient = new();

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

        return Redirect("~/cabinet");
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
                return Redirect("~/");
            }

            //если сессия задана, но администратор не авторизован
            if (!adminSession.IsAuth)
            {
                //отдаем страницу подтверждения авторизации разовым кодом
                return View(new MainModel(null, null) { ShowLeftSide = false });
            }

            //редиректим на страницу кабинета администратора
            return Redirect("~/cabinet");
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
            if (adminSession is not { IsAuth: true }) return Redirect("~/");

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
            if (adminSession is not { IsAuth: true }) return Redirect("~/");

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
            if (adminSession is not { IsAuth: true }) return Redirect("~/");

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
    /// Страница управления новостями Lizerium Steam.
    /// </summary>
    [HttpGet]
    [Route("/news")]
    public async Task<IActionResult> News(string search = "", string status = "all", int page = 1)
    {
        try
        {
            if (!AdminAccessGuard.IsAllowed(HttpContext))
                return View("AccessClosed", new MainModel(null, null) { ShowLeftSide = false });

            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(ip))
                return StatusCode(403);

            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");
            if (adminSession is not { IsAuth: true }) return Redirect("~/");

            var news = await appDb.GetAllAdminLauncherNewsAsync();
            var newsTypeOptions = news
                .Select(item => new LauncherNewsTypeOption
                {
                    Ru = item.NewsTypeRu?.Trim() ?? string.Empty,
                    En = item.NewsTypeEn?.Trim() ?? string.Empty
                })
                .Where(item => !string.IsNullOrWhiteSpace(item.Ru) || !string.IsNullOrWhiteSpace(item.En))
                .GroupBy(item => $"{item.Ru}\u001f{item.En}", StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(item => string.IsNullOrWhiteSpace(item.Ru) ? item.En : item.Ru, StringComparer.OrdinalIgnoreCase)
                .ToList();

            page = Math.Max(1, page);
            status = string.IsNullOrWhiteSpace(status) ? "all" : status;
            search = search?.Trim() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(search))
            {
                news = news
                    .Where(item =>
                        ContainsNewsText(item.TitleRu, search)
                        || ContainsNewsText(item.TitleEn, search)
                        || ContainsNewsText(item.MarkdownRu, search)
                        || ContainsNewsText(item.MarkdownEn, search)
                        || ContainsNewsText(item.YoutubeUrl, search)
                        || ContainsNewsText(item.RutubeUrl, search)
                        || ContainsNewsText(item.VkVideoUrl, search)
                        || ContainsNewsText(item.ImageUrl, search)
                        || ContainsNewsText(item.ImageGalleryJson, search)
                        || ContainsNewsText(item.NewsType, search)
                        || ContainsNewsText(item.NewsTypeRu, search)
                        || ContainsNewsText(item.NewsTypeEn, search)
                        || ContainsNewsText(item.IconUrl, search)
                        || ContainsNewsText(item.GithubProjectName, search)
                        || ContainsNewsText(item.GithubUrl, search))
                    .ToList();
            }

            if (!string.Equals(status, "all", StringComparison.OrdinalIgnoreCase))
            {
                var isPublished = string.Equals(status, "published", StringComparison.OrdinalIgnoreCase);
                news = news.Where(item => item.IsPublished == isPublished).ToList();
            }

            const int newsPageSize = 8;
            var totalCount = news.Count;
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)newsPageSize));
            page = Math.Min(page, totalPages);
            news = news.Skip((page - 1) * newsPageSize).Take(newsPageSize).ToList();

            return View(new MainModel(null, null)
            {
                ShowLeftSide = true,
                LauncherNews = news,
                LauncherNewsTypes = newsTypeOptions,
                NewsSearch = search,
                NewsStatusFilter = status,
                NewsCurrentPage = page,
                NewsTotalPages = totalPages,
                NewsPageSize = newsPageSize,
                NewsTotalCount = totalCount
            });
        }
        catch (Exception exception)
        {
            exception.LogException();
            return StatusCode(404);
        }
    }

    /// <summary>
    /// Shows a closed admin-only preview for a launcher news item, including hidden drafts.
    /// </summary>
    [HttpGet]
    [Route("/news/preview/{id:int}")]
    public async Task<IActionResult> NewsPreview(int id, string culture = "ru")
    {
        try
        {
            if (!AdminAccessGuard.IsAllowed(HttpContext))
                return View("AccessClosed", new MainModel(null, null) { ShowLeftSide = false });

            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(ip))
                return StatusCode(403);

            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");
            if (adminSession is not { IsAuth: true }) return Redirect("~/");

            var news = await appDb.GetAdminLauncherNewsByIdAsync(id);
            if (news == null)
                return StatusCode(404);

            culture = string.Equals(culture, "en", StringComparison.OrdinalIgnoreCase) ? "en" : "ru";
            return View("NewsPreview", new MainModel(null, null)
            {
                ShowLeftSide = true,
                LauncherNews = new List<LauncherNewsDataResponse> { news },
                NewsPreviewCulture = culture
            });
        }
        catch (Exception exception)
        {
            exception.LogException();
            return StatusCode(404);
        }
    }

    // Product catalog admin: category/product/link CRUD plus image library endpoints.
    [HttpGet]
    [Route("/products")]
    public async Task<IActionResult> Products()
    {
        try
        {
            if (!AdminAccessGuard.IsAllowed(HttpContext))
                return View("AccessClosed", new MainModel(null, null) { ShowLeftSide = false });

            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(ip))
                return StatusCode(403);

            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");
            if (adminSession is not { IsAuth: true }) return Redirect("~/");

            return View(new MainModel(null, null)
            {
                ShowLeftSide = true,
                ProductCatalog = await appDb.GetAllAdminProductCatalogAsync()
            });
        }
        catch (Exception exception)
        {
            exception.LogException();
            return StatusCode(404);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/products/category/save")]
    public async Task<IActionResult> SaveProductCategory([FromForm] ProductCategoryDataResponse category)
    {
        if (!await CanEditAdminDataAsync())
            return WantsJsonResponse()
                ? Unauthorized(new { ok = false, message = "need authorization" })
                : Unauthorized("need authorization");

        await appDb.SaveProductCategoryAsync(category);
        return ProductMutationResult();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/products/category/delete")]
    public async Task<IActionResult> DeleteProductCategory([FromForm] int id)
    {
        if (!await CanEditAdminDataAsync())
            return Unauthorized("need authorization");

        await appDb.DeleteProductCategoryAsync(id);
        return Redirect("~/products");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/products/product/save")]
    public async Task<IActionResult> SaveProduct([FromForm] ProductDataResponse product)
    {
        if (!await CanEditAdminDataAsync())
            return WantsJsonResponse()
                ? Unauthorized(new { ok = false, message = "need authorization" })
                : Unauthorized("need authorization");

        await appDb.SaveProductAsync(product);
        return ProductMutationResult();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/products/product/delete")]
    public async Task<IActionResult> DeleteProduct([FromForm] int id)
    {
        if (!await CanEditAdminDataAsync())
            return Unauthorized("need authorization");

        await appDb.DeleteProductAsync(id);
        return Redirect("~/products");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/products/link/save")]
    public async Task<IActionResult> SaveProductDownloadLink([FromForm] ProductDownloadLinkDataResponse link)
    {
        if (!await CanEditAdminDataAsync())
            return WantsJsonResponse()
                ? Unauthorized(new { ok = false, message = "need authorization" })
                : Unauthorized("need authorization");

        await appDb.SaveProductDownloadLinkAsync(link);
        return ProductMutationResult();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/products/link/delete")]
    public async Task<IActionResult> DeleteProductDownloadLink([FromForm] int id)
    {
        if (!await CanEditAdminDataAsync())
            return Unauthorized("need authorization");

        await appDb.DeleteProductDownloadLinkAsync(id);
        return Redirect("~/products");
    }

    [HttpGet]
    [Route("/products/assets")]
    public async Task<IActionResult> GetProductAssets()
    {
        if (!await CanEditAdminDataAsync())
            return Unauthorized(new { ok = false, message = "need authorization" });

        // Product assets are stored under the portal image root so API deploys do not own public media.
        var imagesPath = GetPortalImagesPath();
        if (!Directory.Exists(imagesPath))
            return Json(new { ok = true, assets = Array.Empty<object>() });

        var allowedExtensions = GetProductAssetExtensions();
        var assets = Directory
            .EnumerateFiles(imagesPath, "*.*", SearchOption.AllDirectories)
            .Where(file => allowedExtensions.Contains(Path.GetExtension(file)))
            .Select(file =>
            {
                var relativePath = Path.GetRelativePath(imagesPath, file)
                    .Replace(Path.DirectorySeparatorChar, '/')
                    .Replace(Path.AltDirectorySeparatorChar, '/');

                var group = Path.GetDirectoryName(relativePath)?
                    .Replace(Path.DirectorySeparatorChar, '/')
                    .Replace(Path.AltDirectorySeparatorChar, '/') ?? string.Empty;

                return new
                {
                    url = $"/img/{relativePath}",
                    previewUrl = $"/products/assets/preview?url={Uri.EscapeDataString($"/img/{relativePath}")}",
                    name = Path.GetFileName(file),
                    group
                };
            })
            .OrderBy(asset => asset.group)
            .ThenBy(asset => asset.name)
            .ToList();

        return Json(new { ok = true, assets });
    }

    [HttpGet]
    [Route("/products/assets/preview")]
    public async Task<IActionResult> PreviewProductAsset([FromQuery] string url)
    {
        if (!await CanEditAdminDataAsync())
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("/img/", StringComparison.OrdinalIgnoreCase))
            return BadRequest();

        var relativePath = url.Substring("/img/".Length)
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);

        if (relativePath.Contains("..", StringComparison.Ordinal))
            return BadRequest();

        var imagesPath = GetPortalImagesPath();
        var fullPath = Path.GetFullPath(Path.Combine(imagesPath, relativePath));
        var rootPath = Path.GetFullPath(imagesPath);
        var normalizedRootPath = rootPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;

        // Never serve a preview outside the configured /img root, even if a crafted url reaches this endpoint.
        if (!fullPath.StartsWith(normalizedRootPath, StringComparison.OrdinalIgnoreCase) || !System.IO.File.Exists(fullPath))
            return NotFound();

        var extension = Path.GetExtension(fullPath);
        if (!GetProductAssetExtensions().Contains(extension))
            return BadRequest();

        return PhysicalFile(fullPath, GetProductAssetContentType(extension));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/products/assets/upload")]
    public async Task<IActionResult> UploadProductAsset([FromForm] IFormFile imageFile)
    {
        if (!await CanEditAdminDataAsync())
            return Unauthorized(new { ok = false, message = "need authorization" });

        try
        {
            var imageUrl = await SaveProductAssetAsync(imageFile);
            return Json(new
            {
                ok = true,
                url = imageUrl,
                previewUrl = $"/products/assets/preview?url={Uri.EscapeDataString(imageUrl)}"
            });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { ok = false, message = exception.Message });
        }
    }

    /// <summary>
    /// Сохраняет новость Lizerium Steam.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/news/save")]
    public async Task<IActionResult> SaveNews(
        [FromForm] LauncherNewsDataResponse news,
        [FromForm] IFormFile iconFile,
        [FromForm] IFormFile imageFile,
        [FromForm] List<IFormFile> galleryFiles,
        [FromForm] string publishedAtLocal,
        [FromForm] bool removeImage)
    {
        try
        {
            if (!AdminAccessGuard.IsAllowed(HttpContext))
            {
                if (WantsJsonResponse())
                    return StatusCode(404, new { ok = false, message = "access closed" });

                return StatusCode(404);
            }

            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(ip))
            {
                if (WantsJsonResponse())
                    return StatusCode(403, new { ok = false, message = "blocked" });

                return StatusCode(403);
            }

            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");
            if (adminSession is not { IsAuth: true })
                return WantsJsonResponse()
                    ? Unauthorized(new { ok = false, message = "need authorization" })
                    : Unauthorized("need authorization");

            if (removeImage)
                news.ImageUrl = string.Empty;

            var uploadedIconUrl = await SaveNewsImageAsync(iconFile);
            if (!string.IsNullOrWhiteSpace(uploadedIconUrl))
                news.IconUrl = uploadedIconUrl;

            var uploadedImageUrl = await SaveNewsImageAsync(imageFile);
            if (!string.IsNullOrWhiteSpace(uploadedImageUrl))
                news.ImageUrl = uploadedImageUrl;

            var uploadedGalleryUrls = await SaveNewsImagesAsync(galleryFiles);
            if (uploadedGalleryUrls.Count > 0)
            {
                var galleryUrls = ParseNewsGallery(news.ImageGalleryJson);
                galleryUrls.AddRange(uploadedGalleryUrls);
                news.ImageGalleryJson = SerializeNewsGallery(galleryUrls);
            }

            var selectedPublishedAt = ParseNewsPublishedAt(publishedAtLocal);
            if (selectedPublishedAt > 0)
                news.PublishedAtUnix = selectedPublishedAt;

            if (string.IsNullOrWhiteSpace(news.NewsType))
                news.NewsType = string.IsNullOrWhiteSpace(news.NewsTypeRu) ? news.NewsTypeEn : news.NewsTypeRu;

            if (!await appDb.SaveLauncherNewsAsync(news))
            {
                if (WantsJsonResponse())
                    return BadRequest(new { ok = false, message = "save failed" });

                return Redirect("~/news?save=failed");
            }

            var sitemapRebuild = await RebuildPublicSitemapAsync();

            if (WantsJsonResponse())
            {
                return Json(new
                {
                    ok = true,
                    news.Id,
                    news.ImageUrl,
                    news.ImageGalleryJson,
                    news.NewsType,
                    news.NewsTypeRu,
                    news.NewsTypeEn,
                    news.IconUrl,
                    IconPreviewUrl = GetNewsPreviewUrl(news.IconUrl),
                    news.LikeCount,
                    PreviewImageUrl = GetNewsPreviewUrl(news.ImageUrl),
                    news.IsPublished,
                    news.PublishedAtUnix,
                    PublishedAtLocal = FormatNewsPublishedAtInput(news.PublishedAtUnix),
                    Title = string.IsNullOrWhiteSpace(news.TitleRu) ? news.TitleEn : news.TitleRu,
                    SitemapRebuild = sitemapRebuild
                });
            }

            return Redirect("~/news");
        }
        catch (Exception exception)
        {
            exception.LogException();
            if (WantsJsonResponse())
                return BadRequest(new { ok = false, message = exception.Message });

            return Redirect("~/news?save=failed");
        }
    }

    /// <summary>
    /// Удаляет новость Lizerium Steam.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/news/delete")]
    public async Task<IActionResult> DeleteNews([FromForm] int id)
    {
        try
        {
            if (!AdminAccessGuard.IsAllowed(HttpContext))
            {
                if (WantsJsonResponse())
                    return StatusCode(404, new { ok = false, message = "access closed" });

                return StatusCode(404);
            }

            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (await securityService.IsBlocked(ip))
            {
                if (WantsJsonResponse())
                    return StatusCode(403, new { ok = false, message = "blocked" });

                return StatusCode(403);
            }

            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");
            if (adminSession is not { IsAuth: true })
                return WantsJsonResponse()
                    ? Unauthorized(new { ok = false, message = "need authorization" })
                    : Unauthorized("need authorization");

            if (!await appDb.DeleteLauncherNewsAsync(id))
            {
                if (WantsJsonResponse())
                    return BadRequest(new { ok = false, message = "delete failed" });

                return Redirect("~/news?delete=failed");
            }

            var sitemapRebuild = await RebuildPublicSitemapAsync();

            if (WantsJsonResponse())
                return Json(new { ok = true, id, SitemapRebuild = sitemapRebuild });

            return Redirect("~/news");
        }
        catch (Exception exception)
        {
            exception.LogException();
            if (WantsJsonResponse())
                return BadRequest(new { ok = false, message = exception.Message });

            return Redirect("~/news?delete=failed");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/sitemap/rebuild")]
    public async Task<IActionResult> RebuildSitemap()
    {
        if (!await CanEditAdminDataAsync())
            return WantsJsonResponse()
                ? Unauthorized(new { ok = false, message = "need authorization" })
                : Unauthorized("need authorization");

        var sitemapRebuild = await RebuildPublicSitemapAsync();
        if (WantsJsonResponse())
        {
            return sitemapRebuild.Ok
                ? Json(new { ok = true, sitemapRebuild })
                : BadRequest(new { ok = false, sitemapRebuild, message = sitemapRebuild.Message });
        }

        return Redirect(sitemapRebuild.Ok ? "~/news?sitemap=rebuilt" : "~/news?sitemap=failed");
    }

    /// <summary>
    /// Отдает локальное превью обложки новости для админки.
    /// </summary>
    [HttpGet]
    [Route("/news/image/{*relativePath}")]
    public IActionResult NewsImage(string relativePath)
    {
        try
        {
            if (!AdminAccessGuard.IsAllowed(HttpContext))
                return NotFound();

            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");
            if (adminSession is not { IsAuth: true })
                return NotFound();

            if (string.IsNullOrWhiteSpace(relativePath))
                return NotFound();

            var fullPath = ResolveNewsImagePath($"/img/news/{relativePath}");
            if (string.IsNullOrWhiteSpace(fullPath) || !System.IO.File.Exists(fullPath))
                return NotFound();

            var extension = Path.GetExtension(fullPath)?.ToLowerInvariant();
            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                ".gif" => "image/gif",
                _ => string.Empty
            };

            if (string.IsNullOrWhiteSpace(contentType))
                return NotFound();

            return PhysicalFile(fullPath, contentType);
        }
        catch (Exception exception)
        {
            exception.LogException();
            return NotFound();
        }
    }

    /// <summary>
    /// Uploads an image pasted into the news Markdown editor.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/news/upload-image")]
    public async Task<IActionResult> UploadNewsMarkdownImage([FromForm] IFormFile imageFile)
    {
        try
        {
            if (!AdminAccessGuard.IsAllowed(HttpContext))
                return StatusCode(404, new { ok = false, message = "access closed" });

            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");
            if (adminSession is not { IsAuth: true })
                return Unauthorized(new { ok = false, message = "need authorization" });

            var uploadedImageUrl = await SaveNewsImageAsync(imageFile);
            if (string.IsNullOrWhiteSpace(uploadedImageUrl))
                return BadRequest(new { ok = false, message = "empty image" });

            return Json(new
            {
                ok = true,
                imageUrl = uploadedImageUrl,
                previewImageUrl = GetNewsPreviewUrl(uploadedImageUrl)
            });
        }
        catch (Exception exception)
        {
            exception.LogException();
            return BadRequest(new { ok = false, message = exception.Message });
        }
    }

    [HttpGet]
    [Route("/news/assets")]
    public async Task<IActionResult> GetNewsAssets([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string query = "")
    {
        if (!await CanEditAdminDataAsync())
            return Unauthorized(new { ok = false, message = "need authorization" });

        var imagesPath = GetNewsImagesPath();
        if (!Directory.Exists(imagesPath))
            return Json(new { ok = true, assets = Array.Empty<object>(), page = 1, pageSize = 20, total = 0, pageCount = 0 });

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 10, 60);
        var search = (query ?? string.Empty).Trim();
        var allowedExtensions = GetNewsImageExtensions();
        var assets = Directory
            .EnumerateFiles(imagesPath, "*.*", SearchOption.AllDirectories)
            .Where(file => allowedExtensions.Contains(Path.GetExtension(file)))
            .Select(file =>
            {
                var relativePath = Path.GetRelativePath(imagesPath, file)
                    .Replace(Path.DirectorySeparatorChar, '/')
                    .Replace(Path.AltDirectorySeparatorChar, '/');

                var group = Path.GetDirectoryName(relativePath)?
                    .Replace(Path.DirectorySeparatorChar, '/')
                    .Replace(Path.AltDirectorySeparatorChar, '/') ?? string.Empty;

                var url = $"/img/news/{relativePath}";
                return new
                {
                    url,
                    previewUrl = GetNewsPreviewUrl(url),
                    name = Path.GetFileName(file),
                    group,
                    modifiedAtUnix = new DateTimeOffset(System.IO.File.GetLastWriteTimeUtc(file)).ToUnixTimeSeconds()
                };
            });

        if (!string.IsNullOrWhiteSpace(search))
        {
            assets = assets.Where(asset =>
                asset.name.Contains(search, StringComparison.OrdinalIgnoreCase)
                || asset.group.Contains(search, StringComparison.OrdinalIgnoreCase)
                || asset.url.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        var sortedAssets = assets
            .OrderByDescending(asset => asset.modifiedAtUnix)
            .ThenBy(asset => asset.name)
            .ToList();
        var total = sortedAssets.Count;
        var pageCount = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize);
        if (pageCount > 0)
            page = Math.Min(page, pageCount);

        var pageAssets = sortedAssets
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Json(new { ok = true, assets = pageAssets, page, pageSize, total, pageCount });
    }

    [HttpGet]
    [Route("/news/assets/preview")]
    public async Task<IActionResult> PreviewNewsAsset([FromQuery] string url)
    {
        if (!await CanEditAdminDataAsync())
            return Unauthorized();

        var fullPath = ResolveNewsImagePath(url);
        if (string.IsNullOrWhiteSpace(fullPath) || !System.IO.File.Exists(fullPath))
            return NotFound();

        var extension = Path.GetExtension(fullPath);
        if (!GetNewsImageExtensions().Contains(extension))
            return BadRequest();

        return PhysicalFile(fullPath, GetNewsImageContentType(extension));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/news/assets/delete")]
    public async Task<IActionResult> DeleteNewsAsset([FromForm] string url)
    {
        if (!await CanEditAdminDataAsync())
            return Unauthorized(new { ok = false, message = "need authorization" });

        var fullPath = ResolveNewsImagePath(url);
        if (string.IsNullOrWhiteSpace(fullPath) || !System.IO.File.Exists(fullPath))
            return NotFound(new { ok = false, message = "file not found" });

        try
        {
            System.IO.File.Delete(fullPath);
            return Json(new { ok = true, url });
        }
        catch (Exception exception)
        {
            exception.LogException();
            return BadRequest(new { ok = false, message = exception.Message });
        }
    }

    private bool WantsJsonResponse()
    {
        return string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase)
            || Request.Headers["Accept"].Any(value => value.Contains("application/json", StringComparison.OrdinalIgnoreCase));
    }

    private IActionResult ProductMutationResult()
    {
        // Product admin supports AJAX saves, while plain form submits keep the redirect fallback.
        return WantsJsonResponse()
            ? Json(new { ok = true })
            : Redirect("~/products");
    }

    private static async Task<SitemapRebuildResult> RebuildPublicSitemapAsync()
    {
        var publicSiteBaseUrl = Program.Configuration["appSettings:publicSiteBaseUrl"];
        if (string.IsNullOrWhiteSpace(publicSiteBaseUrl))
            publicSiteBaseUrl = "https://lizup.ru";

        var sitemapToken = Program.Configuration["appSettings:sitemapRebuildToken"];
        if (string.IsNullOrWhiteSpace(sitemapToken))
            return new SitemapRebuildResult(false, "appSettings:sitemapRebuildToken is not configured");

        try
        {
            var rebuildUrl = $"{publicSiteBaseUrl.TrimEnd('/')}/internal/sitemap/rebuild";
            using var request = new HttpRequestMessage(HttpMethod.Post, rebuildUrl);
            request.Headers.TryAddWithoutValidation(SitemapTokenHeader, sitemapToken);

            using var response = await SitemapRebuildHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return new SitemapRebuildResult(true, "sitemap rebuilt");

            return new SitemapRebuildResult(false, $"sitemap rebuild failed: {(int)response.StatusCode}");
        }
        catch (Exception exception)
        {
            exception.LogException();
            return new SitemapRebuildResult(false, exception.Message);
        }
    }

    private async Task<bool> CanEditAdminDataAsync()
    {
        if (!AdminAccessGuard.IsAllowed(HttpContext))
            return false;

        var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
        if (await securityService.IsBlocked(ip))
            return false;

        var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");
        return adminSession is { IsAuth: true };
    }

    private sealed record SitemapRebuildResult(bool Ok, string Message);

    private static bool ContainsNewsText(string value, string search)
    {
        return !string.IsNullOrWhiteSpace(value)
            && value.Contains(search, StringComparison.OrdinalIgnoreCase);
    }

    private static string GetNewsPreviewUrl(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return string.Empty;

        if (Uri.TryCreate(imageUrl, UriKind.Absolute, out _))
            return imageUrl;

        if (imageUrl.StartsWith("/img/news/", StringComparison.OrdinalIgnoreCase))
            return $"https://lizup.ru{imageUrl}";

        return imageUrl;
    }

    private static long ParseNewsPublishedAt(string publishedAtLocal)
    {
        if (string.IsNullOrWhiteSpace(publishedAtLocal))
            return 0;

        if (!DateTime.TryParse(publishedAtLocal, out var localDateTime))
            return 0;

        var offset = TimeZoneInfo.Local.GetUtcOffset(localDateTime);
        return new DateTimeOffset(localDateTime, offset).ToUnixTimeSeconds();
    }

    private static string FormatNewsPublishedAtInput(long publishedAtUnix)
    {
        return publishedAtUnix > 0
            ? DateTimeOffset.FromUnixTimeSeconds(publishedAtUnix).ToLocalTime().ToString("yyyy-MM-ddTHH:mm")
            : string.Empty;
    }

    private static async Task<string> SaveProductAssetAsync(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            throw new InvalidOperationException("Image file is empty.");

        const long maxImageBytes = 8 * 1024 * 1024;
        if (imageFile.Length > maxImageBytes)
            throw new InvalidOperationException("Image file is too large.");

        var extension = Path.GetExtension(imageFile.FileName)?.ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(extension) || !GetProductAssetExtensions().Contains(extension))
            throw new InvalidOperationException("Unsupported image type.");

        var contentType = imageFile.ContentType?.ToLowerInvariant() ?? string.Empty;
        if (!contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) && extension != ".svg")
            throw new InvalidOperationException("Uploaded file is not an image.");

        var uploadPath = GetProductAssetUploadPath();
        Directory.CreateDirectory(uploadPath);

        // Timestamp keeps files sortable; GUID prevents overwrites when uploads share the same name.
        var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(uploadPath, fileName);

        await using (var stream = System.IO.File.Create(fullPath))
        {
            await imageFile.CopyToAsync(stream);
        }

        return $"/img/admin/products/{fileName}";
    }

    private static HashSet<string> GetProductAssetExtensions()
    {
        return new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".webp",
            ".png",
            ".jpg",
            ".jpeg",
            ".gif",
            ".svg"
        };
    }

    private static string GetProductAssetContentType(string extension)
    {
        return extension?.ToLowerInvariant() switch
        {
            ".svg" => "image/svg+xml",
            ".webp" => "image/webp",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            _ => "application/octet-stream"
        };
    }

    private static string GetProductAssetUploadPath()
    {
        return Path.Combine(GetPortalImagesPath(), "admin", "products");
    }

    private static string GetPortalImagesPath()
    {
        var configuredPath = Program.Configuration["appSettings:portalImagesPath"];
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            return Path.GetFullPath(Path.IsPathRooted(configuredPath)
                ? configuredPath
                : Path.Combine(Directory.GetCurrentDirectory(), configuredPath));
        }

        if (Program.SettingsApp.IsRelease)
        {
            // In production the API writes images to the main portal wwwroot, not to its own publish folder.
            var wwwRoot = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName
                ?? Directory.GetCurrentDirectory();

            return Path.GetFullPath(Path.Combine(wwwRoot, "uploader", "wwwroot", "img"));
        }

        return FindDevelopmentPortalImagesPath();
    }

    private static string FindDevelopmentPortalImagesPath()
    {
        var roots = new[]
        {
            AppContext.BaseDirectory,
            Directory.GetCurrentDirectory()
        };

        foreach (var root in roots)
        {
            var current = new DirectoryInfo(Path.GetFullPath(root));
            while (current != null)
            {
                var portalProject = Path.Combine(current.FullName, "LizeriumServer", "LizeriumServer.csproj");
                var portalImages = Path.Combine(current.FullName, "LizeriumServer", "wwwroot", "img");

                if (System.IO.File.Exists(portalProject) && Directory.Exists(portalImages))
                    return portalImages;

                current = current.Parent;
            }
        }

        return Path.GetFullPath(Path.Combine(
            Directory.GetCurrentDirectory(),
            "..",
            "LizeriumServer",
            "wwwroot",
            "img"));
    }

    private static async Task<string> SaveNewsImageAsync(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return string.Empty;

        const long maxImageBytes = 8 * 1024 * 1024;
        if (imageFile.Length > maxImageBytes)
            throw new InvalidOperationException("News image is too large.");

        var extension = Path.GetExtension(imageFile.FileName)?.ToLowerInvariant();
        var allowedExtensions = GetNewsImageExtensions();

        if (string.IsNullOrWhiteSpace(extension) || !allowedExtensions.Contains(extension))
            throw new InvalidOperationException("Unsupported news image type.");

        var contentType = imageFile.ContentType?.ToLowerInvariant() ?? string.Empty;
        if (!contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Uploaded file is not an image.");

        var imagesPath = GetNewsImagesPath();
        Directory.CreateDirectory(imagesPath);

        var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(imagesPath, fileName);

        await using (var stream = System.IO.File.Create(fullPath))
        {
            await imageFile.CopyToAsync(stream);
        }

        return $"/img/news/{fileName}";
    }

    private static HashSet<string> GetNewsImageExtensions()
    {
        return new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp",
            ".gif"
        };
    }

    private static string GetNewsImageContentType(string extension)
    {
        return extension?.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".gif" => "image/gif",
            _ => "application/octet-stream"
        };
    }

    private static string ResolveNewsImagePath(string url)
    {
        if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("/img/news/", StringComparison.OrdinalIgnoreCase))
            return string.Empty;

        var relativePath = url.Substring("/img/news/".Length)
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);

        if (relativePath.Contains("..", StringComparison.Ordinal))
            return string.Empty;

        foreach (var imagesPath in GetNewsImageRootCandidates())
        {
            var fullPath = Path.GetFullPath(Path.Combine(imagesPath, relativePath));
            var rootPath = Path.GetFullPath(imagesPath);
            var normalizedRootPath = rootPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;

            if (fullPath.StartsWith(normalizedRootPath, StringComparison.OrdinalIgnoreCase)
                && System.IO.File.Exists(fullPath))
                return fullPath;
        }

        return string.Empty;
    }

    private static async Task<List<string>> SaveNewsImagesAsync(IEnumerable<IFormFile> imageFiles)
    {
        var urls = new List<string>();
        if (imageFiles == null)
            return urls;

        foreach (var imageFile in imageFiles)
        {
            var url = await SaveNewsImageAsync(imageFile);
            if (!string.IsNullOrWhiteSpace(url))
                urls.Add(url);
        }

        return urls;
    }

    private static List<string> ParseNewsGallery(string imageGalleryJson)
    {
        if (string.IsNullOrWhiteSpace(imageGalleryJson))
            return new List<string>();

        try
        {
            return JsonSerializer.Deserialize<List<string>>(imageGalleryJson)?
                .Where(url => !string.IsNullOrWhiteSpace(url))
                .Select(url => url.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();
        }
        catch (JsonException)
        {
            return imageGalleryJson
                .Split(new[] { "\r\n", "\n", ";", "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(url => url.Trim())
                .Where(url => !string.IsNullOrWhiteSpace(url))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }

    private static string SerializeNewsGallery(IEnumerable<string> imageUrls)
    {
        var urls = imageUrls?
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Select(url => url.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList() ?? new List<string>();

        return urls.Count == 0 ? string.Empty : JsonSerializer.Serialize(urls);
    }

    private static string GetNewsImagesPath()
    {
        var configuredPath = Program.Configuration["appSettings:newsImagesPath"];
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            return Path.GetFullPath(Path.IsPathRooted(configuredPath)
                ? configuredPath
                : Path.Combine(Directory.GetCurrentDirectory(), configuredPath));
        }

        if (Program.SettingsApp.IsRelease)
        {
            var wwwRoot = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName
                ?? Directory.GetCurrentDirectory();

            return Path.GetFullPath(Path.Combine(wwwRoot, "uploader", "wwwroot", "img", "news"));
        }

        return FindDevelopmentNewsImagesPath();
    }

    private static IReadOnlyList<string> GetNewsImageRootCandidates()
    {
        var candidates = new List<string>();

        void AddCandidate(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            var fullPath = Path.GetFullPath(path);
            if (!candidates.Any(candidate => string.Equals(candidate, fullPath, StringComparison.OrdinalIgnoreCase)))
                candidates.Add(fullPath);
        }

        var configuredPath = Program.Configuration["appSettings:newsImagesPath"];
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            AddCandidate(Path.IsPathRooted(configuredPath)
                ? configuredPath
                : Path.Combine(Directory.GetCurrentDirectory(), configuredPath));
        }

        var currentDirectory = Directory.GetCurrentDirectory();
        var currentParent = Directory.GetParent(currentDirectory)?.FullName ?? currentDirectory;

        AddCandidate(Path.Combine(currentParent, "uploader", "wwwroot", "img", "news"));
        AddCandidate(Path.Combine(currentDirectory, "wwwroot", "img", "news"));
        AddCandidate(Path.Combine(AppContext.BaseDirectory, "wwwroot", "img", "news"));
        AddCandidate(Path.Combine(currentDirectory, "..", "LizeriumServer", "wwwroot", "img", "news"));
        AddCandidate(Path.Combine(currentDirectory, "..", "wwwroot", "img", "news"));
        AddCandidate(FindDevelopmentNewsImagesPath());

        return candidates;
    }

    private static string FindDevelopmentNewsImagesPath()
    {
        var roots = new[]
        {
            AppContext.BaseDirectory,
            Directory.GetCurrentDirectory()
        };

        foreach (var root in roots)
        {
            var current = new DirectoryInfo(Path.GetFullPath(root));
            while (current != null)
            {
                var portalProject = Path.Combine(current.FullName, "LizeriumServer", "LizeriumServer.csproj");
                var portalWebRoot = Path.Combine(current.FullName, "LizeriumServer", "wwwroot");

                if (System.IO.File.Exists(portalProject) && Directory.Exists(portalWebRoot))
                {
                    return Path.Combine(portalWebRoot, "img", "news");
                }

                current = current.Parent;
            }
        }

        return Path.GetFullPath(Path.Combine(
            Directory.GetCurrentDirectory(),
            "..",
            "LizeriumServer",
            "wwwroot",
            "img",
            "news"));
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
            if (adminSession is not { IsAuth: true }) return Redirect("~/");

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
        if (adminSession is not { IsAuth: true }) return Redirect("~/");

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
            if (adminSession is not { IsAuth: true }) return Redirect("~/");

            //разрушаем сессию пользователя
            HttpContext.DestroyUserSession();

            //редиректим на главную
            return Redirect("~/");
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
    [HttpGet]
    [Route("/Home/Error")]
    [Route("/Error")]
    public async Task<IActionResult> Error()
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
