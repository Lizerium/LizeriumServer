/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 13 июня 2026 14:19:57
 * Version: 1.0.78
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
