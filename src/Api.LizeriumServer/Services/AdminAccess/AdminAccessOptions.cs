/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 25 сентября 2026 09:40:42
 * Version: 1.0.187
 */

namespace Api.LizeriumServer.Services.AdminAccess;

public class AdminAccessOptions
{
    public bool Enabled { get; set; }

    public List<string> TrustedIps { get; set; } = new();
}
