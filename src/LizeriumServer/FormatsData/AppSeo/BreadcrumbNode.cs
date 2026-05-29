/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 29 мая 2026 14:15:42
 * Version: 1.0.63
 */

namespace LizeriumServer.FormatsData.AppSeo
{
    public class BreadcrumbNode
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public List<BreadcrumbNode> Children { get; set; } = new List<BreadcrumbNode>();
    }
}
