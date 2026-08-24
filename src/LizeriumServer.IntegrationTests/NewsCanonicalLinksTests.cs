/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 24 августа 2026 07:14:27
 * Version: 1.0.156
 */

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

public class NewsCanonicalLinksTests : IAsyncLifetime
{
    private readonly CanonicalLinksFactory _factory = new();
    private LauncherNewsDataResponse _news = new();

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var appDb = scope.ServiceProvider.GetRequiredService<IDataBaseService>();
        await appDb.RebuildAsync();
        await appDb.ExistAndCreateLauncherNewsTable();

        _news = new LauncherNewsDataResponse
        {
            TitleRu = "РђРґСЂРµСЃ РЅРѕРІРѕСЃС‚Рё",
            TitleEn = "News Address",
            MarkdownRu = string.Join(" ", Enumerable.Repeat("РўРµРєСЃС‚ РЅРѕРІРѕСЃС‚Рё РґР»СЏ РїРѕР»РЅРѕР№ РІРµСЂСЃРёРё.", 16)),
            MarkdownEn = string.Join(" ", Enumerable.Repeat("News body for the full version.", 16)),
            NewsTypeRu = "SEO",
            NewsTypeEn = "SEO",
            IsPublished = true,
            PublishedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            SortOrder = -200
        };

        appDb.LauncherNews.Add(_news);
        await appDb.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        _factory.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Launcher_RendersCanonicalNewsHrefAndShareUrl()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Home/Launcher");
        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync();
        Directory.CreateDirectory("TestResults");
        await File.WriteAllTextAsync(Path.Combine("TestResults", "canonical-links-launcher.html"), html);
        var canonicalPath = _news.GetCanonicalNewsPath("ru");

        Assert.Contains($"href=\"{canonicalPath}\"", html);
        Assert.Contains($"data-news-reader-share-url=\"{canonicalPath}\"", html);
    }

    [Fact]
    public async Task NewsRss_UsesCanonicalNewsUrlForLinkAndGuid()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/news/rss.xml");
        response.EnsureSuccessStatusCode();

        var rss = await response.Content.ReadAsStringAsync();
        var canonicalUrl = $"http://localhost{_news.GetCanonicalNewsPath("ru")}";

        Assert.Contains($"<link>{canonicalUrl}</link>", rss);
        Assert.Contains($"<guid>{canonicalUrl}</guid>", rss);
    }

    private sealed class CanonicalLinksFactory : WebApplicationFactory<Program>
    {
        private readonly string _databasePath = Path.Combine(
            Path.GetTempPath(),
            "lizerium-canonical-links-tests",
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
