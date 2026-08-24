/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 24 августа 2026 07:14:27
 * Version: 1.0.156
 */

using System.Net;

using LizeriumDatabase.Services.AppDataBaseService;
using LizeriumDatabase.Services.AppDataBaseService.Implements;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace LizeriumServer.IntegrationTests;

public class KnowledgeCanonicalRouteTests : IDisposable
{
    private readonly KnowledgeFactory _factory = new();

    [Fact]
    public async Task KnowledgeArticle_RedirectsLanguageLessKnowledgeBaseUrlToCurrentCultureCanonicalUrl()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/wiki/KnowledgeBase/guides/understanding-and-adding-infocards.md");
        request.Headers.Add("Cookie", ".AspNetCore.Culture=c=ru|uic=ru");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
        Assert.Equal(
            "/wiki/KnowledgeBase/ru/guides/understanding-and-adding-infocards.md",
            response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task KnowledgeArticle_ExplicitEnglishUrlKeepsEnglishContentAndCanonicalDespiteRussianCookie()
    {
        var client = _factory.CreateClient();
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/wiki/KnowledgeBase/en/guides/understanding-and-adding-infocards.md");
        request.Headers.Add("Cookie", ".AspNetCore.Culture=c=ru|uic=ru");

        var response = await client.SendAsync(request);
        var html = await response.Content.ReadAsStringAsync();
        Directory.CreateDirectory("TestResults");
        await File.WriteAllTextAsync(Path.Combine("TestResults", "knowledge-canonical-en.html"), html);

        response.EnsureSuccessStatusCode();
        Assert.Contains("English Infocard Guide", html);
        Assert.DoesNotContain("Р СѓСЃСЃРєРѕРµ СЂСѓРєРѕРІРѕРґСЃС‚РІРѕ", html);
        Assert.Contains(
            "<link rel=\"canonical\" href=\"http://localhost/wiki/KnowledgeBase/en/guides/understanding-and-adding-infocards.md\" />",
            html);
    }

    [Fact]
    public async Task KnowledgeArticle_ExplicitRussianUrlRendersCanonicalAndAlternateLanguageLinks()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/wiki/KnowledgeBase/ru/guides/understanding-and-adding-infocards.md");
        var html = await response.Content.ReadAsStringAsync();
        Directory.CreateDirectory("TestResults");
        await File.WriteAllTextAsync(Path.Combine("TestResults", "knowledge-canonical-ru.html"), html);

        response.EnsureSuccessStatusCode();
        Assert.Contains("Р СѓСЃСЃРєРѕРµ СЂСѓРєРѕРІРѕРґСЃС‚РІРѕ", html);
        Assert.Contains(
            "<link rel=\"canonical\" href=\"http://localhost/wiki/KnowledgeBase/ru/guides/understanding-and-adding-infocards.md\" />",
            html);
        Assert.Contains(
            "hreflang=\"ru\" href=\"http://localhost/wiki/KnowledgeBase/ru/guides/understanding-and-adding-infocards.md\"",
            html);
        Assert.Contains(
            "hreflang=\"en\" href=\"http://localhost/wiki/KnowledgeBase/en/guides/understanding-and-adding-infocards.md\"",
            html);
    }

    public void Dispose()
    {
        _factory.Dispose();
    }

    private sealed class KnowledgeFactory : WebApplicationFactory<Program>
    {
        private readonly string _databasePath = Path.Combine(
            Path.GetTempPath(),
            "lizerium-knowledge-canonical-tests",
            $"{Guid.NewGuid():N}.db");

        private readonly string _knowledgeBasePath = Path.Combine(
            Path.GetTempPath(),
            "lizerium-knowledge-canonical-tests",
            $"{Guid.NewGuid():N}",
            "KnowledgeBase");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            CreateKnowledgeBaseFiles();
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["StoragePaths:KnowledgeBase"] = _knowledgeBasePath,
                    ["SeoDomains:CanonicalMode"] = "RequestHost",
                    ["SeoDomains:Scheme"] = "http",
                    ["SeoDomains:OpenGraphImage"] = "/img/Main.png",
                    ["SeoDomains:SiteName"] = "Lizerium"
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
                if (Directory.Exists(_knowledgeBasePath))
                    Directory.Delete(_knowledgeBasePath, recursive: true);
            }
            catch (IOException)
            {
            }
        }

        private void CreateKnowledgeBaseFiles()
        {
            var relativePath = Path.Combine("guides", "understanding-and-adding-infocards.md");
            WriteMarkdown(Path.Combine(_knowledgeBasePath, "ru", relativePath), "Р СѓСЃСЃРєРѕРµ СЂСѓРєРѕРІРѕРґСЃС‚РІРѕ");
            WriteMarkdown(Path.Combine(_knowledgeBasePath, "en", relativePath), "English Infocard Guide");
        }

        private static void WriteMarkdown(string path, string title)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, $"""
---
title: {title}
description: Test knowledge page
---

# {title}

Body content.
""");
        }
    }
}
