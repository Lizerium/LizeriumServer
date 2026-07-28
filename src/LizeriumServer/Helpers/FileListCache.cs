/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 28 июля 2026 10:29:56
 * Version: 1.0.122
 */

using Microsoft.Extensions.Caching.Memory;

public static class FileListCache
{
    private static readonly MemoryCache _cache = new(new MemoryCacheOptions());

    /// <summary>
    /// Получает или строит список файлов Markdown/MDX для указанной директории и культуры.
    /// </summary>
    /// <param name="culture">Культура (например, "ru")</param>
    /// <param name="directoryPath">Путь к директории</param>
    /// <param name="knowledgeBaseRoot">Корень KnowledgeBase для вычисления slug</param>
    public static List<FileEntry> GetOrBuild(string culture, string directoryPath, string knowledgeBaseRoot)
    {
        string cacheKey = $"{culture}:{directoryPath}";

        if (!_cache.TryGetValue(cacheKey, out List<FileEntry> cachedList))
        {
            cachedList = Directory.GetFiles(directoryPath, "*.*", SearchOption.TopDirectoryOnly)
                                   .Where(f => f.EndsWith(".md") || f.EndsWith(".mdx"))
                                   .Select(f => new FileEntry
                                   {
                                       FullPath = f,
                                       Slug = Path.GetRelativePath(knowledgeBaseRoot, f).Replace("\\", "/")
                                   })
                                   .OrderBy(f => f.Slug)
                                   .ToList();

            // Кеш на 1 час
            _cache.Set(cacheKey, cachedList, TimeSpan.FromHours(1));
        }

        return cachedList;
    }

    /// <summary>
    /// Структура записи файла
    /// </summary>
    public class FileEntry
    {
        public string FullPath { get; set; }
        public string Slug { get; set; }
    }

    /// <summary>
    /// Очистка кеша для конкретной директории
    /// </summary>
    public static void Invalidate(string culture, string directoryPath)
    {
        string cacheKey = $"{culture}:{directoryPath}";
        _cache.Remove(cacheKey);
    }
}
