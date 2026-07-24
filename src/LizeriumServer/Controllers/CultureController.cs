/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 24 июля 2026 11:59:29
 * Version: 1.0.118
 */

using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace LizeriumServer.Controllers
{
    /// <summary>
    /// Контроллер языковых настроек
    /// </summary>
    public class CultureController : Controller
    {
        /// <summary>
        /// Установка языка сервера
        /// </summary>
        /// <param name="culture">Язык</param>
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(culture))
                culture = "ru";

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                });

            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                returnUrl = Url.Action("Index", "Home");

            // Заменяем старую культуру в пути на новую
            var segments = returnUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length > 1 && (segments.Contains("ru") || segments.Contains("en")))
            {
                int index = Array.FindIndex(segments, s => s == "ru" || s == "en");
                if (index >= 0)
                {
                    segments[index] = culture; // заменяем на выбранную локаль
                    returnUrl = "/" + string.Join('/', segments);
                }
            }

            return LocalRedirect(returnUrl);
        }
    }
}
