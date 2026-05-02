/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 02 мая 2026 19:36:20
 * Version: 1.0.37
 */

namespace LizeriumUtilities.FormatsData.DevelopServiceData
{
    public class DevModeSettings
    {
        /// <summary>
        /// Ведутся ли технические работы
        /// </summary>
        public bool DevelopMode { get; set; }
        /// <summary>
        /// Активен ли загрузчик обновлений для Лаунчера и приложений
        /// </summary>
        public bool UpdaterState { get; set; }
        /// <summary>
        /// Включён ли загрузчик обновлений для Лаунчера и приложений только для исключительных адресов IP
        /// </summary>
        public bool UpdaterDevMode { get; set; }
        /// <summary>
        /// Списо допустимых IP адресов к скачиванию Лаунчера и приложений в UpdaterDevMode
        /// </summary>
        public List<string> UpdaterWhiteList { get; set; }
    }
}
