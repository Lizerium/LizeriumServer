/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 31 июля 2026 16:48:21
 * Version: 1.0.127
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
            var requestHost = request.Host.Host;
            if (IsLocalHost(requestHost))
                return $"{request.Scheme}://{request.Host}";

            var configuredDomain = Domains.FirstOrDefault(domain =>
                string.Equals(NormalizeHost(domain), NormalizeHost(requestHost), StringComparison.OrdinalIgnoreCase));

            var domain = string.Equals(CanonicalMode, "PrimaryDomain", StringComparison.OrdinalIgnoreCase)
                ? PrimaryDomain
                : configuredDomain ?? PrimaryDomain;

            return $"{Scheme}://{NormalizeHost(domain)}";
        }

        public string GetPrimaryBaseUrl()
            => $"{Scheme}://{NormalizeHost(PrimaryDomain)}";

        public string GetOpenGraphImageUrl(HttpRequest request)
            => $"{GetBaseUrl(request)}{NormalizePath(OpenGraphImage)}";

        private static string NormalizeHost(string host)
            => (host ?? string.Empty).Trim().TrimEnd('/').ToLowerInvariant();

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
