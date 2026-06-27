/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 27 июня 2026 13:33:00
 * Version: 1.0.92
 */

namespace TranslationService
{
    public interface ITranslationService
    {
        Task<bool> CheckConnectionAsync();
        Task<string> TranslateAsync(string text, string sourceLang, string targetLang);
    }
}
