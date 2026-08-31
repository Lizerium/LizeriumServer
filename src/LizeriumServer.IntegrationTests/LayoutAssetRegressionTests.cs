/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 31 августа 2026 07:09:02
 * Version: 1.0.162
 */

namespace LizeriumServer.IntegrationTests;

public class LayoutAssetRegressionTests
{
    [Fact]
    public async Task SharedLayout_DoesNotReferenceMissingLayoutBundle()
    {
        var repoRoot = GetRepoRoot();
        var layoutView = Path.Combine(
            repoRoot,
            "src",
            "LizeriumServer",
            "Views",
            "Shared",
            "_Layout.cshtml");

        var view = await File.ReadAllTextAsync(layoutView);

        Assert.DoesNotContain("layout.min.css", view);
    }

    [Fact]
    public async Task StaticGetPages_DoNotLoadRecaptchaScript()
    {
        var repoRoot = GetRepoRoot();
        var staticPages = new[]
        {
            Path.Combine(repoRoot, "src", "LizeriumServer", "Views", "Documents", "Documents.cshtml"),
            Path.Combine(repoRoot, "src", "LizeriumServer", "Views", "Documents", "DocumentsBuild.cshtml"),
            Path.Combine(repoRoot, "src", "LizeriumServer", "Views", "Documents", "DocumentsInstall.cshtml"),
            Path.Combine(repoRoot, "src", "LizeriumServer", "Views", "Documents", "DocumentsHook.cshtml"),
            Path.Combine(repoRoot, "src", "LizeriumServer", "Views", "Documents", "DocHook.cshtml"),
            Path.Combine(repoRoot, "src", "LizeriumServer", "Views", "Home", "Game.cshtml"),
            Path.Combine(repoRoot, "src", "LizeriumServer", "Views", "Home", "Maintenance.cshtml")
        };

        foreach (var page in staticPages)
        {
            var view = await File.ReadAllTextAsync(page);
            Assert.DoesNotContain("@Html.ReCaptcha()", view);
        }
    }

    [Fact]
    public async Task CjkFontFaces_DoNotUseRemoteFontUrls()
    {
        var repoRoot = GetRepoRoot();
        var globalStyles = Path.Combine(
            repoRoot,
            "src",
            "LizeriumServer",
            "ScriptsAndCss",
            "CssFiles",
            "base",
            "_global.scss");

        var styles = await File.ReadAllTextAsync(globalStyles);

        Assert.DoesNotContain("cdn.jsdelivr.net/gh/magiclen", styles);
        Assert.DoesNotContain("SourceHanSansTC-Regular.woff2", styles);
        Assert.DoesNotContain("SourceHanSansTC-Bold.woff2", styles);
        Assert.DoesNotContain("SourceHanSansHWTC-Regular.woff2", styles);
    }

    [Fact]
    public async Task WishPage_UsesPortalSupportSurface()
    {
        var repoRoot = GetRepoRoot();
        var wishView = Path.Combine(
            repoRoot,
            "src",
            "LizeriumServer",
            "Views",
            "Home",
            "Wish.cshtml");
        var wishScript = Path.Combine(
            repoRoot,
            "src",
            "LizeriumServer",
            "ScriptsAndCss",
            "TypeScripts",
            "Home",
            "wish.ts");
        var wishStyles = Path.Combine(
            repoRoot,
            "src",
            "LizeriumServer",
            "ScriptsAndCss",
            "CssFiles",
            "pages",
            "home",
            "wish.scss");

        var view = await File.ReadAllTextAsync(wishView);
        var script = await File.ReadAllTextAsync(wishScript);
        var styles = await File.ReadAllTextAsync(wishStyles);

        Assert.Contains("lizerium-wish-toolbar", view);
        Assert.Contains("lizerium-wish-feed", view);
        Assert.Contains("lizerium-wish-modal-panel", view);
        Assert.Contains("@Html.AntiForgeryToken()", view);
        Assert.Contains("@Html.ReCaptcha()", view);
        Assert.DoesNotContain("<div class=\"modal_body\"></div>", view);
        Assert.Contains("lizerium-wish-card", script);
        Assert.Contains("lizerium-wish-status-badge", script);
        Assert.Contains("is-open", script);
        Assert.Contains(".lizerium-wish-toolbar", styles);
        Assert.Contains(".lizerium-wish-modal-panel", styles);
        Assert.Contains("body.lizerium-modal-open", styles);
        Assert.Contains("status-delete", styles);
    }

    [Fact]
    public async Task WishCreatePost_GuardsInvalidPostsAndReturnsToWish()
    {
        var repoRoot = GetRepoRoot();
        var controller = Path.Combine(
            repoRoot,
            "src",
            "LizeriumServer",
            "Controllers",
            "HomeController.cs");

        var source = await File.ReadAllTextAsync(controller);

        Assert.Contains("PostModel == null", source);
        Assert.Contains("string.IsNullOrWhiteSpace(PostModel.Autor)", source);
        Assert.Contains("string.IsNullOrWhiteSpace(PostModel.Message)", source);
        Assert.Contains("return RedirectToAction(nameof(Wish));", source);
    }

    [Fact]
    public async Task Program_RegistersReCaptchaServiceForWishPostValidation()
    {
        var repoRoot = GetRepoRoot();
        var program = Path.Combine(
            repoRoot,
            "src",
            "LizeriumServer",
            "Program.cs");

        var source = await File.ReadAllTextAsync(program);

        Assert.Contains("builder.Services.AddReCaptcha(builder.Configuration.GetSection(\"GoogleReCaptcha\"));", source);
        Assert.DoesNotContain("builder.Services.Configure<ReCaptchaSettings>(builder.Configuration.GetSection(\"GoogleReCaptcha\"));", source);
    }

    private static string GetRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "README.md")))
            directory = directory.Parent;

        return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root was not found.");
    }
}
