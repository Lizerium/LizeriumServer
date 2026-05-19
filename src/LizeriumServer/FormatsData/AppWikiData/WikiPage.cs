/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 19 мая 2026 10:36:19
 * Version: 1.0.53
 */

namespace LizeriumServer.FormatsData.AppWikiData
{
    public class WikiPage
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public List<WikiPage> Children { get; set; } = new List<WikiPage>();
    }
}
