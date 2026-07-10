/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 10 июля 2026 12:16:48
 * Version: 1.0.104
 */

using LizeriumLogging.Accessories.LoggingAccessories;

using Microsoft.Extensions.Hosting;

namespace LizeriumNetSecurity.Services.SecurityService.Implements
{
    public class BackgroundBlacklistWriter : IHostedService
    {
        private Timer? _timer;
        private readonly IAppSecurityService _securityService;

        public BackgroundBlacklistWriter(IAppSecurityService securityService)
        {
            _securityService = securityService;
        }

        /// <summary>
        /// Фоновое обновление листа с заблокированными IP
        /// </summary>
        /// <param name="cancellationToken">Токе отмены операции</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _securityService.EnsureInitializedAsync();
            _timer = new Timer(async _ =>
            {
                try
                {
                    await _securityService.FlushAsync();
                }
                catch (Exception ex)
                {
                    ex.LogException();
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();
            return Task.CompletedTask;
        }
    }
}
