/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 08 августа 2026 07:13:54
 * Version: 1.0.134
 */

using LizeriumUtilities.FormatsData.AppUserData;
using LizeriumUtilities.FormatsData.DataBase.Requests;
using LizeriumUtilities.FormatsData.DataBase.Response;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LizeriumDatabase.Services.AppDataBaseService;

public interface IDataBaseService : IDisposable
{
    /// <summary>
    /// Р‘Р°Р·Р° РґР°РЅРЅС‹С…
    /// </summary>
    DatabaseFacade Database { get; }
    /// <summary>
    /// РРЅРёС†РёР°Р»РёР·Р°С†РёСЏ С‚Р°Р±Р»РёС†С‹ РїРѕСЃС‚РѕРІ
    /// </summary>
    DbSet<PostDataResponse> Posts { get; set; }
    /// <summary>
    /// РРЅРёС†РёР°Р»РёР·Р°С†РёСЏ С‚Р°Р±Р»РёС†С‹ РїРѕР»СЊР·РѕРІР°С‚РµР»РµР№
    /// </summary>
    DbSet<UserApiKeyResponse> Users { get; set; }
    /// <summary>
    /// РРЅРёС†РёР°Р»РёР·Р°С†РёСЏ С‚Р°Р±Р»РёС†С‹ РєРѕРјР°РЅРґ
    /// </summary>
    DbSet<CommandDataResponse> Commands { get; set; }

    /// <summary>
    /// РРЅРёС†РёР°Р»РёР·Р°С†РёСЏ С‚Р°Р±Р»РёС†С‹ РЅРѕРІРѕСЃС‚РµР№ Lizerium Steam.
    /// </summary>
    DbSet<LauncherNewsDataResponse> LauncherNews { get; set; }

    /// <summary>
    /// РРЅРёС†РёР°Р»РёР·Р°С†РёСЏ С‚Р°Р±Р»РёС†С‹ РєР°С‚РµРіРѕСЂРёР№ РїСЂРѕРґСѓРєС‚РѕРІ.
    /// </summary>
    DbSet<ProductCategoryDataResponse> ProductCategories { get; set; }

    /// <summary>
    /// РРЅРёС†РёР°Р»РёР·Р°С†РёСЏ С‚Р°Р±Р»РёС†С‹ РїСЂРѕРґСѓРєС‚РѕРІ.
    /// </summary>
    DbSet<ProductDataResponse> Products { get; set; }

    /// <summary>
    /// РРЅРёС†РёР°Р»РёР·Р°С†РёСЏ С‚Р°Р±Р»РёС†С‹ РёСЃС‚РѕС‡РЅРёРєРѕРІ СЃРєР°С‡РёРІР°РЅРёСЏ РїСЂРѕРґСѓРєС‚РѕРІ.
    /// </summary>
    DbSet<ProductDownloadLinkDataResponse> ProductDownloadLinks { get; set; }


