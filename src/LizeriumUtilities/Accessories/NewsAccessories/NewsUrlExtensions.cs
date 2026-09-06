/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 06 сентября 2026 11:13:26
 * Version: 1.0.168
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
        ['Р°'] = "a",
        ['Р±'] = "b",
        ['РІ'] = "v",
        ['Рі'] = "g",
        ['Рґ'] = "d",
        ['Рµ'] = "e",
        ['С‘'] = "e",
        ['Р¶'] = "zh",
        ['Р·'] = "z",
        ['Рё'] = "i",
        ['Р№'] = "y",
        ['Рє'] = "k",
        ['Р»'] = "l",
        ['Рј'] = "m",
        ['РЅ'] = "n",
        ['Рѕ'] = "o",
        ['Рї'] = "p",
        ['СЂ'] = "r",
        ['СЃ'] = "s",
        ['С‚'] = "t",
        ['Сѓ'] = "u",
        ['С„'] = "f",
        ['С…'] = "h",
        ['С†'] = "ts",
        ['С‡'] = "ch",
        ['С€'] = "sh",
        ['С‰'] = "sch",
        ['СЉ'] = "",
        ['С‹'] = "y",
        ['СЊ'] = "",
        ['СЌ'] = "e",
        ['СЋ'] = "yu",
        ['СЏ'] = "ya"
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
