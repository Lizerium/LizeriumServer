/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 13 мая 2026 13:35:34
 * Version: 1.0.48
 */

namespace TranslationService
{
    public interface ITranslationService
    {
        Task<bool> CheckConnectionAsync();
        Task<string> TranslateAsync(string text, string sourceLang, string targetLang);
    }
}