    /// <summary>
    /// РЎРѕС…СЂР°РЅРµРЅРёРµ РёР·РјРµРЅРµРЅРёР№ РІ Р±Р°Р·Сѓ РґР°РЅРЅС‹С…
    /// </summary>
    /// <param name="cancellationToken">РўРѕРєРµРЅ РѕС‚РјРµРЅС‹ РѕРїРµСЂР°С†РёРё</param>
    /// <returns></returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// РџСЂРѕРІРµСЂРєР° СЃСѓС‰РµСЃС‚РІРѕРІР°РЅРёСЏ С‚Р°Р±Р»РёС† Рё РіРµРЅРµСЂР°С†РёСЏ РёС…
    /// </summary>
    Task ExistAndCreateCommandsTable();
    /// <summary>
    /// РџРѕР»СѓС‡Р°РµС‚ РєРѕР»РёС‡РµСЃС‚РІРѕ РєРѕРјР°РЅРґ РІ РєР°С‚РµРіРѕСЂРёРё
    /// </summary>
    /// <param name="Category">РљР°С‚РµРіРѕСЂРёСЏ РёРјСЏ</param>
    /// <param name="checkSecureOperate">РџСЂРѕРІРµСЂСЏС‚СЊ Р»Рё СЃСѓС‰РµСЃС‚РІРѕРІР°РЅРёРµ С‚Р°Р±Р»РёС†С‹</param>
    /// <returns>int</returns>
    Task<int> GetCommandsCountAsync(string Category, bool checkSecureOperate = true);
    /// <summary>
    /// РџРѕР»СѓС‡Р°РµС‚ РІСЃРµ РєРѕРјР°РЅРґС‹ РїРѕ РєР°С‚РµРіРѕСЂРёРё
    /// </summary>
    /// <param name="Category">РљР°С‚РµРіРѕСЂРёСЏ РёРјСЏ</param>
    /// <param name="Page">РЎС‚СЂР°РЅРёС†Р°</param>
    /// <param name="Size">РљРѕР»РёС‡РµСЃС‚РІРѕ</param>
    /// <param name="checkSecureOperate">РџСЂРѕРІРµСЂСЏС‚СЊ Р»Рё СЃСѓС‰РµСЃС‚РІРѕРІР°РЅРёРµ С‚Р°Р±Р»РёС†С‹</param>
    /// <param name="shortSize">РћРіСЂР°РЅРёС‡РµРЅРёСЏ РІРєР»СЋС‡РµРЅС‹ РёР»Рё РІС‹РєР»СЋС‡РµРЅС‹</param>
    /// <returns></returns>
    Task<List<CommandDataResponse>> GetCommandsAsync(string Category, int Page = 1, int Size = 10, bool checkSecureOperate = true,
        bool shortSize = true);
    /// <summary>
    /// РџРѕР»СѓС‡Р°РµС‚ СЃРїРёСЃРѕРє РїРµСЂРµРІРѕРґРѕРІ РєРѕРјР°РЅРґ
    /// </summary>
    Task<List<AdminCommandWithTranslations>> GetAllAdminCommandTranslatesAsync(bool checkSecureOperate = true);
    /// <summary>
    /// РџРѕР»СѓС‡Р°РµС‚ СЃРїРёСЃРѕРє РєРѕРјР°РЅРґ
    /// </summary>
    Task<List<CommandDataResponse>> GetAllAdminCommandsAsync(bool checkSecureOperate = true);
    /// <summary>
    /// РџРѕР»СѓС‡Р°РµС‚ СЃРїРёСЃРѕРє РїРѕСЃС‚РѕРІ РїРѕР»СЊР·РѕРІР°С‚РµР»РµР№
    /// </summary>
    Task<List<PostDataResponse>> GetAllAdminPostsAsync();
    /// <summary>
    /// РџРѕРёСЃРє РїРѕ РєРѕРјР°РЅРґР°Рј
    /// </summary>
    /// <param name="query">Р—Р°РїСЂРѕСЃ</param>
    /// <returns>РЎРїРёСЃРѕРє РєРѕРјР°РЅРґ</returns>
    Task<List<CommandDataResponse>> SearchCommandsAsync(string query, bool checkSecureOperate = true);
    /// <summary>
    /// РџРѕР»СѓС‡РµРЅРёРµ СЃРїРёСЃРєР° РєРѕРјР°РЅРґ РїРѕ РєР°С‚РµРіРѕСЂРёРё
    /// </summary>
    /// <param name="checkSecureOperate">РџСЂРѕРІРµСЂСЏС‚СЊ Р»Рё СЃСѓС‰РµСЃС‚РІРѕРІР°РЅРёРµ С‚Р°Р±Р»РёС†С‹</param>
    /// <returns>List<string></returns>
    Task<List<CommandCategoryInfoResponse>> GetAllCommandCategoriesAsync(bool checkSecureOperate = true);
    /// <summary>
    /// РЎРѕС…СЂР°РЅСЏРµС‚ СЃСЂР°Р·Сѓ РїР°С‡РєСѓ РєРѕРјР°РЅРґ РІ Р±Р°Р·Рµ РґР°РЅРЅС‹С…
    /// </summary>
    /// <param name="jsonData">РЎС‡РёС‚Р°РЅРЅС‹Р№ РЅР°Р±РѕСЂ РєРѕРјР°РЅРґ РѕС‚СЃРѕСЂС‚РёСЂРѕРІР°РЅРЅС‹Р№ РїРѕ РєР°С‚РµРіРѕСЂРёСЏРј</param>
    Task SaveCommandsFromJsonAsync(CommandsFileRequest jsonData, bool checkSecureOperate = true);
    /// <summary>
    /// РЎРѕС…СЂР°РЅСЏРµС‚ СЃСЂР°Р·Сѓ РїР°С‡РєСѓ РєР°С‚РµРіРѕСЂРёР№ РєРѕРјР°РЅРґ РІ Р±Р°Р·Рµ РґР°РЅРЅС‹С…
    /// </summary>
    /// <param name="jsonData">РЎС‡РёС‚Р°РЅРЅС‹Р№ РЅР°Р±РѕСЂ РєР°С‚РµРіРѕСЂРёР№ РєРѕРјР°РЅРґ РѕС‚СЃРѕСЂС‚РёСЂРѕРІР°РЅРЅС‹Р№ РїРѕ РєР°С‚РµРіРѕСЂРёСЏРј</param>
    Task SaveCategoriesCommandsFromJsonAsync(CommandsFileRequest jsonData, bool checkSecureOperate = true);
    /// <summary>
    /// РЎРѕС…СЂР°РЅРёС‚СЊ РєРѕРјР°РЅРґСѓ СЃ GIF СЃСЃС‹Р»РєРѕР№ РЅР° С„Р°Р№Р»
    /// </summary>
    /// <param name="fileName">РРјСЏ GIF С„Р°Р№Р»Р°</param>
    /// <param name="id">РРґРµРЅС‚РёС„РёРєР°С‚РѕСЂ РєРѕРјР°РЅРґС‹</param>
    /// <param name="checkSecureOperate">РџСЂРѕРІРµСЂСЏС‚СЊ Р»Рё СЃСѓС‰РµСЃС‚РІРѕРІР°РЅРёРµ С‚Р°Р±Р»РёС†С‹</param>
    /// <returns>bool</returns>
    Task<bool> SaveGifCommandAsync(string fileName, string id, bool checkSecureOperate = true);
    /// <summary>
    /// РР·РјРµРЅРёС‚СЊ РєРѕРјР°РЅРґСѓ
    /// </summary>
    Task<bool> ChangeCommandAsync(CreateCommandViewRequest Command, bool checkSecureOperate = true);
    /// <summary>
    /// РЎРѕР·РґР°С‚СЊ РєРѕРјР°РЅРґСѓ
    /// </summary>
    Task<bool> AddCommandAsync(CreateCommandViewRequest Command, bool checkSecureOperate = true);
    /// <summary>
    /// РЈРґР°Р»РёС‚СЊ РєРѕРјР°РЅРґСѓ Рё РµС‘ РїРµСЂРµРІРѕРґС‹
    /// </summary>
    Task<bool> DeleteCommandAndTranslationsAsync(CreateCommandViewRequest Command, bool checkSecureOperate = true);
    /// <summary>
    /// РЎРѕС…СЂР°РЅРµРЅРёРµ РІСЃРµС… РїРµСЂРµРІРѕРґРѕРІ РІ Р‘Р”
    /// </summary>
    /// <param name="commandTranslations">РЎРїРёСЃРѕРє DTO РїРµСЂРµРІРѕРґРѕРІ</param>
    /// <returns>РЎС‚Р°С‚СѓСЃ РѕРїРµСЂР°С†РёРё</returns>
    Task<bool> SaveAllCommandsTranslationsAsync(List<CommandTranslationResponse> commandTranslations);
    /// <summary>
    /// РџРѕР»СѓС‡РёС‚СЊ РєРѕРјР°РЅРґС‹ Р±РµР· РїРµСЂРµРІРѕРґР°.
    /// </summary>
    /// <param name="toLang">en</param>
    /// <returns>РЎРїРёСЃРѕРє РєРѕРјР°РЅРґ</returns>
    Task<List<CommandTranslationResponse>> GetCommandsMissingTranslationAsync(string toLang);
    /// <summary>
    /// Р”РѕР±Р°РІР»РµРЅРёРµ РєР°С‚РµРіРѕСЂРёРё РєРѕРјР°РЅРґ (РёРЅС„РѕСЂРјР°С†РёРё Рѕ РЅРµР№)
    /// </summary>
    Task<bool> AddCategoryAsync(CategoriesCommands category, bool checkSecureOperate = true);

