/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 15 октября 2026 07:48:58
 * Version: 1.0.206
 */

namespace TranslationService
{
    public interface ITranslationService
    {
        Task<bool> CheckConnectionAsync();
        Task<string> TranslateAsync(string text, string sourceLang, string targetLang);
    }
}
