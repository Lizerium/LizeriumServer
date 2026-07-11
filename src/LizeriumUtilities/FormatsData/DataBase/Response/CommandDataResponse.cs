/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 11 июля 2026 14:23:29
 * Version: 1.0.105
 */

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using LizeriumUtilities.FormatsData.DataBase.Requests;

namespace LizeriumUtilities.FormatsData.DataBase.Response
{
    /// <summary>
    /// Запрос на получение данных о команде
    /// </summary>
    public class CommandDataResponse
    {
        /// <summary>
        /// Указание первичного ключа
        /// </summary>
        [Key]
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Категория например MARK
        /// </summary>
        [JsonPropertyName("Category")]
        public string Category { get; set; }

        /// <summary>
        /// Переводы тайтла категории
        /// </summary>
        [NotMapped]
        [JsonPropertyName("TitlesCategory")]
        public Language TitlesCategory { get; set; }

        /// <summary>
        /// Список комманд категории
        /// </summary>
        [JsonPropertyName("CommandNames")]
        public string CommandNames { get; set; }

        /// <summary>
        /// Список комманд категории
        /// </summary>
        [JsonPropertyName("CommandNamesList")]
        public List<string> CommandNamesList => SetupNames();

		/// <summary>
		/// Пример ввода
		/// </summary>
		[JsonPropertyName("ExampleInput")]
        public string ExampleInput { get; set; }

        /// <summary>
        /// Описание команды
        /// </summary>
        [JsonPropertyName("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Переводы команд по языкам
        /// </summary>
        [JsonPropertyName("Translations")]
        [NotMapped]
        public Dictionary<string, List<string>> Translations { get; set; } = new();

        /// <summary>
        /// GIF источник примера ввода команды
        /// </summary>
        [JsonPropertyName("UrlGif")]
        public string UrlGif { get; set; }

        /// <summary>
        /// Количество откликов
        /// </summary>
        [JsonPropertyName("Likes")]
        public int CountLike { get; set; }

        /// <summary>
        /// Статус команды на сервере
        /// 1 - Активна
        /// 2 - Выключена
        /// 3 - В разработке
        /// </summary>
        [JsonPropertyName("Status")]
        public int Status { get; set; }

        /// <summary>
        /// Удалает префикс / для создания id команды
        /// </summary>
        /// <param name="command">Команда</param>
        /// <returns></returns>
        public string ClearId(string command)
        {
            return command.Replace("/", "");
        }

        private List<string> SetupNames()
        {
            var names = CommandNames.Split(',').Select(x => x.Trim()).ToList();
            return names;
        }
    }
}
