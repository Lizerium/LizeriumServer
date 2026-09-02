/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 02 сентября 2026 07:18:07
 * Version: 1.0.164
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
            "pages",
            "home",
            "news.scss");

        var script = await File.ReadAllTextAsync(launcherScript);
        var styles = await File.ReadAllTextAsync(launcherStyles);

        Assert.Contains("const setRenderedWindow = ", script);
        Assert.Contains("const resetRenderedWindow = ", script);
        Assert.Contains("const ensurePostRendered = ", script);
        Assert.Contains("resetRenderedWindow(postIndex);", script);
        Assert.Contains("ensurePostRendered(targetIndex);", script);
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
    public async Task LauncherCards_LoadVideoIframesOnlyAfterPlatformClick()
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
        Assert.Contains("data-news-card-video-src", script);
        Assert.Contains("frame.src = src;", script);
        Assert.Contains("data-news-card-video-src", view);
        Assert.DoesNotContain("<iframe src=\"@videoUrl\"", view);
        Assert.Contains("launcher-news-video-poster", view);
        Assert.Contains("launcher-news-card-video", view);
    }

    [Fact]
    public async Task LauncherCanonicalNewsLinks_StartReaderAndBypassPageTransition()
    {
        var repoRoot = GetRepoRoot();
        var mainScript = Path.Combine(
            repoRoot,
            "src",
            "LizeriumServer",
            "ScriptsAndCss",
            "TypeScripts",
            "main_api.ts");
        var layout = Path.Combine(
            repoRoot,
            "src",
            "LizeriumServer",
            "Views",
            "Shared",
            "_Layout.cshtml");

        var script = await File.ReadAllTextAsync(mainScript);
        var layoutHtml = await File.ReadAllTextAsync(layout);

        Assert.Contains("partsPath[1] === \"news\"", script);
        Assert.Contains("launcher.start();", script);
        Assert.Contains("trigger.closest('[data-news-reader-open]')", layoutHtml);
    }

    private static string GetRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "README.md")))
            directory = directory.Parent;

        return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root was not found.");
    }
}
