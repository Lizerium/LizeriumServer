/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 03 июня 2026 15:37:21
 * Version: 1.0.68
 */

using LizeriumServer.FormatsData.AppWikiData;

namespace LizeriumServer.Tests.FormatsData.AppWikiData
{
    [TestClass]
    public class MarkdownPageTests
    {
        [TestMethod]
        public void Parse_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var markdownPage = new MarkdownPage();
            string fullPath = null;
            string rootPath = null;

            // Act
            var result = MarkdownPage.Parse(
                fullPath,
                rootPath,
                new MdAlertData()
                {
                    LocInfoName = "Test",
                    LocNoteName = "Test",
                    LocWarningName = "Test"
                });

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void ConvertMDAndYamlToHTML_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            string fullPath = null;
            Dictionary<string, string> frontMatter = null;
            string html = null;

            // Act
            MarkdownPage.ConvertMDAndYamlToHTML(
                fullPath,
                out frontMatter,
                out html);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void ReplaceH1_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            string html = null;

            // Act
            var result = MarkdownPage.ReplaceH1(
                html);

            // Assert
            Assert.Fail();
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

            var normData = @"
                <div class=""theme-doc-markdown markdown"">
                    <div class=""card-row small"">
                        <div class=""subcard"">
                            <div class=""subcard-icon icon-style"">🖥️</div>
                            <div class=""subcard-text"">База знаний о Freelancer (2003)</div>
                        </div>
                    </div>
                    <p>Добро пожаловать</p>
                    <h2 id=""section"">Справочные ресурсы по моддингу</h2>
                    <div class=""knowledge_panel_2"">
                        <p>Информация о структуре файлов, типах INI-файлов и внутренней работе Freelancer. Эти ссылки не содержат
                           конкретных руководств, но чрезвычайно полезны в качестве справочного материала при моддинге.</p>
                        <h3 id=""section-1"">Файловые структуры</h3>
                        <div class=""knowledge_panel_3"">
                            <p><a href=""/wiki/KnowledgeBase/ru/./file-structures/index.md"">Информация о структурах файлов,
                           найденных во Freelancer</a>.</p>
                        </div>                    
                    </div>
                    <h2 id=""section-2"">Обзор лора</h2>
                    <div class=""knowledge_panel_2"">
                        <p>Freelancer богат историей, и на следующих страницах вы найдете слухи, новости и другие тексты из игры,
                           которые позволят вам составить краткий обзор.</p>
                        <p><a href=""/wiki/KnowledgeBase/ru/./lore/index.md"">Указатель на лор</a></p>
                    </div>
                </div>";
          
            var html = MarkdownPage.PanelCreatorH2(testOldData);
            html = MarkdownPage.PanelCreatorH3(html);
        }
    }
}
