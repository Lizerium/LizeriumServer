/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 01 сентября 2026 08:53:24
 * Version: 1.0.163
 */

namespace TranslationService
{
    public interface ITranslationService
    {
        Task<bool> CheckConnectionAsync();
        Task<string> TranslateAsync(string text, string sourceLang, string targetLang);
    }
}
