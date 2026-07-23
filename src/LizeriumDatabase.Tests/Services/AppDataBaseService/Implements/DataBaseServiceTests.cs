/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 23 июля 2026 13:16:14
 * Version: 1.0.117
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
            // Создаем реальные DbContextOptions с InMemory provider
            var optionsBuilder = new DbContextOptionsBuilder<DataBaseService>();
            // Настраиваем реальные опции для in-memory БД
            optionsBuilder.UseInMemoryDatabase($"TestDb_{dbName}_{Guid.NewGuid()}");
            var options = optionsBuilder.Options;
            return new DataBaseService(options);
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
            Assert.IsTrue(result, "AddCommand должен вернуть true при успешном добавлении");

            var commandInDb = await service.Commands.FirstOrDefaultAsync(c => c.CommandNames == "TestCmd");

            Assert.IsNotNull(commandInDb, "Команда должна быть добавлена в базу");
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
                    new() { Russian = "Тестовая категория" },
                    new() { English = "Test Category" }
                }
            };

            // Act
            var result = await service.AddCategoryAsync(category, false);

            // Assert
            Assert.IsTrue(result, "AddCategory должен вернуть true при успешном добавлении");

            var categoriesInDb = await service.GetAllCommandCategoriesAsync(false);
            var categoryInDb = categoriesInDb.FirstOrDefault(c => c.Key == "TestCategoryKey");

            Assert.IsNotNull(categoryInDb, "Категория должна быть добавлена в базу");
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

            // Создаем и добавляем тестовую команду в базу
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
            Assert.IsTrue(result, "ChangeCommand должен вернуть true при успешном обновлении");

            var updatedCommand = await service.Commands.FindAsync(1);
            Assert.IsNotNull(updatedCommand, "Команда должна существовать в базе");
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

            // Добавляем тестовую команду в базу с id = 1
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

            // Проверка поведения при несуществующем id
            var resultNotFound = await service.SaveGifCommandAsync("any.gif", "9999", false);
            Assert.IsFalse(resultNotFound, "SaveGifCommand должен вернуть false если команда не найдена");

            // Проверка поведения с null id (ожидается исключение, можно проверить)
            var resultErrorParams = await service.SaveGifCommandAsync("any.gif", null, false);
            Assert.IsFalse(resultErrorParams, "SaveGifCommand должен вернуть false если параметры не верные");
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

            Assert.AreEqual(2, allCommands.Count, "Должно быть добавлено 2 команды");

            Assert.IsTrue(allCommands.Any(c => c.CommandNames == "cmd1" && c.Category == "TestCategory"));
            Assert.IsTrue(allCommands.Any(c => c.CommandNames == "cmd2" && c.Category == "TestCategory"));
        }

        [TestMethod]
        public async Task GetAllAdminPosts_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Добавляем тестовые посты с разными датами
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
            Assert.IsNotNull(result, "Результат не должен быть null");
            Assert.AreEqual(10, result.Count, "Должно вернуть ровно 10 последних постов");

            // Проверяем, что возвращаются последние 10 по дате (т.е. с самым большим DateTimeUnix)
            var expectedOrder = new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            CollectionAssert.AreEqual(expectedOrder, result.Select(p => p.Id).ToList(), "Посты должны быть в правильном порядке");
        }

        [TestMethod]
        public async Task GetAllAdminCommands_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Подготовка данных: добавим несколько команд
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
            Assert.IsNotNull(result, "Метод должен вернуть непустой список");
            Assert.AreEqual(2, result.Count, "Должно быть 2 команды в списке");

            Assert.IsTrue(result.Any(c => c.CommandNames == "Cmd1"), "В списке должна быть команда Cmd1");
            Assert.IsTrue(result.Any(c => c.CommandNames == "Cmd2"), "В списке должна быть команда Cmd2");
        }

        [TestMethod]
        public async Task SearchCommands_State_Test()
        {
            // Arrange
            var service = this.CreateService();

            // Добавим тестовые команды с разными категориями (включая дубли)
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
                Category = "Category1", // Дубликат категории
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
            Assert.IsNotNull(result, "Результат не должен быть null");
            Assert.AreEqual(1, result.Count, "Должно быть ровно 1 уникальные категории");
            Assert.IsTrue(result[0].Category == "Category2");

            // Act
            var result2 = await service.SearchCommandsAsync("md2", false);

            // Assert
            Assert.IsNotNull(result2, "Результат не должен быть null");
            Assert.AreEqual(1, result2.Count, "Должно быть ровно 1 уникальные категории");
            Assert.IsTrue(result2[0].Category == "Category2");
        }

        [TestMethod]
        public async Task GetAllCommandCategories_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Добавим тестовые команды с разными категориями (включая дубли)
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
                Category = "Category1", // Дубликат категории
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
            Assert.IsNotNull(result, "Результат не должен быть null");
            Assert.AreEqual(2, result.Count, "Должно быть ровно 2 уникальные категории");
            Assert.AreEqual(result[0].Key, "Category1", "Должна содержаться категория Category1");
            Assert.AreEqual(result[1].Key, "Category2", "Должна содержаться категория Category2");
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
            Assert.IsNotNull(result, "Результат не должен быть null");
            Assert.AreEqual(2, result.Count, "Должны быть выбраны только команды указанной категории");

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

            // Добавим 12 постов с разным статусом и временем
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
            Assert.IsNotNull(result, "Результат не должен быть null");
            Assert.AreEqual(10, result.Count, "Должно вернуться 10 постов со статусом > 0");

            // Проверяем, что вернулись последние 10 (без постов со статусом 0)
            var expectedIds = new[] { 12, 11, 10, 9, 8, 7, 6, 5, 3, 2 };
            CollectionAssert.AreEqual(expectedIds, result.Select(p => p.Id).ToList(), "Посты должны быть отсортированы по дате убыванию и только со статусом > 0");
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
            Assert.AreEqual(10, result.Posts.Count, "Должно вернуться 10 последних постов");

            var expectedIds = posts
              .OrderBy(p => p.Id)              // сначала сортировка по Id (но она сразу же перебивается)
              .OrderBy(p => p.DateTimeUnix)    // сортировка по дате
              .Reverse()                       // переворот
              .Take(10)
              .Select(p => p.Id)
              .ToList();

            var actualIds = result.Posts.Select(p => p.Id).ToList();

            CollectionAssert.AreEqual(expectedIds, actualIds, "Посты должны быть отсортированы по убыванию даты");
            Assert.AreEqual(0, result.LastUserId, "LastUserId должен соответствовать переданному параметру");
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
            Assert.IsNotNull(result, "Результат не должен быть null");
            Assert.IsNotNull(result.Posts, "Список постов не должен быть null");
            Assert.AreEqual(lastUserId, result.LastUserId, "LastUserId должен совпадать с переданным значением");

            var expected = posts
                .Where(p => p.Id < lastUserId)
                .OrderBy(p => p.Id)
                .OrderBy(p => p.DateTimeUnix)
                .Reverse()
                .Take(30)
                .Select(p => p.Id)
                .ToList();

            var actual = result.Posts.Select(p => p.Id).ToList();

            CollectionAssert.AreEqual(expected, actual, "Список постов должен соответствовать ожидаемому порядку и фильтрации");
        }

        [TestMethod]
        public async Task UpdateStatusPost_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Создаем и добавляем тестовый пост с Id = 1 и статусом 0
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
            Assert.IsTrue(result, "UpdateStatusPost должен вернуть true при успешном обновлении");

            var updatedPost = await service.Posts.FindAsync((int)lastUserId);
            Assert.IsNotNull(updatedPost);
            Assert.AreEqual(newStatus, updatedPost.Status);

            // Проверка, что метод возвращает false, если пост не найден
            var resultNotFound = await service.UpdateStatusPostAsync(9999, newStatus);
            Assert.IsFalse(resultNotFound, "UpdateStatusPost должен вернуть false, если пост не найден");
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
            Assert.IsTrue(result, "Ожидалось, что API-ключ существует в базе");
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
            Assert.IsNotNull(postInDb, "Пост должен быть добавлен в базу");
            Assert.AreEqual(Post.Message, postInDb.Message, "Сообщение должно совпадать");
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
            // Проверяем, что Dispose не выбрасывает исключений
            service.Dispose();

            Assert.IsTrue(true);
        }
    }
}
