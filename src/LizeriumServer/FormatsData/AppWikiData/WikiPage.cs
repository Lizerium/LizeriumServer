/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 03 июня 2026 15:37:21
 * Version: 1.0.68
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
