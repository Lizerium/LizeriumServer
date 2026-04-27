/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 27 апреля 2026 10:01:53
 * Version: 1.0.32
 */

using System;

using LizeriumServer.FormatsData.AppWikiData;

namespace LizeriumServer.Helpers
{
    public static class KnowledgeMenuCache
    {
        private static readonly Dictionary<string, WikiPage> _cacheMenu = new();
        private static readonly object _lock = new();
        private static readonly Dictionary<string, string> cashedCrashOffsets = new();
        private static readonly Dictionary<string, string> cashedLimitsBreaking = new();

        // Получить или создать меню для конкретной культуры
        public static WikiPage GetOrBuild(string culture, string rootPath, Dictionary<string, string> localizedTitles)
        {
            lock (_lock)
            {
                if (_cacheMenu.TryGetValue(culture, out var menu))
                    return _cacheMenu[culture];

                var menus = MarkdownPage.BuildMenu(rootPath, localizedTitles);
                foreach (var page in menus.Children)
                {
                    if(culture == page.Slug)
                        _cacheMenu[page.Slug] = page;
                }
                return _cacheMenu[culture];
            }
        }

        // Очистить кеш (если KnowledgeBase обновилась)
        public static void Clear()
        {
            lock (_lock)
            {
                _cacheMenu.Clear();
                cashedCrashOffsets.Clear();
                cashedLimitsBreaking.Clear();
            }
        }

        public static void SetCrashOffsets(string data, string culture)
        {
            if (cashedCrashOffsets.ContainsKey(culture))
                cashedCrashOffsets[culture] = data;
            else cashedCrashOffsets.Add(culture, data);
        }

        public static string GetCrashOffsets(string culture)
        {
            if (cashedCrashOffsets.ContainsKey(culture))
                return cashedCrashOffsets[culture];
            else return "";
        }

        public static bool ExistCrashOffsets(string culture)
        {
            if (cashedCrashOffsets.ContainsKey(culture))
                return true;
            return false;
        }

        public static void SetBrLimits(string data, string culture)
        {
            if (cashedLimitsBreaking.ContainsKey(culture))
                cashedLimitsBreaking[culture] = data;
            else cashedLimitsBreaking.Add(culture, data);
        }

        public static string GetBrLimits(string culture)
        {
            if (cashedLimitsBreaking.ContainsKey(culture))
                return cashedLimitsBreaking[culture];
            else return "";
        }

        public static bool ExistBrLimits(string culture)
        {
            if (cashedLimitsBreaking.ContainsKey(culture))
                return true;
            return false;
        }
    }
}
