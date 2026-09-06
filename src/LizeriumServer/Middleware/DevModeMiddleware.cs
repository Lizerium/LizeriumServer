/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 06 сентября 2026 11:13:26
 * Version: 1.0.168
 */

using LizeriumUtilities.Services.DevelopService;

namespace LizeriumServer.Middleware
{
    /// <summary>
    /// Управляет разрешениями на открытие страниц сайта
    /// </summary>
    public class DevModeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly DevModeService _devModeService;

        public DevModeMiddleware(RequestDelegate next, DevModeService devModeService)
        {
            _next = next;
            _devModeService = devModeService;
        }

        public async Task Invoke(HttpContext context)
        {
            if (_devModeService.IsDevelopMode)
            {
                var path = context.Request.Path.Value ?? string.Empty;

                // Разрешаем внутреннее API и статику, например
                if (!path.StartsWith("/maintenance") 
                    && !path.StartsWith("/css") 
                    && !path.StartsWith("/Culture")
                    && !path.StartsWith("/js"))
                {
                    if(_devModeService.IsUpdaterState)
                    {
                        // разрешаем работать загрузчику если включена его работа на сервере
                        if (!path.StartsWith("/uploader"))
                        {
                            context.Response.Redirect("/maintenance");
                            return;
                        }
                        else
                        {
                            // проверяем для кого загрузчик работает для разработчика или всех
                            if(_devModeService.IsUpdaterDevMode) // режим разработчика ограниченное скачивание
                            {
                                var whiteList = _devModeService.UpdaterWhiteList ?? new List<string>();
                                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                                if(string.IsNullOrWhiteSpace(ipAddress) || !whiteList.Contains(ipAddress))
                                {
                                    context.Response.Redirect("/maintenance");
                                    return;
                                }
                                else { /*тогда разрешено скачивать всем*/ }
                            }
                        }
                    }
                    else
                    {
                        context.Response.Redirect("/maintenance");
                        return;
                    }
                }
            }
            else
            {
                var path = context.Request.Path.Value ?? string.Empty;
                if (path == "/maintenance")
                {
                    context.Response.Redirect("/");
                    return;
                }
            }

            await _next(context);
        }
    }
}
