/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 20 июля 2026 12:29:51
 * Version: 1.0.114
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
