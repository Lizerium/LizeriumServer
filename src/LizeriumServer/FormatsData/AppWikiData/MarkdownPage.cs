/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 29 июля 2026 16:02:04
 * Version: 1.0.125
 */

using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

using Markdig;

using Microsoft.Extensions.FileProviders;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LizeriumServer.FormatsData.AppWikiData;

public class MarkdownPage
{
    public Dictionary<string, string> FrontMatter { get; set; } = new();
    public string HtmlContent { get; set; } = "";

    public static MarkdownPage Parse(string fullPath, string rootPath, MdAlertData stringMdAlertData)
    {
        var baseRoute = Path.GetDirectoryName(fullPath)?
            .Replace(rootPath, "")
            .Replace(Path.DirectorySeparatorChar, '/')
            .Trim('/');

        Dictionary<string, string> frontMatter;
        string html;
        ConvertMDAndYamlToHTML(fullPath, out frontMatter, out html);

        html = SetupLinks(html, baseRoute);
        html = PanelCreatorH1(html);
        html = ReplaceH1(html);
        html = PanelCreatorH2(html);
        html = PanelCreatorH3(html);
        html = PanelCreatorH4(html);
        html = PanelCreatorH5(html);
        html = PanelCreatorH6(html);
        html = PrependRandomEmojiToHeaders(html);
        html = CreateAlertsWarning(html, stringMdAlertData);
        html = CreateNotesWarning(html, stringMdAlertData);
        html = CreateNotesInfo(html, stringMdAlertData);
        html = WrapTableColumn(html, "Смещение", "patch-block");
        html = WrapTableColumn(html, "Offset", "patch-block");

        return new MarkdownPage
        {
            FrontMatter = frontMatter,
            HtmlContent = html
        };
    }

    public static string WrapTableColumn(string html, string columnName, string wrapperClass)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // Проходим по всем таблицам
        var tables = doc.DocumentNode.SelectNodes("//table");
        if (tables != null)
        {
            foreach (var table in tables)
            {
                var headers = table.SelectNodes(".//th");
                if (headers == null) continue;

                int targetIndex = -1;
                for (int i = 0; i < headers.Count; i++)
                {
                    if (headers[i].InnerText.Trim() == columnName)
                    {
                        targetIndex = i;
                        break;
                    }
                }
                if (targetIndex == -1) continue;

                var rows = table.SelectNodes(".//tr[td]");
                if (rows == null) continue;

                foreach (var row in rows)
                {
                    var cells = row.SelectNodes("./td");
                    if (cells.Count > targetIndex)
                    {
                        cells[targetIndex].AddClass("custom-cursor-hover");
                        var splitBr = cells[targetIndex].InnerHtml.Split("<br>");
                        // оборачиваем каждый кусок в div с нужным классом
                        var wrapped = splitBr
                            .Select(s => $"<div class=\"{wrapperClass}\">{s}</div>")
                            .ToArray();
                        cells[targetIndex].InnerHtml = $"{string.Join("", wrapped)}";
                    }
                }
            }
        }