    /// <summary>
    /// РџСЂРѕРІРµСЂСЏРµС‚ СЃСѓС‰РµСЃС‚РІРѕРІР°РЅРёРµ С‚Р°Р±Р»РёС†С‹ РЅРѕРІРѕСЃС‚РµР№ Рё СЃРѕР·РґР°РµС‚ РµРµ РїСЂРё РЅРµРѕР±С…РѕРґРёРјРѕСЃС‚Рё.
    /// </summary>
    Task ExistAndCreateLauncherNewsTable();

    /// <summary>
    /// РџРѕР»СѓС‡Р°РµС‚ РѕРїСѓР±Р»РёРєРѕРІР°РЅРЅС‹Рµ РЅРѕРІРѕСЃС‚Рё Lizerium Steam.
    /// </summary>
    Task<List<LauncherNewsDataResponse>> GetPublishedLauncherNewsAsync(bool checkSecureOperate = true);

    /// <summary>
    /// РџРѕР»СѓС‡Р°РµС‚ РІСЃРµ РЅРѕРІРѕСЃС‚Рё РґР»СЏ Р°РґРјРёРЅРєРё.
    /// </summary>
    Task<List<LauncherNewsDataResponse>> GetAllAdminLauncherNewsAsync(bool checkSecureOperate = true);

