/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 05 июня 2026 07:12:29
 * Version: 1.0.70
 */

using System.Text.Json.Serialization;

using Microsoft.Extensions.Logging;

namespace LizeriumUtilities.Services.DownloadLinksService
{
    public class DownloadEntry
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;
    }

    public class DownloadLinksService
    {
        private readonly string _filePath;
        private readonly ILogger<DownloadLinksService> _logger;
        private Dictionary<string, DownloadEntry> _links;
        private readonly object _lock = new();

        public DownloadLinksService(string filePath, ILogger<DownloadLinksService> logger)
        {
            _filePath = filePath;
            _logger = logger;
            _links = new Dictionary<string, DownloadEntry>();

            LoadLinks();
            WatchFile();
        }

        public DownloadEntry? GetLink(string key)
        {
            lock (_lock)
            {
                return _links.TryGetValue(key, out var entry) ? entry : null;
            }
        }

        private void LoadLinks()
        {
            if (!File.Exists(_filePath))
            {
                _logger.LogWarning("Download links file not found: {Path}", _filePath);
                return;
            }

            var json = File.ReadAllText(_filePath);
            var links = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, DownloadEntry>>(json);

            if (links != null)
            {
                lock (_lock)
                {
                    _links = links;
                }
            }
        }

        private void WatchFile()
        {
            var watcher = new FileSystemWatcher(Path.GetDirectoryName(_filePath)!)
            {
                Filter = Path.GetFileName(_filePath),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };

            watcher.Changed += (s, e) =>
            {
                try
                {
                    _logger.LogInformation("Download links file changed, reloading...");
                    Thread.Sleep(100); // дать файлу сохраниться
                    LoadLinks();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while reloading links");
                }
            };

            watcher.EnableRaisingEvents = true;
        }
    }
}
