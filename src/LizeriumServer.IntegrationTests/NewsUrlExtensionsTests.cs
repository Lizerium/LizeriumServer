/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 26 августа 2026 07:14:21
 * Version: 1.0.158
 */

using LizeriumUtilities.Accessories.NewsAccessories;
using LizeriumUtilities.FormatsData.DataBase.Response;

namespace LizeriumServer.IntegrationTests;

public class NewsUrlExtensionsTests
{
    [Fact]
    public void GetCanonicalNewsPath_UsesIdAndEnglishSlug()
    {
        var news = new LauncherNewsDataResponse
        {
            Id = 21,
            TitleRu = "UTF РІ XML",
            TitleEn = "UTF to XML"
        };

        Assert.Equal("/news/21/utf-to-xml.html", news.GetCanonicalNewsPath("en"));
    }

    [Fact]
    public void GetNewsSlug_TransliteratesRussianTitle()
    {
        var news = new LauncherNewsDataResponse
        {
            Id = 21,
            TitleRu = "РќРѕРІРѕСЃС‚СЊ Lizerium Steam!",
            TitleEn = "Lizerium Steam News"
        };

        Assert.Equal("novost-lizerium-steam", news.GetNewsSlug("ru"));
    }

    [Fact]
    public void GetCanonicalNewsPath_FallsBackToNewsIdWhenTitlesAreEmpty()
    {
        var news = new LauncherNewsDataResponse
        {
            Id = 21,
            TitleRu = "",
            TitleEn = ""
        };

        Assert.Equal("/news/21/news-21.html", news.GetCanonicalNewsPath("ru"));
    }
}
