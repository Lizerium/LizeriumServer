/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 05 октября 2026 08:16:37
 * Version: 1.0.197
 */

using Microsoft.AspNetCore.Mvc;

namespace LizeriumServer.Controllers
{
    /// <summary>
    /// API форума
    /// </summary>
    [Route("Forum")]
    public class ForumController : Controller
    {
        /// <summary>
        /// Главная страница с разделами форума
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ForumMain()
        {
            return Redirect("/");
            //return View();
        }
    }
}