    /// <summary>
    /// РџРѕР»СѓС‡Р°РµС‚ РЅРѕРІРѕСЃС‚СЊ РґР»СЏ Р°РґРјРёРЅСЃРєРѕРіРѕ РїСЂРµРґРїСЂРѕСЃРјРѕС‚СЂР° Р±РµР· С„РёР»СЊС‚СЂР° РїСѓР±Р»РёРєР°С†РёРё.
    /// </summary>
    Task<LauncherNewsDataResponse> GetAdminLauncherNewsByIdAsync(int id, bool checkSecureOperate = true);

    /// <summary>
    /// Р”РѕР±Р°РІР»СЏРµС‚ РёР»Рё РѕР±РЅРѕРІР»СЏРµС‚ РЅРѕРІРѕСЃС‚СЊ.
    /// </summary>
    Task<bool> SaveLauncherNewsAsync(LauncherNewsDataResponse news, bool checkSecureOperate = true);

    /// <summary>
    /// РЈРґР°Р»СЏРµС‚ РЅРѕРІРѕСЃС‚СЊ.
    /// </summary>
    Task<bool> DeleteLauncherNewsAsync(int id, bool checkSecureOperate = true);

    /// <summary>
    /// Increments public like counter for a launcher news item.
    /// </summary>
    Task<int?> IncrementLauncherNewsLikeAsync(int id, bool checkSecureOperate = true);

    /// <summary>
    /// РџСЂРѕРІРµСЂСЏРµС‚ СЃСѓС‰РµСЃС‚РІРѕРІР°РЅРёРµ РїСЂРѕРґСѓРєС‚РѕРІС‹С… С‚Р°Р±Р»РёС† Рё СЃРѕР·РґР°РµС‚ РёС… РїСЂРё РЅРµРѕР±С…РѕРґРёРјРѕСЃС‚Рё.
    /// </summary>
    Task ExistAndCreateProductsTables();

