/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 13 августа 2026 07:11:18
 * Version: 1.0.144
 */

using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;

using LizeriumDatabase.Accessories.DataBaseAccessories;

using LizeriumLogging.Accessories.LoggingAccessories;

using LizeriumUtilities.FormatsData.AppUserData;
using LizeriumUtilities.FormatsData.DataBase.Requests;
using LizeriumUtilities.FormatsData.DataBase.Response;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace LizeriumDatabase.Services.AppDataBaseService.Implements;

/// <summary>
/// Контекст работы с БД
/// </summary>
public partial class DataBaseService : DbContext, IDataBaseService
{
    private readonly DbContextOptions _context;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="options"></param>
    public DataBaseService(DbContextOptions<DataBaseService> options) : base(options) { _context = options; }

    /// <summary>
    /// Инициализация таблицы постов
    /// </summary>
    public DbSet<PostDataResponse> Posts { get; set; }

    /// <summary>
    /// Инициализация таблицы пользователей
    /// </summary>
    public DbSet<UserApiKeyResponse> Users { get; set; }

    /// <summary>
    /// Инициализация таблицы команд
    /// </summary>
    public DbSet<CommandDataResponse> Commands { get; set; }

    /// <summary>
    /// Инициализация таблицы подробной информации о командах
    /// </summary>
    public DbSet<CommandCategoryInfoResponse> CommandCategories { get; set; }

    /// <summary>
    /// Инициализация таблицы подробной информации о переводах всех комманд
    /// </summary>
    public DbSet<CommandTranslation> CommandsTranslations { get; set; }

    /// <summary>
    /// Инициализация таблицы новостей Lizerium Steam
    /// </summary>
    public DbSet<LauncherNewsDataResponse> LauncherNews { get; set; }

    /// <summary>
    /// Инициализация таблицы категорий продуктов
    /// </summary>
    public DbSet<ProductCategoryDataResponse> ProductCategories { get; set; }

    /// <summary>
    /// Инициализация таблицы продуктов
    /// </summary>
    public DbSet<ProductDataResponse> Products { get; set; }

    /// <summary>
    /// Инициализация таблицы источников скачивания продуктов
    /// </summary>
    public DbSet<ProductDownloadLinkDataResponse> ProductDownloadLinks { get; set; }

    /// <summary>
    /// Событие создания модели
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PostDataResponse>().ToTable("posts"); // Устанавливаем имя таблицы posts
        modelBuilder.Entity<UserApiKeyResponse>().ToTable("users"); // Устанавливаем имя таблицы users
        modelBuilder.Entity<CommandDataResponse>().ToTable("commands"); // Устанавливаем имя таблицы commands
        modelBuilder.Entity<CommandCategoryInfoResponse>().ToTable("commandCategories"); // Устанавливаем имя таблицы commandCategories
        modelBuilder.Entity<CommandTranslation>().ToTable("command_translations"); // Устанавливаем имя таблицы command_translations
        modelBuilder.Entity<LauncherNewsDataResponse>().ToTable("launcher_news"); // Устанавливаем имя таблицы launcher_news
        modelBuilder.Entity<ProductCategoryDataResponse>().ToTable("product_categories");
        modelBuilder.Entity<ProductDataResponse>().ToTable("products");
        modelBuilder.Entity<ProductDownloadLinkDataResponse>().ToTable("product_download_links");

