/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 27 августа 2026 08:52:27
 * Version: 1.0.159
 */

using LizeriumServer.Services.Breadcrumb;

using Microsoft.AspNetCore.Mvc;

namespace LizeriumServer.Controllers
{
    public class InternalController : Controller
    {
        private const string SitemapTokenHeader = "X-Lizerium-Sitemap-Token";

        private readonly IBreadcrumbService _breadcrumbService;
        private readonly IConfiguration _configuration;

        public InternalController(IBreadcrumbService breadcrumbService, IConfiguration configuration)
        {
            _breadcrumbService = breadcrumbService;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("/internal/sitemap/rebuild")]
        public async Task<IActionResult> RebuildSitemap()
        {
            var configuredToken = _configuration["appSettings:sitemapRebuildToken"];

            if (string.IsNullOrWhiteSpace(configuredToken)
                || !Request.Headers.TryGetValue(SitemapTokenHeader, out var requestTokens)
                || requestTokens.Count != 1
                || string.IsNullOrWhiteSpace(requestTokens[0])
                || !string.Equals(configuredToken, requestTokens[0], StringComparison.Ordinal))
            {
                return NotFound();
            }

            await _breadcrumbService.BuildSiteMapAsync();
            return Json(new { ok = true });
        }
    }
}
