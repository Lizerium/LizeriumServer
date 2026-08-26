/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 26 августа 2026 07:14:21
 * Version: 1.0.158
 */

using LizeriumUtilities.FormatsData.DataBase.Response;

namespace LizeriumServer.Models;

/// <summary>
/// Модель страницы Lizerium Steam.
/// </summary>
public class LauncherViewModel
{
    /// <summary>
    /// Новости для вывода.
    /// </summary>
    public List<LauncherNewsDataResponse> News { get; set; } = new();

    public string Search { get; set; } = string.Empty;

    public string SortOrderFilter { get; set; } = "new";

    public string PlatformFilter { get; set; } = string.Empty;

    public string TypeFilter { get; set; } = string.Empty;

    public bool GithubFilter { get; set; }

    public List<string> NewsTypes { get; set; } = new();

    public int CurrentPage { get; set; } = 1;

    public int TotalPages { get; set; } = 1;

    public int PageSize { get; set; } = 7;

    public int TotalCount { get; set; }

    public int OpenNewsId { get; set; }
}
