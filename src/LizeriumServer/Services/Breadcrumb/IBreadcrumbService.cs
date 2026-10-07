/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 07 октября 2026 06:57:15
 * Version: 1.0.199
 */


using LizeriumServer.FormatsData.AppSeo;

using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace LizeriumServer.Services.Breadcrumb
{
    public interface IBreadcrumbService
    {
        Task BuildSiteMapAsync();
        string GetSitemapXml(string baseUrl);
        string GetRobotsTxt(string baseUrl);
        List<BreadcrumbItem> GetBreadcrumbs(RouteData routeData, IViewLocalizer localizer, ViewDataDictionary viewData);
    }
}
