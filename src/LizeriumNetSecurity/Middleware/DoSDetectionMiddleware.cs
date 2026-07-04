/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 04 июля 2026 08:59:42
 * Version: 1.0.98
 */

using System.Collections.Concurrent;

using LizeriumNetSecurity.FormatsData.AppCDNData;
using LizeriumNetSecurity.Services.SecurityService;

using Microsoft.AspNetCore.Http;

namespace LizeriumNetSecurity.Middleware
{
    public class DoSDetectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAppSecurityService _securityService;
        private readonly ConcurrentDictionary<string, RequestCounter> _requestCounters = new();

        private readonly TimeSpan _timeWindow = TimeSpan.FromSeconds(5);
        private readonly int _maxRequestsPerWindow = 100;

        public DoSDetectionMiddleware(RequestDelegate next, IAppSecurityService securityService)
        {
            _next = next;
            _securityService = securityService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            if (!string.IsNullOrEmpty(ip))
            {
                IncrementRequestCount(ip);

                if (IsIpOverLimit(ip))
                {
                    await _securityService.AddIpAsync(ip); // заблокировать IP
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests; // или 403
                    await context.Response.WriteAsync("Too many requests");
                    return;
                }
            }

            await _next(context);
        }

        private bool IsIpOverLimit(string ip)
        {
            if (!_requestCounters.TryGetValue(ip, out var counter))
                return false;

            lock (counter)
            {
                return counter.Count > _maxRequestsPerWindow;
            }
        }

        private void IncrementRequestCount(string ip)
        {
            var now = DateTime.UtcNow;
            var counter = _requestCounters.GetOrAdd(ip, _ => new RequestCounter { WindowStart = now, Count = 0 });

            lock (counter)
            {
                if (now - counter.WindowStart > _timeWindow)
                {
                    // Обнуляем счётчик по окончании временного окна
                    counter.Count = 1;
                    counter.WindowStart = now;
                }
                else
                {
                    counter.Count++;
                }
            }
        }
    }
}
