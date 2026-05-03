/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 03 мая 2026 07:12:11
 * Version: 1.0.38
 */

namespace LizeriumNetSecurity.Services.SecurityService;

public interface IAppSecurityService
{
    Task<bool> IsBlocked(string ip);
    Task AddIpAsync(string ip);
    Task RemoveIpAsync(string ip);
    Task ReloadAsync();
    Task FlushAsync();
    Task EnsureInitializedAsync();
}
