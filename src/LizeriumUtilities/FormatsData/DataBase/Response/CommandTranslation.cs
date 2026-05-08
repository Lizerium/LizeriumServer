/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 08 мая 2026 07:07:23
 * Version: 1.0.43
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LizeriumUtilities.FormatsData.DataBase.Response
{
    /// <summary>
    /// DTO для передачи данных между слоями
    /// </summary>
    public class CommandTranslationResponse
    {
        public int CommandId { get; set; }
        public string LanguageCode { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class AdminCommandWithTranslations
    {
        public int CommandId { get; set; }
        public string BaseDescription { get; set; } = null!; // описание из основной таблицы Commands
        public List<CommandTranslation> Translations { get; set; } = new();
    }

    /// <summary>
    /// Перевод команды
    /// </summary>
    public class CommandTranslation
    {
        /// <summary>
        /// Указание первичного ключа
        /// </summary>
        [Key]
        [JsonPropertyName("CommandId")]
        public int CommandId { get; set; }
        /// <summary>
        /// Уникальный идентификатор языка
        /// </summary>
        [JsonPropertyName("LanguageCode")]
        [MaxLength(5)]
        public string LanguageCode { get; set; } = null!;
        /// <summary>
        /// Значение перевода
        /// </summary>
        [JsonPropertyName("Description")]
        public string? Description { get; set; }
    }
}
