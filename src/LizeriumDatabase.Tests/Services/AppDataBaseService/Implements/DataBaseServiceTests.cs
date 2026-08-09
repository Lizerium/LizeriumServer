/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 09 августа 2026 15:52:37
 * Version: 1.0.135
 */

using LizeriumDatabase.Services.AppDataBaseService;
using LizeriumDatabase.Services.AppDataBaseService.Implements;

using LizeriumUtilities.FormatsData.AppUserData;
using LizeriumUtilities.FormatsData.DataBase.Requests;
using LizeriumUtilities.FormatsData.DataBase.Response;

using Microsoft.EntityFrameworkCore;

namespace LizeriumDatabase.Tests.Services.AppDataBaseService.Implements
{
    [TestClass]
    public class DataBaseServiceTests
    {
        [TestInitialize]
        public void TestInitialize()
        {

        }

        private IDataBaseService CreateService([System.Runtime.CompilerServices.CallerMemberName] string dbName = "")
        {
            // РЎРѕР·РґР°РµРј СЂРµР°Р»СЊРЅС‹Рµ DbContextOptions СЃ InMemory provider
            var optionsBuilder = new DbContextOptionsBuilder<DataBaseService>();
            // РќР°СЃС‚СЂР°РёРІР°РµРј СЂРµР°Р»СЊРЅС‹Рµ РѕРїС†РёРё РґР»СЏ in-memory Р‘Р”
            optionsBuilder.UseInMemoryDatabase($"TestDb_{dbName}_{Guid.NewGuid()}");
            var options = optionsBuilder.Options;
            return new DataBaseService(options);
        }

        [TestMethod]
        public async Task GetAdminLauncherNewsByIdAsync_ReturnsHiddenNewsForPreview()
        {
            var service = this.CreateService();

            await service.SaveLauncherNewsAsync(new LauncherNewsDataResponse
            {
                TitleRu = "Published",
                MarkdownRu = "Visible",
                IsPublished = true,
                PublishedAtUnix = 100
            }, false);

            await service.SaveLauncherNewsAsync(new LauncherNewsDataResponse
            {
                TitleRu = "Hidden preview",
                MarkdownRu = "Draft body",
                IsPublished = false,
                PublishedAtUnix = 200
            }, false);

            var hiddenNews = (await service.GetAllAdminLauncherNewsAsync(false))
                .Single(news => news.TitleRu == "Hidden preview");

            var result = await service.GetAdminLauncherNewsByIdAsync(hiddenNews.Id, false);

            Assert.IsNotNull(result);
            Assert.AreEqual(hiddenNews.Id, result.Id);
            Assert.AreEqual("Hidden preview", result.TitleRu);
            Assert.IsFalse(result.IsPublished);
        }

        [TestMethod]
        public async Task AddCommand_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            CreateCommandViewRequest Command = new CreateCommandViewRequest
            {
                Category = "TestCategory",
                CommandNames = "TestCmd",
                ExampleInput = "input",
                Status = 1,
                CountLike = 10,
                Description = "Description",
                UrlGif = "http://example.com/gif.gif"
            };

            // Act
            var result = await service.AddCommandAsync(
                Command, false);

            // Assert
            Assert.IsTrue(result, "AddCommand РґРѕР»Р¶РµРЅ РІРµСЂРЅСѓС‚СЊ true РїСЂРё СѓСЃРїРµС€РЅРѕРј РґРѕР±Р°РІР»РµРЅРёРё");

            var commandInDb = await service.Commands.FirstOrDefaultAsync(c => c.CommandNames == "TestCmd");

            Assert.IsNotNull(commandInDb, "РљРѕРјР°РЅРґР° РґРѕР»Р¶РЅР° Р±С‹С‚СЊ РґРѕР±Р°РІР»РµРЅР° РІ Р±Р°Р·Сѓ");
            Assert.AreEqual(Command.Category, commandInDb.Category);
            Assert.AreEqual(Command.CommandNames, commandInDb.CommandNames);
            Assert.AreEqual(Command.ExampleInput, commandInDb.ExampleInput);
            Assert.AreEqual(Command.Status, commandInDb.Status);
            Assert.AreEqual(Command.CountLike, commandInDb.CountLike);
            Assert.AreEqual(Command.Description, commandInDb.Description);
            Assert.AreEqual(Command.UrlGif, commandInDb.UrlGif);
        }

        [TestMethod]
        public async Task AddCategory_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            CategoriesCommands category = new CategoriesCommands
            {
                Name = "TestCategoryKey",
                Version = "1.0",
                Repository = "http://example.com/repo",
                Title = new List<Language>
                {
                    new() { Russian = "РўРµСЃС‚РѕРІР°СЏ РєР°С‚РµРіРѕСЂРёСЏ" },
                    new() { English = "Test Category" }
                }
            };

            // Act
            var result = await service.AddCategoryAsync(category, false);

            // Assert
            Assert.IsTrue(result, "AddCategory РґРѕР»Р¶РµРЅ РІРµСЂРЅСѓС‚СЊ true РїСЂРё СѓСЃРїРµС€РЅРѕРј РґРѕР±Р°РІР»РµРЅРёРё");

