/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 23 августа 2026 07:14:40
 * Version: 1.0.154
 */

using System.Text;

using LizeriumUtilities.FormatsData.DataBase.Response;

namespace LizeriumUtilities.Accessories.NewsAccessories;

/// <summary>
/// Helpers for public launcher news URLs.
/// </summary>
public static class NewsUrlExtensions
{
    private static readonly Dictionary<char, string> CyrillicMap = new()
    {
        ['а'] = "a",
        ['б'] = "b",
        ['в'] = "v",
        ['г'] = "g",
        ['д'] = "d",
        ['е'] = "e",
        ['ё'] = "e",
        ['ж'] = "zh",
        ['з'] = "z",
        ['и'] = "i",
        ['й'] = "y",
        ['к'] = "k",
        ['л'] = "l",
        ['м'] = "m",
        ['н'] = "n",
        ['о'] = "o",
        ['п'] = "p",
        ['р'] = "r",
        ['с'] = "s",
        ['т'] = "t",
        ['у'] = "u",
        ['ф'] = "f",
        ['х'] = "h",
        ['ц'] = "ts",
        ['ч'] = "ch",
        ['ш'] = "sh",
        ['щ'] = "sch",
        ['ъ'] = "",
        ['ы'] = "y",
        ['ь'] = "",
        ['э'] = "e",
        ['ю'] = "yu",
        ['я'] = "ya"
    };

    /// <summary>
    /// Builds canonical public news path.
    /// </summary>
    public static string GetCanonicalNewsPath(this LauncherNewsDataResponse news, string culture = "ru")
    {
        var id = Math.Max(0, news?.Id ?? 0);
        var slug = news?.GetNewsSlug(culture) ?? $"news-{id}";
        return $"/news/{id}/{slug}.html";
    }

    /// <summary>
    /// Builds deterministic URL slug from localized title.
    /// </summary>
    public static string GetNewsSlug(this LauncherNewsDataResponse news, string culture = "ru")
    {
        var title = PickTitle(news, culture);
        var slug = Slugify(title);

        return string.IsNullOrWhiteSpace(slug)
            ? $"news-{Math.Max(0, news?.Id ?? 0)}"
            : slug;
    }

    private static string PickTitle(LauncherNewsDataResponse news, string culture)
    {
        if (news == null)
            return string.Empty;

        var isEnglish = string.Equals(culture, "en", StringComparison.OrdinalIgnoreCase)
            || culture?.StartsWith("en-", StringComparison.OrdinalIgnoreCase) == true;
        var preferred = isEnglish ? news.TitleEn : news.TitleRu;
        var fallback = isEnglish ? news.TitleRu : news.TitleEn;

        return !string.IsNullOrWhiteSpace(preferred) ? preferred : fallback ?? string.Empty;
    }

    private static string Slugify(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var builder = new StringBuilder();
        var previousWasSeparator = false;

        foreach (var rawCharacter in value.Trim().ToLowerInvariant())
        {
            if (rawCharacter is >= 'a' and <= 'z' or >= '0' and <= '9')
            {
                builder.Append(rawCharacter);
                previousWasSeparator = false;
                continue;
            }

            if (CyrillicMap.TryGetValue(rawCharacter, out var transliterated))
            {
                builder.Append(transliterated);
                if (!string.IsNullOrEmpty(transliterated))
                    previousWasSeparator = false;

                continue;
            }

            if (previousWasSeparator)
                continue;

            builder.Append('-');
            previousWasSeparator = true;
        }

        return builder.ToString().Trim('-');
    }
}
