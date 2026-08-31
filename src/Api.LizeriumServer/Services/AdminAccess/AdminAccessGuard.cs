/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 31 августа 2026 07:09:02
 * Version: 1.0.162
 */

using System.Text.Json;

namespace Api.LizeriumServer.Services.AdminAccess;

public static class AdminAccessGuard
{
    private const string ConfigFileName = "admin_access.json";

    public static bool IsAllowed(HttpContext httpContext)
    {
        var options = ReadOptions();
        if (!options.Enabled)
            return true;

        var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrWhiteSpace(remoteIp))
            return false;

        return options.TrustedIps.Any(ip => string.Equals(ip, remoteIp, StringComparison.OrdinalIgnoreCase));
    }

    private static AdminAccessOptions ReadOptions()
    {
        var path = Path.Combine(AppContext.BaseDirectory, ConfigFileName);
        if (!File.Exists(path))
            path = Path.Combine(Directory.GetCurrentDirectory(), ConfigFileName);

        if (!File.Exists(path))
            return new AdminAccessOptions();

        try
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<AdminAccessOptions>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new AdminAccessOptions();
        }
        catch
        {
            return new AdminAccessOptions();
        }
    }
}
