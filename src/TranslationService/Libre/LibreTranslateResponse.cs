/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 10 мая 2026 08:03:40
 * Version: 1.0.45
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TranslationService.Libre
{
    public class LibreTranslateResponse
    {
        [JsonPropertyName("translatedText")]
        public string TranslatedText { get; set; }
    }
}
