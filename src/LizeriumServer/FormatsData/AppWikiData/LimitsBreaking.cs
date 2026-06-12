/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 12 июня 2026 07:13:24
 * Version: 1.0.77
 */

using System.Text.Json.Serialization;

using LizeriumUtilities.FormatsData.DataBase.Requests;

namespace LizeriumServer.FormatsData.AppWikiData
{
    public class LimitsBreaking
    {
        [JsonPropertyName("moduleName")]
        public string[] ModuleName { get; set; }
        [JsonPropertyName("original")]
        public string[] Original { get; set; }
        [JsonPropertyName("replacement")]
        public string[] Replacement { get; set; }
        [JsonPropertyName("offset")]
        public string[] Offset { get; set; }
        [JsonPropertyName("author")]
        public string Author { get; set; }
        [JsonPropertyName("description")]
        public Language Description { get; set; }
        [JsonPropertyName("dateAdded")]
        public long DateAdded { get; set; }
        [JsonPropertyName("categories")]
        public LanguageArray Categories { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