    /// <summary>
    /// РџРѕР»СѓС‡Р°РµС‚ РѕРїСѓР±Р»РёРєРѕРІР°РЅРЅС‹Р№ РєР°С‚Р°Р»РѕРі РїСЂРѕРґСѓРєС‚РѕРІ РґР»СЏ РїСѓР±Р»РёС‡РЅРѕР№ РІРёС‚СЂРёРЅС‹.
    /// </summary>
    Task<List<ProductCategoryDataResponse>> GetPublishedProductCatalogAsync(bool checkSecureOperate = true);

    /// <summary>
    /// РџРѕР»СѓС‡Р°РµС‚ РїРѕР»РЅС‹Р№ РєР°С‚Р°Р»РѕРі РїСЂРѕРґСѓРєС‚РѕРІ РґР»СЏ Р°РґРјРёРЅРєРё.
    /// </summary>
    Task<List<ProductCategoryDataResponse>> GetAllAdminProductCatalogAsync(bool checkSecureOperate = true);

    /// <summary>
    /// Р”РѕР±Р°РІР»СЏРµС‚ РёР»Рё РѕР±РЅРѕРІР»СЏРµС‚ РєР°С‚РµРіРѕСЂРёСЋ РїСЂРѕРґСѓРєС‚РѕРІ.
    /// </summary>
    Task<bool> SaveProductCategoryAsync(ProductCategoryDataResponse category, bool checkSecureOperate = true);

    /// <summary>
    /// РЈРґР°Р»СЏРµС‚ РєР°С‚РµРіРѕСЂРёСЋ РїСЂРѕРґСѓРєС‚РѕРІ.
    /// </summary>
    Task<bool> DeleteProductCategoryAsync(int id, bool checkSecureOperate = true);

    /// <summary>
    /// Р”РѕР±Р°РІР»СЏРµС‚ РёР»Рё РѕР±РЅРѕРІР»СЏРµС‚ РїСЂРѕРґСѓРєС‚.
    /// </summary>
    Task<bool> SaveProductAsync(ProductDataResponse product, bool checkSecureOperate = true);

    /// <summary>
    /// РЈРґР°Р»СЏРµС‚ РїСЂРѕРґСѓРєС‚.
    /// </summary>
    Task<bool> DeleteProductAsync(int id, bool checkSecureOperate = true);

    /// <summary>
    /// Р”РѕР±Р°РІР»СЏРµС‚ РёР»Рё РѕР±РЅРѕРІР»СЏРµС‚ РёСЃС‚РѕС‡РЅРёРє СЃРєР°С‡РёРІР°РЅРёСЏ РїСЂРѕРґСѓРєС‚Р°.
    /// </summary>
    Task<bool> SaveProductDownloadLinkAsync(ProductDownloadLinkDataResponse link, bool checkSecureOperate = true);

