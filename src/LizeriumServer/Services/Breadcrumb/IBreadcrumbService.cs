/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 19 июля 2026 10:21:30
 * Version: 1.0.113
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
