/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 31 марта 2026 11:07:00
 * Version: 1.0.1
 */

namespace LizeriumServer.Models
{
    public class WikiPageViewModel
    {
        public string HtmlContent { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }
}
