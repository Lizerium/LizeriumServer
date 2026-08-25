/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 25 августа 2026 07:14:09
 * Version: 1.0.157
 */

using System.Net;

using LizeriumDatabase.Services.AppDataBaseService;
using LizeriumDatabase.Services.AppDataBaseService.Implements;
using LizeriumUtilities.Accessories.NewsAccessories;
using LizeriumUtilities.FormatsData.DataBase.Response;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace LizeriumServer.IntegrationTests;

public class NewsSitemapTests : IAsyncLifetime
{
    private const string RebuildToken = "test-sitemap-token";
    private readonly SitemapFactory _factory = new();
    private LauncherNewsDataResponse _publishedNews = new();
    private LauncherNewsDataResponse _hiddenNews = new();

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var appDb = scope.ServiceProvider.GetRequiredService<IDataBaseService>();
        await appDb.RebuildAsync();
        await appDb.ExistAndCreateLauncherNewsTable();

        var publishedAt = DateTimeOffset.UtcNow.AddDays(-3).ToUnixTimeSeconds();
        _publishedNews = new LauncherNewsDataResponse
        {
            TitleRu = "РќРѕРІРѕСЃС‚СЊ РґР»СЏ sitemap",
            TitleEn = "Sitemap News",
            MarkdownRu = "РћРїСѓР±Р»РёРєРѕРІР°РЅРЅР°СЏ РЅРѕРІРѕСЃС‚СЊ.",
            MarkdownEn = "Published news.",
            NewsTypeRu = "SEO",
            NewsTypeEn = "SEO",
            IsPublished = true,
            PublishedAtUnix = publishedAt,
            SortOrder = 0
        };
        _hiddenNews = new LauncherNewsDataResponse
        {
            TitleRu = "РЎРєСЂС‹С‚Р°СЏ РЅРѕРІРѕСЃС‚СЊ sitemap",
            TitleEn = "Hidden Sitemap News",
            MarkdownRu = "Р§РµСЂРЅРѕРІРёРє.",
            MarkdownEn = "Draft.",
            NewsTypeRu = "Draft",
            NewsTypeEn = "Draft",
            IsPublished = false,
            PublishedAtUnix = publishedAt - 1,
            SortOrder = 1
        };

        appDb.LauncherNews.AddRange(_publishedNews, _hiddenNews);
        await appDb.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        _factory.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task SitemapRebuild_RejectsRequestsWithoutConfiguredToken()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsync("/internal/sitemap/rebuild", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SitemapRebuild_AddsPublishedNewsAndSkipsHiddenNews()
    {
        var client = _factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "/internal/sitemap/rebuild");
        request.Headers.Add("X-Lizerium-Sitemap-Token", RebuildToken);

        var rebuildResponse = await client.SendAsync(request);
        rebuildResponse.EnsureSuccessStatusCode();

        var sitemapResponse = await client.GetAsync("/sitemap.xml");
        sitemapResponse.EnsureSuccessStatusCode();
        var sitemap = await sitemapResponse.Content.ReadAsStringAsync();

        Assert.Contains(_publishedNews.GetCanonicalNewsPath("ru"), sitemap);
        Assert.DoesNotContain($"/news/{_hiddenNews.Id}/", sitemap);
        Assert.Contains(DateTimeOffset.FromUnixTimeSeconds(_publishedNews.PublishedAtUnix).UtcDateTime.ToString("yyyy-MM-dd"), sitemap);
    }

    private sealed class SitemapFactory : WebApplicationFactory<Program>
    {
        private readonly string _databasePath = Path.Combine(
            Path.GetTempPath(),
            "lizerium-sitemap-news-tests",
            $"{Guid.NewGuid():N}.db");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["appSettings:sitemapRebuildToken"] = RebuildToken
                });
            });
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IDataBaseService>();
                services.RemoveAll<DbContextOptions<DataBaseService>>();
                services.RemoveAll<IHostedService>();
                services.AddDbContext<IDataBaseService, DataBaseService>(options =>
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_databasePath)!);
                    options.UseSqlite($"Data Source={_databasePath}");
                });
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            try
            {
                if (File.Exists(_databasePath))
                    File.Delete(_databasePath);
            }
            catch (IOException)
            {
            }
        }
    }
}
