/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 17 мая 2026 11:31:46
 * Version: 1.0.51
 */

using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestPlatform.Utilities;

using TranslationService.Libre;

using Xunit;
using Xunit.Abstractions;

namespace TranslationService.Tests.Libre
{
    public class LibreTranslateTests
    {
        private readonly ITestOutputHelper _output;

        public LibreTranslateTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task TranslateAsync_StateUnderTest_ExpectedBehavior()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://192.168.64.128:5001");

            // Arrange
            var libreTranslate = new LibreTranslate(client);
            string text = "Тест";
            string sourceLang = "ru";
            string targetLang = "en";

            // Act
            var result = await libreTranslate.TranslateAsync(
                text,
                sourceLang,
                targetLang);

            // Assert
            Assert.Equal("Test", result);

            // Print
            _output.WriteLine($"Translated: {result}");
        }
    }
}
