/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 30 мая 2026 16:01:02
 * Version: 1.0.64
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
