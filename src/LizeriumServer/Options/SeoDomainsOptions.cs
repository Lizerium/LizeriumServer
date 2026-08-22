/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 22 августа 2026 15:02:14
 * Version: 1.0.153
 */

using Microsoft.AspNetCore.Http;

namespace LizeriumServer.Options
{
    public class SeoDomainsOptions
    {
        public string PrimaryDomain { get; set; } = "lizerium.com";
        public string[] Domains { get; set; } = new[] { "lizerium.com", "lizup.ru" };
        public string Scheme { get; set; } = "https";
        public string CanonicalMode { get; set; } = "RequestHost";
        public string OpenGraphImage { get; set; } = "/img/Main.png";
        public string SiteName { get; set; } = "Lizerium";

        public string GetBaseUrl(HttpRequest request)
        {
            var requestHost = GetPublicHost(request);
            if (IsLocalHost(requestHost))
                return $"{request.Scheme}://{request.Host}";

            var domain = string.Equals(CanonicalMode, "PrimaryDomain", StringComparison.OrdinalIgnoreCase)
                ? PrimaryDomain
                : GetConfiguredDomain(requestHost) ?? PrimaryDomain;

            return $"{Scheme}://{NormalizeHost(domain)}";
        }

        public string GetRequestHostBaseUrl(HttpRequest request)
        {
            var requestHost = GetPublicHost(request);
            if (IsLocalHost(requestHost))
                return $"{request.Scheme}://{request.Host}";

            var domain = GetConfiguredDomain(requestHost) ?? PrimaryDomain;
            return $"{Scheme}://{NormalizeHost(domain)}";
        }

        public string GetPrimaryBaseUrl()
            => $"{Scheme}://{NormalizeHost(PrimaryDomain)}";

        public string GetOpenGraphImageUrl(HttpRequest request)
            => $"{GetBaseUrl(request)}{NormalizePath(OpenGraphImage)}";

        private static string NormalizeHost(string host)
            => (host ?? string.Empty).Trim().TrimEnd('/').ToLowerInvariant();

        private string GetConfiguredDomain(string host)
            => Domains.FirstOrDefault(domain =>
                string.Equals(NormalizeHost(domain), NormalizeHost(host), StringComparison.OrdinalIgnoreCase));

        private static string GetPublicHost(HttpRequest request)
        {
            var forwardedHost = request.Headers["X-Forwarded-Host"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwardedHost))
                return forwardedHost.Split(',')[0].Trim();

            var originalHost = request.Headers["X-Original-Host"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(originalHost))
                return originalHost.Split(',')[0].Trim();

            return request.Host.Host;
        }

        private static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return "/img/Main.png";

            return path.StartsWith('/') ? path : "/" + path;
        }

        private static bool IsLocalHost(string host)
            => string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase)
               || string.Equals(host, "127.0.0.1", StringComparison.OrdinalIgnoreCase)
               || string.Equals(host, "0.0.0.0", StringComparison.OrdinalIgnoreCase);
    }
}
