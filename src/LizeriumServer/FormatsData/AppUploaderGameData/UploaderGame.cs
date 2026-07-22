/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 22 июля 2026 13:17:55
 * Version: 1.0.116
 */

using Newtonsoft.Json;

namespace LizeriumServer.FormatsData.AppUploaderGameData
{
    public class UploaderGame
    {
        public UploaderGame()
        {
            Urls = new List<string>();
        }

        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Urls")]
        public List<string> Urls { get; set; }
    }

    public class UploaderGames
    {
        [JsonProperty("count")]
        public int Size
        {
            get => GamesList.Count;
            set => Size = value;
        }

        [JsonProperty("data")]
        public List<UploaderGame> GamesList { get; set; }

        public UploaderGames()
        {
            GamesList = new List<UploaderGame>();
        }
    }
}
