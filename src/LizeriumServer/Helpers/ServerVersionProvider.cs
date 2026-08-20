/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 20 августа 2026 09:58:25
 * Version: 1.0.151
 */

using System.Xml.Linq;

namespace LizeriumServer.Helpers;

/// <summary>
/// Provides the server version displayed by the shared layout.
/// </summary>
public sealed class ServerVersionProvider
{
    private const string VersionFileName = "version.xml";
    private const string DefaultVersion = "1.0.0";

    private readonly Lazy<string> _version = new(LoadVersion);

    /// <summary>
    /// Gets the version from version.xml or a stable fallback.
    /// </summary>
    public string Version => _version.Value;

    private static string LoadVersion()
    {
        var versionPath = Path.Combine(AppContext.BaseDirectory, VersionFileName);

        if (!File.Exists(versionPath))
        {
            return DefaultVersion;
        }

        var version = XDocument.Load(versionPath)
            .Root?
            .Element("version")?
            .Value?
            .Trim();

        return string.IsNullOrWhiteSpace(version)
            ? DefaultVersion
            : version;
    }
}
