/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 16 сентября 2026 10:37:38
 * Version: 1.0.178
 */

namespace Api.LizeriumServer.Services.AdminAccess;

public class AdminAccessOptions
{
    public bool Enabled { get; set; }

    public List<string> TrustedIps { get; set; } = new();
}
