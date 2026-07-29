/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 29 июля 2026 16:02:04
 * Version: 1.0.125
 */

using HtmlAgilityPack;
using Markdig;

namespace LizeriumServer.Helpers;

/// <summary>
/// Р‘РµР·РѕРїР°СЃРЅС‹Р№ СЂРµРЅРґРµСЂ Markdown РґР»СЏ РїРѕР»СЊР·РѕРІР°С‚РµР»СЊСЃРєРёС… СЃС‚СЂР°РЅРёС†.
/// </summary>
public static class MarkdownHtmlHelper
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    /// <summary>
    /// РљРѕРЅРІРµСЂС‚РёСЂСѓРµС‚ Markdown РІ РѕС‡РёС‰РµРЅРЅС‹Р№ HTML.
    /// </summary>
    public static string ToSafeHtml(string markdown, bool lazyImages = false)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        var html = Markdown.ToHtml(markdown, Pipeline);
        var document = new HtmlDocument();
        document.LoadHtml(html);

        RemoveUnsafeNodes(document);
        RemoveUnsafeAttributes(document);
        if (lazyImages)
            MakeImagesLazy(document);

        return document.DocumentNode.InnerHtml;
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
