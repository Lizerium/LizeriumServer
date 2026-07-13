/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 13 июля 2026 07:11:25
 * Version: 1.0.107
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
