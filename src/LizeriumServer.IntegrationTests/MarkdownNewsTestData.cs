/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 08 августа 2026 07:13:54
 * Version: 1.0.134
 */

using LizeriumUtilities.FormatsData.DataBase.Response;

namespace LizeriumServer.IntegrationTests;

internal static class MarkdownNewsTestData
{
    public const string FullMarkdownRu = """
# Markdown QA H1

## Markdown QA H2

### Markdown QA H3

#### Markdown QA H4

##### Markdown QA H5

###### Markdown QA H6

РђР±Р·Р°С† СЃ **Р¶РёСЂРЅС‹Рј**, *РєСѓСЂСЃРёРІРѕРј*, ~~Р·Р°С‡РµСЂРєРЅСѓС‚С‹Рј~~ Рё `inline code`.

[РЎСЃС‹Р»РєР° СЃ title](https://example.com "Example title")

![РўРµСЃС‚РѕРІР°СЏ РєР°СЂС‚РёРЅРєР°](/img/news/markdown-test.webp)

> Р¦РёС‚Р°С‚Р° РІРµСЂС…РЅРµРіРѕ СѓСЂРѕРІРЅСЏ.
>
> > Р’Р»РѕР¶РµРЅРЅР°СЏ С†РёС‚Р°С‚Р°.

- РџСѓРЅРєС‚ СЃРїРёСЃРєР°
- Р’С‚РѕСЂРѕР№ РїСѓРЅРєС‚
    - Р’Р»РѕР¶РµРЅРЅС‹Р№ РїСѓРЅРєС‚

1. РџРµСЂРІС‹Р№ РїСѓРЅРєС‚
2. Р’С‚РѕСЂРѕР№ РїСѓРЅРєС‚
    1. Р’Р»РѕР¶РµРЅРЅС‹Р№ РЅРѕРјРµСЂ

---

| Р’РѕР·РјРѕР¶РЅРѕСЃС‚СЊ | РЎС‚Р°С‚СѓСЃ |
| --- | ---: |
| РўР°Р±Р»РёС†С‹ | 100 |
| Markdown | 200 |

```csharp
var marker = "<safe-code>";
Console.WriteLine(marker);
```

@video(https://www.youtube.com/watch?v=K_HoTF1LGv4)

https://rutube.ru/video/f7359c52b38dbfd9eab1426349de6571/

@video-vertical(https://vk.com/clip121364353_456239467)

<script>alert('bad')</script>
<iframe src="https://example.com/bad"></iframe>
""";

    public const string FullMarkdownEn = """
# Markdown QA H1 EN

## Markdown QA H2 EN

### Markdown QA H3 EN

#### Markdown QA H4 EN

##### Markdown QA H5 EN

###### Markdown QA H6 EN

Paragraph with **bold**, *italic*, ~~strike~~ and `inline code`.

[Titled link](https://example.com "Example title")

![Test image](/img/news/markdown-test-en.webp)

> Root quote.

- List item
    - Nested item

1. First item
2. Second item

| Feature | Status |
| --- | ---: |
| Tables | 100 |

```js
const marker = "<safe-code>";
console.log(marker);
```

@video(https://www.youtube.com/watch?v=K_HoTF1LGv4)

https://rutube.ru/video/f7359c52b38dbfd9eab1426349de6571/

@video-vertical(https://vk.com/clip121364353_456239467)

<script>alert('bad')</script>
<iframe src="https://example.com/bad"></iframe>
""";

    public static LauncherNewsDataResponse CreateFullMarkdownPost(long publishedAtUnix)
    {
        return new LauncherNewsDataResponse
        {
            TitleRu = "Markdown QA: РїРѕР»РЅС‹Р№ С‚РµСЃС‚ СЂР°Р·РјРµС‚РєРё",
            TitleEn = "Markdown QA: full markup test",
            MarkdownRu = FullMarkdownRu,
            MarkdownEn = FullMarkdownEn,
            YoutubeUrl = "https://www.youtube.com/watch?v=K_HoTF1LGv4",
            RutubeUrl = "https://rutube.ru/video/f7359c52b38dbfd9eab1426349de6571/",
            VkVideoUrl = "https://vk.com/clip121364353_456239467",
            IconUrl = "/img/logo.png",
            ImageUrl = "/img/news/markdown-cover.webp",
            ImageGalleryJson = "[\"/img/news/markdown-gallery-1.webp\",\"/img/news/markdown-gallery-2.webp\"]",
            NewsTypeRu = "QA РїСЂРѕРІРµСЂРєР°",
            NewsTypeEn = "QA check",
            GithubUrl = "https://github.com/dvurechensky",
            GithubProjectName = "Markdown QA",
            LikeCount = 7,
            IsPublished = true,
            SortOrder = -100,
            PublishedAtUnix = publishedAtUnix
        };
    }

    public static LauncherNewsDataResponse CreateCompactVideoPost(long publishedAtUnix)
    {
        return new LauncherNewsDataResponse
        {
            TitleRu = "Markdown QA: РІРёРґРµРѕ РІРЅСѓС‚СЂРё С‚РµРєСЃС‚Р°",
            TitleEn = "Markdown QA: inline video",
            MarkdownRu = "РўРµРєСЃС‚ РґРѕ РІРёРґРµРѕ.\n\nhttps://vk.com/clip121364353_456239467\n\nРўРµРєСЃС‚ РїРѕСЃР»Рµ РІРёРґРµРѕ.",
            MarkdownEn = "Text before video.\n\nhttps://www.youtube.com/shorts/K_HoTF1LGv4\n\nText after video.",
            NewsTypeRu = "Р’РёРґРµРѕ",
            NewsTypeEn = "Video",
            IsPublished = true,
            SortOrder = -90,
            PublishedAtUnix = publishedAtUnix - 1
        };
    }

    public static LauncherNewsDataResponse CreateGithubPostWithoutMarkdown(long publishedAtUnix)
    {
        return new LauncherNewsDataResponse
        {
            TitleRu = "Markdown QA: GitHub without body",
            TitleEn = "Markdown QA: GitHub without body EN",
            RutubeUrl = "https://rutube.ru/video/166b1de79791472c13f79c24838847c3/",
            GithubUrl = "https://github.com/Lizerium/LizeriumSteam",
            GithubProjectName = "LizeriumSteam",
            NewsTypeRu = "QA GitHub",
            NewsTypeEn = "QA GitHub",
            IsPublished = true,
            SortOrder = -80,
            PublishedAtUnix = publishedAtUnix - 2
        };
    }
}
