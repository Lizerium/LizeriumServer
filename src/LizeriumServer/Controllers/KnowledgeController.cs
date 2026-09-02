/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 02 сентября 2026 07:18:07
 * Version: 1.0.164
 */

using System.Globalization;
using System.Text.RegularExpressions;

using LizeriumServer.FormatsData.AppWikiData;
using LizeriumServer.Helpers;
using LizeriumServer.Models;
using LizeriumServer.Options;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace LizeriumServer.Controllers
{
    /// <summary>
    /// Контроллер управления страницей знаний для создателей модов
    /// </summary>
    public class KnowledgeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly ResourceHelper _resourceHelper;
        private readonly StoragePathsOptions _storagePaths;
        private readonly SeoDomainsOptions _seoDomains;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="env">Окружение сервера</param>
        public KnowledgeController(IWebHostEnvironment env, IStringLocalizerFactory localizer,
            IOptions<StoragePathsOptions> storagePathsOptions,
            IOptions<SeoDomainsOptions> seoDomainsOptions)
        {
            _env = env;
            _storagePaths = storagePathsOptions.Value;
            _seoDomains = seoDomainsOptions.Value;

            var location = typeof(KnowledgeController).Assembly.GetName().Name;
            if (location != null)
                _stringLocalizer =
                    localizer.Create("SharedResource", location);

            var assemblyName = typeof(KnowledgeController).Assembly.GetName().Name;
            _resourceHelper = new ResourceHelper(localizer);
        }

        [Route("wiki/{**slug}")]
        public IActionResult Article(string slug)
        {
            var cultureCookie = "ru";
            if (!string.IsNullOrEmpty(Request.Cookies[".AspNetCore.Culture"]))
                cultureCookie = Request.Cookies[".AspNetCore.Culture"];
            else
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

            var match = Regex.Match(cultureCookie, @"c=(?<culture>[a-zA-Z\-]+)");
            var cookieCulture = string.IsNullOrEmpty(match.Groups["culture"].Value) ? cultureCookie : match.Groups["culture"].Value;
            var route = NormalizeKnowledgeRoute(slug, cookieCulture);

            if (route.ShouldRedirect)
                return RedirectPermanent(route.CanonicalPath);

            var culture = route.Culture;
            var cultureInfo = new CultureInfo(culture);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            string fullPath = route.IsKnowledgeBaseRoute
                ? Path.Combine(_storagePaths.KnowledgeBase, culture, route.RelativePath.Replace('/', Path.DirectorySeparatorChar))
                : MarkdownPage.ValidateLink(ref slug, cultureCookie, _env.ContentRootPath, _storagePaths.KnowledgeBase);

            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            var seoBaseUrl = _seoDomains.GetBaseUrl(Request);
            ViewData["CanonicalUrl"] = $"{seoBaseUrl}{route.CanonicalPath}";
            SetAlternateWikiUrls(route, seoBaseUrl);

            var parsed = MarkdownPage.Parse(fullPath, _storagePaths.KnowledgeBase, new MdAlertData() {
                LocInfoName = _stringLocalizer["Shared_Info"],
                LocNoteName = _stringLocalizer["Shared_Note"],
                LocWarningName = _stringLocalizer["Shared_Caution"],
            });

            ViewData["Title"] = parsed.FrontMatter.TryGetValue("title", out var title) ? title : "База знаний";
            ViewData["Description"] = parsed.FrontMatter.TryGetValue("description", out var desc) ? desc : "";


            string knowledgeBaseRoot = _storagePaths.KnowledgeBase;

            var files = FileListCache.GetOrBuild(culture, Path.GetDirectoryName(fullPath), knowledgeBaseRoot);

            int currentIndex = files.FindIndex(f =>
                string.Equals(
                    Path.GetFullPath(f.FullPath),
                    Path.GetFullPath(fullPath),
                    StringComparison.OrdinalIgnoreCase));

            string prevPageSlug = currentIndex > 0 ? files[currentIndex - 1].Slug : null;
            string nextPageSlug = currentIndex < files.Count - 1 ? files[currentIndex + 1].Slug : null;

            WikiPage prevPage = null;
            WikiPage nextPage = null;

            if(!string.IsNullOrEmpty(prevPageSlug))
                prevPage = MarkdownPage.GetPageWiki(files[currentIndex - 1].FullPath, prevPageSlug);
            if (!string.IsNullOrEmpty(nextPageSlug))
                nextPage = MarkdownPage.GetPageWiki(files[currentIndex + 1].FullPath, nextPageSlug);

            ViewData["PrevPage"] = prevPage;
            ViewData["NextPage"] = nextPage;

            string knowledgeBaseMenuRoot = Path.Combine(_storagePaths.KnowledgeBase, culture);

            var allStrings = _resourceHelper.GetAllStrings("Views.Knowledge.MarkdownPage");
            var menuTree = KnowledgeMenuCache.GetOrBuild(culture, knowledgeBaseRoot, allStrings);

            ViewData["WikiMenu"] = menuTree;

            // рендерим смещения файлов из /payloads/crash-offsets.json
            if (slug == $"KnowledgeBase/{culture}/fl-binaries/crash-offsets.mdx")
            {
                // кешируем данные на сервере о crash-offsets.json или возвращаем их
                if (KnowledgeMenuCache.ExistCrashOffsets(culture))
                    parsed.HtmlContent += KnowledgeMenuCache.GetCrashOffsets(culture);
                else
                {
                    var parsedCrashed = MarkdownPage.GetPayloadsCrashOffsetHTML(_env.WebRootPath, culture, allStrings);
                    parsed.HtmlContent += parsedCrashed;
                    KnowledgeMenuCache.SetCrashOffsets(parsedCrashed, culture);
                }
            }

            // рендерим смещения файлов из /payloads/limit-breaking.json
            if (slug == $"KnowledgeBase/{culture}/fl-binaries/limit-breaking-v2.mdx")
            {
                // кешируем данные на сервере о limit-breaking.json или возвращаем их
                if (KnowledgeMenuCache.ExistBrLimits(culture))
                    parsed.HtmlContent += KnowledgeMenuCache.GetBrLimits(culture);
                else
                {
                    var parsedCrashed = MarkdownPage.GetPayloadsLimitsBreakingHTML(_env.WebRootPath, culture, allStrings);
                    parsed.HtmlContent += parsedCrashed;
                    KnowledgeMenuCache.SetBrLimits(parsedCrashed, culture);
                }
            }

            var model = new WikiPageViewModel
            {
                HtmlContent = parsed.HtmlContent,
                Title = ViewData["Title"]?.ToString(),
                Description = ViewData["Description"]?.ToString(),
                Url = $"{seoBaseUrl}{route.CanonicalPath}"
            };

            return View("MarkdownPage", model);
        }

        private void SetAlternateWikiUrls(KnowledgeRouteInfo route, string seoBaseUrl)
        {
            if (!route.IsKnowledgeBaseRoute)
                return;

            foreach (var culture in SupportedWikiCultures)
            {
                var relativePath = route.RelativePath.Replace('/', Path.DirectorySeparatorChar);
                var fullPath = Path.Combine(_storagePaths.KnowledgeBase, culture, relativePath);
                if (!System.IO.File.Exists(fullPath))
                    continue;

                var url = $"{seoBaseUrl}/wiki/KnowledgeBase/{culture}/{route.RelativePath}";
                if (culture == "ru")
                    ViewData["AlternateRuUrl"] = url;
                if (culture == "en")
                    ViewData["AlternateEnUrl"] = url;
            }

            if (ViewData["AlternateRuUrl"] is string ruUrl)
                ViewData["AlternateDefaultUrl"] = ruUrl;
        }

        private static KnowledgeRouteInfo NormalizeKnowledgeRoute(string slug, string culture)
        {
            var normalizedSlug = (slug ?? string.Empty).Trim('/');
            var normalizedCulture = NormalizeCulture(culture);
            var segments = normalizedSlug.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length == 0)
            {
                return new KnowledgeRouteInfo(
                    normalizedCulture,
                    "index.md",
                    $"/wiki/KnowledgeBase/{normalizedCulture}/index.md",
                    true,
                    false);
            }

            if (!segments[0].Equals("KnowledgeBase", StringComparison.OrdinalIgnoreCase))
            {
                return new KnowledgeRouteInfo(
                    normalizedCulture,
                    normalizedSlug,
                    $"/wiki/{normalizedSlug}",
                    false,
                    false);
            }

            if (segments.Length > 1 && IsSupportedWikiCulture(segments[1]))
            {
                var routeCulture = segments[1].ToLowerInvariant();
                var relativePath = string.Join('/', segments.Skip(2));
                if (string.IsNullOrWhiteSpace(relativePath))
                    relativePath = "index.md";

                return new KnowledgeRouteInfo(
                    routeCulture,
                    relativePath,
                    $"/wiki/KnowledgeBase/{routeCulture}/{relativePath}",
                    true,
                    false);
            }

            var fallbackRelativePath = string.Join('/', segments.Skip(1));
            if (string.IsNullOrWhiteSpace(fallbackRelativePath))
                fallbackRelativePath = "index.md";

            return new KnowledgeRouteInfo(
                normalizedCulture,
                fallbackRelativePath,
                $"/wiki/KnowledgeBase/{normalizedCulture}/{fallbackRelativePath}",
                true,
                true);
        }

        private static string NormalizeCulture(string culture)
        {
            return IsSupportedWikiCulture(culture) ? culture.ToLowerInvariant() : "ru";
        }

        private static bool IsSupportedWikiCulture(string culture)
        {
            return SupportedWikiCultures.Contains(culture?.ToLowerInvariant() ?? string.Empty);
        }

        private static readonly HashSet<string> SupportedWikiCultures = new(StringComparer.OrdinalIgnoreCase)
        {
            "ru",
            "en"
        };

        private sealed record KnowledgeRouteInfo(
            string Culture,
            string RelativePath,
            string CanonicalPath,
            bool IsKnowledgeBaseRoute,
            bool ShouldRedirect);
    }
}
