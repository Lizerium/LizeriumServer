/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 05 октября 2026 08:16:37
 * Version: 1.0.197
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
