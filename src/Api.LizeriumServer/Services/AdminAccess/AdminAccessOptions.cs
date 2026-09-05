/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 05 сентября 2026 08:57:17
 * Version: 1.0.167
 */

namespace Api.LizeriumServer.Services.AdminAccess;

public class AdminAccessOptions
{
    public bool Enabled { get; set; }

    public List<string> TrustedIps { get; set; } = new();
}
