/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 05 июля 2026 11:14:13
 * Version: 1.0.99
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
