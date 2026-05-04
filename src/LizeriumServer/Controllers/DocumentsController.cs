/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 04 мая 2026 07:13:32
 * Version: 1.0.39
 */

using System.Text.Json;

using LizeriumDatabase.Services.AppDataBaseService;

using LizeriumServer.Models;
using LizeriumServer.Options;

using LizeriumUtilities.FormatsData.AppHookCommandData;
using LizeriumUtilities.FormatsData.DataBase.Response;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using FileIO = System.IO.File;

namespace LizeriumServer.Controllers
{
    [Route("docs")]
    public class DocumentsController : Controller
    {
        private IDataBaseService AppDb { get; set; }
        private IWebHostEnvironment environments;
        private readonly IMemoryCache _cache;
        private readonly StoragePathsOptions _storagePaths;

        public DocumentsController(IDataBaseService dataBaseService, IMemoryCache cache,
            IWebHostEnvironment environment,
            IOptions<StoragePathsOptions> storagePathsOptions)
        {
            AppDb = dataBaseService;
            environments = environment;
            _cache = cache;
            _storagePaths = storagePathsOptions.Value;
        }

        /// <summary>
        /// Страница документации
        /// </summary>
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> Documents()
        {
            return View();
        }

        /// <summary>
        /// Страница документации по установке
        /// </summary>
        [HttpGet]
        [Route("install")]
        public async Task<IActionResult> DocumentsInstall()
        {
            return View();
        }

        [HttpGet]
        [Route("build/{buildName}")]
        public async Task<IActionResult> GetBuildComponents(string buildName)
        {
            BuildComponent buildComponent = null;

            if (_cache.TryGetValue("ListBuilds", out List<BuildsComponent> listBuilds))
            {
                buildComponent = listBuilds.SelectMany(it => it.Components)
                    .Where(it => it.Nickname == buildName).FirstOrDefault();
            }

            if (buildComponent == null) return NotFound();

            return PartialView("BuildComponentView", buildComponent.Components);
        }

        /// <summary>
        /// Страница документации по крафту
        /// </summary>
        [HttpGet]
        [Route("builds")]
        public async Task<IActionResult> DocumentsBuild()
        {
            try
            {
                // Проверка кеша
                if (!_cache.TryGetValue("ListBuilds", out List<BuildsComponent> listBuilds))
                {
                    var searchPath = Path.Combine(_storagePaths.GameServerConfigs, "BUILDS");

                    if (!Directory.Exists(searchPath)) return Ok();

                    var paths = new[]
                    {
                        Path.Combine(searchPath, "craft_builda.json"),
                        Path.Combine(searchPath, "craft_builde.json"),
                        Path.Combine(searchPath, "craft_buildl.json"),
                        Path.Combine(searchPath, "craft_buildw.json"),
                    };

                    var builds = new List<BuildsComponent>();

                    foreach (var path in paths)
                    {
                        if (!FileIO.Exists(path)) continue;
                        var json = FileIO.ReadAllText(path);
                        var parsed = JsonSerializer.Deserialize<BuildsComponent>(json);
                        if (parsed != null)
                            builds.Add(parsed);
                    }

                    listBuilds = builds;

                    // Кешируем данные на 30 минут
                    _cache.Set("ListBuilds", listBuilds, TimeSpan.FromMinutes(30));
                }

                return View(new DocumentViewModel()
                {
                    ListBuilds = listBuilds
                });
            }
            catch
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Страница списка команд
        /// </summary>
        [HttpGet]
        [Route("hook")]
        public async Task<IActionResult> DocumentsHook()
        {
            var CategoriesHook = await AppDb.GetAllCommandCategoriesAsync();
            return View(new DocumentViewModel(null, "", CategoriesHook));
        }


        [HttpGet]
        [Route("hook/{CategoryHook}")]
        public async Task<IActionResult> DocHook(string CategoryHook, [FromQuery] int page = 1)
        {
            var pageSize = 6;
            var commands = await AppDb.GetCommandsAsync(CategoryHook, page, pageSize);
            var CategoriesHook = await AppDb.GetAllCommandCategoriesAsync();
            var counts = new Dictionary<string, int>();
            counts[CategoryHook] = await AppDb.GetCommandsCountAsync(CategoryHook);

            var model = new DocumentViewModel
            {
                Commands = commands,
                CategoriesHook = CategoriesHook,
                Category = CategoryHook,
                CommandsCount = counts,
                PageSize = pageSize,
                Page = page
            };
            return View(model);
        }

        [HttpGet("hook/{category}/index")]
        [Produces("application/json")]
        public async Task<IActionResult> GetCategoryIndex(string category)
        {
            const int pageSize = 6;

            // ВСЕ команды категории в правильном порядке
            var all = await AppDb.GetCommandsAsync(category, 1, 10, true, false);

            var index = new List<CommandIndexItem>(all.Count);
            for (int i = 0; i < all.Count; i++)
            {
                var cmd = all[i];
                var first = (cmd.CommandNamesList?.FirstOrDefault() ?? cmd.CommandNames.Split(',')[0]).Trim();
                var anchor = ToAnchorId(first);

                var page = (i / pageSize) + 1;

                index.Add(new CommandIndexItem
                {
                    Category = category,
                    FirstName = first,
                    Anchor = anchor,
                    Page = page
                });
            }

           return Ok(index);
        }

        // Простейшая очистка для id
        private static string ToAnchorId(string command)
            => command.Replace("/", "").Replace(" ", "-").ToLowerInvariant();

        [HttpGet]
        [Route("afk")]
        public async Task<IActionResult> DocAfk()
        {
            var commands = await AppDb.GetCommandsAsync("AFK");

            return View(new DocumentViewModel(commands, "AFK"));
        }

        [HttpGet]
        [Route("alley")]
        public async Task<IActionResult> DocAlley()
        {
            var commands = await AppDb.GetCommandsAsync("ALLEY");

            return View(new DocumentViewModel(commands, "AFK"));
        }
    }
}
