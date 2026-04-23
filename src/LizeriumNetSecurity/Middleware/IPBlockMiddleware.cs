/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 23 апреля 2026 07:07:59
 * Version: 1.0.28
 */

using LizeriumNetSecurity.Services.SecurityService;

using Microsoft.AspNetCore.Http;

namespace LizeriumUtilities.Middleware
{
    public class IpBlockMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAppSecurityService _securityService;

        public IpBlockMiddleware(RequestDelegate next, IAppSecurityService securityService)
        {
            _next = next;
            _securityService = securityService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            if (!string.IsNullOrEmpty(ip))
            {
                if (await _securityService.IsBlocked(ip))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("You are blocked. Access denied");
                    return; // Не вызываем следующий middleware, блокируем запрос
                }
            }

            // IP не заблокирован, передаём запрос дальше
            await _next(context);
        }
    }
}
