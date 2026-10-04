/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 04 октября 2026 11:22:19
 * Version: 1.0.196
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
