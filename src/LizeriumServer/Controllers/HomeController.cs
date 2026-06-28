/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 28 июня 2026 12:06:36
 * Version: 1.0.93
 */

using System.Diagnostics;

using AspNetCore.ReCaptcha;

using LizeriumDatabase.Services.AppDataBaseService;

using LizeriumServer.Models;

using LizeriumUtilities.FormatsData.DataBase.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LizeriumServer.Controllers
{
    /// <summary>
    /// Центральный контроллер
    /// </summary>
    [ValidateReCaptcha]
    public class HomeController : Controller
    {
        private IDataBaseService AppDb { get; set; }

        public HomeController(IDataBaseService dataBaseService)
        {
            AppDb = dataBaseService;
        }

        /// <summary>
        /// Главная страница сервера
        /// </summary>
        [HttpGet]
        [Route("/")]
        public async Task<IActionResult> Index()
        {
            // Проверяем, есть ли кука локали
            if (!Request.Cookies.ContainsKey(".AspNetCore.Culture"))
            {
                // Устанавливаем куку по умолчанию на "ru"
                var cultureValue = "c=ru|uic=ru";
                Response.Cookies.Append(
                    ".AspNetCore.Culture",
                    cultureValue,
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddYears(1),
                        IsEssential = true,
                        HttpOnly = false,
                        SameSite = SameSiteMode.Lax
                    }
                );
            }

            return View();
        }

        /// <summary>
        /// Страница технический работ сервера
        /// </summary>
        [HttpGet]
        [Route("/maintenance")]
        public async Task<IActionResult> Maintenance()
        {
            return View();
        }

        /// <summary>
        /// Главная страница загрузчика
        /// </summary>
        public async Task<IActionResult> Launcher()
        {
            return View();
        }

        /// <summary>
        /// Главная страница игры
        /// </summary>
        public async Task<IActionResult> Game()
        {
            return View();
        }

        /// <summary>
        /// Пожелания по игре
        /// </summary>
        public async Task<IActionResult> Wish()
        {
            //используем базу приложения
            var posts = await AppDb.GetAllPostsAsync();

            return View(new WishViewModel(posts));
        }

        /// <summary>
        /// Создание пожелания по игре
        /// </summary>
        /// <param name="PostModel">Данные пожелания</param>
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostViewRequest PostModel)
        {
            if (string.IsNullOrEmpty(PostModel.Autor) || string.IsNullOrEmpty(PostModel.Message)) return RedirectToAction("Index");

            //используем базу приложения
            PostModel.Status = -1;
            await AppDb.AddPostAsync(PostModel);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Пересоздание базы данных
        /// </summary>
        /// <param name="PostModel">Данные</param>
        [HttpGet]
        [Route("rebuild")]
        public async Task<IActionResult> Rebuild([FromForm] CreatePostViewRequest PostModel)
        {
            //используем базу приложения
            await AppDb.RebuildAsync();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Страница приватности
        /// </summary>
        public async Task<IActionResult> Privacy()
        {
            return View();
        }

        /// <summary>
        /// Страница ошибки
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            HttpContext.Response.Cookies.Delete(".Aws.Session");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
