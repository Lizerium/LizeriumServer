/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 09 августа 2026 15:52:37
 * Version: 1.0.135
 */

namespace LizeriumServer.IntegrationTests;

public class LauncherScriptRegressionTests
{
    [Fact]
    public async Task LauncherVideoHydrator_DoesNotOverrideInlineVideoSourcesWithPostPlatformButtons()
    {
        var repoRoot = GetRepoRoot();
        var launcherScript = Path.Combine(
            repoRoot,
            "src",
            "LizeriumServer",
            "ScriptsAndCss",
            "TypeScripts",
            "Home",
            "launcher.ts");

        var script = await File.ReadAllTextAsync(launcherScript);

        Assert.Contains("launcher-news-reader-inline-video", script);
        Assert.Contains("const buttons = isInlineVideo", script);
        Assert.Contains("newsVideoAvailability", script);
        Assert.Contains("newsVideoResetDefault", script);
        Assert.Contains("const activatePlatform = (button: HTMLButtonElement): void", script);
        Assert.DoesNotContain("await this.checkVideoPlatformAvailability", script);
        Assert.DoesNotContain("sessionStorage", script);
        Assert.DoesNotContain("generate_204", script);
    }

    [Fact]
    public async Task LauncherReader_OpensOnlyNearbyPostsAndExtendsWindowOnScroll()
    {
        var repoRoot = GetRepoRoot();
        var launcherScript = Path.Combine(
            repoRoot,
            "src",
            "LizeriumServer",
            "ScriptsAndCss",
            "TypeScripts",
            "Home",
            "launcher.ts");
        var launcherStyles = Path.Combine(
            repoRoot,
            "src",
            "LizeriumServer",
            "ScriptsAndCss",
            "CssFiles",
            "base",
            "_global.scss");

        var script = await File.ReadAllTextAsync(launcherScript);
        var styles = await File.ReadAllTextAsync(launcherStyles);

        Assert.Contains("const setRenderedWindow = ", script);
        Assert.Contains("const resetRenderedWindow = ", script);
        Assert.Contains("appendReaderPost();", script);
        Assert.Contains("prependReaderPost();", script);
        Assert.Contains("item.classList.toggle(\"is-unloaded\", !isRendered);", script);
        Assert.Contains(".launcher-news-reader-post.is-unloaded", styles);
        Assert.Contains("display: none;", styles);
    }

    [Fact]
    public async Task LauncherShareFallback_UsesLocalizedCopiedLabel()
    {
        var repoRoot = GetRepoRoot();
        var launcherScript = Path.Combine(
            repoRoot,
            "src",
            "LizeriumServer",
            "ScriptsAndCss",
            "TypeScripts",
            "Home",
            "launcher.ts");
        var launcherView = Path.Combine(
            repoRoot,
            "src",
            "LizeriumServer",
            "Views",
            "Home",
            "Launcher.cshtml");

        var script = await File.ReadAllTextAsync(launcherScript);
        var view = await File.ReadAllTextAsync(launcherView);

        Assert.Contains("data-share-label", view);
        Assert.Contains("data-share-copied-label", view);
        Assert.Contains("Launcher_ShareCopied", view);
        Assert.Contains("button.dataset.shareCopiedLabel", script);
        Assert.Contains("button.textContent = copiedLabel;", script);
        Assert.Contains("button.textContent = shareLabel;", script);
    }

    [Fact]
    public async Task LauncherCards_DoNotAutoloadVideoIframes()
    {
        var repoRoot = GetRepoRoot();
        var launcherScript = Path.Combine(
            repoRoot,
            "src",
            "LizeriumServer",
            "ScriptsAndCss",
            "TypeScripts",
            "Home",
            "launcher.ts");
        var launcherView = Path.Combine(
            repoRoot,
            "src",
            "LizeriumServer",
            "Views",
            "Home",
            "Launcher.cshtml");

        var script = await File.ReadAllTextAsync(launcherScript);
        var view = await File.ReadAllTextAsync(launcherView);

        Assert.DoesNotContain("bindLazyNewsCards", script);
        Assert.DoesNotContain("data-news-card-video-src", script);
        Assert.DoesNotContain("data-news-card-video-src", view);
        Assert.Contains("launcher-news-card-play-button", view);
        Assert.Contains("launcher-news-media video-preview", view);
    }

    private static string GetRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "README.md")))
            directory = directory.Parent;

        return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root was not found.");
    }
}
