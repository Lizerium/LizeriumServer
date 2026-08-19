/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 19 августа 2026 10:43:54
 * Version: 1.0.150
 */

using System.Text.Json.Serialization;

namespace LizeriumUtilities.FormatsData.DataBase.Requests;

[Serializable]
public class CommandInfo
{
    [JsonPropertyName("name")]
    public string Commands { get; set; }
    [JsonPropertyName("desc")]
    public string Description { get; set; }
    [JsonPropertyName("ex")]
    public string Example { get; set; }
    [JsonPropertyName("status")]
    public int Status { get; set; }
}

[Serializable]
public class CategoryItem
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("commands")]
    public List<CommandInfo> Commands { get; set; }
}

[Serializable]
public class CategoriesCommands
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("version")]
    public string Version { get; set; }
    [JsonPropertyName("url_repo")]
    public string Repository { get; set; }
    [JsonPropertyName("title")]
    public List<Language> Title { get; set; }
}

[Serializable]
public class Language
{
    [JsonPropertyName("ru")]
    public string Russian { get; set; }
    [JsonPropertyName("en")]
    public string English { get; set; }
}

[Serializable]
public class LanguageArray
{
    [JsonPropertyName("ru")]
    public string[] Russian { get; set; }
    [JsonPropertyName("en")]
    public string[] English { get; set; }
}

[Serializable]
public class CommandsFileRequest
{
    [JsonPropertyName("count")]
    public int Count { get; set; }
    [JsonPropertyName("categories")]
    public List<CategoriesCommands> Categories { get; set; }

    [JsonPropertyName("data")]
    public List<CategoryItem> Data { get; set; }
}