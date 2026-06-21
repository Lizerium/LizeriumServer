/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 21 июня 2026 07:10:47
 * Version: 1.0.86
 */

using System.Net.Http.Json;

namespace TranslationService.Libre
{
    public class LibreTranslate : ITranslationService
    {
        private readonly HttpClient _client;

        public LibreTranslate(HttpClient client)
        {
            _client = client;
        }

        public async Task<bool> CheckConnectionAsync()
        {
            try
            {
                var response = await _client.GetAsync("/languages");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CheckConnectionAsync failed: {ex}");
                return false;
            }
        }

        public async Task<string> TranslateAsync(string text, string sourceLang, string targetLang)
        {
            var requestData = new
            {
                q = text,
                source = sourceLang,
                target = targetLang,
                format = "text"
            };

            try
            {
                var response = await _client.PostAsJsonAsync("/translate", requestData);
                if (!response.IsSuccessStatusCode)
                {
                    // Можно залогировать или кинуть исключение
                    return $"[ERROR: {response.StatusCode}]";
                }

                var result = await response.Content.ReadFromJsonAsync<LibreTranslateResponse>();
                return result?.TranslatedText ?? "[NO_RESULT]";
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                Console.WriteLine($"TranslateAsync failed: {ex}");
                return "[ERROR]";
            }
        }
    }
}
