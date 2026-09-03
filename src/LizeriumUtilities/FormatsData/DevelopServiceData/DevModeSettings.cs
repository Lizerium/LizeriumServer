/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 03 сентября 2026 07:38:14
 * Version: 1.0.165
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