        return doc.DocumentNode.OuterHtml;
    }

    public static string CreateNotesInfo(string html, MdAlertData stringMdAlertData)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var cautionDivs = doc.DocumentNode.SelectNodes("//div[contains(@class,'info')]");

        if (cautionDivs != null)
        {
            foreach (var caution in cautionDivs)
            {
                var originalContent = caution.InnerHtml;

                caution.InnerHtml = $@"
                <div class=""markdown_alert_badge"">
                <svg viewBox=""0 0 16 16"" class=""attention-icon attention-info svg octicon-light-bulb"" aria-hidden=""true"" width=""16"" height=""16""><path d=""M8 1.5c-2.363 0-4 1.69-4 3.75 0 .984.424 1.625.984 2.304l.214.253c.223.264.47.556.673.848.284.411.537.896.621 1.49a.75.75 0 0 1-1.484.211c-.04-.282-.163-.547-.37-.847a9 9 0 0 0-.542-.68q-.126-.149-.268-.32C3.201 7.75 2.5 6.766 2.5 5.25 2.5 2.31 4.863 0 8 0s5.5 2.31 5.5 5.25c0 1.516-.701 2.5-1.328 3.259q-.142.172-.268.319c-.207.245-.383.453-.541.681-.208.3-.33.565-.37.847a.751.751 0 0 1-1.485-.212c.084-.593.337-1.078.621-1.489.203-.292.45-.584.673-.848q.113-.133.213-.253c.561-.679.985-1.32.985-2.304 0-2.06-1.637-3.75-4-3.75M5.75 12h4.5a.75.75 0 0 1 0 1.5h-4.5a.75.75 0 0 1 0-1.5M6 15.25a.75.75 0 0 1 .75-.75h2.5a.75.75 0 0 1 0 1.5h-2.5a.75.75 0 0 1-.75-.75""></path></svg><strong class=""attention-info"">{stringMdAlertData.LocInfoName}</strong>
                </div>
                <div class=""markdown_alert_content_info"">
                {originalContent}
                </div>";
            }
        }
        return doc.DocumentNode.OuterHtml;
    }

    public static string CreateNotesWarning(string html, MdAlertData stringMdAlertData)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var cautionDivs = doc.DocumentNode.SelectNodes("//div[contains(@class,'note')]");

        if (cautionDivs != null)
        {
            foreach (var caution in cautionDivs)
            {
                var originalContent = caution.InnerHtml;

                caution.InnerHtml = $@"
                <div class=""markdown_alert_badge"">
                <svg viewBox=""0 0 16 16"" class=""attention-icon attention-note svg octicon-info"" aria-hidden=""true"" width=""16"" height=""16""><path d=""M0 8a8 8 0 1 1 16 0A8 8 0 0 1 0 8m8-6.5a6.5 6.5 0 1 0 0 13 6.5 6.5 0 0 0 0-13M6.5 7.75A.75.75 0 0 1 7.25 7h1a.75.75 0 0 1 .75.75v2.75h.25a.75.75 0 0 1 0 1.5h-2a.75.75 0 0 1 0-1.5h.25v-2h-.25a.75.75 0 0 1-.75-.75M8 6a1 1 0 1 1 0-2 1 1 0 0 1 0 2""></path></svg><strong class=""attention-note"">{stringMdAlertData.LocNoteName}</strong>
                </div>
                <div class=""markdown_alert_content_note"">
                {originalContent}
                </div>";
            }
        }
        return doc.DocumentNode.OuterHtml;
    }

    public static string CreateAlertsWarning(string html, MdAlertData stringMdAlertData)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var cautionDivs = doc.DocumentNode.SelectNodes("//div[contains(@class,'caution')]");

        if (cautionDivs != null)
        {
            foreach (var caution in cautionDivs)
            {
                var originalContent = caution.InnerHtml;

                caution.InnerHtml = $@"
                <div class=""markdown_alert_badge"">
                <svg viewBox=""0 0 16 16"" class=""attention-icon attention-warning svg octicon-alert"" aria-hidden=""true"" width=""16"" height=""16""><path d=""M6.457 1.047c.659-1.234 2.427-1.234 3.086 0l6.082 11.378A1.75 1.75 0 0 1 14.082 15H1.918a1.75 1.75 0 0 1-1.543-2.575Zm1.763.707a.25.25 0 0 0-.44 0L1.698 13.132a.25.25 0 0 0 .22.368h12.164a.25.25 0 0 0 .22-.368Zm.53 3.996v2.5a.75.75 0 0 1-1.5 0v-2.5a.75.75 0 0 1 1.5 0M9 11a1 1 0 1 1-2 0 1 1 0 0 1 2 0""></path></svg><strong class=""attention-warning"">{stringMdAlertData.LocWarningName}</strong>
                </div>
                <div class=""markdown_alert_content_warning"">
                {originalContent}
                </div>";
            }
        }
        return doc.DocumentNode.OuterHtml;
    }

    public static string PanelCreatorH1(string html)
    {
        string pattern = @"(<h1.*?>.*?</h1>)([\s\S]*?)(?=<h1|$)";
        string result = Regex.Replace(html, pattern, m =>
        {
            return $"{m.Groups[1].Value}<div class=\"knowledge_panel_1\">{m.Groups[2].Value}</div>";
        }, RegexOptions.IgnoreCase);

        return result;
    }

    public static string PanelCreatorH2(string html)
    {
        string pattern = @"(<h2.*?>.*?</h2>)([\s\S]*?)(?=<h[1-2]|$)";
        string result = Regex.Replace(html, pattern, m =>
        {
            return $"{m.Groups[1].Value}<div class=\"knowledge_panel_2\">{m.Groups[2].Value}</div>";
        }, RegexOptions.IgnoreCase);

        return result;
    }

    public static string PanelCreatorH3(string html)
    {
        string pattern = @"(<h3.*?>.*?</h3>)([\s\S]*?)(?=<h[1-3]|$)";
        string result = Regex.Replace(html, pattern, m =>
        {
            return $"{m.Groups[1].Value}<div class=\"knowledge_panel_3\">{m.Groups[2].Value}</div>";
        }, RegexOptions.IgnoreCase);

        return result;
    }

    public static string PanelCreatorH4(string html)
    {
        string pattern = @"(<h4.*?>.*?</h4>)([\s\S]*?)(?=<h[1-4]|$)";
        string result = Regex.Replace(html, pattern, m =>
        {
            return $"{m.Groups[1].Value}<div class=\"knowledge_panel_4\">{m.Groups[2].Value}</div>";
        }, RegexOptions.IgnoreCase);

        return result;
    }

    public static string PanelCreatorH5(string html)
    {
        string pattern = @"(<h5.*?>.*?</h5>)([\s\S]*?)(?=<h[1-5]|$)";
        string result = Regex.Replace(html, pattern, m =>
        {
            return $"{m.Groups[1].Value}<div class=\"knowledge_panel_5\">{m.Groups[2].Value}</div>";
        }, RegexOptions.IgnoreCase);

        return result;
    }

    public static string PanelCreatorH6(string html)
    {
        string pattern = @"(<h6.*?>.*?</h6>)([\s\S]*?)(?=<h[1-6]|$)";
        string result = Regex.Replace(html, pattern, m =>
        {
            return $"{m.Groups[1].Value}<div class=\"knowledge_panel_6\">{m.Groups[2].Value}</div>";
        }, RegexOptions.IgnoreCase);

        return result;
    }

    public static string ValidateLink(ref string slug, string cultureCookie, string contentRootPath, string knowledgeBasePath = "")
    {
        var locale = "ru"; // значение по умолчанию

        if (!string.IsNullOrEmpty(cultureCookie))
        {
            // Пытаемся извлечь культуру, например: c=ru|uic=ru
            var match = Regex.Match(cultureCookie, @"c=(?<culture>[a-zA-Z\-]+)");
            if (match.Success)
            {
                locale = match.Groups["culture"].Value;
            }
            if (!string.IsNullOrEmpty(slug))
            {
                var segments = slug.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length > 1)
                {
                    var firstSegment = segments[1].ToLower();
                    if (firstSegment != locale)
                    {
                        segments[1] = locale.ToLower();
                        // Собираем обратно в строку с '/'
                        slug = string.Join('/', segments);
                    }
                }
            }
        }

        var path = string.IsNullOrEmpty(slug) ? Path.Combine(locale, "index.md") : Path.Combine(locale, slug);
        string fullPath = "";
        if (string.IsNullOrEmpty(slug))
            fullPath = Path.Combine(knowledgeBasePath, path.Replace('/', Path.DirectorySeparatorChar));
        else
        {
            if(slug.Contains("KnowledgeBase"))
            {
                var index = slug.IndexOf("/") + 1;
                var np = slug.Substring(index);
                fullPath = Path.Combine(knowledgeBasePath, np);
            }
            else  fullPath = Path.Combine(contentRootPath, slug);
        }
        return fullPath;
    }

    public static void ConvertMDAndYamlToHTML(string fullPath, out Dictionary<string, string> frontMatter, out string html)
    {
        var markdown = File.ReadAllText(fullPath);

        frontMatter = new Dictionary<string, string>();
        var contentWithoutYaml = markdown;

        if (markdown.StartsWith("---"))
        {
            var endYaml = markdown.IndexOf("---", 3);
            if (endYaml != -1)
            {
                var yamlBlock = markdown.Substring(3, endYaml - 3).Trim();
                contentWithoutYaml = markdown.Substring(endYaml + 3).Trim();

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(UnderscoredNamingConvention.Instance)
                    .Build();

                frontMatter = deserializer.Deserialize<Dictionary<string, string>>(yamlBlock);
            }
        }

        // После парсинга YAML и перед генерацией HTML:
        if (!frontMatter.ContainsKey("title"))
        {
            var match = Regex.Match(contentWithoutYaml, @"^#\s*(.+)", RegexOptions.Multiline);
            if (match.Success)
            {
                frontMatter["title"] = match.Groups[1].Value.Trim();
            }
        }

        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        html = Markdown.ToHtml(contentWithoutYaml, pipeline);
        if (frontMatter.TryGetValue("title", out var title))
        {
            html = $"<h1>{title}</h1>\n" + html;
        }
    }

    public static List<WikiPage> ScanMarkdownFiles(string rootPath, string baseUrl)
    {
        var result = new List<WikiPage>();

        foreach (var file in Directory.EnumerateFiles(rootPath, "*.md", SearchOption.AllDirectories)
                                     .Concat(Directory.EnumerateFiles(rootPath, "*.mdx", SearchOption.AllDirectories)))
        {
            ConvertMDAndYamlToHTML(file, out var frontMatter, out _);
            var title = frontMatter.TryGetValue("title", out var t) ? t : Path.GetFileNameWithoutExtension(file);

            // Преобразуем путь в URL: относительно корня портала
            var relativePath = Path.GetRelativePath(rootPath, file).Replace("\\", "/");
            var url = baseUrl.TrimEnd('/') + "/" + relativePath;

            result.Add(new WikiPage
            {
                Slug = url,
                Title = title
            });
        }

        return result;
    }

    public static string ReplaceH1(string html)
    {
        var techEmojis = new[]
        {
            "💻", "🧠", "🛠️", "🧪", "📡", "📱", "🖥️", "📊", "📦", "🔐",
            "🗂️", "🛰️", "🔧", "⚙️", "💡", "🧬", "📘", "🌐", "📝", "🔭",
            "💾", "🖱️", "⌨️", "🖨️", "🧮", "📟", "📠", "📺", "🎛️", "🎚️",
            "📷", "📸", "🎥", "🔬", "🧫", "🧯", "💡", "⚡", "🪐", "🚀",
            "🛸", "🗜️", "🔋", "🧲", "🔌", "💿", "📀", "📡", "🧹", "🧼",
            "🛡️", "🧱", "📈", "📉", "📇", "🖇️", "🗃️", "🧾", "🔎", "🧩"
        };


        var random = new Random();
        // Заменяем все h1
        html = Regex.Replace(
            html,
            @"<h1[^>]*>(.*?)<\/h1>",
            match =>
            {
                var titleText = match.Groups[1].Value.Trim();
                var emoji = techEmojis[random.Next(techEmojis.Length)];

                return $@"
                <div class=""card-row small"">
                    <div class=""subcard"">
                        <div class=""subcard-icon icon-style"">{emoji}</div>
                        <div class=""subcard-text"">{titleText}</div>
                    </div>
                </div>";
            },
            RegexOptions.IgnoreCase | RegexOptions.Singleline
        );
        return html;
    }

    public static string PrependRandomEmojiToHeaders(string html, Random? rng = null)
    {
        if (string.IsNullOrWhiteSpace(html)) return html;

        string[] emojis = { "🔥", "⚡", "📌", "📎", "🔭", "💢", "💨", "💥" };
        rng ??= new Random();

        // Учтём, что в теге могут быть атрибуты, например <h2 id="section">
        return Regex.Replace(html, @"<(h[2-6])(\s[^>]*)?>(.*?)</\1>", match =>
        {
            string tag = match.Groups[1].Value;           // h2, h3, ...
            string attrs = match.Groups[2].Value ?? "";   // атрибуты, если есть
            string content = match.Groups[3].Value;

            if (Regex.IsMatch(content, @"^\s*(🔥|⚡|📌|📎|🔭|💢|💨|💥)"))
                return match.Value;

            string randomEmoji = emojis[rng.Next(emojis.Length)];
            return $"<{tag}{attrs}>{randomEmoji} {content}</{tag}>";
        }, RegexOptions.Singleline | RegexOptions.IgnoreCase);
    }

    public static string SetupLinks(string html, string baseRoute)
    {
        html = Regex.Replace(
                html,
                "<a\\s+href=[\"'](\\.?\\.?/[^\"']+?)(\\.mdx?|\\.md)(#[^\"']*)?[\"']",
                match =>
                {
                    var relativePath = match.Groups[1].Value;
                    var extension = match.Groups[2].Value; // ← сохраняем .md/.mdx
                    var anchor = match.Groups[3].Success ? match.Groups[3].Value : "";

                    var combinedPath = Path.Combine(baseRoute ?? "", relativePath + extension)
                        .Replace("\\", "/")
                        .Replace("//", "/")
                        .TrimStart('/');

                    // Сохраняем KnowledgeBase/en в пути
                    return $"<a href=\"/wiki/{combinedPath}{anchor}\"";
                },
                RegexOptions.IgnoreCase
            );
        return html;
    }

    public static WikiPage GetPageWiki(string path, string slug)
    {
        var content = File.ReadAllText(path);
        var fm = MarkdownPage.ParseFrontMatter(content);

        return new WikiPage
        {
            Slug = slug,
            Title = fm.TryGetValue("title", out var t) ? t : Path.GetFileNameWithoutExtension(path)
        };
    }

    public static Dictionary<string, string> ParseFrontMatter(string markdown)
    {
        var frontMatter = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(markdown))
            return frontMatter;

        if (markdown.StartsWith("---"))
        {
            // Находим конец блока YAML
            var endYaml = markdown.IndexOf("---", 3);
            if (endYaml != -1)
            {
                var yamlBlock = markdown.Substring(3, endYaml - 3).Trim();

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(UnderscoredNamingConvention.Instance)
                    .Build();

                try
                {
                    var yamlDict = deserializer.Deserialize<Dictionary<string, string>>(yamlBlock);
                    if (yamlDict != null)
                        frontMatter = yamlDict;
                }
                catch
                {
                    // Игнорируем ошибки парсинга YAML
                }
            }
        }

        // Если нет title в YAML, пробуем взять из первого h1
        if (!frontMatter.ContainsKey("title"))
        {
            var match = Regex.Match(markdown, @"^#\s*(.+)", RegexOptions.Multiline);
            if (match.Success)
                frontMatter["title"] = match.Groups[1].Value.Trim();
        }

        return frontMatter;
    }

    public static WikiPage BuildMenu(string rootPath, Dictionary<string, string> folderTitles = null, string relativePath = "")
    {
        var currentDir = Path.Combine(rootPath, relativePath);

        var folderTitle = Path.GetFileName(currentDir);
        // подменяем тайтлами из словаря если они были зашиты в таблице локализации (папки должны иметь свои названия)
        if (folderTitles != null && folderTitles.TryGetValue(folderTitle, out var localizedTitle))
            folderTitle = localizedTitle;

        // Пропускаем папку  "payloads")
        if (folderTitle.Equals("payloads", StringComparison.OrdinalIgnoreCase))
            return null;

        var menu = new WikiPage
        {
            Slug = relativePath.Replace("\\", "/"),
            Title = folderTitle // для папки
        };

        // Рекурсивно добавляем подпапки
        foreach (var dir in Directory.GetDirectories(currentDir))
        {
            var dirName = Path.GetFileName(dir);
            var childMenu = BuildMenu(rootPath, folderTitles, Path.Combine(relativePath, dirName));
            menu.Children.Add(childMenu);
        }

        // Получаем файлы .md и .mdx текущей папки
        var files = Directory.GetFiles(currentDir, "*.*", SearchOption.TopDirectoryOnly)
                             .Where(f => f.EndsWith(".md") || f.EndsWith(".mdx"))
                             .Select(f =>
                             {
                                 string content = File.ReadAllText(f); // читаем содержимое файла
                                 var fm = MarkdownPage.ParseFrontMatter(content); // парсим front matter

                                 // если есть title — используем, иначе имя файла
                                 var title = fm.TryGetValue("title", out var t) ? t : Path.GetFileNameWithoutExtension(f);

                                 return new WikiPage
                                 {
                                     Slug = Path.Combine(relativePath, Path.GetFileName(f)).Replace("\\", "/"),
                                     Title = title
                                 };
                             })
                             .OrderBy(f => f.Slug)
                             .ToList();

        menu.Children.AddRange(files);

        return menu;
    }

    public static string GetPayloadsCrashOffsetHTML(string rootPath, string culture, Dictionary<string, string> allStrings)
    {
        string jsonPath = Path.Combine(rootPath, "payloads", "crash-offsets.json");
        if (File.Exists(jsonPath))
        {
            var json = File.ReadAllText(jsonPath);
            var records = JsonSerializer.Deserialize<List<CrashOffset>>(json);

            var techEmojis = new[]
            {
                "💻", "🧠", "🛠️", "🧪", "📡", "📱", "🖥️", "📊", "📦", "🔐",
                "🛡️", "🧱", "📈", "📉", "📇", "🖇️", "🗃️", "🧾", "🔎", "🧩"
            };
            var random = new Random();

            if (records != null)
            {
                var grouped = records
                    .GroupBy(r => r.ModuleName)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Генерим HTML таблиц/вкладок
                var sb = new StringBuilder();
                sb.AppendLine("<div class=\"knowledge_panel_1\">");

                sb.AppendLine("<ul id=\"builds_category\" class=\"nav nav-tabs d-flex " +
                    "align-items-center justify-content-center " +
                    "align-content-center flex-row flex-wrap\" role=\"tablist\">");
                string nameActivate = "coomon.dll";
                foreach (var dllGroup in grouped)
                {
                    sb.AppendLine("<li class=\"nav-item\">\r\n                        " +
                        "<button class=\"nav-link\" " +
                        "data-bs-toggle=\"tab\" " +
                        $"data-bs-target=\"#{Path.GetFileNameWithoutExtension(dllGroup.Key)}\" " +
                        $"type=\"button\">{dllGroup.Key}</button>\r\n               " +
                        "</li>");
                }
                sb.AppendLine("</ul>");

                sb.AppendLine("<div class=\"tab-content mt-4\" id=\"buildsTabsContent\">");
                foreach (var dllGroup in grouped)
                {
                    var emoji = techEmojis[random.Next(techEmojis.Length)];
                    sb.AppendLine($"<div class=\"tab-pane fade\" id=\"{Path.GetFileNameWithoutExtension(dllGroup.Key)}\">");
                    sb.AppendLine("<div class=\"knowledge_panel_2\">");
                    sb.AppendLine($"<h2>{emoji} {dllGroup.Key}</h2>");
                    sb.AppendLine("<table>");
                    sb.AppendLine($"<thead><tr><th>{allStrings["Offset_Table"]}</th><th>{allStrings["Author_Table"]}</th><th>{allStrings["Description_Table"]}</th><th>{allStrings["Date_Table"]}</th></tr></thead>");
                    sb.AppendLine("<tbody>");

                    foreach (var rec in dllGroup.Value)
                    {
                        string desc = (culture == "en") ? rec.Description.English : rec.Description.Russian;
                        long ts = rec.DateAdded;

                        // если 13 цифр — миллисекунды, если 10 — секунды
                        var dto = ts > 9_999_999_999
                            ? DateTimeOffset.FromUnixTimeMilliseconds(ts)
                            : DateTimeOffset.FromUnixTimeSeconds(ts);

                        string date = dto.ToString("yyyy-MM-dd");

                        sb.AppendLine($"<tr><td class=\"custom-cursor-hover\"><div class=\"patch-block\">{rec.Offset}</div></td><td>{rec.Author}</td><td class=\"p-3 fw-bold\">{desc}</td><td>{date}</td></tr>");
                    }

                    sb.AppendLine("</tbody></table>");
                    sb.AppendLine("</div>");
                    sb.AppendLine("</div>");
                }
                sb.AppendLine("</div>");

                sb.AppendLine("</div>");
                return sb.ToString();
            }
        }
        return "";
    }

    public static string GetPayloadsLimitsBreakingHTML(string rootPath, string culture, Dictionary<string, string> allStrings)
    {
        string jsonPath = Path.Combine(rootPath, "payloads", "limit-breaking.json");
        if (File.Exists(jsonPath))
        {
            var json = File.ReadAllText(jsonPath);
            var records = JsonSerializer.Deserialize<List<LimitsBreaking>>(json);

            var techEmojis = new[]
            {
                "💻", "🧠", "🛠️", "🧪", "📡", "📱", "🖥️", "📊", "📦", "🔐",
                "🛡️", "🧱", "📈", "📉", "📇", "🖇️", "🗃️", "🧾", "🔎", "🧩"
            };
            var random = new Random();

            if (records != null)
            {
                var grouped = records
                     .SelectMany(r => r.ModuleName.Select(m => new { Module = m, Record = r }))
                     .GroupBy(x => x.Module, StringComparer.OrdinalIgnoreCase) // игнорируем регистр
                     .ToDictionary(g => g.Key, g => g.Select(x => x.Record).ToList());

                // Генерим HTML таблиц/вкладок
                var sb = new StringBuilder();
                sb.AppendLine("<div class=\"knowledge_panel_1\">");

                // Собираем уникальные категории из всех записей
                var allCategories = grouped
                    .SelectMany(g => g.Value.SelectMany(r => (culture == "en") ? r.Categories.English : r.Categories.Russian ?? Array.Empty<string>()))
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                // Кнопка для открытия панели фильтров
                sb.AppendLine("<div class='d-flex justify-content-end mb-2'>");
                sb.AppendLine("<button class='btn btn-sm btn-filters btn-outline-primary' type='button' data-bs-toggle='collapse' data-bs-target='#filtersPanel' aria-expanded='false' aria-controls='filtersPanel'>");
                sb.AppendLine(allStrings["Filter_Btn_Table"]);
                sb.AppendLine("</button>");
                sb.AppendLine("</div>");

                // Скрытая панель с фильтрами
                sb.AppendLine("<div class='collapse' id='filtersPanel'>");
                sb.AppendLine("<div class='card card-body mb-3 p-2'>"); // можно card для красивого фона

                sb.AppendLine("<div class=\"category-filters mb-3\">");
                foreach (var cat in allCategories)
                {
                    sb.AppendLine(
                        $"<div class=\"form-check form-switch\">" +
                        $"<input class=\"form-check-input category-checkbox\" type=\"checkbox\" value=\"{cat}\" id=\"cat_{cat}\" checked>" +
                        $"<label class=\"form-check-label\" for=\"cat_{cat}\">{cat}</label>" +
                        $"</div>"
                    );
                }
                // Чекбокс "Выбрать все" категорий
                sb.AppendLine("<div class=\"form-check form-switch\">");
                sb.AppendLine("<input type='checkbox' class='form-check-input category-checkbox category-checkbox-all' id='selectAllCategories' checked>");
                sb.AppendLine($"<label for='selectAllCategories'>{allStrings["AllCategories_Select_Table"]}</label>");
                sb.AppendLine("</div>");
                sb.AppendLine("</div>");

                sb.AppendLine("</div>"); // card-body
                sb.AppendLine("</div>"); // collapse

                sb.AppendLine("<ul id=\"builds_category\" class=\"nav nav-tabs d-flex " +
                    "align-items-center justify-content-center " +
                    "align-content-center flex-row flex-wrap\" role=\"tablist\">");
                string nameActivate = "coomon.dll";
                foreach (var dllGroup in grouped)
                {
                    var dllId = Path.GetFileNameWithoutExtension(dllGroup.Key);

                    sb.AppendLine("<li class=\"nav-item\">\r\n                        " +
                        "<button class=\"nav-link\" " +
                        "data-bs-toggle=\"tab\" " +
                        $"data-bs-target=\"#{Path.GetFileNameWithoutExtension(dllGroup.Key)}\" " +
                        $"type=\"button\">{dllGroup.Key}<span class=\"tab-count\" id=\"count-{dllId}\">{dllGroup.Value.Count}</span></button>\r\n               " +
                        "</li>");
                }
                sb.AppendLine("</ul>");

                sb.AppendLine("<div class=\"tab-content mt-4\" id=\"buildsTabsContent\">");
                foreach (var dllGroup in grouped)
                {
                    var emoji = techEmojis[random.Next(techEmojis.Length)];
                    sb.AppendLine($"<div class=\"tab-pane fade\" id=\"{Path.GetFileNameWithoutExtension(dllGroup.Key)}\">");
                    sb.AppendLine("<div class=\"knowledge_panel_2\">");
                    sb.AppendLine($"<h2>{emoji} {dllGroup.Key}</h2>");
                    sb.AppendLine("<table>");
                    sb.AppendLine($"<thead><tr><th>{allStrings["Offset_Table"]}</th>" +
                                  $"<th>{allStrings["Patch_Table"]}</th>" +
                                  $"<th>{allStrings["Author_Table"]}</th>" +
                                  $"<th>{allStrings["Description_Table"]}</th>" +
                                  $"<th>{allStrings["Date_Table"]}</th></tr></thead>");
                    sb.AppendLine("<tbody>");

                    foreach (var rec in dllGroup.Value)
                    {
                        string desc = (culture == "en") ? rec.Description.English : rec.Description.Russian;

                        long ts = rec.DateAdded;
                        string dateHtml;
                        if (ts <= 0) // нулевое или отрицательное время считаем неизвестным
                        {
                            dateHtml = "—";
                        }
                        else
                        {
                            var dto = ts > 9_999_999_999
                                ? DateTimeOffset.FromUnixTimeMilliseconds(ts)
                                : DateTimeOffset.FromUnixTimeSeconds(ts);

                            string dateText = dto.ToString("yyyy-MM-dd");
                            dateHtml = !string.IsNullOrEmpty(rec.Url)
                                ? $"<a href=\"{rec.Url}\" target=\"_blank\">{dateText}</a>"
                                : dateText;
                        }

                        // Patch: склеиваем Original → Offset
                        var patchPairs = rec.Original
                        .Select((o, i) =>
                        {
                            // Если Replacement есть и у него есть i-й элемент — берём его
                            string r = (rec.Replacement != null && i < rec.Replacement.Length && !string.IsNullOrWhiteSpace(rec.Replacement[i]))
                                ? rec.Replacement[i]
                                : o;

                            return $"<div class=\"patch-block\">{FormatPatchValue(o)} → {FormatPatchValue(r)}</div>";
                        });

                        string patchHtml = string.Join(Environment.NewLine, patchPairs);

                        var offsetPairs = rec.Offset
                          .Select((o, i) =>
                          {
                              return $"<div class=\"patch-block\">{o}</div>";
                          });

                        string offsetHtml = string.Join(Environment.NewLine, offsetPairs);
                        string categoriesAttr = rec.Categories != null ? string.Join(",", (culture == "en") ? rec.Categories.English : rec.Categories.Russian) : "";

                        sb.AppendLine("<tr class=\"patch-row\" data-categories=\"" + categoriesAttr + "\">" +
                            $"<td class=\"custom-cursor-hover\">{offsetHtml}</td>" +
                            $"<td class=\"custom-cursor-hover\">{patchHtml}</td>" +
                            $"<td>{rec.Author}</td>" +
                            $"<td class=\"p-3 fw-bold\">{desc}</td>" +
                            $"<td>{dateHtml}</td>" +
                            "</tr>");
                    }

                    sb.AppendLine("</tbody></table>");
                    sb.AppendLine("</div>");
                    sb.AppendLine("</div>");
                }
                sb.AppendLine("</div>");

                sb.AppendLine("</div>");
                return sb.ToString();
            }
        }
        return "";
    }

    private static string FormatPatchValue(string val)
    {
        // float с суффиксом f
        if (val.EndsWith("f", StringComparison.OrdinalIgnoreCase) && float.TryParse(val.TrimEnd('f'), NumberStyles.Float, CultureInfo.InvariantCulture, out var f))
        {
            var bytes = BitConverter.GetBytes(f); // little-endian
            string hex = BitConverter.ToString(bytes).Replace("-", "");
            return $"{val} (~{hex})";
        }

        // int с суффиксом i
        if (val.EndsWith("i", StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(val.TrimEnd('i'), NumberStyles.Integer, CultureInfo.InvariantCulture, out var i))
        {
            return $"{val} (~0x{i:X8})"; // HEX как 8 символов
        }

        // uint с суффиксом u
        if (val.EndsWith("u", StringComparison.OrdinalIgnoreCase) &&
            uint.TryParse(val.TrimEnd('u'), NumberStyles.Integer, CultureInfo.InvariantCulture, out var u))
        {
            return $"{val} (~0x{u:X8})"; // HEX 8 символов
        }

        // double с суффиксом d
        if (val.EndsWith("d", StringComparison.OrdinalIgnoreCase) &&
            double.TryParse(val.TrimEnd('d'), NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
        {
            var bytes = BitConverter.GetBytes(d); // little-endian
            string hex = BitConverter.ToString(bytes).Replace("-", "");
            return $"{val} (~{hex})";
        }

        return val;
    }
}
