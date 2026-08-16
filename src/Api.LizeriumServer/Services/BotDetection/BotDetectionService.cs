/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 16 августа 2026 14:46:38
 * Version: 1.0.147
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
