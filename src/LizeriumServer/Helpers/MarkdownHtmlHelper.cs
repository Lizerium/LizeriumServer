/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 09 августа 2026 15:52:37
 * Version: 1.0.135
 */

using HtmlAgilityPack;
using Markdig;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace LizeriumServer.Helpers;

/// <summary>
/// Безопасный рендер Markdown для пользовательских страниц.
/// </summary>
public static class MarkdownHtmlHelper
{
    private const string NewsVideoTokenPrefix = "LIZERIUM_NEWS_VIDEO_";

    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    /// <summary>
    /// Конвертирует Markdown в очищенный HTML.
    /// </summary>
    public static string ToSafeHtml(string markdown, bool lazyImages = false)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        var newsVideos = ExtractNewsVideos(ref markdown);
        var html = Markdown.ToHtml(markdown, Pipeline);
        var document = new HtmlDocument();
        document.LoadHtml(html);

        RemoveUnsafeNodes(document);
        RemoveUnsafeAttributes(document);
        if (lazyImages)
            MakeImagesLazy(document);

        return RestoreNewsVideos(document.DocumentNode.InnerHtml, newsVideos);
    }

    private static List<NewsInlineVideo> ExtractNewsVideos(ref string markdown)
    {
        var videos = new List<NewsInlineVideo>();
        var inFence = false;
        var lines = markdown.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];
            var trimmed = line.Trim();

            if (trimmed.StartsWith("```", StringComparison.Ordinal) || trimmed.StartsWith("~~~", StringComparison.Ordinal))
            {
                inFence = !inFence;
                continue;
            }

            if (inFence)
                continue;

            var video = ExtractNewsVideo(trimmed);
            var videoUrl = video.Url;
            var embedUrl = BuildNewsVideoEmbedUrl(videoUrl);
            if (string.IsNullOrWhiteSpace(embedUrl))
                continue;

            var token = $"{NewsVideoTokenPrefix}{videos.Count}";
            videos.Add(new NewsInlineVideo(embedUrl, video.IsVertical || IsVerticalNewsVideoUrl(videoUrl)));
            lines[index] = token;
        }

        markdown = string.Join('\n', lines);
        return videos;
    }

    private readonly record struct NewsInlineVideoCandidate(string Url, bool IsVertical);

    private readonly record struct NewsInlineVideo(string EmbedUrl, bool IsVertical);

    private static NewsInlineVideoCandidate ExtractNewsVideo(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return new NewsInlineVideoCandidate(string.Empty, false);

        var verticalMacro = Regex.Match(line, @"^@video-vertical\((?<url>https?://[^)\s]+)\)$", RegexOptions.IgnoreCase);
        if (verticalMacro.Success)
            return new NewsInlineVideoCandidate(verticalMacro.Groups["url"].Value, true);

        var macro = Regex.Match(line, @"^@video\((?<url>https?://[^)\s]+)\)$", RegexOptions.IgnoreCase);
        if (macro.Success)
            return new NewsInlineVideoCandidate(macro.Groups["url"].Value, false);

        if (Regex.IsMatch(line, @"^https?://\S+$", RegexOptions.IgnoreCase))
            return new NewsInlineVideoCandidate(line, false);

        return new NewsInlineVideoCandidate(string.Empty, false);
    }

    private static string BuildNewsVideoEmbedUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        var youtube = Regex.Match(url, @"(?:youtube\.com/watch\?v=|youtube\.com/embed/|youtube\.com/shorts/|youtu\.be/)([A-Za-z0-9_-]{6,})", RegexOptions.IgnoreCase);
        if (youtube.Success)
            return $"https://www.youtube.com/embed/{youtube.Groups[1].Value}";

        var rutube = Regex.Match(url, @"rutube\.ru/(?:video|play/embed)/([A-Za-z0-9]+)", RegexOptions.IgnoreCase);
        if (rutube.Success)
            return $"https://rutube.ru/play/embed/{rutube.Groups[1].Value}/";

        var vk = Regex.Match(url, @"(?:video|clip)(-?\d+)_(\d+)", RegexOptions.IgnoreCase);
        if (vk.Success)
            return $"https://vk.com/video_ext.php?oid={vk.Groups[1].Value}&id={vk.Groups[2].Value}";

        return string.Empty;
    }

    private static bool IsVerticalNewsVideoUrl(string url)
    {
        return Regex.IsMatch(url ?? string.Empty, @"(?:youtube\.com/shorts/|vk\.com/clip)", RegexOptions.IgnoreCase);
    }

    private static string RestoreNewsVideos(string html, IReadOnlyList<NewsInlineVideo> newsVideos)
    {
        if (newsVideos.Count == 0 || string.IsNullOrWhiteSpace(html))
            return html;

        for (var index = 0; index < newsVideos.Count; index++)
        {
            var token = $"{NewsVideoTokenPrefix}{index}";
            var embedUrl = HtmlEncoder.Default.Encode(newsVideos[index].EmbedUrl);
            var videoClass = newsVideos[index].IsVertical
                ? "launcher-news-reader-video launcher-news-reader-inline-video vertical"
                : "launcher-news-reader-video launcher-news-reader-inline-video";
            var videoHtml =
                $"<div class=\"{videoClass}\" data-news-video-player>" +
                $"<iframe data-news-reader-video-src=\"{embedUrl}\" data-news-video-frame frameborder=\"0\" allow=\"clipboard-write; autoplay; encrypted-media; picture-in-picture\" allowfullscreen webkitallowfullscreen mozallowfullscreen></iframe>" +
                "</div>";

            html = Regex.Replace(
                html,
                $@"<p>\s*{Regex.Escape(token)}\s*</p>",
                videoHtml,
                RegexOptions.IgnoreCase);
            html = html.Replace(token, videoHtml, StringComparison.Ordinal);
        }

        return html;
    }

    private static void RemoveUnsafeNodes(HtmlDocument document)
    {
        var unsafeNodes = document.DocumentNode
            .Descendants()
            .Where(node => node.Name.Equals("script", StringComparison.OrdinalIgnoreCase)
                || node.Name.Equals("style", StringComparison.OrdinalIgnoreCase)
                || node.Name.Equals("iframe", StringComparison.OrdinalIgnoreCase)
                || node.Name.Equals("object", StringComparison.OrdinalIgnoreCase)
                || node.Name.Equals("embed", StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var node in unsafeNodes)
            node.Remove();
    }

    private static void RemoveUnsafeAttributes(HtmlDocument document)
    {
        foreach (var node in document.DocumentNode.Descendants().ToList())
        {
            foreach (var attribute in node.Attributes.ToList())
            {
                var name = attribute.Name;
                var value = attribute.Value ?? string.Empty;

                if (name.StartsWith("on", StringComparison.OrdinalIgnoreCase)
                    || name.Equals("style", StringComparison.OrdinalIgnoreCase)
                    || IsUnsafeUriAttribute(name, value))
                {
                    node.Attributes.Remove(attribute);
                }
            }
        }
    }

    private static bool IsUnsafeUriAttribute(string name, string value)
    {
        if (!name.Equals("href", StringComparison.OrdinalIgnoreCase)
            && !name.Equals("src", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uri))
            return true;

        return uri.IsAbsoluteUri
            && uri.Scheme != Uri.UriSchemeHttp
            && uri.Scheme != Uri.UriSchemeHttps
            && uri.Scheme != Uri.UriSchemeMailto;
    }

    private static void MakeImagesLazy(HtmlDocument document)
    {
        foreach (var image in document.DocumentNode.Descendants("img").ToList())
        {
            var src = image.GetAttributeValue("src", string.Empty);
            if (string.IsNullOrWhiteSpace(src))
                continue;

            image.SetAttributeValue("data-news-reader-image-src", src);
            image.SetAttributeValue("loading", "lazy");
            image.SetAttributeValue("decoding", "async");
            image.Attributes.Remove("src");
        }
    }
}
