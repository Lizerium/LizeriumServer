/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 10 октября 2026 06:58:05
 * Version: 1.0.202
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
