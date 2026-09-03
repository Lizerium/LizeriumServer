/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 03 сентября 2026 07:38:14
 * Version: 1.0.165
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
