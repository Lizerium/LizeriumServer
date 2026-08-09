/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 09 августа 2026 15:52:37
 * Version: 1.0.135
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
/// РљРѕРЅС‚РµРєСЃС‚ СЂР°Р±РѕС‚С‹ СЃ Р‘Р”
/// </summary>
public partial class DataBaseService : DbContext, IDataBaseService
{
    private readonly DbContextOptions _context;

    /// <summary>
    /// РљРѕРЅСЃС‚СЂСѓРєС‚РѕСЂ
    /// </summary>
    /// <param name="options"></param>
    public DataBaseService(DbContextOptions<DataBaseService> options) : base(options) { _context = options; }

    /// <summary>
    /// РРЅРёС†РёР°Р»РёР·Р°С†РёСЏ С‚Р°Р±Р»РёС†С‹ РїРѕСЃС‚РѕРІ
    /// </summary>
    public DbSet<PostDataResponse> Posts { get; set; }

    /// <summary>
    /// РРЅРёС†РёР°Р»РёР·Р°С†РёСЏ С‚Р°Р±Р»РёС†С‹ РїРѕР»СЊР·РѕРІР°С‚РµР»РµР№
    /// </summary>
    public DbSet<UserApiKeyResponse> Users { get; set; }

    /// <summary>
    /// РРЅРёС†РёР°Р»РёР·Р°С†РёСЏ С‚Р°Р±Р»РёС†С‹ РєРѕРјР°РЅРґ
    /// </summary>
    public DbSet<CommandDataResponse> Commands { get; set; }

    /// <summary>
    /// РРЅРёС†РёР°Р»РёР·Р°С†РёСЏ С‚Р°Р±Р»РёС†С‹ РїРѕРґСЂРѕР±РЅРѕР№ РёРЅС„РѕСЂРјР°С†РёРё Рѕ РєРѕРјР°РЅРґР°С…
    /// </summary>
    public DbSet<CommandCategoryInfoResponse> CommandCategories { get; set; }

    /// <summary>
    /// РРЅРёС†РёР°Р»РёР·Р°С†РёСЏ С‚Р°Р±Р»РёС†С‹ РїРѕРґСЂРѕР±РЅРѕР№ РёРЅС„РѕСЂРјР°С†РёРё Рѕ РїРµСЂРµРІРѕРґР°С… РІСЃРµС… РєРѕРјРјР°РЅРґ
    /// </summary>
    public DbSet<CommandTranslation> CommandsTranslations { get; set; }

    /// <summary>
    /// РРЅРёС†РёР°Р»РёР·Р°С†РёСЏ С‚Р°Р±Р»РёС†С‹ РЅРѕРІРѕСЃС‚РµР№ Lizerium Steam
    /// </summary>
    public DbSet<LauncherNewsDataResponse> LauncherNews { get; set; }

    /// <summary>
    /// РРЅРёС†РёР°Р»РёР·Р°С†РёСЏ С‚Р°Р±Р»РёС†С‹ РєР°С‚РµРіРѕСЂРёР№ РїСЂРѕРґСѓРєС‚РѕРІ
    /// </summary>
    public DbSet<ProductCategoryDataResponse> ProductCategories { get; set; }

    /// <summary>
    /// РРЅРёС†РёР°Р»РёР·Р°С†РёСЏ С‚Р°Р±Р»РёС†С‹ РїСЂРѕРґСѓРєС‚РѕРІ
    /// </summary>
    public DbSet<ProductDataResponse> Products { get; set; }

    /// <summary>
    /// РРЅРёС†РёР°Р»РёР·Р°С†РёСЏ С‚Р°Р±Р»РёС†С‹ РёСЃС‚РѕС‡РЅРёРєРѕРІ СЃРєР°С‡РёРІР°РЅРёСЏ РїСЂРѕРґСѓРєС‚РѕРІ
    /// </summary>
    public DbSet<ProductDownloadLinkDataResponse> ProductDownloadLinks { get; set; }

