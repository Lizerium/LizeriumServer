/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 20 сентября 2026 08:50:06
 * Version: 1.0.182
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
