/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 30 июля 2026 07:10:07
 * Version: 1.0.126
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
    /// Страница управления новостями Lizerium Launcher.
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
    /// Сохраняет новость Lizerium Launcher.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/news/save")]
    public async Task<IActionResult> SaveNews(
        [FromForm] LauncherNewsDataResponse news,
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

            HttpContext.Session.SetSession("admin", adminSession);
            await HttpContext.Session.CommitAsync();

            if (removeImage)
                news.ImageUrl = string.Empty;

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
                    news.LikeCount,
                    PreviewImageUrl = GetNewsPreviewUrl(news.ImageUrl),
                    news.IsPublished,
                    news.PublishedAtUnix,
                    PublishedAtLocal = FormatNewsPublishedAtInput(news.PublishedAtUnix),
                    Title = string.IsNullOrWhiteSpace(news.TitleRu) ? news.TitleEn : news.TitleRu
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
    /// Удаляет новость Lizerium Launcher.
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

            HttpContext.Session.SetSession("admin", adminSession);
            await HttpContext.Session.CommitAsync();

            if (!await appDb.DeleteLauncherNewsAsync(id))
            {
                if (WantsJsonResponse())
                    return BadRequest(new { ok = false, message = "delete failed" });

                return Redirect("~/news?delete=failed");
            }

            if (WantsJsonResponse())
                return Json(new { ok = true, id });

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

    /// <summary>
    /// Отдает локальное превью обложки новости для админки.
    /// </summary>
    [HttpGet]
    [Route("/news/image/{fileName}")]
    public IActionResult NewsImage(string fileName)
    {
        try
        {
            if (!AdminAccessGuard.IsAllowed(HttpContext))
                return NotFound();

            var adminSession = HttpContext.Session.GetSession<AdminSession>("admin");
            if (adminSession is not { IsAuth: true })
                return NotFound();

            if (string.IsNullOrWhiteSpace(fileName) || fileName != Path.GetFileName(fileName))
                return NotFound();

            var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
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

            var fullPath = Path.Combine(GetNewsImagesPath(), fileName);
            if (!System.IO.File.Exists(fullPath))
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

            HttpContext.Session.SetSession("admin", adminSession);
            await HttpContext.Session.CommitAsync();

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

    private bool WantsJsonResponse()
    {
        return string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase)
            || Request.Headers["Accept"].Any(value => value.Contains("application/json", StringComparison.OrdinalIgnoreCase));
    }

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

        if (!imageUrl.StartsWith("/img/news/", StringComparison.OrdinalIgnoreCase))
            return imageUrl;

        var fileName = Path.GetFileName(imageUrl);
        return string.IsNullOrWhiteSpace(fileName) ? imageUrl : $"/news/image/{fileName}";
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

    private static async Task<string> SaveNewsImageAsync(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return string.Empty;

        const long maxImageBytes = 8 * 1024 * 1024;
        if (imageFile.Length > maxImageBytes)
            throw new InvalidOperationException("News image is too large.");

        var extension = Path.GetExtension(imageFile.FileName)?.ToLowerInvariant();
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp",
            ".gif"
        };

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
