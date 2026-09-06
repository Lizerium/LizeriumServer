/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 06 сентября 2026 11:13:26
 * Version: 1.0.168
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