    /// <summary>
    /// РЈРґР°Р»СЏРµС‚ РёСЃС‚РѕС‡РЅРёРє СЃРєР°С‡РёРІР°РЅРёСЏ РїСЂРѕРґСѓРєС‚Р°.
    /// </summary>
    Task<bool> DeleteProductDownloadLinkAsync(int id, bool checkSecureOperate = true);
    /// <summary>
    /// Р“РµРЅРµСЂРёСЂРѕРІР°С‚СЊ Р±Р°Р·РѕРІСѓСЋ С‚Р°Р±Р»РёС†Сѓ
    /// </summary>
    Task<bool> AddPostAsync(CreatePostViewRequest Post);
    /// <summary>
    /// РџСЂРѕРІРµСЂСЏРµС‚ СЃСѓС‰РµСЃС‚РІРѕРІР°РЅРёРµ РєР»СЋС‡Р° РїРѕР»СЊР·РѕРІР°С‚РµР»СЏ
    /// </summary>
    /// <param name="Data">РРЅС„РѕСЂРјР°С†РёСЏ Рѕ РїРѕР»СЊР·РѕРІР°С‚РµР»Рµ</param>
    Task<bool> IsValidUserApiKeyAsync(UserApiKeyData Data);
    /// <summary>
    /// РџРѕР»СѓС‡Р°РµС‚ СЃРїРёСЃРѕРє РїРѕСЃС‚РѕРІ РїРѕР»СЊР·РѕРІР°С‚РµР»РµР№
    /// </summary>
    Task<List<PostDataResponse>> GetAllPostsAsync();
    /// <summary>
    /// РџРѕР»СѓС‡Р°РµС‚ СЃРїРёСЃРѕРє РїРѕСЃС‚РѕРІ РїРѕР»СЊР·РѕРІР°С‚РµР»РµР№
    /// </summary>
    /// <param name="lastUserId">РРґРµРЅС‚РёС„РёРєР°С‚РѕСЂ РєСЂР°Р№РЅРµРіРѕ РїРѕР»СѓС‡РµРЅРЅРѕРіРѕ РїРѕР»СЊР·РѕРІР°С‚РµР»СЏ</param>
    Task<DataPosts> GetAllPostsAsync(long lastUserId);
    /// <summary>
    /// РџРѕР»СѓС‡Р°РµС‚ СЃРїРёСЃРѕРє РїРѕСЃС‚РѕРІ РїРѕР»СЊР·РѕРІР°С‚РµР»РµР№
    /// </summary>
    /// <param name="id">РљСЂР°Р№РЅРёР№ РїРѕСЃС‚</param>
    /// <param name="status">СЃС‚Р°С‚СѓСЃ</param>
    /// <param name="scroll">СЃРєСЂРѕР»РёРЅРіРѕРј Р»Рё Р·Р°РіСЂСѓР·РєР° РёР»Рё С„РёР»СЊС‚СЂС‹</param>
    Task<DataPosts> GetAllPostsAsync(int id, int status, bool scroll = false);
    /// <summary>
    /// РЎРѕР·РґР°РЅРёРµ Р‘Р”
    /// </summary>
    Task RebuildAsync();
    /// <summary>
    /// РћР±РЅРѕРІР»СЏРµС‚ СЃС‚Р°С‚СѓСЃ Р·Р°СЏРІРєРё РїРѕР»СЊР·РѕРІР°С‚РµР»СЏ
    /// </summary>
    /// <param name="lastUserId">РРґРµРЅС‚РёС„РёРєР°С‚РѕСЂ РїРѕСЃС‚Р° РїРѕР»СЊР·РѕРІР°С‚РµР»СЏ</param>
    /// <param name="status">РЎС‚Р°С‚СѓСЃ РѕР±СЂР°Р±РѕС‚РєРё</param>
    /// <returns></returns>
    Task<bool> UpdateStatusPostAsync(long lastUserId, int status);
    /// <summary>
    /// РџСЂРѕРІРµСЂРєР° СЃСѓС‰РµСЃС‚РІРѕРІР°РЅРёСЏ С‚Р°Р±Р»РёС†С‹ РїРѕ РёРјРµРЅРё
    /// </summary>
    /// <param name="tableName">РРјСЏ С‚Р°Р±Р»РёС†С‹</param>
    /// <returns>bool</returns>
    Task<bool> TableExistsAsync(string tableName);

    /// <summary>
    /// РЎРѕС…СЂР°РЅРµРЅРёРµ/РѕР±РЅРѕРІР»РµРЅРёРµ РєРѕРјР°РЅРґС‹ РІ Р‘Р”
    /// </summary>
    Task SaveCommandTranslationsAsync(AdminCommandWithTranslations command);

    /// <summary>
    /// Р Р°Р·СЂСѓС€РёС‚РµР»СЊ СЃРѕРµРґРёРЅРµРЅРёСЏ Postgresql
    /// </summary>
    void Dispose();

}
