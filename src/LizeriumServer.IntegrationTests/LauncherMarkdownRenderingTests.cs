/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 23 августа 2026 07:14:40
 * Version: 1.0.154
 */

using LizeriumDatabase.Services.AppDataBaseService;
using LizeriumDatabase.Services.AppDataBaseService.Implements;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace LizeriumServer.IntegrationTests;

public class LauncherMarkdownRenderingTests : IAsyncLifetime
{
    private readonly MarkdownLauncherFactory _factory = new();

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var appDb = scope.ServiceProvider.GetRequiredService<IDataBaseService>();
        await appDb.RebuildAsync();
        await appDb.ExistAndCreateLauncherNewsTable();

        var publishedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        appDb.LauncherNews.AddRange(
            MarkdownNewsTestData.CreateFullMarkdownPost(publishedAtUnix),
            MarkdownNewsTestData.CreateCompactVideoPost(publishedAtUnix),
            MarkdownNewsTestData.CreateGithubPostWithoutMarkdown(publishedAtUnix));
        await appDb.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        _factory.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Launcher_RendersMarkdownQaPostsWithExpectedHtml()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Home/Launcher?search=Markdown%20QA&order=old");
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        Directory.CreateDirectory("TestResults");
        await File.WriteAllTextAsync(Path.Combine("TestResults", "launcher-markdown-rendering.html"), html);

        Assert.Contains("Markdown QA", html);
        Assert.Contains("launcher-news-reader-post", html);
        Assert.Contains("<h1", html);
        Assert.Contains("<h2", html);
        Assert.Contains("<h3", html);
        Assert.Contains("<h4", html);
        Assert.Contains("<h5", html);
        Assert.Contains("<h6", html);
        Assert.Contains("<strong>", html);
        Assert.Contains("<em>", html);
        Assert.Contains("<del>", html);
        Assert.Contains("<code>inline code</code>", html);
        Assert.Contains("<blockquote>", html);
        Assert.Contains("<ul>", html);
        Assert.Contains("<ol>", html);
        Assert.Contains("<table>", html);
        Assert.Contains("language-csharp", html);
        Assert.Contains("data-news-reader-video-src=\"https://www.youtube.com/embed/K_HoTF1LGv4\"", html);
        Assert.Contains("data-news-reader-video-src=\"https://rutube.ru/play/embed/f7359c52b38dbfd9eab1426349de6571/\"", html);
        Assert.Contains("data-news-reader-video-src=\"https://vk.com/video_ext.php?oid=121364353&amp;id=456239467\"", html);
        Assert.Contains("data-news-video-src=\"https://vk.com/video_ext.php?oid=121364353&amp;id=456239467\"", html);
        Assert.Contains("data-news-reader-platform=\"youtube\"", html);
        Assert.Contains("data-news-reader-platform=\"rutube\"", html);
        Assert.Contains("data-news-reader-platform=\"vk\"", html);
        Assert.Contains("launcher-news-reader-inline-video vertical", html);

        Assert.DoesNotContain("alert('bad')", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("src=\"https://example.com/bad\"", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("@video(", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("@video-vertical(", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Launcher_OrdersPostVideoPlatformsByLocale()
    {
        var ruClient = _factory.CreateClient();
        var ruResponse = await ruClient.GetAsync("/Home/Launcher?search=Markdown%20QA&order=old");
        ruResponse.EnsureSuccessStatusCode();
        var ruHtml = await ruResponse.Content.ReadAsStringAsync();
        AssertPlatformOrder(ruHtml, "rutube", "vk", "youtube");
        Assert.DoesNotContain("data-news-card-video-src", ruHtml);
        Assert.Contains("data-news-reader-platform=\"rutube\"", ruHtml);

        var enClient = _factory.CreateClient();
        enClient.DefaultRequestHeaders.Add("Cookie", ".AspNetCore.Culture=c=en|uic=en");
        var enResponse = await enClient.GetAsync("/Home/Launcher?search=Markdown%20QA&order=old");
        enResponse.EnsureSuccessStatusCode();
        var enHtml = await enResponse.Content.ReadAsStringAsync();
        AssertPlatformOrder(enHtml, "youtube", "vk", "rutube");
        Assert.DoesNotContain("data-news-card-video-src", enHtml);
        Assert.Contains("data-news-reader-platform=\"youtube\"", enHtml);
    }

    [Fact]
    public async Task Launcher_RendersCardTextForGithubPostWithoutMarkdown()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Home/Launcher?search=LizeriumSteam&order=old");
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();

        Assert.Contains("launcher-news-markdown", html);
        Assert.Contains("LizeriumSteam", html);
        Assert.Contains("launcher-news-github", html);
    }

    private static void AssertPlatformOrder(string html, params string[] platforms)
    {
        var positions = platforms
            .Select(platform => html.IndexOf($"data-news-video-platform=\"{platform}\"", StringComparison.Ordinal))
            .ToArray();

        Assert.All(positions, position => Assert.True(position >= 0));
        for (var index = 1; index < positions.Length; index++)
            Assert.True(positions[index - 1] < positions[index]);
    }

    private sealed class MarkdownLauncherFactory : WebApplicationFactory<Program>
    {
        private readonly string _databasePath = Path.Combine(
            Path.GetTempPath(),
            "lizerium-markdown-tests",
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
                // SQLite can release the file slightly after the test host is disposed.
            }
        }
    }
}
