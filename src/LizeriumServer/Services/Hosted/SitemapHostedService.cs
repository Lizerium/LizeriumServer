/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 20 сентября 2026 08:50:06
 * Version: 1.0.182
 */

using LizeriumServer.Services.Breadcrumb;
using LizeriumServer.Services.Breadcrumb.Implements;

namespace LizeriumServer.Services.Hosted
{
    public class SitemapHostedService : IHostedService
    {
        private readonly IBreadcrumbService _breadcrumbService;

        public SitemapHostedService(IBreadcrumbService breadcrumbService)
        {
            _breadcrumbService = breadcrumbService;
        }

        // Этот метод вызывается при старте приложения
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Асинхронная генерация sitemap и заполнение _siteMap
            await _breadcrumbService.BuildSiteMapAsync();
        }

        // Вызывается при остановке приложения, можно ничего не делать
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
