/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 26 апреля 2026 10:10:48
 * Version: 1.0.31
 */

namespace TranslationService
{
    public interface ITranslationService
    {
        Task<bool> CheckConnectionAsync();
        Task<string> TranslateAsync(string text, string sourceLang, string targetLang);
    }
}
