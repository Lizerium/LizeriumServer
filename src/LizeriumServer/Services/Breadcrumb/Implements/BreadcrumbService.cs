/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 23 августа 2026 07:14:40
 * Version: 1.0.154
 */

using System.Globalization;
using System.Text;
using System.Xml.Linq;

using LizeriumDatabase.Services.AppDataBaseService;
using LizeriumDatabase.Services.AppDataBaseService.Implements;

using LizeriumServer.FormatsData.AppSeo;
using LizeriumServer.FormatsData.AppWikiData;
using LizeriumServer.Options;

using LizeriumUtilities.Accessories.NewsAccessories;
using LizeriumUtilities.FormatsData.DataBase.Requests;

using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace LizeriumServer.Services.Breadcrumb.Implements
{
    // Узел дерева сайта
    public class BreadcrumbNode
    {
        public Language Title { get; set; }
        public string Url { get; set; }
        public List<BreadcrumbNode> Children { get; set; } = new List<BreadcrumbNode>();
        public BreadcrumbNode Parent { get; set; }
        public DateTime? LastModifiedUtc { get; set; }

        public string GetName(string culture)
       => culture switch
       {
           "ru" => Title.Russian,
           _ => Title.English
       };
    }

    public class BreadcrumbService : IBreadcrumbService
    {
        private readonly LinkGenerator _linkGenerator;
        private IServiceProvider _serviceProvider { get; set; }
        private readonly IWebHostEnvironment _env;
        private readonly StoragePathsOptions _storagePaths;
        private readonly SeoDomainsOptions _seoDomains;

        // Дерево сайта: URL -> Node
        private readonly Dictionary<string, BreadcrumbNode> _siteMap;

        public BreadcrumbService(LinkGenerator linkGenerator,
            IServiceProvider serviceProvider,
            IWebHostEnvironment env,
            IOptions<StoragePathsOptions> storagePathsOptions,
            IOptions<SeoDomainsOptions> seoDomainsOptions)
        {
            _linkGenerator = linkGenerator;
            _siteMap = new Dictionary<string, BreadcrumbNode>();
            _serviceProvider = serviceProvider;
            _env = env;
            _storagePaths = storagePathsOptions.Value;
            _seoDomains = seoDomainsOptions.Value;
        }

        /// <summary>
        /// Построение SEO для сайта (крошки + sitemap.xml)
        /// </summary>
        /// <returns></returns>
        public async Task BuildSiteMapAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var appDb = scope.ServiceProvider.GetRequiredService<IDataBaseService>();

            _siteMap.Clear();

            var nameIndexEn = "Lizerium - Main";
            var nameIndexRu = "Lizerium - Главная";
            // Главная
            var home = new BreadcrumbNode
            {
                Title = new Language()
                {
                    English = nameIndexEn,
                    Russian = nameIndexRu
                },
                Url = "/"
            };
            _siteMap[home.Url] = home;
            var CategoriesHook = await appDb.GetAllCommandCategoriesAsync();

            // Пример страниц
            AddNode("/Home/Error", new Language() { Russian = "Lizerium - Ошибка", English = "Lizerium - Error" }, home);
            AddNode("/Home/Launcher", new Language() { Russian = "Lizerium - Загрузчик", English = "Lizerium Steam" }, home);
            var launcherNode = _siteMap["/Home/Launcher"];
            AddNode("/Home/Game", new Language() { Russian = "Lizerium Mode - Игры", English = "Lizerium Mode - Games" }, home);
            AddNode("/docs/all", new Language() { Russian = "Lizerium - Информация", English = "Lizerium - Information" }, home);
            AddNode("/docs/install", new Language() { Russian = "Lizerium - Установка", English = "Lizerium - Installation" }, _siteMap["/docs/all"]);
            AddNode("/docs/builds", new Language() { Russian = "Lizerium - Создание предметов", English = "Lizerium - Crafting Items" }, _siteMap["/docs/all"]);
            AddNode("/docs/hook", new Language() { Russian = "Lizerium - Документация", English = "Lizerium - Documentation" }, _siteMap["/docs/all"]);
            foreach (var category in CategoriesHook)
                AddNode("/docs/hook/" + category.Key, new Language() { Russian = "Lizerium - " + category.NameRu, English = "Lizerium - " + category.NameEn }, _siteMap["/docs/hook"]);

            var launcherNews = await appDb.GetPublishedLauncherNewsAsync();
            foreach (var news in launcherNews.OrderByDescending(item => item.PublishedAtUnix).ThenBy(item => item.SortOrder))
            {
                var titleRu = string.IsNullOrWhiteSpace(news.TitleRu) ? news.TitleEn : news.TitleRu;
                var titleEn = string.IsNullOrWhiteSpace(news.TitleEn) ? news.TitleRu : news.TitleEn;
                var lastModifiedUtc = news.PublishedAtUnix > 0
                    ? DateTimeOffset.FromUnixTimeSeconds(news.PublishedAtUnix).UtcDateTime
                    : (DateTime?)null;

                AddNode(
                    news.GetCanonicalNewsPath("ru"),
                    new Language
                    {
                        Russian = $"Lizerium - {titleRu}",
                        English = $"Lizerium - {titleEn}"
                    },
                    launcherNode,
                    lastModifiedUtc);
            }

            // Узел Wiki
            var wikiNode = new BreadcrumbNode
            {
                Url = "/wiki",
                Title = new Language
                {
                    Russian = "База знаний о Freelancer (2003)",
                    English = "The Freelancer Knowledge Base"
                },
                Parent = _siteMap["/docs/all"]
            };
            _siteMap["/docs/all"].Children.Add(wikiNode);
            _siteMap[wikiNode.Url] = wikiNode;

            var siteMapRu = MarkdownPage.ScanMarkdownFiles(Path.Combine(_storagePaths.KnowledgeBase, "ru"), "/wiki/KnowledgeBase/ru");
            var siteMapEn = MarkdownPage.ScanMarkdownFiles(Path.Combine(_storagePaths.KnowledgeBase, "en"), "/wiki/KnowledgeBase/en");

            var enDict = siteMapEn.ToDictionary(
                    p => p.Slug.Substring("/wiki/KnowledgeBase/en".Length),
                    p => p.Title
                );
            var ruDict = siteMapRu.ToDictionary(
                    p => p.Slug.Substring("/wiki/KnowledgeBase/ru".Length),
                    p => p.Title
                );
            foreach (var pageRu in siteMapRu)
            {
                var key = pageRu.Slug.Substring("/wiki/KnowledgeBase/ru".Length);
                var titleEn = enDict.TryGetValue(key, out var tEn) ? tEn : pageRu.Title;

                var node = new BreadcrumbNode
                {
                    Url = pageRu.Slug,
                    Title = new Language { Russian = pageRu.Title, English = titleEn },
                    Parent = wikiNode
                };

                _siteMap[wikiNode.Url].Children.Add(node);
                node.Parent = wikiNode;
                _siteMap[node.Url] = node;
            }

            foreach (var pageEn in siteMapEn)
            {
                var key = pageEn.Slug.Substring("/wiki/KnowledgeBase/en".Length);
                var titleRu = ruDict.TryGetValue(key, out var tEn) ? tEn : pageEn.Title;

                var node = new BreadcrumbNode
                {
                    Url = pageEn.Slug,
                    Title = new Language { Russian = titleRu, English = pageEn.Title },
                    Parent = wikiNode
                };

                _siteMap[wikiNode.Url].Children.Add(node);
                node.Parent = wikiNode;
                _siteMap[node.Url] = node;
            }

            // --- Генерация sitemap.xml ---
            GenerateSitemapXml(_siteMap.Values);
        }

        private void GenerateSitemapXml(IEnumerable<BreadcrumbNode> nodes)
        {
            var sitemapPath = Path.Combine(GetWritableWebRootPath(), "sitemap.xml");
            File.WriteAllText(sitemapPath, GetSitemapXml(_seoDomains.GetPrimaryBaseUrl()), Encoding.UTF8);
            GenerateRobotsTxt();
        }

        public string GetSitemapXml(string baseUrl)
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

            var xmlDoc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset")
            );

            var normalizedBaseUrl = (baseUrl ?? _seoDomains.GetPrimaryBaseUrl()).TrimEnd('/');
            var seenUrls = new HashSet<string>();

            void AddNodeRecursive(BreadcrumbNode node)
            {
                if (!seenUrls.Add(node.Url)) return;

                xmlDoc.Root.Add(new XElement(ns + "url",
                    new XElement(ns + "loc", $"{normalizedBaseUrl}{node.Url}"),
                    new XElement(ns + "lastmod", (node.LastModifiedUtc ?? DateTime.UtcNow).ToString("yyyy-MM-dd")),
                    new XElement(ns + "changefreq", "weekly"),
                    new XElement(ns + "priority", node.Url == "/" ? "1.0" : "0.8")
                ));

                foreach (var child in node.Children)
                    AddNodeRecursive(child);
            }

            foreach (var rootNode in _siteMap.Values.Where(n => n.Parent == null))
                AddNodeRecursive(rootNode);

            return xmlDoc.ToString(SaveOptions.DisableFormatting);
        }

        private void GenerateRobotsTxt()
        {
            var robotsPath = Path.Combine(GetWritableWebRootPath(), "robots.txt");
            File.WriteAllText(robotsPath, GetRobotsTxt(_seoDomains.GetPrimaryBaseUrl()), Encoding.UTF8);
        }

        private string GetWritableWebRootPath()
        {
            var webRootPath = _env.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                var contentRootPath = string.IsNullOrWhiteSpace(_env.ContentRootPath)
                    ? AppContext.BaseDirectory
                    : _env.ContentRootPath;

                webRootPath = Path.Combine(contentRootPath, "wwwroot");
            }

            Directory.CreateDirectory(webRootPath);
            return webRootPath;
        }

        public string GetRobotsTxt(string baseUrl)
        {
            var normalizedBaseUrl = (baseUrl ?? _seoDomains.GetPrimaryBaseUrl()).TrimEnd('/');

            return new StringBuilder()
                .AppendLine("User-agent: *")
                .AppendLine("Disallow: /error")
                .AppendLine("Allow: /")
                .AppendLine()
                .AppendLine($"Sitemap: {normalizedBaseUrl}/sitemap.xml")
                .ToString();
        }


        private void AddNode(string url, Language title, BreadcrumbNode parent, DateTime? lastModifiedUtc = null)
        {
            var node = new BreadcrumbNode
            {
                Url = url,
                Title = title,
                Parent = parent,
                LastModifiedUtc = lastModifiedUtc
            };
            parent.Children.Add(node);
            _siteMap[url] = node;
        }

        public List<BreadcrumbItem> GetBreadcrumbs(RouteData routeData, IViewLocalizer localizer, ViewDataDictionary viewData)
        {
            var breadcrumbs = new List<BreadcrumbItem>();
            var nameIndexEn = "Lizerium - Main";
            var nameIndexRu = "Lizerium - Главная";
            var homeName = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch
            {
                "ru" => nameIndexRu,
                _ => nameIndexEn
            };
            // Главная
            breadcrumbs.Add(new BreadcrumbItem
            {
                Name = homeName,
                Url = "/"
            });

            // Текущий URL
            var currentPath = routeData.Values["controller"]?.ToString() == "Wiki"
                ? "/" + string.Join("/", routeData.Values.Values)
                : routeData.Values["controller"]?.ToString() != null
                    ? _linkGenerator.GetPathByAction(routeData.Values["action"]?.ToString(), routeData.Values["controller"]?.ToString(), routeData.Values)
                    : "/";
            currentPath = string.IsNullOrWhiteSpace(currentPath) ? "/" : currentPath;

            if (_siteMap.TryGetValue(currentPath, out var node))
            {
                // Двигаемся к родителям, пропуская главную
                var stack = new Stack<BreadcrumbNode>();
                var temp = node;
                while (temp != null && temp.Url != "/")
                {
                    stack.Push(temp);
                    temp = temp.Parent;
                }

                var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

                while (stack.Count > 0)
                {
                    var n = stack.Pop();
                    breadcrumbs.Add(new BreadcrumbItem
                    {
                        Name = n.GetName(culture),
                        Url = n.Url
                    });
                }
            }
            else
            {
                // fallback: берём заголовок страницы из ViewData
                string pageTitle = viewData["Title"] is LocalizedHtmlString localized
                    ? localized.Value
                    : viewData["Title"] as string;

                if (!string.IsNullOrEmpty(pageTitle) && pageTitle != homeName)
                {
                    breadcrumbs.Add(new BreadcrumbItem
                    {
                        Name = pageTitle,
                        Url = currentPath
                    });
                }
            }

            return breadcrumbs;
        }
    }
}
