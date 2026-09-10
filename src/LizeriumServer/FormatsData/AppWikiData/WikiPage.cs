/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 10 сентября 2026 10:12:30
 * Version: 1.0.172
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
