/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 19 мая 2026 10:36:19
 * Version: 1.0.53
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