    /// <summary>
    /// РЎРѕР±С‹С‚РёРµ СЃРѕР·РґР°РЅРёСЏ РјРѕРґРµР»Рё
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PostDataResponse>().ToTable("posts"); // РЈСЃС‚Р°РЅР°РІР»РёРІР°РµРј РёРјСЏ С‚Р°Р±Р»РёС†С‹ posts
        modelBuilder.Entity<UserApiKeyResponse>().ToTable("users"); // РЈСЃС‚Р°РЅР°РІР»РёРІР°РµРј РёРјСЏ С‚Р°Р±Р»РёС†С‹ users
        modelBuilder.Entity<CommandDataResponse>().ToTable("commands"); // РЈСЃС‚Р°РЅР°РІР»РёРІР°РµРј РёРјСЏ С‚Р°Р±Р»РёС†С‹ commands
        modelBuilder.Entity<CommandCategoryInfoResponse>().ToTable("commandCategories"); // РЈСЃС‚Р°РЅР°РІР»РёРІР°РµРј РёРјСЏ С‚Р°Р±Р»РёС†С‹ commandCategories
        modelBuilder.Entity<CommandTranslation>().ToTable("command_translations"); // РЈСЃС‚Р°РЅР°РІР»РёРІР°РµРј РёРјСЏ С‚Р°Р±Р»РёС†С‹ command_translations
        modelBuilder.Entity<LauncherNewsDataResponse>().ToTable("launcher_news"); // РЈСЃС‚Р°РЅР°РІР»РёРІР°РµРј РёРјСЏ С‚Р°Р±Р»РёС†С‹ launcher_news
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
    /// РЎРѕР·РґР°С‚СЊ РєРѕРјР°РЅРґСѓ
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
    /// РЈРґР°Р»РёС‚СЊ РєРѕРјР°РЅРґСѓ Рё РµС‘ РїРµСЂРµРІРѕРґС‹
    /// </summary>
    public async Task<bool> DeleteCommandAndTranslationsAsync(CreateCommandViewRequest Command, bool checkSecureOperate = true)
    {
        try
        {
            var commandId = Command.Id;



            // РќР°С‡РёРЅР°РµРј С‚СЂР°РЅР·Р°РєС†РёСЋ РґР»СЏ Р±РµР·РѕРїР°СЃРЅРѕСЃС‚Рё
            await using var transaction = await Database.BeginTransactionAsync();

            var translation = await CommandsTranslations.FindAsync(commandId);
            var command = await Commands.FindAsync(commandId);
            if (command != null)
            {
                // РћС‚РІСЏР·С‹РІР°РµРј РѕС‚ С‚СЂРµРєРёРЅРіР°, РµСЃР»Рё РЅСѓР¶РЅРѕ
                Entry(command).State = EntityState.Detached;

                if (translation != null)
                    Entry(translation).State = EntityState.Detached;

                // РЈРґР°Р»СЏРµРј С‡РµСЂРµР· SQL РёР»Рё РЅР°РїСЂСЏРјСѓСЋ
                await Database.ExecuteSqlInterpolatedAsync($@"
                    DELETE FROM command_translations WHERE CommandId = {commandId};
                    DELETE FROM commands WHERE Id = {commandId};
                ");
            }

            // 2пёЏвѓЈ РЈРґР°Р»СЏРµРј РєРѕРјР°РЅРґСѓ
            await Database.ExecuteSqlInterpolatedAsync($@"
                    DELETE FROM commands
                    WHERE Id = {commandId};
                ");

            // Р¤РёРєСЃРёСЂСѓРµРј РёР·РјРµРЅРµРЅРёСЏ
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
    /// Р”РѕР±Р°РІР»РµРЅРёРµ РєР°С‚РµРіРѕСЂРёРё РєРѕРјР°РЅРґ (РёРЅС„РѕСЂРјР°С†РёРё Рѕ РЅРµР№)
    /// </summary>
    public async Task<bool> AddCategoryAsync(CategoriesCommands category, bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateCommandsTable();

            // Р‘РµСЂС‘Рј СЂСѓСЃСЃРєРёР№ Рё Р°РЅРіР»РёР№СЃРєРёР№ Р·Р°РіРѕР»РѕРІРѕРє
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
    /// РР·РјРµРЅРёС‚СЊ РєРѕРјР°РЅРґСѓ
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
    /// РЎРѕС…СЂР°РЅРёС‚СЊ РєРѕРјР°РЅРґСѓ СЃ GIF СЃСЃС‹Р»РєРѕР№ РЅР° С„Р°Р№Р»
    /// </summary>
    /// <param name="fileName">РРјСЏ GIF С„Р°Р№Р»Р°</param>
    /// <param name="id">РРґРµРЅС‚РёС„РёРєР°С‚РѕСЂ РєРѕРјР°РЅРґС‹</param>
    /// <param name="checkSecureOperate">РџСЂРѕРІРµСЂСЏС‚СЊ Р»Рё СЃСѓС‰РµСЃС‚РІРѕРІР°РЅРёРµ С‚Р°Р±Р»РёС†С‹</param>
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
    /// РЎРѕС…СЂР°РЅСЏРµС‚ СЃСЂР°Р·Сѓ РїР°С‡РєСѓ РєРѕРјР°РЅРґ РІ Р±Р°Р·Рµ РґР°РЅРЅС‹С…
    /// </summary>
    /// <param name="jsonData">РЎС‡РёС‚Р°РЅРЅС‹Р№ РЅР°Р±РѕСЂ РєРѕРјР°РЅРґ РѕС‚СЃРѕСЂС‚РёСЂРѕРІР°РЅРЅС‹Р№ РїРѕ РєР°С‚РµРіРѕСЂРёСЏРј</param>
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
    /// РЎРѕС…СЂР°РЅСЏРµС‚ СЃСЂР°Р·Сѓ РїР°С‡РєСѓ РєР°С‚РµРіРѕСЂРёР№ РєРѕРјР°РЅРґ РІ Р±Р°Р·Рµ РґР°РЅРЅС‹С…
    /// </summary>
    /// <param name="jsonData">РЎС‡РёС‚Р°РЅРЅС‹Р№ РЅР°Р±РѕСЂ РєР°С‚РµРіРѕСЂРёР№ РєРѕРјР°РЅРґ РѕС‚СЃРѕСЂС‚РёСЂРѕРІР°РЅРЅС‹Р№ РїРѕ РєР°С‚РµРіРѕСЂРёСЏРј</param>
    public async Task SaveCategoriesCommandsFromJsonAsync(CommandsFileRequest jsonData, bool checkSecureOperate = true)
    {
        foreach (var category in jsonData.Categories)
        {
            await AddCategoryAsync(category, checkSecureOperate);
        }
    }

    /// <summary>
    /// РџРѕР»СѓС‡Р°РµС‚ СЃРїРёСЃРѕРє РїРѕСЃС‚РѕРІ РїРѕР»СЊР·РѕРІР°С‚РµР»РµР№
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
    /// РџРѕР»СѓС‡Р°РµС‚ СЃРїРёСЃРѕРє РєРѕРјР°РЅРґ
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
    /// РџРѕР»СѓС‡Р°РµС‚ СЃРїРёСЃРѕРє РїРµСЂРµРІРѕРґРѕРІ РєРѕРјР°РЅРґ
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
    /// РџРѕРёСЃРє РїРѕ РєРѕРјР°РЅРґР°Рј
    /// </summary>
    /// <param name="query">Р—Р°РїСЂРѕСЃ</param>
    /// <returns>РЎРїРёСЃРѕРє РєРѕРјР°РЅРґ</returns>
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

            // РїРѕР»СѓС‡Р°РµРј РІСЃРµ РїРµСЂРµРІРѕРґС‹ РґР»СЏ РєРѕРјР°РЅРґ
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

            // РїРѕР»СѓС‡Р°РµРј РІСЃРµ РїРµСЂРµРІРѕРґС‹ РґР»СЏ РєР°С‚РµРіРѕСЂРёР№
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
    /// РџРѕР»СѓС‡РµРЅРёРµ СЃРїРёСЃРєР° РєРѕРјР°РЅРґ РїРѕ РєР°С‚РµРіРѕСЂРёРё
    /// </summary>
    /// <param name="checkSecureOperate">РџСЂРѕРІРµСЂСЏС‚СЊ Р»Рё СЃСѓС‰РµСЃС‚РІРѕРІР°РЅРёРµ С‚Р°Р±Р»РёС†С‹</param>
    /// <returns>List<string></returns>
    public async Task<List<CommandCategoryInfoResponse>> GetAllCommandCategoriesAsync(bool checkSecureOperate = true)
    {
        try
        {
            if (checkSecureOperate)
                await ExistAndCreateCommandsTable();

            // Р‘РµСЂС‘Рј РІСЃРµ РєР°С‚РµРіРѕСЂРёРё РЅР°РїСЂСЏРјСѓСЋ РёР· С‚Р°Р±Р»РёС†С‹
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
    /// РџРѕР»СѓС‡Р°РµС‚ РІСЃРµ РєРѕРјР°РЅРґС‹ РїРѕ РєР°С‚РµРіРѕСЂРёРё
    /// </summary>
    /// <param name="Category">РљР°С‚РµРіРѕСЂРёСЏ РёРјСЏ</param>
    /// <param name="Page">РЎС‚СЂР°РЅРёС†Р°</param>
    /// <param name="Size">РљРѕР»РёС‡РµСЃС‚РІРѕ</param>
    /// <param name="checkSecureOperate">РџСЂРѕРІРµСЂСЏС‚СЊ Р»Рё СЃСѓС‰РµСЃС‚РІРѕРІР°РЅРёРµ С‚Р°Р±Р»РёС†С‹</param>
    /// <param name="shortSize">РћРіСЂР°РЅРёС‡РµРЅРёСЏ РІРєР»СЋС‡РµРЅС‹ РёР»Рё РІС‹РєР»СЋС‡РµРЅС‹</param>
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
                .OrderBy(c => c.Id); // РІРѕР·РІСЂР°С‰Р°РµС‚ IOrderedQueryable, РЅРѕ СЌС‚Рѕ РѕРє РґР»СЏ IQueryable

            if (shortSize)
            {
                if (Page < 1) Page = 1;
                if (Size < 1) Size = 10;
                query = query.Skip((Page - 1) * Size).Take(Size); // С‚РёРї IQueryable<CommandDataResponse>
            }

            var commands = await query.ToListAsync();

            // РјР°РїРїРёРј РЅР° CommandDataResponse
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
    /// РџРѕР»СѓС‡Р°РµС‚ РєРѕР»РёС‡РµСЃС‚РІРѕ РєРѕРјР°РЅРґ РІ РєР°С‚РµРіРѕСЂРёРё
    /// </summary>
    /// <param name="Category">РљР°С‚РµРіРѕСЂРёСЏ РёРјСЏ</param>
    /// <param name="checkSecureOperate">РџСЂРѕРІРµСЂСЏС‚СЊ Р»Рё СЃСѓС‰РµСЃС‚РІРѕРІР°РЅРёРµ С‚Р°Р±Р»РёС†С‹</param>
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
    /// РџРѕР»СѓС‡РёС‚СЊ РєРѕРјР°РЅРґС‹ Р±РµР· РїРµСЂРµРІРѕРґР°.
    /// </summary>
    /// <param name="toLang">en</param>
    /// <returns>РЎРїРёСЃРѕРє РєРѕРјР°РЅРґ</returns>
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
    /// РЎРѕС…СЂР°РЅРµРЅРёРµ/РѕР±РЅРѕРІР»РµРЅРёРµ РєРѕРјР°РЅРґС‹ РІ Р‘Р” (SQLite РІРµСЂСЃРёСЏ)
    /// </summary>
    public async Task SaveCommandTranslationsAsync(AdminCommandWithTranslations command)
    {
        try
        {
            foreach (var translation in command.Translations)
            {
                // SQLite РїРѕРґРґРµСЂР¶РёРІР°РµС‚ "INSERT OR REPLACE" РґР»СЏ РѕР±РЅРѕРІР»РµРЅРёСЏ РёР»Рё РІСЃС‚Р°РІРєРё
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
    /// РЎРѕС…СЂР°РЅРµРЅРёРµ РІСЃРµС… РїРµСЂРµРІРѕРґРѕРІ РІ Р‘Р”
    /// </summary>
    /// <param name="commandTranslations">РЎРїРёСЃРѕРє DTO РїРµСЂРµРІРѕРґРѕРІ</param>
    /// <returns>РЎС‚Р°С‚СѓСЃ РѕРїРµСЂР°С†РёРё</returns>
    public async Task<bool> SaveAllCommandsTranslationsAsync(List<CommandTranslationResponse> commandTranslations)
    {
        foreach (var dto in commandTranslations)
        {
            // РџСЂРѕРІРµСЂСЏРµРј, РµСЃС‚СЊ Р»Рё СѓР¶Рµ РїРµСЂРµРІРѕРґ РґР»СЏ СЌС‚РѕР№ РєРѕРјР°РЅРґС‹ Рё СЏР·С‹РєР°
            var existing = await CommandsTranslations
                .FirstOrDefaultAsync(t => t.CommandId == dto.CommandId && t.LanguageCode == dto.LanguageCode);

            if (existing != null)
            {
                // РћР±РЅРѕРІР»СЏРµРј РѕРїРёСЃР°РЅРёРµ
                existing.Description = dto.Description;
            }
            else
            {
                // РЎРѕР·РґР°С‘Рј РЅРѕРІСѓСЋ Р·Р°РїРёСЃСЊ
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
    /// РџСЂРѕРІРµСЂСЏРµС‚ СЃСѓС‰РµСЃС‚РІРѕРІР°РЅРёРµ С‚Р°Р±Р»РёС†С‹ РЅРѕРІРѕСЃС‚РµР№ Рё СЃРѕР·РґР°РµС‚ РµРµ РїСЂРё РЅРµРѕР±С…РѕРґРёРјРѕСЃС‚Рё.
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
                    TitleRu = "РџСЂРµРґСЃС‚РѕСЏС‰РёРµ РѕР±РЅРѕРІР»РµРЅРёСЏ РІ 99.5.1 Freelancer Lizerium",
                    TitleEn = "Upcoming updates in 99.5.1 Freelancer Lizerium",
                    RutubeUrl = "https://rutube.ru/video/166b1de79791472c13f79c24838847c3/",
                    IsPublished = true,
                    SortOrder = 10,
                    PublishedAtUnix = publishedAtUnix
                },
                new()
                {
                    TitleRu = "РћС„РёС†РёР°Р»СЊРЅС‹Р№ СЂСѓСЃСЃРєРѕСЏР·С‹С‡РЅС‹Р№ С‚СЂРµР№Р»РµСЂ РёРіСЂС‹ Freelancer Lizerium",
                    TitleEn = "Official Russian trailer of Freelancer Lizerium",
                    RutubeUrl = "https://rutube.ru/video/f7359c52b38dbfd9eab1426349de6571/",
                    IsPublished = true,
                    SortOrder = 20,
                    PublishedAtUnix = publishedAtUnix
                },
                new()
                {
                    TitleRu = "Р”РµРјРѕРЅСЃС‚СЂР°С†РёСЏ РІС‚РѕСЂРѕР№ РІРµСЂСЃРёРё РїРѕР»РµС‚Р°, СЌС„С„РµРєС‚РѕРІ, Р·РІСѓРєРѕРІ Freelancer Lizerium (Unity ver.)",
                    TitleEn = "Second flight, effects and sound demo for Freelancer Lizerium (Unity ver.)",
                    RutubeUrl = "https://rutube.ru/video/da9bd6b780314bb96ca23b10110dcfd9/",
                    IsPublished = true,
                    SortOrder = 30,
                    PublishedAtUnix = publishedAtUnix
                },
                new()
                {
                    TitleRu = "РџРµСЂРІРѕРµ РёСЃРїС‹С‚Р°РЅРёРµ С‚СЂРµР№Р»РѕРІ РІРѕ Freelancer Lizerium (Unity ver.)",
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
    /// РџРѕР»СѓС‡Р°РµС‚ РѕРїСѓР±Р»РёРєРѕРІР°РЅРЅС‹Рµ РЅРѕРІРѕСЃС‚Рё Lizerium Steam.
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
    /// РџРѕР»СѓС‡Р°РµС‚ РІСЃРµ РЅРѕРІРѕСЃС‚Рё РґР»СЏ Р°РґРјРёРЅРєРё.
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
    /// РџРѕР»СѓС‡Р°РµС‚ РѕРґРЅСѓ РЅРѕРІРѕСЃС‚СЊ РґР»СЏ Р·Р°РєСЂС‹С‚РѕРіРѕ Р°РґРјРёРЅСЃРєРѕРіРѕ РїСЂРµРґРїСЂРѕСЃРјРѕС‚СЂР°, РІРєР»СЋС‡Р°СЏ СЃРєСЂС‹С‚С‹Рµ С‡РµСЂРЅРѕРІРёРєРё.
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
    /// Р”РѕР±Р°РІР»СЏРµС‚ РёР»Рё РѕР±РЅРѕРІР»СЏРµС‚ РЅРѕРІРѕСЃС‚СЊ.
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
    /// РЈРґР°Р»СЏРµС‚ РЅРѕРІРѕСЃС‚СЊ.
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
    /// РџРѕР»СѓС‡Р°РµС‚ РѕРїСѓР±Р»РёРєРѕРІР°РЅРЅС‹Р№ РєР°С‚Р°Р»РѕРі РїСЂРѕРґСѓРєС‚РѕРІ РґР»СЏ РїСѓР±Р»РёС‡РЅРѕР№ РІРёС‚СЂРёРЅС‹.
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
    /// РџРѕР»СѓС‡Р°РµС‚ РїРѕР»РЅС‹Р№ РєР°С‚Р°Р»РѕРі РїСЂРѕРґСѓРєС‚РѕРІ РґР»СЏ Р°РґРјРёРЅРєРё.
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
    /// Р”РѕР±Р°РІР»СЏРµС‚ РёР»Рё РѕР±РЅРѕРІР»СЏРµС‚ РєР°С‚РµРіРѕСЂРёСЋ РїСЂРѕРґСѓРєС‚РѕРІ.
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
    /// РЈРґР°Р»СЏРµС‚ РєР°С‚РµРіРѕСЂРёСЋ РїСЂРѕРґСѓРєС‚РѕРІ.
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
    /// Р”РѕР±Р°РІР»СЏРµС‚ РёР»Рё РѕР±РЅРѕРІР»СЏРµС‚ РїСЂРѕРґСѓРєС‚.
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
    /// РЈРґР°Р»СЏРµС‚ РїСЂРѕРґСѓРєС‚.
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
    /// Р”РѕР±Р°РІР»СЏРµС‚ РёР»Рё РѕР±РЅРѕРІР»СЏРµС‚ РёСЃС‚РѕС‡РЅРёРє СЃРєР°С‡РёРІР°РЅРёСЏ РїСЂРѕРґСѓРєС‚Р°.
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
    /// РЈРґР°Р»СЏРµС‚ РёСЃС‚РѕС‡РЅРёРє СЃРєР°С‡РёРІР°РЅРёСЏ РїСЂРѕРґСѓРєС‚Р°.
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

            // РњРёРіСЂР°С†РёСЏ С‚РµРєСѓС‰РёС… РґР°РЅРЅС‹С… РІ С‚Р°Р±Р»РёС†Сѓ РїРµСЂРµРІРѕРґРѕРІ (СЂСѓСЃСЃРєРёР№ СЏР·С‹Рє)
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
    /// РџРѕР»СѓС‡Р°РµС‚ СЃРїРёСЃРѕРє РїРѕСЃС‚РѕРІ РїРѕР»СЊР·РѕРІР°С‚РµР»РµР№
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
    /// РџРѕР»СѓС‡Р°РµС‚ СЃРїРёСЃРѕРє РїРѕСЃС‚РѕРІ РїРѕР»СЊР·РѕРІР°С‚РµР»РµР№
    /// </summary>
    /// <param name="id">РљСЂР°Р№РЅРёР№ РїРѕСЃС‚</param>
    /// <param name="status">СЃС‚Р°С‚СѓСЃ</param>
    /// <param name="scroll">СЃРєСЂРѕР»РёРЅРіРѕРј Р»Рё Р·Р°РіСЂСѓР·РєР° РёР»Рё С„РёР»СЊС‚СЂС‹</param>
    public async Task<DataPosts> GetAllPostsAsync(int id, int status, bool scroll = false)
    {
        try
        {
            if (!scroll && id == 0 && status == 0)
            {
                var postsNullIndex = await Posts.OrderBy(post => post.Id) // РЎРѕСЂС‚РёСЂСѓРµРј РїРѕ РІРѕР·СЂР°СЃС‚Р°РЅРёСЋ Id
                                              .OrderBy(post => post.DateTimeUnix)
                                              .Reverse() // РџРµСЂРµРІРѕСЂР°С‡РёРІР°РµРј РїРѕСЂСЏРґРѕРє
                                              .Take(10) // РћРіСЂР°РЅРёС‡РёРІР°РµРј РІС‹Р±РѕСЂРєСѓ 10 РїРѕСЃС‚Р°РјРё
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
                              .Reverse() // РџРµСЂРµРІРѕСЂР°С‡РёРІР°РµРј РїРѕСЂСЏРґРѕРє
                              .Take(10) // РћРіСЂР°РЅРёС‡РёРІР°РµРј РІС‹Р±РѕСЂРєСѓ 10 РїРѕСЃС‚Р°РјРё
                              .ToListAsync();
                return new DataPosts()
                {
                    Posts = postsNullIndex,
                    LastUserId = id
                };
            }

            if (status == 0)
            {
                var postsNull = await Posts.Where(post => post.Id < id) // Р¤РёР»СЊС‚СЂСѓРµРј РїРѕСЃС‚С‹, id РєРѕС‚РѕСЂС‹С… Р±РѕР»СЊС€Рµ, С‡РµРј lastUserId
                                                .OrderBy(post => post.Id) // РЎРѕСЂС‚РёСЂСѓРµРј РїРѕ РІРѕР·СЂР°СЃС‚Р°РЅРёСЋ Id
                                                .OrderBy(post => post.DateTimeUnix)
                                                .Reverse() // РџРµСЂРµРІРѕСЂР°С‡РёРІР°РµРј РїРѕСЂСЏРґРѕРє
                                                .Take(10) // РћРіСЂР°РЅРёС‡РёРІР°РµРј РІС‹Р±РѕСЂРєСѓ 10 РїРѕСЃС‚Р°РјРё
                                                .ToListAsync();

                return new DataPosts()
                {
                    Posts = postsNull,
                    LastUserId = id
                };
            }

            // Р¤РёР»СЊС‚СЂСѓРµРј РїРѕСЃС‚С‹, id РєРѕС‚РѕСЂС‹С… Р±РѕР»СЊС€Рµ, С‡РµРј lastUserId
            var posts = await Posts.Where(post => post.Id < id && post.Status == status)
            .OrderBy(post => post.Id) // РЎРѕСЂС‚РёСЂСѓРµРј РїРѕ РІРѕР·СЂР°СЃС‚Р°РЅРёСЋ Id
            .OrderBy(post => post.DateTimeUnix)
            .Reverse() // РџРµСЂРµРІРѕСЂР°С‡РёРІР°РµРј РїРѕСЂСЏРґРѕРє
            .Take(10) // РћРіСЂР°РЅРёС‡РёРІР°РµРј РІС‹Р±РѕСЂРєСѓ 10 РїРѕСЃС‚Р°РјРё
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
    /// РџРѕР»СѓС‡Р°РµС‚ СЃРїРёСЃРѕРє РїРѕСЃС‚РѕРІ РїРѕР»СЊР·РѕРІР°С‚РµР»РµР№
    /// </summary>
    /// <param name="lastUserId">РРґРµРЅС‚РёС„РёРєР°С‚РѕСЂ РєСЂР°Р№РЅРµРіРѕ РїРѕР»СѓС‡РµРЅРЅРѕРіРѕ РїРѕР»СЊР·РѕРІР°С‚РµР»СЏ</param>
    public async Task<DataPosts> GetAllPostsAsync(long lastUserId)
    {
        try
        {
            var posts = await Posts.Where(post => post.Id < lastUserId) // Р¤РёР»СЊС‚СЂСѓРµРј РїРѕСЃС‚С‹, id РєРѕС‚РѕСЂС‹С… Р±РѕР»СЊС€Рµ, С‡РµРј lastUserId
                        .OrderBy(post => post.Id) // РЎРѕСЂС‚РёСЂСѓРµРј РїРѕ РІРѕР·СЂР°СЃС‚Р°РЅРёСЋ Id
                        .OrderBy(post => post.DateTimeUnix)
                        .Reverse() // РџРµСЂРµРІРѕСЂР°С‡РёРІР°РµРј РїРѕСЂСЏРґРѕРє
                        .Take(30) // РћРіСЂР°РЅРёС‡РёРІР°РµРј РІС‹Р±РѕСЂРєСѓ 30 РїРѕСЃС‚Р°РјРё
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
    /// РћР±РЅРѕРІР»СЏРµС‚ СЃС‚Р°С‚СѓСЃ Р·Р°СЏРІРєРё РїРѕР»СЊР·РѕРІР°С‚РµР»СЏ
    /// </summary>
    /// <param name="lastUserId">РРґРµРЅС‚РёС„РёРєР°С‚РѕСЂ РїРѕСЃС‚Р° РїРѕР»СЊР·РѕРІР°С‚РµР»СЏ</param>
    /// <param name="status">РЎС‚Р°С‚СѓСЃ РѕР±СЂР°Р±РѕС‚РєРё</param>
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
    /// РџСЂРѕРІРµСЂСЏРµС‚ СЃСѓС‰РµСЃС‚РІРѕРІР°РЅРёРµ РєР»СЋС‡Р° РїРѕР»СЊР·РѕРІР°С‚РµР»СЏ
    /// </summary>
    /// <param name="Data">РРЅС„РѕСЂРјР°С†РёСЏ Рѕ РїРѕР»СЊР·РѕРІР°С‚РµР»Рµ</param>
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
    /// РџСЂРѕРІРµСЂРєР° СЃСѓС‰РµСЃС‚РІРѕРІР°РЅРёСЏ С‚Р°Р±Р»РёС†С‹ РїРѕ РёРјРµРЅРё
    /// </summary>
    /// <param name="tableName">РРјСЏ С‚Р°Р±Р»РёС†С‹</param>
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
    /// Р“РµРЅРµСЂРёСЂРѕРІР°С‚СЊ Р±Р°Р·РѕРІСѓСЋ С‚Р°Р±Р»РёС†Сѓ
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
    /// РЎРѕР·РґР°РЅРёРµ С„Р°Р№Р»Р° Р‘Р”
    /// </summary>
    /// <param name="optionsBuilder">РћРїС†РёРё</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return; // РЈР¶Рµ РЅР°СЃС‚СЂРѕРµРЅРѕ, РЅРµ С‚СЂРѕРіР°РµРј

        try
        {
            var dataSecretRecords = DatabaseExtensions.Configuration.GetValue<string>("path");
            var dir = Path.GetDirectoryName(Environment.ProcessPath);
            var path = Path.Combine(dir, dataSecretRecords);
            //Р»РѕРіРёСЂСѓРµРј РёСЃРєР»СЋС‡РµРЅРёРµ
            ("DatabasePath: " + path).LogMessage();
            optionsBuilder.UseSqlite("Data Source=" + path);
        }
        catch (Exception ex)
        {
            ex.LogException();
        }
    }

    /// <summary>
    /// РЎРѕР·РґР°РЅРёРµ Р‘Р”
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
    /// Р Р°Р·СЂСѓС€РёС‚РµР»СЊ СЃРѕРµРґРёРЅРµРЅРёСЏ Postgresql
    /// </summary>
    public void Dispose()
    {
        try
        {
            //СЂР°Р·СЂСѓС€Р°РµРј СЃРѕРµРґРёРЅРµРЅРёРµ Postgresql
            Database.CloseConnection();
        }
        catch (Exception exception)
        {
            //Р»РѕРіРёСЂСѓРµРј РёСЃРєР»СЋС‡РµРЅРёРµ
            exception.LogException();
        }
        finally
        {
            GC.SuppressFinalize(this);
        }
    }
}
