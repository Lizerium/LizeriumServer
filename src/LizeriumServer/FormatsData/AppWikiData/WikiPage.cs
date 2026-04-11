/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 11 апреля 2026 13:54:51
 * Version: 1.0.13
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
