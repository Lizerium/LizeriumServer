/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 23 мая 2026 11:33:14
 * Version: 1.0.57
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
