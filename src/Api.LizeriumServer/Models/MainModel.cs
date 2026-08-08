/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 08 августа 2026 07:13:54
 * Version: 1.0.134
 */

using Api.LizeriumServer.FormatsData.Stats;
using LizeriumUtilities.FormatsData.AppUserData;
using LizeriumUtilities.FormatsData.DataBase.Response;

namespace Api.LizeriumServer.Models;

/// <summary>
/// РћР±СЉРµРєС‚ РјРѕРґРµР»Рё РґР»СЏ РїРµСЂРµРґР°С‡Рё РІРѕ View
/// </summary>
public class MainModel
{
    /// <summary>
    /// РўРѕС‡РєР° РІС…РѕРґР°
    /// </summary>
    /// <param name="posts">РЎРїРёСЃРѕРє РїРѕСЃС‚РѕРІ</param>
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
    /// Р¤Р»Р°Рі РѕС‚РѕР±СЂР°Р¶РµРЅРёСЏ LeftSide
    /// </summary>
    public bool ShowLeftSide { get; init; }

    /// <summary>
    /// РРґРµРЅС‚РёС„РёРєР°С‚РѕСЂ РїРѕР»СЊР·РѕРІР°С‚РµР»СЏ
    /// </summary>
    public long IdUser { get; init; }

    /// <summary>
    /// Р”Р°РЅРЅС‹Рµ СЃС‚Р°С‚РёСЃС‚РёРєРё
    /// </summary>
    public List<MonitorData> MonitorData { get; set; }

    /// <summary>
    /// РџРѕС‡Р°СЃРѕРІР°СЏ СЃС‚Р°С‚РёСЃС‚РёРєР° РјРѕРЅРёС‚РѕСЂРёРЅРіР° Р·Р° СЃСѓС‚РєРё
    /// </summary>
    public List<MonitorHourlyData> MonitorHourlyData { get; set; } = new();

    /// <summary>
    /// Р”Р°РЅРЅС‹Рµ РїРѕР»СЊР·РѕРІР°С‚РµР»СЏ
    /// </summary>
    public List<MonitorData> UserDataStats { get; set; }

    /// <summary>
    /// РљРѕР»РёС‡РµСЃС‚РІРѕ РїРѕСЃРµС‰РµРЅРёР№ Р·Р° РґРµРЅСЊ
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
    /// РќРѕРІРѕСЃС‚Рё Lizerium Steam.
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
    /// РћР±СЉРµРєС‚ РґР°РЅРЅС‹С… Рѕ РїРѕР»СЊР·РѕРІР°С‚РµР»СЏС…
    /// </summary>
    public DataPosts Posts { get; init; }

    /// <summary>
    /// РћР±СЉРµРєС‚ РґР°РЅРЅС‹С…
    /// </summary>
    public DataCommands Commands { get; init; }

    /// <summary>
    /// Р’РµСЂСЃРёСЏ API
    /// </summary>
    public string Version { get; init; }
}

public class LauncherNewsTypeOption
{
    public string Ru { get; set; } = string.Empty;

    public string En { get; set; } = string.Empty;
}
