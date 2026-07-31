/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 31 июля 2026 16:48:21
 * Version: 1.0.127
 */

using LizeriumUtilities.FormatsData.DataBase.Response;

namespace LizeriumServer.Models;

/// <summary>
/// РњРѕРґРµР»СЊ СЃС‚СЂР°РЅРёС†С‹ Lizerium Launcher.
/// </summary>
public class LauncherViewModel
{
    /// <summary>
    /// РќРѕРІРѕСЃС‚Рё РґР»СЏ РІС‹РІРѕРґР°.
    /// </summary>
    public List<LauncherNewsDataResponse> News { get; set; } = new();

    public string Search { get; set; } = string.Empty;

    public string SortOrderFilter { get; set; } = "new";

    public int CurrentPage { get; set; } = 1;

    public int TotalPages { get; set; } = 1;

    public int PageSize { get; set; } = 6;

    public int TotalCount { get; set; }
}
