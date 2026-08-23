/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 23 августа 2026 07:14:40
 * Version: 1.0.154
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
    /// База данных
    /// </summary>
    DatabaseFacade Database { get; }
    /// <summary>
    /// Инициализация таблицы постов
    /// </summary>
    DbSet<PostDataResponse> Posts { get; set; }
    /// <summary>
    /// Инициализация таблицы пользователей
    /// </summary>
    DbSet<UserApiKeyResponse> Users { get; set; }
    /// <summary>
    /// Инициализация таблицы команд
    /// </summary>
    DbSet<CommandDataResponse> Commands { get; set; }

    /// <summary>
    /// Инициализация таблицы новостей Lizerium Steam.
    /// </summary>
    DbSet<LauncherNewsDataResponse> LauncherNews { get; set; }

    /// <summary>
    /// Инициализация таблицы категорий продуктов.
    /// </summary>
    DbSet<ProductCategoryDataResponse> ProductCategories { get; set; }

    /// <summary>
    /// Инициализация таблицы продуктов.
    /// </summary>
    DbSet<ProductDataResponse> Products { get; set; }

    /// <summary>
    /// Инициализация таблицы источников скачивания продуктов.
    /// </summary>
    DbSet<ProductDownloadLinkDataResponse> ProductDownloadLinks { get; set; }


    /// <summary>
    /// Сохранение изменений в базу данных
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns></returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Проверка существования таблиц и генерация их
    /// </summary>
    Task ExistAndCreateCommandsTable();
    /// <summary>
    /// Получает количество команд в категории
    /// </summary>
    /// <param name="Category">Категория имя</param>
    /// <param name="checkSecureOperate">Проверять ли существование таблицы</param>
    /// <returns>int</returns>
    Task<int> GetCommandsCountAsync(string Category, bool checkSecureOperate = true);
    /// <summary>
    /// Получает все команды по категории
    /// </summary>
    /// <param name="Category">Категория имя</param>
    /// <param name="Page">Страница</param>
    /// <param name="Size">Количество</param>
    /// <param name="checkSecureOperate">Проверять ли существование таблицы</param>
    /// <param name="shortSize">Ограничения включены или выключены</param>
    /// <returns></returns>
    Task<List<CommandDataResponse>> GetCommandsAsync(string Category, int Page = 1, int Size = 10, bool checkSecureOperate = true,
        bool shortSize = true);
    /// <summary>
    /// Получает список переводов команд
    /// </summary>
    Task<List<AdminCommandWithTranslations>> GetAllAdminCommandTranslatesAsync(bool checkSecureOperate = true);
    /// <summary>
    /// Получает список команд
    /// </summary>
    Task<List<CommandDataResponse>> GetAllAdminCommandsAsync(bool checkSecureOperate = true);
    /// <summary>
    /// Получает список постов пользователей
    /// </summary>
    Task<List<PostDataResponse>> GetAllAdminPostsAsync();
    /// <summary>
    /// Поиск по командам
    /// </summary>
    /// <param name="query">Запрос</param>
    /// <returns>Список команд</returns>
    Task<List<CommandDataResponse>> SearchCommandsAsync(string query, bool checkSecureOperate = true);
    /// <summary>
    /// Получение списка команд по категории
    /// </summary>
    /// <param name="checkSecureOperate">Проверять ли существование таблицы</param>
    /// <returns>List<string></returns>
    Task<List<CommandCategoryInfoResponse>> GetAllCommandCategoriesAsync(bool checkSecureOperate = true);
    /// <summary>
    /// Сохраняет сразу пачку команд в базе данных
    /// </summary>
    /// <param name="jsonData">Считанный набор команд отсортированный по категориям</param>
    Task SaveCommandsFromJsonAsync(CommandsFileRequest jsonData, bool checkSecureOperate = true);
    /// <summary>
    /// Сохраняет сразу пачку категорий команд в базе данных
    /// </summary>
    /// <param name="jsonData">Считанный набор категорий команд отсортированный по категориям</param>
    Task SaveCategoriesCommandsFromJsonAsync(CommandsFileRequest jsonData, bool checkSecureOperate = true);
    /// <summary>
    /// Сохранить команду с GIF ссылкой на файл
    /// </summary>
    /// <param name="fileName">Имя GIF файла</param>
    /// <param name="id">Идентификатор команды</param>
    /// <param name="checkSecureOperate">Проверять ли существование таблицы</param>
    /// <returns>bool</returns>
    Task<bool> SaveGifCommandAsync(string fileName, string id, bool checkSecureOperate = true);
    /// <summary>
    /// Изменить команду
    /// </summary>
    Task<bool> ChangeCommandAsync(CreateCommandViewRequest Command, bool checkSecureOperate = true);
    /// <summary>
    /// Создать команду
    /// </summary>
    Task<bool> AddCommandAsync(CreateCommandViewRequest Command, bool checkSecureOperate = true);
    /// <summary>
    /// Удалить команду и её переводы
    /// </summary>
    Task<bool> DeleteCommandAndTranslationsAsync(CreateCommandViewRequest Command, bool checkSecureOperate = true);
    /// <summary>
    /// Сохранение всех переводов в БД
    /// </summary>
    /// <param name="commandTranslations">Список DTO переводов</param>
    /// <returns>Статус операции</returns>
    Task<bool> SaveAllCommandsTranslationsAsync(List<CommandTranslationResponse> commandTranslations);
    /// <summary>
    /// Получить команды без перевода.
    /// </summary>
    /// <param name="toLang">en</param>
    /// <returns>Список команд</returns>
    Task<List<CommandTranslationResponse>> GetCommandsMissingTranslationAsync(string toLang);
    /// <summary>
    /// Добавление категории команд (информации о ней)
    /// </summary>
    Task<bool> AddCategoryAsync(CategoriesCommands category, bool checkSecureOperate = true);

    /// <summary>
    /// Проверяет существование таблицы новостей и создает ее при необходимости.
    /// </summary>
    Task ExistAndCreateLauncherNewsTable();

    /// <summary>
    /// Получает опубликованные новости Lizerium Steam.
    /// </summary>
    Task<List<LauncherNewsDataResponse>> GetPublishedLauncherNewsAsync(bool checkSecureOperate = true);

    /// <summary>
    /// Получает все новости для админки.
    /// </summary>
    Task<List<LauncherNewsDataResponse>> GetAllAdminLauncherNewsAsync(bool checkSecureOperate = true);

    /// <summary>
    /// Получает новость для админского предпросмотра без фильтра публикации.
    /// </summary>
    Task<LauncherNewsDataResponse> GetAdminLauncherNewsByIdAsync(int id, bool checkSecureOperate = true);

    /// <summary>
    /// Добавляет или обновляет новость.
    /// </summary>
    Task<bool> SaveLauncherNewsAsync(LauncherNewsDataResponse news, bool checkSecureOperate = true);

    /// <summary>
    /// Удаляет новость.
    /// </summary>
    Task<bool> DeleteLauncherNewsAsync(int id, bool checkSecureOperate = true);

    /// <summary>
    /// Increments public like counter for a launcher news item.
    /// </summary>
    Task<int?> IncrementLauncherNewsLikeAsync(int id, bool checkSecureOperate = true);

    /// <summary>
    /// Проверяет существование продуктовых таблиц и создает их при необходимости.
    /// </summary>
    Task ExistAndCreateProductsTables();

    /// <summary>
    /// Получает опубликованный каталог продуктов для публичной витрины.
    /// </summary>
    Task<List<ProductCategoryDataResponse>> GetPublishedProductCatalogAsync(bool checkSecureOperate = true);

    /// <summary>
    /// Получает полный каталог продуктов для админки.
    /// </summary>
    Task<List<ProductCategoryDataResponse>> GetAllAdminProductCatalogAsync(bool checkSecureOperate = true);

    /// <summary>
    /// Добавляет или обновляет категорию продуктов.
    /// </summary>
    Task<bool> SaveProductCategoryAsync(ProductCategoryDataResponse category, bool checkSecureOperate = true);

    /// <summary>
    /// Удаляет категорию продуктов.
    /// </summary>
    Task<bool> DeleteProductCategoryAsync(int id, bool checkSecureOperate = true);

    /// <summary>
    /// Добавляет или обновляет продукт.
    /// </summary>
    Task<bool> SaveProductAsync(ProductDataResponse product, bool checkSecureOperate = true);

    /// <summary>
    /// Удаляет продукт.
    /// </summary>
    Task<bool> DeleteProductAsync(int id, bool checkSecureOperate = true);

    /// <summary>
    /// Добавляет или обновляет источник скачивания продукта.
    /// </summary>
    Task<bool> SaveProductDownloadLinkAsync(ProductDownloadLinkDataResponse link, bool checkSecureOperate = true);

    /// <summary>
    /// Удаляет источник скачивания продукта.
    /// </summary>
    Task<bool> DeleteProductDownloadLinkAsync(int id, bool checkSecureOperate = true);
    /// <summary>
    /// Генерировать базовую таблицу
    /// </summary>
    Task<bool> AddPostAsync(CreatePostViewRequest Post);
    /// <summary>
    /// Проверяет существование ключа пользователя
    /// </summary>
    /// <param name="Data">Информация о пользователе</param>
    Task<bool> IsValidUserApiKeyAsync(UserApiKeyData Data);
    /// <summary>
    /// Получает список постов пользователей
    /// </summary>
    Task<List<PostDataResponse>> GetAllPostsAsync();
    /// <summary>
    /// Получает список постов пользователей
    /// </summary>
    /// <param name="lastUserId">Идентификатор крайнего полученного пользователя</param>
    Task<DataPosts> GetAllPostsAsync(long lastUserId);
    /// <summary>
    /// Получает список постов пользователей
    /// </summary>
    /// <param name="id">Крайний пост</param>
    /// <param name="status">статус</param>
    /// <param name="scroll">скролингом ли загрузка или фильтры</param>
    Task<DataPosts> GetAllPostsAsync(int id, int status, bool scroll = false);
    /// <summary>
    /// Создание БД
    /// </summary>
    Task RebuildAsync();
    /// <summary>
    /// Обновляет статус заявки пользователя
    /// </summary>
    /// <param name="lastUserId">Идентификатор поста пользователя</param>
    /// <param name="status">Статус обработки</param>
    /// <returns></returns>
    Task<bool> UpdateStatusPostAsync(long lastUserId, int status);
    /// <summary>
    /// Проверка существования таблицы по имени
    /// </summary>
    /// <param name="tableName">Имя таблицы</param>
    /// <returns>bool</returns>
    Task<bool> TableExistsAsync(string tableName);

    /// <summary>
    /// Сохранение/обновление команды в БД
    /// </summary>
    Task SaveCommandTranslationsAsync(AdminCommandWithTranslations command);

    /// <summary>
    /// Разрушитель соединения Postgresql
    /// </summary>
    void Dispose();

}
