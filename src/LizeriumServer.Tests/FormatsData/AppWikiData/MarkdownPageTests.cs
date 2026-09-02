/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 02 сентября 2026 07:18:07
 * Version: 1.0.164
 */

using LizeriumServer.FormatsData.AppWikiData;

namespace LizeriumServer.Tests.FormatsData.AppWikiData
{
    [TestClass]
    public class MarkdownPageTests
    {
        [TestMethod]
        public void Parse_ExternalKnowledgeBaseRoot_RewritesRelativeLinksToWikiRoutes()
        {
            var tempRoot = Path.Combine(Path.GetTempPath(), $"LizeriumKnowledgeBaseTests_{Guid.NewGuid():N}");
            var knowledgeBaseRoot = Path.Combine(tempRoot, "KnowledgeBase");
            var pageDirectory = Path.Combine(knowledgeBaseRoot, "ru");
            var fullPath = Path.Combine(pageDirectory, "index.md");

            try
            {
                Directory.CreateDirectory(pageDirectory);
                File.WriteAllText(fullPath, @"---
title: База знаний
---

[Информация о структурах файлов, найденных во Freelancer](./file-structures/index.md).
");

                var result = MarkdownPage.Parse(
                    fullPath,
                    knowledgeBaseRoot,
                    new MdAlertData()
                    {
                        LocInfoName = "Info",
                        LocNoteName = "Note",
                        LocWarningName = "Warning"
                });

                StringAssert.Contains(result.HtmlContent, "href=\"/wiki/KnowledgeBase/ru/file-structures/index.md\"");
                Assert.IsFalse(result.HtmlContent.Contains(tempRoot.Replace("\\", "/")));
            }
            finally
            {
                if (Directory.Exists(tempRoot))
                    Directory.Delete(tempRoot, true);
            }
        }

        [TestMethod]
        public void ConvertMDAndYamlToHTML_ParsesFrontMatterAndMarkdown()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), $"lizerium_markdown_{Guid.NewGuid():N}.md");

            try
            {
                File.WriteAllText(tempFile, @"---
title: Test title
description: Test description
---

## Section

Body text
");

                MarkdownPage.ConvertMDAndYamlToHTML(
                    tempFile,
                    out var frontMatter,
                    out var html);

                Assert.AreEqual("Test title", frontMatter["title"]);
                Assert.AreEqual("Test description", frontMatter["description"]);
                StringAssert.Contains(html, "<h1>Test title</h1>");
                StringAssert.Contains(html, "<h2 id=\"section\">Section</h2>");
                StringAssert.Contains(html, "<p>Body text</p>");
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void ReplaceH1_ReplacesFirstHeadingWithWikiHeader()
        {
            var html = "<h1>База знаний о Freelancer (2003)</h1><p>Добро пожаловать</p>";

            var result = MarkdownPage.ReplaceH1(html);

            StringAssert.Contains(result, "card-row small");
            StringAssert.Contains(result, "База знаний о Freelancer (2003)");
            StringAssert.Contains(result, "<p>Добро пожаловать</p>");
            Assert.IsFalse(result.Contains("<h1>"));
        }

        [TestMethod]
        public void SetupLinks_NormalizesAbsoluteKnowledgeBaseLinks()
        {
            var html = @"<p><a href=""/wiki/F:/LIZERIUM/LIZERIUM_SERVER/KnowledgeBase/ru/./file-structures/index.md"">Информация о структурах файлов, найденных во Freelancer</a>.</p>";

            var result = MarkdownPage.SetupLinks(html, "KnowledgeBase/ru");

            StringAssert.Contains(result, "href=\"/wiki/KnowledgeBase/ru/file-structures/index.md\"");
            Assert.IsFalse(result.Contains("F:/LIZERIUM"));
        }

        [TestMethod]
        public void SetupLinks_NormalizesRussianTableOfContentsAnchorsToGeneratedHeadingIds()
        {
            var html = @"
<ul>
    <li><a href=""#%D0%BE%D0%B3%D0%BB%D0%B0%D0%B2%D0%BB%D0%B5%D0%BD%D0%B8%D0%B5"">Оглавление</a></li>
    <li><a href=""#%D1%80%D0%B0%D1%81%D1%87%D0%B5%D1%82-%D1%81%D0%BB%D0%BE%D0%B6%D0%BD%D0%BE%D1%81%D1%82%D0%B8-%D0%BF%D1%80%D0%B5%D0%B4%D0%BB%D0%BE%D0%B6%D0%B5%D0%BD%D0%B8%D0%B9-%D0%BE-%D1%80%D0%B0%D0%B1%D0%BE%D1%82%D0%B5-mbaseini"">Расчет сложности предложений о работе (mBase.ini)</a></li>
</ul>
<h2 id=""section"">Оглавление</h2>
<h3 id=""mbase.ini"">Расчет сложности предложений о работе (<code>mBase.ini</code>)</h3>";

            var result = MarkdownPage.SetupLinks(html, "KnowledgeBase/ru");

            StringAssert.Contains(result, "href=\"#section\"");
            StringAssert.Contains(result, "href=\"#mbase.ini\"");
        }

        [TestMethod]
        public void PanelWrapHeadingsSetup_Test()
        {
            var testOldData = @"
                <div class=""theme-doc-markdown markdown"">
                    <div class=""card-row small"">
                        <div class=""subcard"">
                            <div class=""subcard-icon icon-style"">🖥️</div>
                            <div class=""subcard-text"">База знаний о Freelancer (2003)</div>
                        </div>
                    </div>
                    <p>Добро пожаловать</p>
                    <h2 id=""section"">Справочные ресурсы по моддингу</h2>
                    <p>Информация о структуре файлов, типах INI-файлов и внутренней работе Freelancer. Эти ссылки не содержат
                       конкретных руководств, но чрезвычайно полезны в качестве справочного материала при моддинге.</p>
                    <h3 id=""section-1"">Файловые структуры</h3>
                    <p><a href=""/wiki/KnowledgeBase/ru/./file-structures/index.md"">Информация о структурах файлов,
                       найденных во Freelancer</a>.</p>
                    <h2 id=""section-2"">Обзор лора</h2>
                    <p>Freelancer богат историей, и на следующих страницах вы найдете слухи, новости и другие тексты из игры,
                       которые позволят вам составить краткий обзор.</p>
                    <p><a href=""/wiki/KnowledgeBase/ru/./lore/index.md"">Указатель на лор</a></p>
                </div>";

            var html = MarkdownPage.PanelCreatorH2(testOldData);
            html = MarkdownPage.PanelCreatorH3(html);

            StringAssert.Contains(html, "knowledge_panel_2");
            StringAssert.Contains(html, "knowledge_panel_3");
        }
    }
}
