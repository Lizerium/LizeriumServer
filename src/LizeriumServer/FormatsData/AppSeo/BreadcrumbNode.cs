/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 11 мая 2026 10:57:48
 * Version: 1.0.46
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
