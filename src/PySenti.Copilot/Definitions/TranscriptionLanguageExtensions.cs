﻿using PySenti.Copilot.Config;

namespace PySenti.Copilot.Definitions
{
    public static class LanguageExtensions
    {
        private static readonly Dictionary<Language, string> LanguageCodes = new()
        {
            { Language.English, "en" },
            { Language.Chinese, "zh" },
            { Language.German, "de" },
            { Language.Spanish, "es" },
            { Language.Russian, "ru" },
            { Language.Korean, "ko" },
            { Language.French, "fr" },
            { Language.Japanese, "ja" },
            { Language.Portuguese, "pt" },
            { Language.Turkish, "tr" },
            { Language.Polish, "pl" },
            { Language.Catalan, "ca" },
            { Language.Dutch, "nl" },
            { Language.Arabic, "ar" },
            { Language.Swedish, "sv" },
            { Language.Italian, "it" },
            { Language.Indonesian, "id" },
            { Language.Hindi, "hi" },
            { Language.Finnish, "fi" },
            { Language.Vietnamese, "vi" },
            { Language.Hebrew, "he" },
            { Language.Ukrainian, "uk" },
            { Language.Greek, "el" },
            { Language.Malay, "ms" },
            { Language.Czech, "cs" },
            { Language.Romanian, "ro" },
            { Language.Danish, "da" },
            { Language.Hungarian, "hu" },
            { Language.Tamil, "ta" },
            { Language.Norwegian, "no" },
            { Language.Thai, "th" },
            { Language.Urdu, "ur" },
            { Language.Croatian, "hr" },
            { Language.Bulgarian, "bg" },
            { Language.Lithuanian, "lt" },
            { Language.Latin, "la" },
            { Language.Maori, "mi" },
            { Language.Malayalam, "ml" },
            { Language.Welsh, "cy" },
            { Language.Slovak, "sk" },
            { Language.Telugu, "te" },
            { Language.Persian, "fa" },
            { Language.Latvian, "lv" },
            { Language.Bengali, "bn" },
            { Language.Serbian, "sr" },
            { Language.Azerbaijani, "az" },
            { Language.Slovenian, "sl" },
            { Language.Kannada, "kn" },
            { Language.Estonian, "et" },
            { Language.Macedonian, "mk" },
            { Language.Breton, "br" },
            { Language.Basque, "eu" },
            { Language.Icelandic, "is" },
            { Language.Armenian, "hy" },
            { Language.Nepali, "ne" },
            { Language.Mongolian, "mn" },
            { Language.Bosnian, "bs" },
            { Language.Kazakh, "kk" },
            { Language.Albanian, "sq" },
            { Language.Swahili, "sw" },
            { Language.Galician, "gl" },
            { Language.Marathi, "mr" },
            { Language.Punjabi, "pa" },
            { Language.Sinhala, "si" },
            { Language.Khmer, "km" },
            { Language.Shona, "sn" },
            { Language.Yoruba, "yo" },
            { Language.Somali, "so" },
            { Language.Afrikaans, "af" },
            { Language.Occitan, "oc" },
            { Language.Georgian, "ka" },
            { Language.Belarusian, "be" },
            { Language.Tajik, "tg" },
            { Language.Sindhi, "sd" },
            { Language.Gujarati, "gu" },
            { Language.Amharic, "am" },
            { Language.Yiddish, "yi" },
            { Language.Lao, "lo" },
            { Language.Uzbek, "uz" },
            { Language.Faroese, "fo" },
            { Language.HaitianCreole, "ht" },
            { Language.Pashto, "ps" },
            { Language.Turkmen, "tk" },
            { Language.Nynorsk, "nn" },
            { Language.Maltese, "mt" },
            { Language.Sanskrit, "sa" },
            { Language.Luxembourgish, "lb" },
            { Language.Myanmar, "my" },
            { Language.Tibetan, "bo" },
            { Language.Tagalog, "tl" },
            { Language.Malagasy, "mg" },
            { Language.Assamese, "as" },
            { Language.Tatar, "tt" },
            { Language.Hawaiian, "haw" },
            { Language.Lingala, "ln" },
            { Language.Hausa, "ha" },
            { Language.Bashkir, "ba" },
            { Language.Javanese, "jw" },
            { Language.Sundanese, "su" },
            { Language.Cantonese, "yue" }
        };

        public static string ToLanguageCode(this Language language)
        {
            if (LanguageCodes.TryGetValue(language, out string code))
            {
                return code;
            }

            throw new ArgumentException($"Language code not found for {language}");
        }
    }
}