        modelBuilder.Entity<ProductDataResponse>()
            .HasOne(product => product.ProductCategory)
            .WithMany(category => category.Products)
            .HasForeignKey(product => product.ProductCategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProductDownloadLinkDataResponse>()
            .HasOne(link => link.Product)
            .WithMany(product => product.DownloadLinks)
            .HasForeignKey(link => link.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    /// <summary>
    /// Создать команду
    /// </summary>
    public async Task<bool> AddCommandAsync(CreateCommandViewRequest Command, bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateCommandsTable();
            var count = Commands.Count() + 1;
            await Commands.AddRangeAsync(new CommandDataResponse()
            {
                Id = count,
                Category = Command.Category,
                CommandNames = Command.CommandNames,
                ExampleInput = Command.ExampleInput,
                Status = Command.Status,
                CountLike = Command.CountLike,
                Description = Command.Description,
                UrlGif = Command.UrlGif
            });
            await SaveChangesAsync();
            Console.WriteLine("Add command database Complete!");
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            ex.LogException();
            return await Task.FromResult(false);
        }
    }

    /// <summary>
    /// Удалить команду и её переводы
    /// </summary>
    public async Task<bool> DeleteCommandAndTranslationsAsync(CreateCommandViewRequest Command, bool checkSecureOperate = true)
    {
        try
        {
            var commandId = Command.Id;



            // Начинаем транзакцию для безопасности
            await using var transaction = await Database.BeginTransactionAsync();

            var translation = await CommandsTranslations.FindAsync(commandId);
            var command = await Commands.FindAsync(commandId);
            if (command != null)
            {
                // Отвязываем от трекинга, если нужно
                Entry(command).State = EntityState.Detached;

                if (translation != null)
                    Entry(translation).State = EntityState.Detached;

                // Удаляем через SQL или напрямую
                await Database.ExecuteSqlInterpolatedAsync($@"
                    DELETE FROM command_translations WHERE CommandId = {commandId};
                    DELETE FROM commands WHERE Id = {commandId};
                ");
            }

            // 2️⃣ Удаляем команду
            await Database.ExecuteSqlInterpolatedAsync($@"
                    DELETE FROM commands
                    WHERE Id = {commandId};
                ");

            // Фиксируем изменения
            await transaction.CommitAsync();

            Console.WriteLine($"Command {commandId} and its translations deleted successfully!");
            return true;
        }
        catch (Exception ex)
        {
            ex.LogException();
            return false;
        }
    }

    /// <summary>
    /// Добавление категории команд (информации о ней)
    /// </summary>
    public async Task<bool> AddCategoryAsync(CategoriesCommands category, bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateCommandsTable();

            // Берём русский и английский заголовок
            string nameRu = category.Title.FirstOrDefault(t => !string.IsNullOrEmpty(t.Russian))?.Russian ?? category.Name;
            string nameEn = category.Title.FirstOrDefault(t => !string.IsNullOrEmpty(t.English))?.English ?? category.Name;

            await CommandCategories.AddAsync(new CommandCategoryInfoResponse
            {
                Key = category.Name,
                NameRu = nameRu,
                NameEn = nameEn,
                Version = category.Version,
                Repository = category.Repository
            });

            await SaveChangesAsync();

            Console.WriteLine($"Category {category.Name} added to database.");
            return true;
        }
        catch (Exception ex)
        {
            ex.LogException();
            return false;
        }
    }

    /// <summary>
    /// Изменить команду
    /// </summary>
    public async Task<bool> ChangeCommandAsync(CreateCommandViewRequest Command, bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateCommandsTable();

            var command = await Commands.FindAsync((int)Command.Id);
            if (command == null)
            {
                var msgError = "ERROR ChangeCommand";
                msgError.LogMessage();
                return false;
            }
            command.Status = Command.Status;
            command.Description = Command.Description;
            command.UrlGif = Command.UrlGif;
            command.ExampleInput = Command.ExampleInput;
            command.CommandNames = Command.CommandNames;
            command.CountLike = Command.CountLike;
            await SaveChangesAsync();
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            ex.LogException();
            return await Task.FromResult(false);
        }
    }

    /// <summary>
    /// Сохранить команду с GIF ссылкой на файл
    /// </summary>
    /// <param name="fileName">Имя GIF файла</param>
    /// <param name="id">Идентификатор команды</param>
    /// <param name="checkSecureOperate">Проверять ли существование таблицы</param>
    /// <returns>bool</returns>
    public async Task<bool> SaveGifCommandAsync(string fileName, string id, bool checkSecureOperate = true)
    {
        if (checkSecureOperate)
            await ExistAndCreateCommandsTable();

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(fileName)) return false;

        var command = await Commands.FindAsync(int.Parse(id));
        if (command == null) return false;
        command.UrlGif = fileName;
        await SaveChangesAsync();
        return await Task.FromResult(true);
    }

    /// <summary>
    /// Сохраняет сразу пачку команд в базе данных
    /// </summary>
    /// <param name="jsonData">Считанный набор команд отсортированный по категориям</param>
    public async Task SaveCommandsFromJsonAsync(CommandsFileRequest jsonData, bool checkSecureOperate = true)
    {
        foreach (var category in jsonData.Data)
        {
            foreach (var command in category.Commands)
            {
                await AddCommandAsync(new CreateCommandViewRequest()
                {
                    Category = category.Name,
                    CommandNames = command.Commands,
                    CountLike = 0,
                    Status = command.Status,
                    Description = command.Description,
                    ExampleInput = command.Example,
                    UrlGif = string.Empty
                }, checkSecureOperate);
            }
        }
    }

    /// <summary>
    /// Сохраняет сразу пачку категорий команд в базе данных
    /// </summary>
    /// <param name="jsonData">Считанный набор категорий команд отсортированный по категориям</param>
    public async Task SaveCategoriesCommandsFromJsonAsync(CommandsFileRequest jsonData, bool checkSecureOperate = true)
    {
        foreach (var category in jsonData.Categories)
        {
            await AddCategoryAsync(category, checkSecureOperate);
        }
    }

    /// <summary>
    /// Получает список постов пользователей
    /// </summary>
    public async Task<List<PostDataResponse>> GetAllAdminPostsAsync()
    {
        try
        {
            return await Posts.OrderBy(post => post.DateTimeUnix)
                        .Reverse().Take(10).ToListAsync();
        }
        catch (Exception ex)
        {
            ex.LogException();
            return null;
        }
    }

