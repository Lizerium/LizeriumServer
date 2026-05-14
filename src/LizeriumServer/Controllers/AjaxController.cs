/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 14 мая 2026 10:55:37
 * Version: 1.0.49
 */

using System.Reflection;

using LizeriumDatabase.Services.AppDataBaseService;

using LizeriumLogging.Accessories.LoggingAccessories;

using LizeriumUtilities.Accessories.JsonAccessories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace LizeriumServer.Controllers;

/// <summary>
/// Контроллер Ajax запросов с сайта
/// </summary>
[Route("[controller]/[action]")]
public class AjaxController : Controller
{
    private IDataBaseService appDb { get; set; }
    private readonly IStringLocalizerFactory _localizerFactory;

    public AjaxController(IDataBaseService dataBaseService,
        IStringLocalizerFactory localizerFactory)
    {
        appDb = dataBaseService;
        _localizerFactory = localizerFactory;
    }

    /// <summary>
    /// Метод получает все локализованные строки для указанного ресурса,
    /// согласно текущей культуре пользователя.
    /// </summary>
    /// <param name="resourceName">Полное имя ресурса (например, "Views.Home.Wish")</param>
    /// <returns>JSON словарь: ключ-значение локализованных строк</returns>
    [HttpPost]
    [Route("{resourceName}")]
    [Produces("application/json")]
    [ValidateAntiForgeryToken]
    public IActionResult GetAllLocalizedStrings(string resourceName)
    {
        if (string.IsNullOrWhiteSpace(resourceName))
            return BadRequest(new { error = "Resource name is required." });

        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

        try
        {
            var localizer = _localizerFactory.Create(resourceName, assemblyName);

            var allStrings = localizer.GetAllStrings(includeParentCultures: true)
                .ToDictionary(s => s.Name, s => s.Value);

            if (!allStrings.Any())
                return NotFound(new { error = $"No strings found in resource '{resourceName}'." });

            return allStrings.SuccessResponse();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = "Error loading localization resource.",
                details = ex.Message
            });
        }
    }

    /// <summary>
    /// Метод отдает данные постов пользователей в админку
    /// </summary>
    /// <param name="lastUserId">Идентификатор крайнего пользователя</param>
    /// <param name="status">Статус</param>
    /// <returns>Результат действия</returns>
    [HttpPost]
    [Route("{lastUserId:long}/{status:int}/{scroll:int}")]
    [Produces("application/json")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetPosts(long lastUserId, int status, int scroll)
    {
        try
        {
            //получаем данные постов пользователей
            var dataUsers = await appDb.GetAllPostsAsync((int)lastUserId, status, (scroll == 0) ? false : true);

            //проверяем данные постов пользователей
            if (dataUsers == null) return BadRequest("error get users");

            if (dataUsers.Posts.Count <= 0) dataUsers.LastUserId = 0;
            else dataUsers.LastUserId = dataUsers.Posts[^1].Id;

            //отдаем данные постов пользователей
            return dataUsers.SuccessResponse();
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем BadRequest
            return BadRequest("failed");
        }
    }

    /// <summary>
    /// Поиск команд по имени
    /// </summary>
    /// <param name="query">Поисковая строка (имя команды)</param>
    /// <returns>Список подходящих команд</returns>
    [HttpPost]
    [Route("{query}")]
    [Produces("application/json")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SearchCommands(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Ok(new List<string>());

        var result = await appDb.SearchCommandsAsync(query.Trim());

        return result.SuccessResponse();
    }
}
