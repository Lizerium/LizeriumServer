/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 01 мая 2026 07:13:51
 * Version: 1.0.36
 */

using System.Text.Json.Serialization;

using LizeriumUtilities.FormatsData.DataBase.Requests;

namespace LizeriumServer.FormatsData.AppWikiData
{
    public class CrashOffset
    {
        [JsonPropertyName("moduleName")]
        public string ModuleName { get; set; }
        [JsonPropertyName("offset")]
        public string Offset { get; set; }
        [JsonPropertyName("author")]
        public string Author { get; set; }
        [JsonPropertyName("description")]
        public Language Description { get; set; }
        [JsonPropertyName("dateAdded")]
        public long DateAdded { get; set; }
    }
}
