/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 20 сентября 2026 08:50:06
 * Version: 1.0.182
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
