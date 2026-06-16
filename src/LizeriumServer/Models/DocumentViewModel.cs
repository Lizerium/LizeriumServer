/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 16 июня 2026 07:12:27
 * Version: 1.0.81
 */

using LizeriumUtilities.FormatsData.DataBase.Response;

namespace LizeriumServer.Models
{
    /// <summary>
    /// Модель представления документации
    /// </summary>
    public class DocumentViewModel
    {
        /// <summary>
        /// Список категорий документации Hook
        /// </summary>
        public List<CommandCategoryInfoResponse> CategoriesHook { get; set; }
        /// <summary>
        /// Словарь категория => количество команд
        /// </summary>
        public Dictionary<string, int> CommandsCount { get; set; } = new();
        /// <summary>
        /// Количество элементов в странице с командами
        /// </summary>
        public int PageSize { get; set; } = 6;
        /// <summary>
        /// Категория документации
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Список выбранных команд в документации
        /// </summary>
        public List<CommandDataResponse> Commands {  get; set; }
        /// <summary>
        /// Список категорий создания предметов на сервере
        /// </summary>
        public List<BuildsComponent> ListBuilds { get;  set; }

        /// <summary>
        /// Выбранная детализация сборки предмена
        /// </summary>
        public BuildComponent BuildComponent { get; set; }
        public int Page { get; internal set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="commands">Список команд</param>
        public DocumentViewModel(List<CommandDataResponse> commands = null, 
            string Category = "",
            List<CommandCategoryInfoResponse> CategoriesHook = null)
        {
            this.CategoriesHook = CategoriesHook;
            this.Category = Category;
            Commands = commands;
        }
    }
}
