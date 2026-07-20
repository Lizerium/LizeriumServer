/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 20 июля 2026 12:29:51
 * Version: 1.0.114
 */

namespace LizeriumUtilities.FormatsData.AppHookCommandData;

public sealed class CommandIndexItem
{
    /// <summary>
    /// Категория команды
    /// </summary>
    public string Category { get; set; }
    /// <summary>
    /// Первое имя команды
    /// </summary>
    public string FirstName { get; set; }
    /// <summary>
    /// Id в доке
    /// </summary>
    public string Anchor { get; set; }
    /// <summary>
    /// Страница пагинации
    /// </summary>
    public int Page { get; set; }        
}

