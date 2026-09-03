/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 03 сентября 2026 07:38:14
 * Version: 1.0.165
 */

using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using AspNetCore.ReCaptcha;

using LizeriumDatabase.Services.AppDataBaseService;

using LizeriumServer.Models;

using LizeriumUtilities.Accessories.NewsAccessories;
using LizeriumUtilities.FormatsData.DataBase.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LizeriumServer.Controllers
{
    /// <summary>
    /// Центральный контроллер
    /// </summary>
    public class HomeController : Controller
    {
        private IDataBaseService AppDb { get; set; }

        public HomeController(IDataBaseService dataBaseService)
        {
            AppDb = dataBaseService;
        }

        /// <summary>
        /// Главная страница сервера
        /// </summary>
        [HttpGet]
        [Route("/")]
        public async Task<IActionResult> Index()
        {
            // Проверяем, есть ли кука локали
            if (!Request.Cookies.ContainsKey(".AspNetCore.Culture"))
            {
                // Устанавливаем куку по умолчанию на "ru"
                var cultureValue = "c=ru|uic=ru";
                Response.Cookies.Append(
                    ".AspNetCore.Culture",
                    cultureValue,
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddYears(1),
                        IsEssential = true,
                        HttpOnly = false,
                        SameSite = SameSiteMode.Lax
                    }
                );
            }

            return View();
        }

        /// <summary>
        /// Страница технический работ сервера
        /// </summary>
        [HttpGet]
        [Route("/maintenance")]
        public async Task<IActionResult> Maintenance()
        {
            return View();
        }

        /// <summary>
        /// Главная страница загрузчика
        /// </summary>
        public async Task<IActionResult> Launcher(string search = "", string order = "new", string platform = "", string type = "", bool github = false, int page = 1, int openNewsId = 0)
        {
            var news = await AppDb.GetPublishedLauncherNewsAsync();
            page = Math.Max(1, page);
            order = string.IsNullOrWhiteSpace(order) ? "new" : order;
            search = search?.Trim() ?? string.Empty;
            platform = platform?.Trim().ToLowerInvariant() ?? string.Empty;
            type = type?.Trim() ?? string.Empty;

            var newsTypes = news
                .SelectMany(item => new[] { item.NewsTypeRu, item.NewsTypeEn, item.NewsType })
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => item!.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item)
                .ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                news = news
                    .Where(item =>
                        ContainsText(item.TitleRu, search)
                        || ContainsText(item.TitleEn, search)
                        || ContainsText(item.MarkdownRu, search)
                        || ContainsText(item.MarkdownEn, search)
                        || ContainsText(item.YoutubeUrl, search)
                        || ContainsText(item.RutubeUrl, search)
                        || ContainsText(item.VkVideoUrl, search)
                        || ContainsText(item.ImageUrl, search)
                        || ContainsText(item.ImageGalleryJson, search)
                        || ContainsText(item.NewsType, search)
                        || ContainsText(item.NewsTypeRu, search)
                        || ContainsText(item.NewsTypeEn, search)
                        || ContainsText(item.GithubProjectName, search)
                        || ContainsText(item.GithubUrl, search))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(platform))
            {
                news = news
                    .Where(item => platform switch
                    {
                        "youtube" => !string.IsNullOrWhiteSpace(item.YoutubeUrl),
                        "rutube" => !string.IsNullOrWhiteSpace(item.RutubeUrl),
                        "vk" => !string.IsNullOrWhiteSpace(item.VkVideoUrl),
                        _ => true
                    })
                    .ToList();
            }

            if (github)
            {
                news = news
                    .Where(item => !string.IsNullOrWhiteSpace(item.GithubUrl))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                news = news
                    .Where(item =>
                        string.Equals(item.NewsTypeRu?.Trim(), type, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(item.NewsTypeEn?.Trim(), type, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(item.NewsType?.Trim(), type, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            news = string.Equals(order, "old", StringComparison.OrdinalIgnoreCase)
                ? news.OrderBy(item => item.PublishedAtUnix).ThenBy(item => item.SortOrder).ToList()
                : news.OrderByDescending(item => item.PublishedAtUnix).ThenBy(item => item.SortOrder).ToList();

            const int pageSize = 7;
            var totalCount = news.Count;
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
            page = Math.Min(page, totalPages);
            news = news.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return View("Launcher", new LauncherViewModel
            {
                News = news,
                Search = search,
                SortOrderFilter = order,
                PlatformFilter = platform,
                TypeFilter = type,
                GithubFilter = github,
                NewsTypes = newsTypes,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalCount = totalCount,
                OpenNewsId = openNewsId
            });
        }

        /// <summary>
        /// Canonical public launcher news URL that opens the existing reader modal.
        /// </summary>
        [HttpGet]
        [Route("/news/{id:int}/{slug}.html")]
        public async Task<IActionResult> NewsArticle(int id, string slug)
        {
            var newsItem = await AppDb.GetPublishedLauncherNewsByIdAsync(id);
            if (newsItem == null)
                return NotFound();

            var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var canonicalPath = newsItem.GetCanonicalNewsPath(culture);
            if (!string.Equals($"/news/{id}/{slug}.html", canonicalPath, StringComparison.OrdinalIgnoreCase))
                return RedirectPermanent(canonicalPath);

            var allNews = await AppDb.GetPublishedLauncherNewsAsync();
            allNews = allNews
                .OrderByDescending(item => item.PublishedAtUnix)
                .ThenBy(item => item.SortOrder)
                .ToList();

            const int pageSize = 7;
            var newsIndex = allNews.FindIndex(item => item.Id == id);
            var page = newsIndex >= 0
                ? (newsIndex / pageSize) + 1
                : 1;

            return await Launcher(page: page, openNewsId: id);
        }

        private static bool ContainsText(string value, string search)
        {
            return !string.IsNullOrWhiteSpace(value)
                && value.Contains(search, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// RSS-лента опубликованных новостей Lizerium Steam.
        /// </summary>
        [HttpGet]
        [Route("/news/rss.xml")]
        [Route("/rss/news.xml")]
        public async Task<IActionResult> NewsRss(string lang = "ru")
        {
            var isRussian = !string.Equals(lang, "en", StringComparison.OrdinalIgnoreCase);
            var news = await AppDb.GetPublishedLauncherNewsAsync();
            news = news
                .OrderByDescending(item => item.PublishedAtUnix)
                .ThenBy(item => item.SortOrder)
                .Take(50)
                .ToList();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                Async = true
            };

            await using var stream = new MemoryStream();
            await using (var writer = XmlWriter.Create(stream, settings))
            {
                await writer.WriteStartDocumentAsync();
                await writer.WriteStartElementAsync(null, "rss", null);
                await writer.WriteAttributeStringAsync(null, "version", null, "2.0");
                await writer.WriteStartElementAsync(null, "channel", null);

                await writer.WriteElementStringAsync(null, "title", null, isRussian ? "Новости Lizerium" : "Lizerium News");
                await writer.WriteElementStringAsync(null, "link", null, $"{baseUrl}/Home/Launcher");
                await writer.WriteElementStringAsync(null, "description", null, isRussian
                    ? "Опубликованные новости Lizerium Steam"
                    : "Published Lizerium Steam news");
                await writer.WriteElementStringAsync(null, "language", null, isRussian ? "ru" : "en");

                foreach (var item in news)
                {
                    var title = PickLocalizedNewsText(item.TitleRu, item.TitleEn, isRussian);
                    var markdown = PickLocalizedNewsText(item.MarkdownRu, item.MarkdownEn, isRussian);
                    var description = BuildRssDescription(markdown, item.ImageUrl, baseUrl);
                    var canonicalUrl = $"{baseUrl}{item.GetCanonicalNewsPath(isRussian ? "ru" : "en")}";
                    var publishedAt = item.PublishedAtUnix > 0
                        ? DateTimeOffset.FromUnixTimeSeconds(item.PublishedAtUnix)
                        : DateTimeOffset.UtcNow;

                    await writer.WriteStartElementAsync(null, "item", null);
                    await writer.WriteElementStringAsync(null, "title", null, title);
                    await writer.WriteElementStringAsync(null, "link", null, canonicalUrl);
                    await writer.WriteElementStringAsync(null, "guid", null, canonicalUrl);
                    await writer.WriteElementStringAsync(null, "pubDate", null, publishedAt.UtcDateTime.ToString("R"));
                    await writer.WriteElementStringAsync(null, "description", null, description);
                    await writer.WriteEndElementAsync();
                }

                await writer.WriteEndElementAsync();
                await writer.WriteEndElementAsync();
                await writer.WriteEndDocumentAsync();
            }

            return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
        }

        private static string PickLocalizedNewsText(string russian, string english, bool isRussian)
        {
            var preferred = isRussian ? russian : english;
            var fallback = isRussian ? english : russian;
            return !string.IsNullOrWhiteSpace(preferred) ? preferred : fallback ?? string.Empty;
        }

        private static string BuildRssDescription(string markdown, string imageUrl, string baseUrl)
        {
            var text = StripMarkdown(markdown);
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                var fullImageUrl = Uri.TryCreate(imageUrl, UriKind.Absolute, out _)
                    ? imageUrl
                    : $"{baseUrl}{imageUrl}";

                return $"<p><img src=\"{fullImageUrl}\" alt=\"\" /></p><p>{text}</p>";
            }

            return text;
        }

        private static string StripMarkdown(string markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
                return string.Empty;

            var text = Regex.Replace(markdown, @"!\[[^\]]*\]\([^)]+\)", string.Empty);
            text = Regex.Replace(text, @"\[([^\]]+)\]\([^)]+\)", "$1");
            text = Regex.Replace(text, @"[#>*_`~\-]+", " ");
            text = Regex.Replace(text, @"\s+", " ").Trim();
            return text.Length > 500 ? $"{text[..500]}..." : text;
        }

        /// <summary>
        /// Adds one public like to a launcher news item.
        /// </summary>
        [HttpPost]
        [Route("/news/like/{id:int}")]
        public async Task<IActionResult> LikeNews(int id)
        {
            var likeCount = await AppDb.IncrementLauncherNewsLikeAsync(id);
            if (likeCount == null)
                return NotFound(new { ok = false });

            return Json(new { ok = true, likeCount });
        }

        /// <summary>
        /// Главная страница игры
        /// </summary>
        public async Task<IActionResult> Game()
        {
            var categories = await AppDb.GetPublishedProductCatalogAsync();

            return View(new GameProductsViewModel
            {
                Categories = categories ?? new List<LizeriumUtilities.FormatsData.DataBase.Response.ProductCategoryDataResponse>()
            });
        }

        /// <summary>
        /// Страница сообщества Lizerium.
        /// </summary>
        public async Task<IActionResult> Community()
        {
            return View();
        }

        /// <summary>
        /// Пожелания по игре
        /// </summary>
        public async Task<IActionResult> Wish()
        {
            //используем базу приложения
            var posts = await AppDb.GetAllPostsAsync();

            return View(new WishViewModel(posts));
        }

        /// <summary>
        /// Создание пожелания по игре
        /// </summary>
        /// <param name="PostModel">Данные пожелания</param>
        [HttpPost]
        [ValidateReCaptcha]
        [Route("create")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostViewRequest PostModel)
        {
            if (PostModel == null
                || string.IsNullOrWhiteSpace(PostModel.Autor)
                || string.IsNullOrWhiteSpace(PostModel.Message))
                return RedirectToAction(nameof(Wish));

            PostModel.Autor = PostModel.Autor.Trim();
            PostModel.Message = PostModel.Message.Trim();

            //используем базу приложения
            PostModel.Status = -1;
            await AppDb.AddPostAsync(PostModel);
            return RedirectToAction(nameof(Wish));
        }

        /// <summary>
        /// Пересоздание базы данных
        /// </summary>
        /// <param name="PostModel">Данные</param>
        [HttpGet]
        [Route("rebuild")]
        public async Task<IActionResult> Rebuild([FromForm] CreatePostViewRequest PostModel)
        {
            //используем базу приложения
            await AppDb.RebuildAsync();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Страница приватности
        /// </summary>
        public async Task<IActionResult> Privacy()
        {
            return View();
        }

        /// <summary>
        /// Страница ошибки
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            HttpContext.Response.Cookies.Delete(".Aws.Session");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