            var categoriesInDb = await service.GetAllCommandCategoriesAsync(false);
            var categoryInDb = categoriesInDb.FirstOrDefault(c => c.Key == "TestCategoryKey");

            Assert.IsNotNull(categoryInDb, "РљР°С‚РµРіРѕСЂРёСЏ РґРѕР»Р¶РЅР° Р±С‹С‚СЊ РґРѕР±Р°РІР»РµРЅР° РІ Р±Р°Р·Сѓ");
            Assert.AreEqual(category.Name, categoryInDb.Key);
            Assert.AreEqual(category.Title.FirstOrDefault(t => !string.IsNullOrEmpty(t.Russian))?.Russian, categoryInDb.NameRu);
            Assert.AreEqual(category.Title.FirstOrDefault(t => !string.IsNullOrEmpty(t.English))?.English, categoryInDb.NameEn);
            Assert.AreEqual(category.Version, categoryInDb.Version);
            Assert.AreEqual(category.Repository, categoryInDb.Repository);
        }


        [TestMethod]
        public async Task ChangeCommand_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // РЎРѕР·РґР°РµРј Рё РґРѕР±Р°РІР»СЏРµРј С‚РµСЃС‚РѕРІСѓСЋ РєРѕРјР°РЅРґСѓ РІ Р±Р°Р·Сѓ
            var initialCommand = new CommandDataResponse
            {
                Id = 1,
                Category = "InitialCategory",
                CommandNames = "InitialCmd",
                ExampleInput = "initial input",
                Status = 0,
                CountLike = 0,
                Description = "Initial description",
                UrlGif = "http://initial.url/gif.gif"
            };
            await service.Commands.AddAsync(initialCommand);
            await service.SaveChangesAsync();

            CreateCommandViewRequest changedCommand = new CreateCommandViewRequest
            {
                Id = 1,
                Status = 1,
                Description = "Changed description",
                UrlGif = "http://changed.url/gif.gif",
                ExampleInput = "changed input",
                CommandNames = "ChangedCmd",
                CountLike = 5
            };

            // Act
            var result = await service.ChangeCommandAsync(
                changedCommand, false);

            // Assert
            Assert.IsTrue(result, "ChangeCommand РґРѕР»Р¶РµРЅ РІРµСЂРЅСѓС‚СЊ true РїСЂРё СѓСЃРїРµС€РЅРѕРј РѕР±РЅРѕРІР»РµРЅРёРё");

            var updatedCommand = await service.Commands.FindAsync(1);
            Assert.IsNotNull(updatedCommand, "РљРѕРјР°РЅРґР° РґРѕР»Р¶РЅР° СЃСѓС‰РµСЃС‚РІРѕРІР°С‚СЊ РІ Р±Р°Р·Рµ");
            Assert.AreEqual(changedCommand.Status, updatedCommand.Status);
            Assert.AreEqual(changedCommand.Description, updatedCommand.Description);
            Assert.AreEqual(changedCommand.UrlGif, updatedCommand.UrlGif);
            Assert.AreEqual(changedCommand.ExampleInput, updatedCommand.ExampleInput);
            Assert.AreEqual(changedCommand.CommandNames, updatedCommand.CommandNames);
            Assert.AreEqual(changedCommand.CountLike, updatedCommand.CountLike);
        }

        [TestMethod]
        public async Task SaveGifCommand_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Р”РѕР±Р°РІР»СЏРµРј С‚РµСЃС‚РѕРІСѓСЋ РєРѕРјР°РЅРґСѓ РІ Р±Р°Р·Сѓ СЃ id = 1
            var command = new CommandDataResponse
            {
                Id = 1,
                UrlGif = "http://old.url/gif.gif"
            };
            await service.Commands.AddAsync(command);
            await service.SaveChangesAsync();

            string fileName = "http://new.url/gif.gif";
            string id = "1";

            // Act
            var result = await service.SaveGifCommandAsync(
                fileName,
                id, false);

            var updatedCommand = await service.Commands.FindAsync(1);
            Assert.IsNotNull(updatedCommand);
            Assert.AreEqual(fileName, updatedCommand.UrlGif);

            // РџСЂРѕРІРµСЂРєР° РїРѕРІРµРґРµРЅРёСЏ РїСЂРё РЅРµСЃСѓС‰РµСЃС‚РІСѓСЋС‰РµРј id
            var resultNotFound = await service.SaveGifCommandAsync("any.gif", "9999", false);
            Assert.IsFalse(resultNotFound, "SaveGifCommand РґРѕР»Р¶РµРЅ РІРµСЂРЅСѓС‚СЊ false РµСЃР»Рё РєРѕРјР°РЅРґР° РЅРµ РЅР°Р№РґРµРЅР°");

            // РџСЂРѕРІРµСЂРєР° РїРѕРІРµРґРµРЅРёСЏ СЃ null id (РѕР¶РёРґР°РµС‚СЃСЏ РёСЃРєР»СЋС‡РµРЅРёРµ, РјРѕР¶РЅРѕ РїСЂРѕРІРµСЂРёС‚СЊ)
            var resultErrorParams = await service.SaveGifCommandAsync("any.gif", null, false);
            Assert.IsFalse(resultErrorParams, "SaveGifCommand РґРѕР»Р¶РµРЅ РІРµСЂРЅСѓС‚СЊ false РµСЃР»Рё РїР°СЂР°РјРµС‚СЂС‹ РЅРµ РІРµСЂРЅС‹Рµ");
        }

        [TestMethod]
        public async Task SaveCommandsFromJson_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            var jsonData = new CommandsFileRequest
            {
                Data = new List<CategoryItem>
                {
                    new CategoryItem
                    {
                        Name = "TestCategory",
                        Commands = new List<CommandInfo>
                        {
                            new CommandInfo
                            {
                                Commands = "cmd1",
                                Status = 1,
                                Description = "Test Description",
                                Example = "example input"
                            },
                            new CommandInfo
                            {
                                Commands = "cmd2",
                                Status = 2,
                                Description = "Another Description",
                                Example = "another input"
                            }
                        }
                    }
                }
            };

            // Act
            await service.SaveCommandsFromJsonAsync(jsonData, false);

            // Assert
            var allCommands = await service.Commands.ToListAsync();

            Assert.AreEqual(2, allCommands.Count, "Р”РѕР»Р¶РЅРѕ Р±С‹С‚СЊ РґРѕР±Р°РІР»РµРЅРѕ 2 РєРѕРјР°РЅРґС‹");

            Assert.IsTrue(allCommands.Any(c => c.CommandNames == "cmd1" && c.Category == "TestCategory"));
            Assert.IsTrue(allCommands.Any(c => c.CommandNames == "cmd2" && c.Category == "TestCategory"));
        }

        [TestMethod]
        public async Task GetAllAdminPosts_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Р”РѕР±Р°РІР»СЏРµРј С‚РµСЃС‚РѕРІС‹Рµ РїРѕСЃС‚С‹ СЃ СЂР°Р·РЅС‹РјРё РґР°С‚Р°РјРё
            var post1 = new PostDataResponse { Id = 1, Message = "Post1", DateTimeUnix = 100 };
            var post2 = new PostDataResponse { Id = 2, Message = "Post2", DateTimeUnix = 200 };
            var post3 = new PostDataResponse { Id = 3, Message = "Post3", DateTimeUnix = 300 };
            var post4 = new PostDataResponse { Id = 4, Message = "Post4", DateTimeUnix = 400 };
            var post5 = new PostDataResponse { Id = 5, Message = "Post5", DateTimeUnix = 500 };
            var post6 = new PostDataResponse { Id = 6, Message = "Post6", DateTimeUnix = 600 };
            var post7 = new PostDataResponse { Id = 7, Message = "Post7", DateTimeUnix = 700 };
            var post8 = new PostDataResponse { Id = 8, Message = "Post8", DateTimeUnix = 800 };
            var post9 = new PostDataResponse { Id = 9, Message = "Post9", DateTimeUnix = 900 };
            var post10 = new PostDataResponse { Id = 10, Message = "Post10", DateTimeUnix = 1000 };
            var post11 = new PostDataResponse { Id = 11, Message = "Post11", DateTimeUnix = 1100 };

            await service.Posts.AddRangeAsync(post1, post2, post3, post4, post5, post6, post7, post8, post9, post10, post11);
            await service.SaveChangesAsync();

            // Act
            var result = await service.GetAllAdminPostsAsync();

            // Assert
            Assert.IsNotNull(result, "Р РµР·СѓР»СЊС‚Р°С‚ РЅРµ РґРѕР»Р¶РµРЅ Р±С‹С‚СЊ null");
            Assert.AreEqual(10, result.Count, "Р”РѕР»Р¶РЅРѕ РІРµСЂРЅСѓС‚СЊ СЂРѕРІРЅРѕ 10 РїРѕСЃР»РµРґРЅРёС… РїРѕСЃС‚РѕРІ");

            // РџСЂРѕРІРµСЂСЏРµРј, С‡С‚Рѕ РІРѕР·РІСЂР°С‰Р°СЋС‚СЃСЏ РїРѕСЃР»РµРґРЅРёРµ 10 РїРѕ РґР°С‚Рµ (С‚.Рµ. СЃ СЃР°РјС‹Рј Р±РѕР»СЊС€РёРј DateTimeUnix)
            var expectedOrder = new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            CollectionAssert.AreEqual(expectedOrder, result.Select(p => p.Id).ToList(), "РџРѕСЃС‚С‹ РґРѕР»Р¶РЅС‹ Р±С‹С‚СЊ РІ РїСЂР°РІРёР»СЊРЅРѕРј РїРѕСЂСЏРґРєРµ");
        }

        [TestMethod]
        public async Task GetAllAdminCommands_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // РџРѕРґРіРѕС‚РѕРІРєР° РґР°РЅРЅС‹С…: РґРѕР±Р°РІРёРј РЅРµСЃРєРѕР»СЊРєРѕ РєРѕРјР°РЅРґ
            var command1 = new CommandDataResponse
            {
                Id = 1,
                Category = "Cat1",
                CommandNames = "Cmd1",
                ExampleInput = "Ex1",
                Status = 1,
                CountLike = 5,
                Description = "Desc1",
                UrlGif = "http://gif1"
            };

            var command2 = new CommandDataResponse
            {
                Id = 2,
                Category = "Cat2",
                CommandNames = "Cmd2",
                ExampleInput = "Ex2",
                Status = 2,
                CountLike = 10,
                Description = "Desc2",
                UrlGif = "http://gif2"
            };

            await service.Commands.AddRangeAsync(command1, command2);
            await service.SaveChangesAsync();

            // Act
            var result = await service.GetAllAdminCommandsAsync(false);

            // Assert
            Assert.IsNotNull(result, "РњРµС‚РѕРґ РґРѕР»Р¶РµРЅ РІРµСЂРЅСѓС‚СЊ РЅРµРїСѓСЃС‚РѕР№ СЃРїРёСЃРѕРє");
            Assert.AreEqual(2, result.Count, "Р”РѕР»Р¶РЅРѕ Р±С‹С‚СЊ 2 РєРѕРјР°РЅРґС‹ РІ СЃРїРёСЃРєРµ");

            Assert.IsTrue(result.Any(c => c.CommandNames == "Cmd1"), "Р’ СЃРїРёСЃРєРµ РґРѕР»Р¶РЅР° Р±С‹С‚СЊ РєРѕРјР°РЅРґР° Cmd1");
            Assert.IsTrue(result.Any(c => c.CommandNames == "Cmd2"), "Р’ СЃРїРёСЃРєРµ РґРѕР»Р¶РЅР° Р±С‹С‚СЊ РєРѕРјР°РЅРґР° Cmd2");
        }

        [TestMethod]
        public async Task GetPublishedProductsCatalog_ReturnsActiveCategoriesProductsAndLinksInSortOrder()
        {
            // Arrange
            var service = this.CreateService();

            await service.ProductCategories.AddRangeAsync(
                new ProductCategoryDataResponse
                {
                    Id = 1,
                    Key = "hidden",
                    NameRu = "РЎРєСЂС‹С‚Р°СЏ",
                    NameEn = "Hidden",
                    SortOrder = 1,
                    IsActive = false
                },
                new ProductCategoryDataResponse
                {
                    Id = 2,
                    Key = "launcher",
                    NameRu = "Р›Р°СѓРЅС‡РµСЂ",
                    NameEn = "Launcher",
                    SortOrder = 2,
                    IsActive = true
                });

            await service.Products.AddRangeAsync(
                new ProductDataResponse
                {
                    Id = 1,
                    ProductCategoryId = 2,
                    TitleRu = "РЎРєСЂС‹С‚С‹Р№ РїСЂРѕРґСѓРєС‚",
                    TitleEn = "Hidden product",
                    SortOrder = 1,
                    IsActive = false
                },
                new ProductDataResponse
                {
                    Id = 2,
                    ProductCategoryId = 2,
                    TitleRu = "Р—Р°РіСЂСѓР·С‡РёРє Р›РёР·РµСЂРёСѓРј",
                    TitleEn = "Lizerium uploader",
                    DescriptionRu = "РЎРєР°С‡РёРІР°РЅРёРµ РѕР±РЅРѕРІР»РµРЅРёР№",
                    DescriptionEn = "Downloads updates",
                    IconUrl = "/img/pages/game/launcher.webp",
                    SortOrder = 2,
                    IsActive = true
                });

            await service.ProductDownloadLinks.AddRangeAsync(
                new ProductDownloadLinkDataResponse
                {
                    Id = 1,
                    ProductId = 2,
                    NameRu = "РЇРЅРґРµРєСЃ Р”РёСЃРє",
                    NameEn = "Yandex Disk",
                    Url = "https://disk.yandex.ru/example",
                    IconUrl = "/img/pages/game/yandex-disk.webp",
                    SortOrder = 2,
                    IsActive = true
                },
                new ProductDownloadLinkDataResponse
                {
                    Id = 2,
                    ProductId = 2,
                    NameRu = "РЎ РџРѕСЂС‚Р°Р»Р°",
                    NameEn = "Portal",
                    Url = "/uploader/projects/download/steam",
                    IconUrl = "/img/pages/game/portal.webp",
                    SortOrder = 1,
                    IsActive = true
                });
            await service.SaveChangesAsync();

            // Act
            var result = await service.GetPublishedProductCatalogAsync(false);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("launcher", result[0].Key);
            Assert.AreEqual(1, result[0].Products.Count);
            Assert.AreEqual("Р—Р°РіСЂСѓР·С‡РёРє Р›РёР·РµСЂРёСѓРј", result[0].Products[0].TitleRu);
            Assert.AreEqual(2, result[0].Products[0].DownloadLinks.Count);
            Assert.AreEqual("РЎ РџРѕСЂС‚Р°Р»Р°", result[0].Products[0].DownloadLinks[0].NameRu);
            Assert.AreEqual("РЇРЅРґРµРєСЃ Р”РёСЃРє", result[0].Products[0].DownloadLinks[1].NameRu);
        }

        [TestMethod]
        public async Task SearchCommands_State_Test()
        {
            // Arrange
            var service = this.CreateService();

            // Р”РѕР±Р°РІРёРј С‚РµСЃС‚РѕРІС‹Рµ РєРѕРјР°РЅРґС‹ СЃ СЂР°Р·РЅС‹РјРё РєР°С‚РµРіРѕСЂРёСЏРјРё (РІРєР»СЋС‡Р°СЏ РґСѓР±Р»Рё)
            var command1 = new CommandDataResponse
            {
                Id = 1,
                Category = "Category1",
                CommandNames = "Cmd1",
                ExampleInput = "Input1",
                Status = 1,
                CountLike = 5,
                Description = "Desc1",
                UrlGif = "http://gif1"
            };
            var command2 = new CommandDataResponse
            {
                Id = 2,
                Category = "Category2",
                CommandNames = "Cmd2",
                ExampleInput = "Input2",
                Status = 1,
                CountLike = 3,
                Description = "Desc2",
                UrlGif = "http://gif2"
            };
            var command3 = new CommandDataResponse
            {
                Id = 3,
                Category = "Category1", // Р”СѓР±Р»РёРєР°С‚ РєР°С‚РµРіРѕСЂРёРё
                CommandNames = "Cmd3",
                ExampleInput = "Input3",
                Status = 1,
                CountLike = 8,
                Description = "Desc3",
                UrlGif = "http://gif3"
            };

            await service.Commands.AddRangeAsync(command1, command2, command3);
            await service.SaveChangesAsync();

            // Act
            var result = await service.SearchCommandsAsync("Cmd2", false);

            // Assert
            Assert.IsNotNull(result, "Р РµР·СѓР»СЊС‚Р°С‚ РЅРµ РґРѕР»Р¶РµРЅ Р±С‹С‚СЊ null");
            Assert.AreEqual(1, result.Count, "Р”РѕР»Р¶РЅРѕ Р±С‹С‚СЊ СЂРѕРІРЅРѕ 1 СѓРЅРёРєР°Р»СЊРЅС‹Рµ РєР°С‚РµРіРѕСЂРёРё");
            Assert.IsTrue(result[0].Category == "Category2");

            // Act
            var result2 = await service.SearchCommandsAsync("md2", false);

            // Assert
            Assert.IsNotNull(result2, "Р РµР·СѓР»СЊС‚Р°С‚ РЅРµ РґРѕР»Р¶РµРЅ Р±С‹С‚СЊ null");
            Assert.AreEqual(1, result2.Count, "Р”РѕР»Р¶РЅРѕ Р±С‹С‚СЊ СЂРѕРІРЅРѕ 1 СѓРЅРёРєР°Р»СЊРЅС‹Рµ РєР°С‚РµРіРѕСЂРёРё");
            Assert.IsTrue(result2[0].Category == "Category2");
        }

        [TestMethod]
        public async Task GetAllCommandCategories_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Р”РѕР±Р°РІРёРј С‚РµСЃС‚РѕРІС‹Рµ РєРѕРјР°РЅРґС‹ СЃ СЂР°Р·РЅС‹РјРё РєР°С‚РµРіРѕСЂРёСЏРјРё (РІРєР»СЋС‡Р°СЏ РґСѓР±Р»Рё)
            var command1 = new CommandDataResponse
            {
                Id = 1,
                Category = "Category1",
                CommandNames = "Cmd1",
                ExampleInput = "Input1",
                Status = 1,
                CountLike = 5,
                Description = "Desc1",
                UrlGif = "http://gif1"
            };
            var command2 = new CommandDataResponse
            {
                Id = 2,
                Category = "Category2",
                CommandNames = "Cmd2",
                ExampleInput = "Input2",
                Status = 1,
                CountLike = 3,
                Description = "Desc2",
                UrlGif = "http://gif2"
            };
            var command3 = new CommandDataResponse
            {
                Id = 3,
                Category = "Category1", // Р”СѓР±Р»РёРєР°С‚ РєР°С‚РµРіРѕСЂРёРё
                CommandNames = "Cmd3",
                ExampleInput = "Input3",
                Status = 1,
                CountLike = 8,
                Description = "Desc3",
                UrlGif = "http://gif3"
            };

            await service.Commands.AddRangeAsync(command1, command2, command3);
            await service.SaveChangesAsync();

            // Act
            var result = await service.GetAllCommandCategoriesAsync(false);

            // Assert
            Assert.IsNotNull(result, "Р РµР·СѓР»СЊС‚Р°С‚ РЅРµ РґРѕР»Р¶РµРЅ Р±С‹С‚СЊ null");
            Assert.AreEqual(2, result.Count, "Р”РѕР»Р¶РЅРѕ Р±С‹С‚СЊ СЂРѕРІРЅРѕ 2 СѓРЅРёРєР°Р»СЊРЅС‹Рµ РєР°С‚РµРіРѕСЂРёРё");
            Assert.AreEqual(result[0].Key, "Category1", "Р”РѕР»Р¶РЅР° СЃРѕРґРµСЂР¶Р°С‚СЊСЃСЏ РєР°С‚РµРіРѕСЂРёСЏ Category1");
            Assert.AreEqual(result[1].Key, "Category2", "Р”РѕР»Р¶РЅР° СЃРѕРґРµСЂР¶Р°С‚СЊСЃСЏ РєР°С‚РµРіРѕСЂРёСЏ Category2");
        }

        [TestMethod]
        public async Task GetCommands_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            string category = "TestCategory";

            var commands = new List<CommandDataResponse>
                {
                    new CommandDataResponse { Id = 1, Category = "TestCategory", CommandNames = "Cmd1" },
                    new CommandDataResponse { Id = 2, Category = "TestCategory", CommandNames = "Cmd2" },
                    new CommandDataResponse { Id = 3, Category = "OtherCategory", CommandNames = "Cmd3" }
                };

            await service.Commands.AddRangeAsync(commands);
            await service.SaveChangesAsync();

            // Act
            var result = await service.GetCommandsAsync(category, 1, 6, false);

            // Assert
            Assert.IsNotNull(result, "Р РµР·СѓР»СЊС‚Р°С‚ РЅРµ РґРѕР»Р¶РµРЅ Р±С‹С‚СЊ null");
            Assert.AreEqual(2, result.Count, "Р”РѕР»Р¶РЅС‹ Р±С‹С‚СЊ РІС‹Р±СЂР°РЅС‹ С‚РѕР»СЊРєРѕ РєРѕРјР°РЅРґС‹ СѓРєР°Р·Р°РЅРЅРѕР№ РєР°С‚РµРіРѕСЂРёРё");

            var commandNames = result.Select(c => c.CommandNames).ToList();
            CollectionAssert.Contains(commandNames, "Cmd1");
            CollectionAssert.Contains(commandNames, "Cmd2");
            CollectionAssert.DoesNotContain(commandNames, "Cmd3");
        }

        [TestMethod]
        public async Task GetAllPosts_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Р”РѕР±Р°РІРёРј 12 РїРѕСЃС‚РѕРІ СЃ СЂР°Р·РЅС‹Рј СЃС‚Р°С‚СѓСЃРѕРј Рё РІСЂРµРјРµРЅРµРј
            var posts = new List<PostDataResponse>
                {
                    new PostDataResponse { Id = 1, Status = 0, DateTimeUnix = 100 },
                    new PostDataResponse { Id = 2, Status = 1, DateTimeUnix = 200 },
                    new PostDataResponse { Id = 3, Status = 1, DateTimeUnix = 300 },
                    new PostDataResponse { Id = 4, Status = 0, DateTimeUnix = 400 },
                    new PostDataResponse { Id = 5, Status = 1, DateTimeUnix = 500 },
                    new PostDataResponse { Id = 6, Status = 1, DateTimeUnix = 600 },
                    new PostDataResponse { Id = 7, Status = 1, DateTimeUnix = 700 },
                    new PostDataResponse { Id = 8, Status = 1, DateTimeUnix = 800 },
                    new PostDataResponse { Id = 9, Status = 1, DateTimeUnix = 900 },
                    new PostDataResponse { Id = 10, Status = 1, DateTimeUnix = 1000 },
                    new PostDataResponse { Id = 11, Status = 1, DateTimeUnix = 1100 },
                    new PostDataResponse { Id = 12, Status = 1, DateTimeUnix = 1200 }
                };

            await service.Posts.AddRangeAsync(posts);
            await service.SaveChangesAsync();

            // Act
            var result = await service.GetAllPostsAsync();

            // Assert
            Assert.IsNotNull(result, "Р РµР·СѓР»СЊС‚Р°С‚ РЅРµ РґРѕР»Р¶РµРЅ Р±С‹С‚СЊ null");
            Assert.AreEqual(10, result.Count, "Р”РѕР»Р¶РЅРѕ РІРµСЂРЅСѓС‚СЊСЃСЏ 10 РїРѕСЃС‚РѕРІ СЃРѕ СЃС‚Р°С‚СѓСЃРѕРј > 0");

            // РџСЂРѕРІРµСЂСЏРµРј, С‡С‚Рѕ РІРµСЂРЅСѓР»РёСЃСЊ РїРѕСЃР»РµРґРЅРёРµ 10 (Р±РµР· РїРѕСЃС‚РѕРІ СЃРѕ СЃС‚Р°С‚СѓСЃРѕРј 0)
            var expectedIds = new[] { 12, 11, 10, 9, 8, 7, 6, 5, 3, 2 };
            CollectionAssert.AreEqual(expectedIds, result.Select(p => p.Id).ToList(), "РџРѕСЃС‚С‹ РґРѕР»Р¶РЅС‹ Р±С‹С‚СЊ РѕС‚СЃРѕСЂС‚РёСЂРѕРІР°РЅС‹ РїРѕ РґР°С‚Рµ СѓР±С‹РІР°РЅРёСЋ Рё С‚РѕР»СЊРєРѕ СЃРѕ СЃС‚Р°С‚СѓСЃРѕРј > 0");
        }

        [TestMethod]
        public async Task GetAllPosts_StateUnderTest_ExpectedBehavior1()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            int status = 0;
            bool scroll = false;

            var posts = new List<PostDataResponse>
                {
                    new PostDataResponse { Id = 1, Status = 1, DateTimeUnix = 100 },
                    new PostDataResponse { Id = 2, Status = 0, DateTimeUnix = 200 },
                    new PostDataResponse { Id = 3, Status = 1, DateTimeUnix = 300 },
                    new PostDataResponse { Id = 4, Status = 2, DateTimeUnix = 400 },
                    new PostDataResponse { Id = 5, Status = 1, DateTimeUnix = 500 },
                    new PostDataResponse { Id = 6, Status = 2, DateTimeUnix = 600 },
                    new PostDataResponse { Id = 7, Status = 0, DateTimeUnix = 700 },
                    new PostDataResponse { Id = 8, Status = 1, DateTimeUnix = 800 },
                    new PostDataResponse { Id = 9, Status = 2, DateTimeUnix = 900 },
                    new PostDataResponse { Id = 10, Status = 1, DateTimeUnix = 1000 },
                    new PostDataResponse { Id = 11, Status = 1, DateTimeUnix = 1100 }
                };

            await service.Posts.AddRangeAsync(posts);
            await service.SaveChangesAsync();

            // Act
            var result = await service.GetAllPostsAsync(
                id,
                status,
                scroll);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Posts);
            Assert.AreEqual(10, result.Posts.Count, "Р”РѕР»Р¶РЅРѕ РІРµСЂРЅСѓС‚СЊСЃСЏ 10 РїРѕСЃР»РµРґРЅРёС… РїРѕСЃС‚РѕРІ");

            var expectedIds = posts
              .OrderBy(p => p.Id)              // СЃРЅР°С‡Р°Р»Р° СЃРѕСЂС‚РёСЂРѕРІРєР° РїРѕ Id (РЅРѕ РѕРЅР° СЃСЂР°Р·Сѓ Р¶Рµ РїРµСЂРµР±РёРІР°РµС‚СЃСЏ)
              .OrderBy(p => p.DateTimeUnix)    // СЃРѕСЂС‚РёСЂРѕРІРєР° РїРѕ РґР°С‚Рµ
              .Reverse()                       // РїРµСЂРµРІРѕСЂРѕС‚
              .Take(10)
              .Select(p => p.Id)
              .ToList();

            var actualIds = result.Posts.Select(p => p.Id).ToList();

            CollectionAssert.AreEqual(expectedIds, actualIds, "РџРѕСЃС‚С‹ РґРѕР»Р¶РЅС‹ Р±С‹С‚СЊ РѕС‚СЃРѕСЂС‚РёСЂРѕРІР°РЅС‹ РїРѕ СѓР±С‹РІР°РЅРёСЋ РґР°С‚С‹");
            Assert.AreEqual(0, result.LastUserId, "LastUserId РґРѕР»Р¶РµРЅ СЃРѕРѕС‚РІРµС‚СЃС‚РІРѕРІР°С‚СЊ РїРµСЂРµРґР°РЅРЅРѕРјСѓ РїР°СЂР°РјРµС‚СЂСѓ");
        }

        [TestMethod]
        public async Task GetAllPosts_StateUnderTest_ExpectedBehavior2()
        {
            // Arrange
            var service = this.CreateService();
            long lastUserId = 0;

            var posts = new List<PostDataResponse>
                {
                    new PostDataResponse { Id = 1, DateTimeUnix = 100 },
                    new PostDataResponse { Id = 2, DateTimeUnix = 200 },
                    new PostDataResponse { Id = 3, DateTimeUnix = 300 },
                    new PostDataResponse { Id = 4, DateTimeUnix = 400 },
                    new PostDataResponse { Id = 5, DateTimeUnix = 500 },
                    new PostDataResponse { Id = 6, DateTimeUnix = 600 },
                    new PostDataResponse { Id = 7, DateTimeUnix = 700 },
                    new PostDataResponse { Id = 8, DateTimeUnix = 800 },
                    new PostDataResponse { Id = 9, DateTimeUnix = 900 },
                    new PostDataResponse { Id = 10, DateTimeUnix = 1000 },
                    new PostDataResponse { Id = 11, DateTimeUnix = 1100 }
                };

            await service.Posts.AddRangeAsync(posts);
            await service.SaveChangesAsync();

            // Act
            var result = await service.GetAllPostsAsync(
                lastUserId);


            // Assert
            Assert.IsNotNull(result, "Р РµР·СѓР»СЊС‚Р°С‚ РЅРµ РґРѕР»Р¶РµРЅ Р±С‹С‚СЊ null");
            Assert.IsNotNull(result.Posts, "РЎРїРёСЃРѕРє РїРѕСЃС‚РѕРІ РЅРµ РґРѕР»Р¶РµРЅ Р±С‹С‚СЊ null");
            Assert.AreEqual(lastUserId, result.LastUserId, "LastUserId РґРѕР»Р¶РµРЅ СЃРѕРІРїР°РґР°С‚СЊ СЃ РїРµСЂРµРґР°РЅРЅС‹Рј Р·РЅР°С‡РµРЅРёРµРј");

            var expected = posts
                .Where(p => p.Id < lastUserId)
                .OrderBy(p => p.Id)
                .OrderBy(p => p.DateTimeUnix)
                .Reverse()
                .Take(30)
                .Select(p => p.Id)
                .ToList();

            var actual = result.Posts.Select(p => p.Id).ToList();

            CollectionAssert.AreEqual(expected, actual, "РЎРїРёСЃРѕРє РїРѕСЃС‚РѕРІ РґРѕР»Р¶РµРЅ СЃРѕРѕС‚РІРµС‚СЃС‚РІРѕРІР°С‚СЊ РѕР¶РёРґР°РµРјРѕРјСѓ РїРѕСЂСЏРґРєСѓ Рё С„РёР»СЊС‚СЂР°С†РёРё");
        }

        [TestMethod]
        public async Task UpdateStatusPost_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // РЎРѕР·РґР°РµРј Рё РґРѕР±Р°РІР»СЏРµРј С‚РµСЃС‚РѕРІС‹Р№ РїРѕСЃС‚ СЃ Id = 1 Рё СЃС‚Р°С‚СѓСЃРѕРј 0
            var post = new PostDataResponse
            {
                Id = 1,
                Status = 0
            };
            await service.Posts.AddAsync(post);
            await service.SaveChangesAsync();

            long lastUserId = 1;
            int newStatus = 5;

            // Act
            var result = await service.UpdateStatusPostAsync(
                lastUserId,
                newStatus);

            // Assert
            Assert.IsTrue(result, "UpdateStatusPost РґРѕР»Р¶РµРЅ РІРµСЂРЅСѓС‚СЊ true РїСЂРё СѓСЃРїРµС€РЅРѕРј РѕР±РЅРѕРІР»РµРЅРёРё");

            var updatedPost = await service.Posts.FindAsync((int)lastUserId);
            Assert.IsNotNull(updatedPost);
            Assert.AreEqual(newStatus, updatedPost.Status);

            // РџСЂРѕРІРµСЂРєР°, С‡С‚Рѕ РјРµС‚РѕРґ РІРѕР·РІСЂР°С‰Р°РµС‚ false, РµСЃР»Рё РїРѕСЃС‚ РЅРµ РЅР°Р№РґРµРЅ
            var resultNotFound = await service.UpdateStatusPostAsync(9999, newStatus);
            Assert.IsFalse(resultNotFound, "UpdateStatusPost РґРѕР»Р¶РµРЅ РІРµСЂРЅСѓС‚СЊ false, РµСЃР»Рё РїРѕСЃС‚ РЅРµ РЅР°Р№РґРµРЅ");
        }

        [TestMethod]
        public async Task IsValidUserApiKeyAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            UserApiKeyResponse testUser = new UserApiKeyResponse
            {
                IdUser = 1,
                ApiKey = "eb63be42-ed12611c-ae3e4e70-3bb03d82"
            };

            await service.Users.AddAsync(testUser);
            await service.SaveChangesAsync();

            var request = new UserApiKeyData
            {
                ApiKey = "eb63be42-ed12611c-ae3e4e70-3bb03d82"
            };


            // Act
            var result = await service.IsValidUserApiKeyAsync(
                request);

            // Assert
            Assert.IsTrue(result, "РћР¶РёРґР°Р»РѕСЃСЊ, С‡С‚Рѕ API-РєР»СЋС‡ СЃСѓС‰РµСЃС‚РІСѓРµС‚ РІ Р±Р°Р·Рµ");
        }

        [TestMethod]
        public async Task AddPost_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            CreatePostViewRequest Post = new CreatePostViewRequest
            {
                Autor = "TestUser",
                Status = 1,
                Message = "Test message"
            };

            // Act
            var result = await service.AddPostAsync(
                Post);

            var postInDb = await service.Posts.FirstOrDefaultAsync(p => p.Autor == "TestUser");

            // Assert
            Assert.IsNotNull(postInDb, "РџРѕСЃС‚ РґРѕР»Р¶РµРЅ Р±С‹С‚СЊ РґРѕР±Р°РІР»РµРЅ РІ Р±Р°Р·Сѓ");
            Assert.AreEqual(Post.Message, postInDb.Message, "РЎРѕРѕР±С‰РµРЅРёРµ РґРѕР»Р¶РЅРѕ СЃРѕРІРїР°РґР°С‚СЊ");
            Assert.AreEqual(Post.Status, postInDb.Status);
        }

        [TestMethod]
        public async Task Rebuild_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            await service.RebuildAsync();

            // Assert
            var canConnect = await service.Database.CanConnectAsync();
            Assert.IsTrue(canConnect, "Database should be available after Rebuild");
        }

        [TestMethod]
        public void Dispose_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            // РџСЂРѕРІРµСЂСЏРµРј, С‡С‚Рѕ Dispose РЅРµ РІС‹Р±СЂР°СЃС‹РІР°РµС‚ РёСЃРєР»СЋС‡РµРЅРёР№
            service.Dispose();

            Assert.IsTrue(true);
        }
    }
}