    /// <summary>
    /// Получает список команд
    /// </summary>
    public async Task<List<CommandDataResponse>> GetAllAdminCommandsAsync(bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateCommandsTable();

            return await Commands.ToListAsync();
        }
        catch (Exception ex)
        {
            ex.LogException();
            return null;
        }
    }

    /// <summary>
    /// Получает список переводов команд
    /// </summary>
    public async Task<List<AdminCommandWithTranslations>> GetAllAdminCommandTranslatesAsync(bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateCommandsTable();

            var commands = await Commands.AsNoTracking().ToListAsync();
            var translations = await CommandsTranslations.AsNoTracking().ToListAsync();

            var result = commands.Select(c => new AdminCommandWithTranslations
            {
                CommandId = c.Id,
                BaseDescription = c.Description,
                Translations = translations.Where(t => t.CommandId == c.Id).ToList()
            }).ToList();

            return result;
        }
        catch (Exception ex)
        {
            ex.LogException();
            return new List<AdminCommandWithTranslations>();
        }
    }

    /// <summary>
    /// Поиск по командам
    /// </summary>
    /// <param name="query">Запрос</param>
    /// <returns>Список команд</returns>
    public async Task<List<CommandDataResponse>> SearchCommandsAsync(string query, bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateCommandsTable();
            query = query.ToLower();

            var commands = await (
                from c in Commands
                join t in CommandsTranslations on c.Id equals t.CommandId into translations
                from t in translations.DefaultIfEmpty()
                where c.CommandNames.ToLower().Contains(query)
                      || c.Description.ToLower().Contains(query)
                      || c.Category.ToLower().Contains(query)
                      || t.Description.ToLower().Contains(query)
                select c
            ).Distinct().ToListAsync();

            // получаем все переводы для команд
            foreach (var command in commands)
            {
                var translations = await GetCommandTranslationsAsync(command.Id);
                command.Translations = translations
                    .GroupBy(t => t.LanguageCode)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(t => t.Description).ToList()
                    );
            }

            // получаем все переводы для категорий
            var categoriesInDb = await GetAllCommandCategoriesAsync();
            foreach (var command in commands)
            {
                command.TitlesCategory = new Language();
                var categoryInDb = categoriesInDb.FirstOrDefault(c => c.Key == command.Category);
                if (categoryInDb != null)
                {
                    command.TitlesCategory.Russian = categoryInDb.NameRu;
                    command.TitlesCategory.English = categoryInDb.NameEn;
                }
            }

            return commands;
        }
        catch (Exception ex)
        {
            ex.LogException();
            return null;
        }
    }

    /// <summary>
    /// Получение списка команд по категории
    /// </summary>
    /// <param name="checkSecureOperate">Проверять ли существование таблицы</param>
    /// <returns>List<string></returns>
    public async Task<List<CommandCategoryInfoResponse>> GetAllCommandCategoriesAsync(bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateCommandsTable();

            // Берём все категории напрямую из таблицы
            var categories = await CommandCategories
                .ToListAsync();

            return categories;
        }
        catch (Exception ex)
        {
            ex.LogException();
            return null;
        }
    }

    /// <summary>
    /// Получает все команды по категории
    /// </summary>
    /// <param name="Category">Категория имя</param>
    /// <param name="Page">Страница</param>
    /// <param name="Size">Количество</param>
    /// <param name="checkSecureOperate">Проверять ли существование таблицы</param>
    /// <param name="shortSize">Ограничения включены или выключены</param>
    /// <returns></returns>
    public async Task<List<CommandDataResponse>> GetCommandsAsync(string Category, int Page = 1, int Size = 10, bool checkSecureOperate = true,
        bool shortSize = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateCommandsTable();

            IQueryable<CommandDataResponse> query = Commands
                .Where(c => c.Category == Category)
                .OrderBy(c => c.Id); // возвращает IOrderedQueryable, но это ок для IQueryable

            if (shortSize)
            {
                if (Page < 1) Page = 1;
                if (Size < 1) Size = 10;
                query = query.Skip((Page - 1) * Size).Take(Size); // тип IQueryable<CommandDataResponse>
            }

            var commands = await query.ToListAsync();

            // маппим на CommandDataResponse
            var result = commands.Select(c => new CommandDataResponse
            {
                Id = c.Id,
                Category = c.Category,
                CommandNames = c.CommandNames,
                ExampleInput = c.ExampleInput,
                Description = c.Description,
                UrlGif = c.UrlGif,
                CountLike = c.CountLike,
                Status = c.Status
            }).ToList();

            foreach (var command in commands)
            {
                var translations = await GetCommandTranslationsAsync(command.Id);
                command.Translations = translations
                    .GroupBy(t => t.LanguageCode)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(t => t.Description).ToList()
                    );
            }

            return commands;
        }
        catch (Exception ex)
        {
            ex.LogException();
            return null;
        }
    }

    public async Task<List<CommandTranslationResponse>> GetCommandTranslationsAsync(int commandId)
    {
        return await CommandsTranslations
            .Where(t => t.CommandId == commandId)
            .Select(t => new CommandTranslationResponse
            {
                CommandId = t.CommandId,
                LanguageCode = t.LanguageCode,
                Description = t.Description
            })
            .ToListAsync();
    }

    /// <summary>
    /// Получает количество команд в категории
    /// </summary>
    /// <param name="Category">Категория имя</param>
    /// <param name="checkSecureOperate">Проверять ли существование таблицы</param>
    /// <returns>int</returns>
    public async Task<int> GetCommandsCountAsync(string Category, bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateCommandsTable();

            return await Commands.CountAsync(command => command.Category == Category);
        }
        catch (Exception ex)
        {
            ex.LogException();
            return 0;
        }
    }

    /// <summary>
    /// Получить команды без перевода.
    /// </summary>
    /// <param name="toLang">en</param>
    /// <returns>Список команд</returns>
    public async Task<List<CommandTranslationResponse>> GetCommandsMissingTranslationAsync(string toLang)
    {
        var missing = await Commands
            .Where(c => !CommandsTranslations.Any(t => t.CommandId == c.Id && t.LanguageCode == toLang))
            .Select(c => new CommandTranslationResponse
            {
                CommandId = c.Id,
                Description = c.Description,
                LanguageCode = toLang
            })
            .AsNoTracking()
            .ToListAsync();

        return missing;
    }

    /// <summary>
    /// Сохранение/обновление команды в БД (SQLite версия)
    /// </summary>
    public async Task SaveCommandTranslationsAsync(AdminCommandWithTranslations command)
    {
        try
        {
            foreach (var translation in command.Translations)
            {
                // SQLite поддерживает "INSERT OR REPLACE" для обновления или вставки
                await Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO command_translations (CommandId, LanguageCode, Description)
                VALUES ({translation.CommandId}, {translation.LanguageCode}, {translation.Description.Trim()})
                ON CONFLICT(CommandId, LanguageCode) 
                DO UPDATE SET Description = excluded.Description;
            ");
            }

            await SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                entry.Reload();
            }
            await SaveChangesAsync();
        }
        catch (Exception ex)
        {
            ex.LogException();
        }
    }


    /// <summary>
    /// Сохранение всех переводов в БД
    /// </summary>
    /// <param name="commandTranslations">Список DTO переводов</param>
    /// <returns>Статус операции</returns>
    public async Task<bool> SaveAllCommandsTranslationsAsync(List<CommandTranslationResponse> commandTranslations)
    {
        foreach (var dto in commandTranslations)
        {
            // Проверяем, есть ли уже перевод для этой команды и языка
            var existing = await CommandsTranslations
                .FirstOrDefaultAsync(t => t.CommandId == dto.CommandId && t.LanguageCode == dto.LanguageCode);

            if (existing != null)
            {
                // Обновляем описание
                existing.Description = dto.Description;
            }
            else
            {
                // Создаём новую запись
                CommandsTranslations.Add(new CommandTranslation()
                {
                    CommandId = dto.CommandId,
                    LanguageCode = dto.LanguageCode,
                    Description = dto.Description
                });
            }
        }

        await SaveChangesAsync();
        Console.WriteLine($"Commands Translations saved/updated to database.");
        return true;
    }

    /// <summary>
    /// Проверяет существование таблицы новостей и создает ее при необходимости.
    /// </summary>
    public async Task ExistAndCreateLauncherNewsTable()
    {
        try
        {
            await Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS launcher_news (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    TitleRu TEXT,
                    TitleEn TEXT,
                    MarkdownRu TEXT,
                    MarkdownEn TEXT,
                    YoutubeUrl TEXT,
                    RutubeUrl TEXT,
                    VkVideoUrl TEXT,
                    ImageUrl TEXT,
                    ImageGalleryJson TEXT,
                    NewsType TEXT,
                    NewsTypeRu TEXT,
                    NewsTypeEn TEXT,
                    IconUrl TEXT,
                    LikeCount INTEGER NOT NULL DEFAULT 0,
                    GithubUrl TEXT,
                    GithubProjectName TEXT,
                    IsPublished INTEGER NOT NULL DEFAULT 1,
                    SortOrder INTEGER NOT NULL DEFAULT 0,
                    PublishedAtUnix INTEGER NOT NULL
                );"
            );

            await EnsureLauncherNewsColumnAsync("TitleRu", "TEXT");
            await EnsureLauncherNewsColumnAsync("TitleEn", "TEXT");
            await EnsureLauncherNewsColumnAsync("MarkdownRu", "TEXT");
            await EnsureLauncherNewsColumnAsync("MarkdownEn", "TEXT");
            await EnsureLauncherNewsColumnAsync("YoutubeUrl", "TEXT");
            await EnsureLauncherNewsColumnAsync("RutubeUrl", "TEXT");
            await EnsureLauncherNewsColumnAsync("VkVideoUrl", "TEXT");
            await EnsureLauncherNewsColumnAsync("ImageUrl", "TEXT");
            await EnsureLauncherNewsColumnAsync("ImageGalleryJson", "TEXT");
            await EnsureLauncherNewsColumnAsync("NewsType", "TEXT");
            await EnsureLauncherNewsColumnAsync("NewsTypeRu", "TEXT");
            await EnsureLauncherNewsColumnAsync("NewsTypeEn", "TEXT");
            await EnsureLauncherNewsColumnAsync("IconUrl", "TEXT");
            await EnsureLauncherNewsColumnAsync("LikeCount", "INTEGER NOT NULL DEFAULT 0");
            await EnsureLauncherNewsColumnAsync("GithubUrl", "TEXT");
            await EnsureLauncherNewsColumnAsync("GithubProjectName", "TEXT");
            await EnsureLauncherNewsColumnAsync("IsPublished", "INTEGER NOT NULL DEFAULT 1");
            await EnsureLauncherNewsColumnAsync("SortOrder", "INTEGER NOT NULL DEFAULT 0");
            await EnsureLauncherNewsColumnAsync("PublishedAtUnix", "INTEGER NOT NULL DEFAULT 0");

            if (await LauncherNews.AnyAsync())
                return;

            var publishedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            await LauncherNews.AddRangeAsync(new List<LauncherNewsDataResponse>
            {
                new()
                {
                    TitleRu = "Предстоящие обновления в 99.5.1 Freelancer Lizerium",
                    TitleEn = "Upcoming updates in 99.5.1 Freelancer Lizerium",
                    RutubeUrl = "https://rutube.ru/video/166b1de79791472c13f79c24838847c3/",
                    IsPublished = true,
                    SortOrder = 10,
                    PublishedAtUnix = publishedAtUnix
                },
                new()
                {
                    TitleRu = "Официальный русскоязычный трейлер игры Freelancer Lizerium",
                    TitleEn = "Official Russian trailer of Freelancer Lizerium",
                    RutubeUrl = "https://rutube.ru/video/f7359c52b38dbfd9eab1426349de6571/",
                    IsPublished = true,
                    SortOrder = 20,
                    PublishedAtUnix = publishedAtUnix
                },
                new()
                {
                    TitleRu = "Демонстрация второй версии полета, эффектов, звуков Freelancer Lizerium (Unity ver.)",
                    TitleEn = "Second flight, effects and sound demo for Freelancer Lizerium (Unity ver.)",
                    RutubeUrl = "https://rutube.ru/video/da9bd6b780314bb96ca23b10110dcfd9/",
                    IsPublished = true,
                    SortOrder = 30,
                    PublishedAtUnix = publishedAtUnix
                },
                new()
                {
                    TitleRu = "Первое испытание трейлов во Freelancer Lizerium (Unity ver.)",
                    TitleEn = "First trail test in Freelancer Lizerium (Unity ver.)",
                    RutubeUrl = "https://rutube.ru/video/0f20131048cc69a38337431fafdc4597/",
                    IsPublished = true,
                    SortOrder = 40,
                    PublishedAtUnix = publishedAtUnix
                }
            });
            await SaveChangesAsync();
        }
        catch (Exception ex)
        {
            ex.LogException();
        }
    }

    private async Task EnsureLauncherNewsColumnAsync(string columnName, string columnDefinition)
    {
        if (await ColumnExistsAsync("launcher_news", columnName))
            return;

        await Database.ExecuteSqlRawAsync($"ALTER TABLE launcher_news ADD COLUMN {columnName} {columnDefinition};");
    }

    private async Task<bool> ColumnExistsAsync(string tableName, string columnName)
    {
        try
        {
            if (!await Database.CanConnectAsync())
                return false;

            var connection = Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;

            if (shouldClose)
                await connection.OpenAsync();

            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText = $"PRAGMA table_info({tableName});";

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    if (string.Equals(reader["name"]?.ToString(), columnName, StringComparison.OrdinalIgnoreCase))
                        return true;
                }

                return false;
            }
            finally
            {
                if (shouldClose)
                    await connection.CloseAsync();
            }
        }
        catch (DbException ex)
        {
            ex.LogException();
            return false;
        }
    }

    /// <summary>
    /// Получает опубликованные новости Lizerium Steam.
    /// </summary>
    public async Task<List<LauncherNewsDataResponse>> GetPublishedLauncherNewsAsync(bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateLauncherNewsTable();

            return await LauncherNews
                .AsNoTracking()
                .Where(news => news.IsPublished)
                .OrderByDescending(news => news.PublishedAtUnix)
                .ThenBy(news => news.SortOrder)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            ex.LogException();
            return new List<LauncherNewsDataResponse>();
        }
    }

    /// <summary>
    /// Получает все новости для админки.
    /// </summary>
    public async Task<List<LauncherNewsDataResponse>> GetAllAdminLauncherNewsAsync(bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateLauncherNewsTable();

            return await LauncherNews
                .AsNoTracking()
                .OrderByDescending(news => news.PublishedAtUnix)
                .ThenBy(news => news.SortOrder)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            ex.LogException();
            return new List<LauncherNewsDataResponse>();
        }
    }

    /// <summary>
    /// Получает одну новость для закрытого админского предпросмотра, включая скрытые черновики.
    /// </summary>
    public async Task<LauncherNewsDataResponse> GetAdminLauncherNewsByIdAsync(int id, bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateLauncherNewsTable();

            return await LauncherNews
                .AsNoTracking()
                .FirstOrDefaultAsync(news => news.Id == id);
        }
        catch (Exception ex)
        {
            ex.LogException();
            return null;
        }
    }

    /// <summary>
    /// Добавляет или обновляет новость.
    /// </summary>
    public async Task<bool> SaveLauncherNewsAsync(LauncherNewsDataResponse news, bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateLauncherNewsTable();

            if (news.Id > 0)
            {
                var existingNews = await LauncherNews.FindAsync(news.Id);
                if (existingNews == null)
                    return false;

                existingNews.TitleRu = news.TitleRu;
                existingNews.TitleEn = news.TitleEn;
                existingNews.MarkdownRu = news.MarkdownRu;
                existingNews.MarkdownEn = news.MarkdownEn;
                existingNews.YoutubeUrl = news.YoutubeUrl;
                existingNews.RutubeUrl = news.RutubeUrl;
                existingNews.VkVideoUrl = news.VkVideoUrl;
                existingNews.ImageUrl = news.ImageUrl;
                existingNews.ImageGalleryJson = news.ImageGalleryJson;
                existingNews.NewsType = news.NewsType;
                existingNews.NewsTypeRu = news.NewsTypeRu;
                existingNews.NewsTypeEn = news.NewsTypeEn;
                existingNews.IconUrl = news.IconUrl;
                existingNews.LikeCount = news.LikeCount;
                existingNews.GithubUrl = news.GithubUrl;
                existingNews.GithubProjectName = news.GithubProjectName;
                existingNews.IsPublished = news.IsPublished;
                existingNews.SortOrder = news.SortOrder;
                existingNews.PublishedAtUnix = news.PublishedAtUnix > 0
                    ? news.PublishedAtUnix
                    : DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
            else
            {
                news.PublishedAtUnix = news.PublishedAtUnix > 0
                    ? news.PublishedAtUnix
                    : DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                await LauncherNews.AddAsync(news);
            }

            await SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            ex.LogException();
            return false;
        }
    }

    /// <summary>
    /// Удаляет новость.
    /// </summary>
    public async Task<bool> DeleteLauncherNewsAsync(int id, bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateLauncherNewsTable();

            var news = await LauncherNews.FindAsync(id);
            if (news == null)
                return false;

            LauncherNews.Remove(news);
            await SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            ex.LogException();
            return false;
        }
    }

    /// <summary>
    /// Increments public like counter for a launcher news item.
    /// </summary>
    public async Task<int?> IncrementLauncherNewsLikeAsync(int id, bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateLauncherNewsTable();

            var news = await LauncherNews.FindAsync(id);
            if (news == null || !news.IsPublished)
                return null;

            news.LikeCount = Math.Max(0, news.LikeCount) + 1;
            await SaveChangesAsync();
            return news.LikeCount;
        }
        catch (Exception ex)
        {
            ex.LogException();
            return null;
        }
    }

    /// <summary>
    /// Получает опубликованный каталог продуктов для публичной витрины.
    /// </summary>
    public async Task<List<ProductCategoryDataResponse>> GetPublishedProductCatalogAsync(bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateProductsTables();

            // Public catalog hides inactive branches and keeps ordering stable for the portal UI.
            var categories = await ProductCategories
                .AsNoTracking()
                .Include(category => category.Products)
                    .ThenInclude(product => product.DownloadLinks)
                .Where(category => category.IsActive)
                .OrderBy(category => category.SortOrder)
                .ThenBy(category => category.Id)
                .ToListAsync();

            foreach (var category in categories)
            {
                category.Products = category.Products
                    .Where(product => product.IsActive)
                    .OrderBy(product => product.SortOrder)
                    .ThenBy(product => product.Id)
                    .ToList();

                foreach (var product in category.Products)
                {
                    product.DownloadLinks = product.DownloadLinks
                        .Where(link => link.IsActive)
                        .OrderBy(link => link.SortOrder)
                        .ThenBy(link => link.Id)
                        .ToList();
                }
            }

            return categories
                .Where(category => category.Products.Count > 0)
                .ToList();
        }
        catch (Exception ex)
        {
            ex.LogException();
            return new List<ProductCategoryDataResponse>();
        }
    }

    /// <summary>
    /// Получает полный каталог продуктов для админки.
    /// </summary>
    public async Task<List<ProductCategoryDataResponse>> GetAllAdminProductCatalogAsync(bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateProductsTables();

            // Admin catalog returns inactive items too, but still normalizes child ordering for editing.
            var categories = await ProductCategories
                .AsNoTracking()
                .Include(category => category.Products)
                    .ThenInclude(product => product.DownloadLinks)
                .OrderBy(category => category.SortOrder)
                .ThenBy(category => category.Id)
                .ToListAsync();

            foreach (var category in categories)
            {
                category.Products = category.Products
                    .OrderBy(product => product.SortOrder)
                    .ThenBy(product => product.Id)
                    .ToList();

                foreach (var product in category.Products)
                {
                    product.DownloadLinks = product.DownloadLinks
                        .OrderBy(link => link.SortOrder)
                        .ThenBy(link => link.Id)
                        .ToList();
                }
            }

            return categories;
        }
        catch (Exception ex)
        {
            ex.LogException();
            return new List<ProductCategoryDataResponse>();
        }
    }

    /// <summary>
    /// Добавляет или обновляет категорию продуктов.
    /// </summary>
    public async Task<bool> SaveProductCategoryAsync(ProductCategoryDataResponse category, bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateProductsTables();

            if (category.Id > 0)
            {
                var existingCategory = await ProductCategories.FindAsync(category.Id);
                if (existingCategory == null)
                    return false;

                existingCategory.Key = category.Key;
                existingCategory.NameRu = category.NameRu;
                existingCategory.NameEn = category.NameEn;
                existingCategory.DescriptionRu = category.DescriptionRu;
                existingCategory.DescriptionEn = category.DescriptionEn;
                existingCategory.IconUrl = category.IconUrl;
                existingCategory.BackgroundUrl = category.BackgroundUrl;
                existingCategory.SortOrder = category.SortOrder;
                existingCategory.IsActive = category.IsActive;
            }
            else
            {
                await ProductCategories.AddAsync(category);
            }

            await SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            ex.LogException();
            return false;
        }
    }

    /// <summary>
    /// Удаляет категорию продуктов.
    /// </summary>
    public async Task<bool> DeleteProductCategoryAsync(int id, bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateProductsTables();

            var category = await ProductCategories
                .Include(item => item.Products)
                    .ThenInclude(product => product.DownloadLinks)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (category == null)
                return false;

            ProductCategories.Remove(category);
            await SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            ex.LogException();
            return false;
        }
    }

    /// <summary>
    /// Добавляет или обновляет продукт.
    /// </summary>
    public async Task<bool> SaveProductAsync(ProductDataResponse product, bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateProductsTables();

            if (product.Id > 0)
            {
                var existingProduct = await Products.FindAsync(product.Id);
                if (existingProduct == null)
                    return false;

                existingProduct.ProductCategoryId = product.ProductCategoryId;
                existingProduct.TitleRu = product.TitleRu;
                existingProduct.TitleEn = product.TitleEn;
                existingProduct.DescriptionRu = product.DescriptionRu;
                existingProduct.DescriptionEn = product.DescriptionEn;
                existingProduct.IconUrl = product.IconUrl;
                existingProduct.SortOrder = product.SortOrder;
                existingProduct.IsActive = product.IsActive;
            }
            else
            {
                await Products.AddAsync(product);
            }

            await SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            ex.LogException();
            return false;
        }
    }

    /// <summary>
    /// Удаляет продукт.
    /// </summary>
    public async Task<bool> DeleteProductAsync(int id, bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateProductsTables();

            var product = await Products
                .Include(item => item.DownloadLinks)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (product == null)
                return false;

            Products.Remove(product);
            await SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            ex.LogException();
            return false;
        }
    }

    /// <summary>
    /// Добавляет или обновляет источник скачивания продукта.
    /// </summary>
    public async Task<bool> SaveProductDownloadLinkAsync(ProductDownloadLinkDataResponse link, bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateProductsTables();

            if (link.Id > 0)
            {
                var existingLink = await ProductDownloadLinks.FindAsync(link.Id);
                if (existingLink == null)
                    return false;

                existingLink.ProductId = link.ProductId;
                existingLink.NameRu = link.NameRu;
                existingLink.NameEn = link.NameEn;
                existingLink.Url = link.Url;
                existingLink.IconUrl = link.IconUrl;
                existingLink.SortOrder = link.SortOrder;
                existingLink.IsActive = link.IsActive;
            }
            else
            {
                await ProductDownloadLinks.AddAsync(link);
            }

            await SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            ex.LogException();
            return false;
        }
    }

    /// <summary>
    /// Удаляет источник скачивания продукта.
    /// </summary>
    public async Task<bool> DeleteProductDownloadLinkAsync(int id, bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateProductsTables();

            var link = await ProductDownloadLinks.FindAsync(id);
            if (link == null)
                return false;

            ProductDownloadLinks.Remove(link);
            await SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            ex.LogException();
            return false;
        }
    }


    public async Task ExistAndCreateProductsTables()
    {
        try
        {
            // Self-provisioning keeps old deployments alive when EF migrations are not applied manually.
            await Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS product_categories (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Key TEXT NOT NULL UNIQUE,
                    NameRu TEXT,
                    NameEn TEXT,
                    DescriptionRu TEXT,
                    DescriptionEn TEXT,
                    IconUrl TEXT,
                    BackgroundUrl TEXT,
                    SortOrder INTEGER NOT NULL DEFAULT 0,
                    IsActive INTEGER NOT NULL DEFAULT 1
                );");

            await Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS products (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProductCategoryId INTEGER NOT NULL,
                    TitleRu TEXT,
                    TitleEn TEXT,
                    DescriptionRu TEXT,
                    DescriptionEn TEXT,
                    IconUrl TEXT,
                    SortOrder INTEGER NOT NULL DEFAULT 0,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    FOREIGN KEY (ProductCategoryId) REFERENCES product_categories(Id) ON DELETE CASCADE
                );");

            await Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS product_download_links (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProductId INTEGER NOT NULL,
                    NameRu TEXT,
                    NameEn TEXT,
                    Url TEXT,
                    IconUrl TEXT,
                    SortOrder INTEGER NOT NULL DEFAULT 0,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    FOREIGN KEY (ProductId) REFERENCES products(Id) ON DELETE CASCADE
                );");

            // Seed only a completely empty catalog; existing production content must not be overwritten.
            if (!await ProductCategories.AnyAsync())
                await SeedDefaultProductsCatalogAsync();
        }
        catch (Exception ex)
        {
            ex.LogException();
        }
    }

    public async Task ExistAndCreateCommandsTable()
    {
        bool exist = await TableExistsAsync("commands");

        if (!exist)
        {
            try
            {
                await Database.ExecuteSqlRawAsync(@"
                    CREATE TABLE IF NOT EXISTS commands (
                        Id INTEGER PRIMARY KEY,
                        Category VARCHAR(255),
                        Description TEXT,                        
                        CommandNames TEXT,
                        ExampleInput TEXT,
                        UrlGif TEXT,
                        CountLike INT,
                        Status INT
                    );"
                );
                await ExistAndCreateCommandCategoriesTable();
            }
            catch (Exception ex)
            {
                exist = true;
            }
        }
        else await ExistAndCreateCommandTranslationsTable();
    }

    public async Task ExistAndCreateCommandTranslationsTable()
    {
        bool exist = await TableExistsAsync("command_translations");

        if (!exist)
        {
            await Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS command_translations (
                    CommandId INTEGER NOT NULL,
                    LanguageCode VARCHAR(5) NOT NULL,
                    Description TEXT,
                    PRIMARY KEY (CommandId, LanguageCode),
                    FOREIGN KEY (CommandId) REFERENCES commands(Id)
                );
                ");

            // Миграция текущих данных в таблицу переводов (русский язык)
            await Database.ExecuteSqlRawAsync(@"
                INSERT INTO command_translations (CommandId, LanguageCode, Description)
                SELECT Id, 'ru', Description
                FROM commands
                WHERE Description IS NOT NULL;
            ");

            Console.WriteLine("Command translations table created and existing descriptions migrated.");
        }
    }

    private async Task ExistAndCreateCommandCategoriesTable()
    {
        bool exist = await TableExistsAsync("CommandCategories");

        if (!exist)
        {
            try
            {
                await Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS CommandCategories (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Key TEXT NOT NULL UNIQUE,
                    NameEn TEXT NOT NULL,
                    NameRu TEXT NOT NULL,
                    Version TEXT,
                    Repository TEXT
                );"
                );
            }
            catch (Exception ex)
            {
                ex.LogException();
                exist = true;
            }
        }
    }

    /// <summary>
    /// Получает список постов пользователей
    /// </summary>
    public async Task<List<PostDataResponse>> GetAllPostsAsync()
    {
        try
        {
            return await Posts.Where(post => post.Status > 0).OrderBy(post => post.DateTimeUnix)
                        .Reverse().Take(10).ToListAsync();
        }
        catch (Exception ex)
        {
            ex.LogException();
            return null;
        }
    }

    /// <summary>
    /// Получает список постов пользователей
    /// </summary>
    /// <param name="id">Крайний пост</param>
    /// <param name="status">статус</param>
    /// <param name="scroll">скролингом ли загрузка или фильтры</param>
    public async Task<DataPosts> GetAllPostsAsync(int id, int status, bool scroll = false)
    {
        try
        {
            if (!scroll && id == 0 && status == 0)
            {
                var postsNullIndex = await Posts.OrderBy(post => post.Id) // Сортируем по возрастанию Id
                                              .OrderBy(post => post.DateTimeUnix)
                                              .Reverse() // Переворачиваем порядок
                                              .Take(10) // Ограничиваем выборку 10 постами
                                              .ToListAsync();

                return new DataPosts()
                {
                    Posts = postsNullIndex,
                    LastUserId = id
                };
            }
            else if (!scroll && id == 0 && status > 0)
            {
                var postsNullIndex = await Posts.Where(post => post.Status == status)
                              .OrderBy(post => post.DateTimeUnix)
                              .Reverse() // Переворачиваем порядок
                              .Take(10) // Ограничиваем выборку 10 постами
                              .ToListAsync();
                return new DataPosts()
                {
                    Posts = postsNullIndex,
                    LastUserId = id
                };
            }

            if (status == 0)
            {
                var postsNull = await Posts.Where(post => post.Id < id) // Фильтруем посты, id которых больше, чем lastUserId
                                                .OrderBy(post => post.Id) // Сортируем по возрастанию Id
                                                .OrderBy(post => post.DateTimeUnix)
                                                .Reverse() // Переворачиваем порядок
                                                .Take(10) // Ограничиваем выборку 10 постами
                                                .ToListAsync();

                return new DataPosts()
                {
                    Posts = postsNull,
                    LastUserId = id
                };
            }

            // Фильтруем посты, id которых больше, чем lastUserId
            var posts = await Posts.Where(post => post.Id < id && post.Status == status)
            .OrderBy(post => post.Id) // Сортируем по возрастанию Id
            .OrderBy(post => post.DateTimeUnix)
            .Reverse() // Переворачиваем порядок
            .Take(10) // Ограничиваем выборку 10 постами
            .ToListAsync();

            return new DataPosts()
            {
                Posts = posts,
                LastUserId = id
            };
        }
        catch (Exception ex)
        {
            ex.LogException();
            return null;
        }
    }

    /// <summary>
    /// Получает список постов пользователей
    /// </summary>
    /// <param name="lastUserId">Идентификатор крайнего полученного пользователя</param>
    public async Task<DataPosts> GetAllPostsAsync(long lastUserId)
    {
        try
        {
            var posts = await Posts.Where(post => post.Id < lastUserId) // Фильтруем посты, id которых больше, чем lastUserId
                        .OrderBy(post => post.Id) // Сортируем по возрастанию Id
                        .OrderBy(post => post.DateTimeUnix)
                        .Reverse() // Переворачиваем порядок
                        .Take(30) // Ограничиваем выборку 30 постами
                        .ToListAsync();

            return new DataPosts()
            {
                Posts = posts,
                LastUserId = lastUserId
            };
        }
        catch (Exception ex)
        {
            ex.LogException();
            return null;
        }
    }

    /// <summary>
    /// Обновляет статус заявки пользователя
    /// </summary>
    /// <param name="lastUserId">Идентификатор поста пользователя</param>
    /// <param name="status">Статус обработки</param>
    /// <returns></returns>
    public async Task<bool> UpdateStatusPostAsync(long lastUserId, int status)
    {
        try
        {
            var post = await Posts.FindAsync((int)lastUserId);
            if (post == null) return false;
            post.Status = status;
            Entry(post).State = EntityState.Modified;
            SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            ex.LogException();
            return false;
        }
    }

    /// <summary>
    /// Проверяет существование ключа пользователя
    /// </summary>
    /// <param name="Data">Информация о пользователе</param>
    public async Task<bool> IsValidUserApiKeyAsync(UserApiKeyData Data)
    {
        try
        {
            var users = Users.AnyAsync(user => user.ApiKey == Data.ApiKey);
            return await users;
        }
        catch (Exception ex)
        {
            ex.LogException();
            return false;
        }
    }

    /// <summary>
    /// Проверка существования таблицы по имени
    /// </summary>
    /// <param name="tableName">Имя таблицы</param>
    /// <returns>bool</returns>
    public async Task<bool> TableExistsAsync(string tableName)
    {
        try
        {
            if (!await Database.CanConnectAsync())
                return false;

            var connection = Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;

            if (shouldClose)
                await connection.OpenAsync();

            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName;";
                var param = command.CreateParameter();
                param.ParameterName = "@tableName";
                param.Value = tableName;
                command.Parameters.Add(param);

                var result = await command.ExecuteScalarAsync();
                return result != null;
            }
            finally
            {
                if (shouldClose)
                    await connection.CloseAsync();
            }
        }
        catch (DbException ex)
        {
            ex.LogException();
            return false;
        }
    }

    /// <summary>
    /// Генерировать базовую таблицу
    /// </summary>
    public async Task<bool> AddPostAsync(CreatePostViewRequest Post)
    {
        try
        {
            await Posts.AddRangeAsync(new PostDataResponse()
            {
                Autor = Post.Autor,
                Status = Post.Status,
                DateTimeUnix = Post.DateTimeUnix,
                Message = Post.Message
            });
            await SaveChangesAsync();
            Console.WriteLine("Add post database Complete!");
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            ex.LogException();
            return await Task.FromResult(false);
        }
    }

    /// <summary>
    /// Создание файла БД
    /// </summary>
    /// <param name="optionsBuilder">Опции</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return; // Уже настроено, не трогаем

        try
        {
            var dataSecretRecords = DatabaseExtensions.Configuration.GetValue<string>("path");
            var dir = Path.GetDirectoryName(Environment.ProcessPath);
            var path = Path.Combine(dir, dataSecretRecords);
            //логируем исключение
            ("DatabasePath: " + path).LogMessage();
            optionsBuilder.UseSqlite("Data Source=" + path);
        }
        catch (Exception ex)
        {
            ex.LogException();
        }
    }

    /// <summary>
    /// Создание БД
    /// </summary>
    public async Task RebuildAsync()
    {
        try
        {
            Console.WriteLine($"Create database");
            await Database.EnsureDeletedAsync();
            await Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            ex.LogException();
        }
    }

    /// <summary>
    /// Разрушитель соединения Postgresql
    /// </summary>
    public void Dispose()
    {
        try
        {
            //разрушаем соединение Postgresql
            Database.CloseConnection();
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();
        }
        finally
        {
            GC.SuppressFinalize(this);
        }
    }
}
