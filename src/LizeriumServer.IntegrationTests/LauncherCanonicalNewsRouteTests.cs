/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 29 августа 2026 07:13:02
 * Version: 1.0.161
 */

using System.Net;

using LizeriumDatabase.Services.AppDataBaseService;
using LizeriumDatabase.Services.AppDataBaseService.Implements;
using LizeriumUtilities.Accessories.NewsAccessories;
using LizeriumUtilities.FormatsData.DataBase.Response;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace LizeriumServer.IntegrationTests;

public class LauncherCanonicalNewsRouteTests : IAsyncLifetime
{
    private readonly CanonicalNewsFactory _factory = new();
    private LauncherNewsDataResponse _publishedNews = new();
    private LauncherNewsDataResponse _hiddenNews = new();

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var appDb = scope.ServiceProvider.GetRequiredService<IDataBaseService>();
        await appDb.RebuildAsync();
        await appDb.ExistAndCreateLauncherNewsTable();

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        _publishedNews = new LauncherNewsDataResponse
        {
            TitleRu = "UTF РІ XML",
            TitleEn = "UTF to XML",
            MarkdownRu = "РўРµРєСЃС‚ РѕРїСѓР±Р»РёРєРѕРІР°РЅРЅРѕР№ РЅРѕРІРѕСЃС‚Рё.",
            MarkdownEn = "Published news text.",
            NewsTypeRu = "РћР±РЅРѕРІР»РµРЅРёРµ",
            NewsTypeEn = "Update",
            IsPublished = true,
            PublishedAtUnix = now,
            SortOrder = 0
        };
        _hiddenNews = new LauncherNewsDataResponse
        {
            TitleRu = "РЎРєСЂС‹С‚Р°СЏ РЅРѕРІРѕСЃС‚СЊ",
            TitleEn = "Hidden News",
            MarkdownRu = "Р§РµСЂРЅРѕРІРёРє.",
            MarkdownEn = "Draft.",
            NewsTypeRu = "Р§РµСЂРЅРѕРІРёРє",
            NewsTypeEn = "Draft",
            IsPublished = false,
            PublishedAtUnix = now - 1,
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
    public async Task NewsArticle_RedirectsWrongSlugToCanonicalPath()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync($"/news/{_publishedNews.Id}/wrong.html");

        Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
        Assert.Equal(_publishedNews.GetCanonicalNewsPath("ru"), response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task NewsArticle_RendersLauncherPageWithInitialModalTarget()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(_publishedNews.GetCanonicalNewsPath("ru"));

        var html = await response.Content.ReadAsStringAsync();
        Directory.CreateDirectory("TestResults");
        await File.WriteAllTextAsync(Path.Combine("TestResults", "canonical-news-route.html"), html);
        response.EnsureSuccessStatusCode();
        Assert.Contains($"data-news-initial-open=\"{_publishedNews.Id}\"", html);
        Assert.Contains($"data-news-reader-post=\"{_publishedNews.Id}\"", html);
    }

    [Fact]
    public async Task NewsArticle_ReturnsNotFoundForHiddenNews()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(_hiddenNews.GetCanonicalNewsPath("ru"));
        Directory.CreateDirectory("TestResults");
        await File.WriteAllTextAsync(
            Path.Combine("TestResults", "hidden-news-route.txt"),
            $"{(int)response.StatusCode} {response.RequestMessage?.RequestUri}\n{await response.Content.ReadAsStringAsync()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed class CanonicalNewsFactory : WebApplicationFactory<Program>
    {
        private readonly string _databasePath = Path.Combine(
            Path.GetTempPath(),
            "lizerium-canonical-news-tests",
            $"{Guid.NewGuid():N}.db");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
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
