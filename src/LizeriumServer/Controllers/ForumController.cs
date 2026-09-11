/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 11 сентября 2026 06:57:25
 * Version: 1.0.173
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
