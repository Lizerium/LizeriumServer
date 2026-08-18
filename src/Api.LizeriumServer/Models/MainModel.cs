/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 18 августа 2026 07:14:22
 * Version: 1.0.149
 */

using Api.LizeriumServer.FormatsData.Stats;
using LizeriumUtilities.FormatsData.AppUserData;
using LizeriumUtilities.FormatsData.DataBase.Response;

namespace Api.LizeriumServer.Models;

/// <summary>
/// Объект модели для передачи во View
/// </summary>
public class MainModel
{
    /// <summary>
    /// Точка входа
    /// </summary>
    /// <param name="posts">Список постов</param>
    public MainModel(List<PostDataResponse> posts, List<CommandDataResponse> commands,
        List<AdminCommandWithTranslations> translationsCommand = null)
    {
        Posts = new DataPosts();
        Posts.Posts = new List<PostDataResponse>();
        if (posts != null)
        {
            Posts.Posts.AddRange(posts);
            Posts.LastUserId = Posts.Posts[^1].Id;
        }

        Commands = new DataCommands();
        Commands.Commands = new List<CommandDataResponse>();
        if (commands != null && commands.Count > 0)
        {
            Commands.Commands.AddRange(commands);
            Commands.LastId = Commands.Commands[^1].Id;
        }

        if (translationsCommand != null && translationsCommand.Count > 0)
            Commands.CommandTranslations.AddRange(translationsCommand);
    }

    /// <summary>
    /// Флаг отображения LeftSide
    /// </summary>
    public bool ShowLeftSide { get; init; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public long IdUser { get; init; }

    /// <summary>
    /// Данные статистики
    /// </summary>
    public List<MonitorData> MonitorData { get; set; }

    /// <summary>
    /// Почасовая статистика мониторинга за сутки
    /// </summary>
    public List<MonitorHourlyData> MonitorHourlyData { get; set; } = new();

    /// <summary>
    /// Данные пользователя
    /// </summary>
    public List<MonitorData> UserDataStats { get; set; }

    /// <summary>
    /// Количество посещений за день
    /// </summary>
    public int AllUsersPerDay { get; set; }

    public int AllVisitsPerDay { get; set; }

    public int HumanUsersPerDay { get; set; }

    public int BotUsersPerDay { get; set; }

    public int BotVisitsPerDay { get; set; }

    public int CurrentPage { get; set; } = 1;

    public int PageSize { get; set; } = 50;

    public int TotalPages { get; set; } = 1;

    public int TotalMonitorRows { get; set; }

    public int SelectedCommandStatus { get; set; } = 1;

    public string SelectedCommandCategory { get; set; } = "all";

    public List<string> CommandCategories { get; set; } = new();

    /// <summary>
    /// Новости Lizerium Steam.
    /// </summary>
    public List<LauncherNewsDataResponse> LauncherNews { get; set; } = new();

    public List<LauncherNewsTypeOption> LauncherNewsTypes { get; set; } = new();

    public string NewsSearch { get; set; } = string.Empty;

    public string NewsStatusFilter { get; set; } = "all";

    public int NewsCurrentPage { get; set; } = 1;

    public int NewsTotalPages { get; set; } = 1;

    public int NewsPageSize { get; set; } = 10;

    public int NewsTotalCount { get; set; }

    public string NewsPreviewCulture { get; set; } = "ru";

    public List<ProductCategoryDataResponse> ProductCatalog { get; set; } = new();

    /// <summary>
    /// Объект данных о пользователях
    /// </summary>
    public DataPosts Posts { get; init; }

    /// <summary>
    /// Объект данных
    /// </summary>
    public DataCommands Commands { get; init; }

    /// <summary>
    /// Версия API
    /// </summary>
    public string Version { get; init; }
}

public class LauncherNewsTypeOption
{
    public string Ru { get; set; } = string.Empty;

    public string En { get; set; } = string.Empty;
}
