/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 28 августа 2026 07:12:37
 * Version: 1.0.160
 */

using NetCrawlerDetect;

namespace Api.LizeriumServer.Services.BotDetection;

public static class BotDetectionService
{
    private static readonly CrawlerDetect Detector = new();

    public static bool IsBot(string userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            return false;

        return Detector.IsCrawler(userAgent);
    }
}
