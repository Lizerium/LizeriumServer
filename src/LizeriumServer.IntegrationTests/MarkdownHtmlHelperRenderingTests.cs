/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 14 августа 2026 08:37:41
 * Version: 1.0.145
 */

using LizeriumServer.Helpers;

namespace LizeriumServer.IntegrationTests;

public class MarkdownHtmlHelperRenderingTests
{
    [Fact]
    public void ToSafeHtml_RendersMarkdownAndControlledInlineVideos()
    {
        const string markdown = """
# Heading 1

## Heading 2

Paragraph with **bold**, *italic*, ~~strike~~ and `inline code`.

[Link](https://example.com "Example title")

![Image alt](/img/news/test.webp)

> Quote

- Item
    - Nested item

1. First
2. Second

| Feature | Value |
| --- | ---: |
| Table | 100 |

```csharp
var marker = "<safe-code>";
```

@video(https://www.youtube.com/watch?v=K_HoTF1LGv4)

https://rutube.ru/video/f7359c52b38dbfd9eab1426349de6571/

@video-vertical(https://vk.com/clip121364353_456239467)

<script>alert('bad')</script>
<iframe src="https://example.com/bad"></iframe>
""";

        var html = MarkdownHtmlHelper.ToSafeHtml(markdown, lazyImages: true);

        Assert.Contains("<h1", html);
        Assert.Contains("<h2", html);
        Assert.Contains("<strong>bold</strong>", html);
        Assert.Contains("<em>italic</em>", html);
        Assert.Contains("<del>strike</del>", html);
        Assert.Contains("<code>inline code</code>", html);
        Assert.Contains("<a href=\"https://example.com\"", html);
        Assert.Contains("<img", html);
        Assert.Contains("loading=\"lazy\"", html);
        Assert.Contains("<blockquote>", html);
        Assert.Contains("<ul>", html);
        Assert.Contains("<ol>", html);
        Assert.Contains("<table>", html);
        Assert.Contains("language-csharp", html);
        Assert.Contains("data-news-reader-video-src=\"https://www.youtube.com/embed/K_HoTF1LGv4\"", html);
        Assert.Contains("data-news-reader-video-src=\"https://rutube.ru/play/embed/f7359c52b38dbfd9eab1426349de6571/\"", html);
        Assert.Contains("launcher-news-reader-inline-video vertical", html);

        Assert.DoesNotContain("alert('bad')", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("src=\"https://example.com/bad\"", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToSafeHtml_DoesNotConvertVideoUrlsInsideCodeFences()
    {
        const string markdown = """
```text
https://www.youtube.com/watch?v=K_HoTF1LGv4
@video(https://vk.com/clip121364353_456239467)
```
""";

        var html = MarkdownHtmlHelper.ToSafeHtml(markdown);

        Assert.DoesNotContain("data-news-video-player", html, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("youtube.com/watch?v=K_HoTF1LGv4", html);
        Assert.Contains("@video", html);
    }
}
