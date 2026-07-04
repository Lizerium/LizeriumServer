/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 04 июля 2026 08:59:42
 * Version: 1.0.98
 */

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LizeriumUtilities.FormatsData.DataBase.Response
{
    /// <summary>
    /// Запрос на получение данных о постах
    /// </summary>
    public class PostDataResponse
    {
        /// <summary>
        /// Первичный идентификатор поста
        /// Указание первичного ключа
        /// </summary>
        [Key]
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        [JsonPropertyName("Date")]
        public long DateTimeUnix { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        [JsonPropertyName("DateTimeUnixString")]
        public string DateTimeUnixString
        {
            get
            {
                DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(DateTimeUnix).DateTime;
                return dateTime.ToString("g");
            }
        }

        /// <summary>
        /// Автор
        /// </summary>
        [JsonPropertyName("Autor")]
        public string Autor { get; set; }

        /// <summary>
        /// Сообщение пользователя
        /// </summary>
        [JsonPropertyName("Message")]
        public string Message { get; set; }

        /// <summary>
        /// Сообщение пользователя
        /// </summary>
        [JsonPropertyName("MessageMini")]
        public string MessageMini {
            get 
            {
                if (!string.IsNullOrEmpty(Message)
                    && Message.Length > 300)
                {
                    var ret = Message.Substring(0, 300).ToString() + "...";
                    return ret;
                }
                else return Message;
            } 
        }

        /// <summary>
        /// Статус рассмотрения 
        /// Новое 1 
        /// Прочитано 2
        /// В работе 3
        /// Отказано 4
        /// Выполнено 5
        /// </summary>
        [JsonPropertyName("Status")]
        public int Status { get; set; }
    }
}
