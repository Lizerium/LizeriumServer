/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 28 августа 2026 07:12:37
 * Version: 1.0.160
 */

namespace Api.LizeriumServer.Services.AdminAccess;

public class AdminAccessOptions
{
    public bool Enabled { get; set; }

    public List<string> TrustedIps { get; set; } = new();
}
