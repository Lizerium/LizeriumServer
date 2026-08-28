/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 28 августа 2026 07:12:37
 * Version: 1.0.160
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
