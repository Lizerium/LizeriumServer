/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 25 июня 2026 16:51:00
 * Version: 1.0.90
 */

using System.Text.Json.Serialization;

using LizeriumUtilities.FormatsData.DataBase.Requests;

namespace LizeriumUtilities.FormatsData.DataBase.Response
{
    [Serializable]
    public class BuildComponent
    {
        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }
        [JsonPropertyName("translationsNameComponent")]
        public Language TranslationsNameComponent { get; set; }
        [JsonPropertyName("count")]
        public string Count { get; set; }
        [JsonPropertyName("components")]
        public List<BuildComponent> Components { get; set; } = new List<BuildComponent>();
    }

    [Serializable]
    public class BuildsComponent
    {
        [JsonPropertyName("nameFile")]
        public string NameFile { get; set; }
        [JsonPropertyName("translationsNameCategory")]
        public Language TranslationsNameCategory { get; set; }
        [JsonPropertyName("total")]
        public string Total { get; set; }
        [JsonPropertyName("date")]
        public DateTime Date { get; internal set; }
        [JsonPropertyName("components")]
        public List<BuildComponent> Components { get; set; } = new List<BuildComponent>();
    }
}
