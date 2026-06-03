/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 03 июня 2026 15:37:21
 * Version: 1.0.68
 */

using System.Text.Json;

using LizeriumUtilities.FormatsData.DevelopServiceData;

namespace LizeriumUtilities.Services.DevelopService
{
    public class DevModeService
    {
        private DevModeSettings _mode;
        private readonly string _filePath;
        private readonly Timer _timer;

        public DevModeService(string filePath)
        {
            _filePath = filePath;

            LoadState();

            _timer = new Timer(_ =>
            {
                LoadState();
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5)); // обновляем каждые 5 секунд
        }

        private void LoadState()
        {
            try
            {
                var confPath = Path.Combine(AppContext.BaseDirectory, _filePath);
                var json = File.ReadAllText(confPath);
                var settings = JsonSerializer.Deserialize<DevModeSettings>(json);
                _mode = settings;
            }
            catch
            {
                _mode = null;
            }
        }

        public bool IsDevelopMode => _mode.DevelopMode;
        public bool IsUpdaterState => _mode.UpdaterState;
        public bool IsUpdaterDevMode => _mode.UpdaterDevMode;
        public List<string> UpdaterWhiteList => _mode.UpdaterWhiteList;
    }
}
