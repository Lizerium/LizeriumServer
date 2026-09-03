/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 03 сентября 2026 07:38:14
 * Version: 1.0.165
 */

namespace Api.LizeriumServer.Services.AdminAccess;

public class AdminAccessOptions
{
    public bool Enabled { get; set; }

    public List<string> TrustedIps { get; set; } = new();
}
