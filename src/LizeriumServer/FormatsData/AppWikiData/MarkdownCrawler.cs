/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 08 июня 2026 07:12:33
 * Version: 1.0.73
 */

using System.Text.RegularExpressions;

namespace LizeriumServer.FormatsData.AppWikiData;

public static class MarkdownCrawler
{
    private static readonly HashSet<string> visited = new(StringComparer.OrdinalIgnoreCase);
    private static readonly List<string> missingFiles = new();
    private static readonly List<string> okFiles = new();

    public static void ValidateRecursively(string slug, string cultureCookie, string projectRoot)
    {
        try
        {
            if (visited.Contains(slug))
                return;

            visited.Add(slug);
            string slugCopy = slug;

            string fullPath = MarkdownPage.ValidateLink(ref slugCopy, cultureCookie, projectRoot);

            if (!File.Exists(fullPath))
            {
                string message = $"❌ {fullPath} not exist...";
                Console.WriteLine(message);
                missingFiles.Add(message);
                return;
            }
            else
            {
                okFiles.Add(fullPath);
            }

            Console.WriteLine($"✅ {slugCopy}");

            MarkdownPage.ConvertMDAndYamlToHTML(fullPath, out var frontMatter, out var html);

            var baseRoute = Path.GetDirectoryName(fullPath)?
                  .Replace(projectRoot, "")
                  .Replace(Path.DirectorySeparatorChar, '/')
                  .Trim('/');

            var htmlConvertLinks = MarkdownPage.SetupLinks(html, baseRoute);
            var links = ExtractRelativeMarkdownLinks(htmlConvertLinks);

            foreach (var link in links)
            {
                var rawHref = link.Split('#')[0];
                var nextSlug = NormalizeSlug(rawHref);
                ValidateRecursively(nextSlug, cultureCookie, projectRoot);
            }
        } catch (Exception ex) {
        
        var message = ex.Message;
        }
    }

    private static string NormalizeSlug(string slug)
    {
        // Убираем ./ и ../ корректно
        var parts = slug.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
        var stack = new Stack<string>();

        foreach (var part in parts)
        {
            if (part == ".")
                continue;
            else if (part == ".." && stack.Count > 0)
                stack.Pop();
            else if (part != "..")
                stack.Push(part);
        }

        var normalized = string.Join("/", stack.Reverse());
        return normalized;
    }

    public static List<string> GetMissingFiles() => missingFiles;
    public static List<string> GetOkFiles() => okFiles;

    private static List<string> ExtractRelativeMarkdownLinks(string html)
    {
        var matches = Regex.Matches(html, "<a\\s+href=[\"'](?:/wiki/)?([^\"']+\\.mdx?|[^\"']+\\.md)(#[^\"']*)?[\"']", RegexOptions.IgnoreCase);
        var links = new List<string>();

        foreach (Match match in matches)
        {
            var path = match.Groups[1].Value;
            links.Add(path);
        }

        return links;
    }
}
