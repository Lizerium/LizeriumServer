/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 28 апреля 2026 14:44:11
 * Version: 1.0.33
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
