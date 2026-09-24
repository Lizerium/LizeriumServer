/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 24 сентября 2026 09:42:15
 * Version: 1.0.186
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
