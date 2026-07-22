/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 22 июля 2026 13:17:55
 * Version: 1.0.116
 */


using LizeriumServer.FormatsData.AppSeo;

using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace LizeriumServer.Services.Breadcrumb
{
    public interface IBreadcrumbService
    {
        Task BuildSiteMapAsync();
        List<BreadcrumbItem> GetBreadcrumbs(RouteData routeData, IViewLocalizer localizer, ViewDataDictionary viewData);
    }
}
