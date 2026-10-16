/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 16 октября 2026 08:55:11
 * Version: 1.0.207
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
