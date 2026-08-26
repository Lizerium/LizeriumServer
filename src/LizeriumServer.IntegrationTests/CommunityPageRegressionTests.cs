/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 26 августа 2026 07:14:21
 * Version: 1.0.158
 */

namespace LizeriumServer.IntegrationTests;

public class CommunityPageRegressionTests
{
    [Fact]
    public async Task DocumentsPage_GroupsServerSolutionsAndUsesPortalAnimation()
    {
        var repoRoot = GetRepoRoot();
        var documentsView = Path.Combine(
            repoRoot,
            "src",
            "LizeriumServer",
            "Views",
            "Documents",
            "Documents.cshtml");
        var globalStyles = Path.Combine(
            repoRoot,
            "src",
            "LizeriumServer",
            "ScriptsAndCss",
            "CssFiles",
            "base",
            "_global.scss");

        var view = await File.ReadAllTextAsync(documentsView);
        var styles = await File.ReadAllTextAsync(globalStyles);

        Assert.Contains("lizerium-docs-primary-grid", view);
        Assert.Contains("lizerium-docs-server-section", view);
        Assert.Contains("lizerium-doc-card-server", view);
        Assert.Contains("ecosystem-server.webp", view);
        Assert.Contains("Documents_Section_Main", view);
        Assert.Contains("Documents_Section_Server", view);

        Assert.Contains("ecosystem-server-bg.webp", styles);
        Assert.Contains(".lizerium-docs-server-section", styles);
        Assert.Contains(".is-ready .lizerium-doc-card", styles);
        Assert.Contains(".is-ready .lizerium-docs-section-heading", styles);
    }

    [Fact]
    public async Task CommunityPage_UsesWebpPlatformIconsAndForumDevelopmentState()
    {
        var repoRoot = GetRepoRoot();
        var communityView = Path.Combine(
            repoRoot,
            "src",
            "LizeriumServer",
            "Views",
            "Home",
            "Community.cshtml");
        var globalStyles = Path.Combine(
            repoRoot,
            "src",
            "LizeriumServer",
            "ScriptsAndCss",
            "CssFiles",
            "base",
            "_global.scss");

        var view = await File.ReadAllTextAsync(communityView);
        var styles = await File.ReadAllTextAsync(globalStyles);

        Assert.Contains("community-video-tv.webp", view);
        Assert.Contains("vk.webp", view);
        Assert.Contains("discord.webp", view);
        Assert.Contains("youtube.webp", view);
        Assert.Contains("rutube.webp", view);
        Assert.Contains("vk-video.webp", view);
        Assert.Contains("community-forum-development", view);
        Assert.Contains("lizerium-reveal", view);

        Assert.Contains(".community-link-icon", styles);
        Assert.Contains(".community-forum-development", styles);
        Assert.Contains(".community-forum-progress", styles);
        Assert.Contains("@media (max-width: 1180px)", styles);
        Assert.Contains("@media (max-width: 840px)", styles);
        Assert.Contains("white-space: nowrap", styles);
    }

    private static string GetRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "README.md")))
            directory = directory.Parent;

        return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root was not found.");
    }
}
