/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 28 августа 2026 07:12:37
 * Version: 1.0.160
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

Абзац с **жирным**, *курсивом*, ~~зачеркнутым~~ и `inline code`.

[Ссылка с title](https://example.com "Example title")

![Тестовая картинка](/img/news/markdown-test.webp)

> Цитата верхнего уровня.
>
> > Вложенная цитата.

- Пункт списка
- Второй пункт
    - Вложенный пункт

1. Первый пункт
2. Второй пункт
    1. Вложенный номер

---

| Возможность | Статус |
| --- | ---: |
| Таблицы | 100 |
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
            TitleRu = "Markdown QA: полный тест разметки",
            TitleEn = "Markdown QA: full markup test",
            MarkdownRu = FullMarkdownRu,
            MarkdownEn = FullMarkdownEn,
            YoutubeUrl = "https://www.youtube.com/watch?v=K_HoTF1LGv4",
            RutubeUrl = "https://rutube.ru/video/f7359c52b38dbfd9eab1426349de6571/",
            VkVideoUrl = "https://vk.com/clip121364353_456239467",
            IconUrl = "/img/logo.png",
            ImageUrl = "/img/news/markdown-cover.webp",
            ImageGalleryJson = "[\"/img/news/markdown-gallery-1.webp\",\"/img/news/markdown-gallery-2.webp\"]",
            NewsTypeRu = "QA проверка",
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
            TitleRu = "Markdown QA: видео внутри текста",
            TitleEn = "Markdown QA: inline video",
            MarkdownRu = "Текст до видео.\n\nhttps://vk.com/clip121364353_456239467\n\nТекст после видео.",
            MarkdownEn = "Text before video.\n\nhttps://www.youtube.com/shorts/K_HoTF1LGv4\n\nText after video.",
            NewsTypeRu = "Видео",
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
