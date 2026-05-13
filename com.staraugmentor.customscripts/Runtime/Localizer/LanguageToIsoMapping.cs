using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StarCooperation.Localization {
	public static class LanguageToIsoMapping {

		public static Dictionary<SystemLanguage, string> map = new Dictionary<SystemLanguage, string>() {
			{SystemLanguage.Afrikaans, "af"}, {SystemLanguage.Arabic, "ar"}, {SystemLanguage.Basque, "eu"},
			{SystemLanguage.Belarusian, "be"}, {SystemLanguage.Bulgarian, "bg"}, {SystemLanguage.Catalan, "ca"},
			{SystemLanguage.Chinese, "cn"}, {SystemLanguage.Czech, "cs"}, {SystemLanguage.Danish, "da"},
			{SystemLanguage.Dutch, "nl"}, {SystemLanguage.English, "en"}, {SystemLanguage.Estonian, "et"},
			{SystemLanguage.Faroese, "fo"}, {SystemLanguage.Finnish, "fi"}, {SystemLanguage.French, "fr"},
			{SystemLanguage.German, "de"}, {SystemLanguage.Greek, "el"}, {SystemLanguage.Hebrew, "he"},
			{SystemLanguage.Hungarian, "hu"}, {SystemLanguage.Icelandic, "is"}, {SystemLanguage.Indonesian, "id"},
			{SystemLanguage.Italian, "it"}, {SystemLanguage.Japanese, "ja"}, {SystemLanguage.Korean, "ko"},
			{SystemLanguage.Latvian, "lv"}, {SystemLanguage.Lithuanian, "lt"}, {SystemLanguage.Norwegian, "no"},
			{SystemLanguage.Polish, "pl"}, {SystemLanguage.Portuguese, "pt"}, {SystemLanguage.Romanian, "ro"},
			{SystemLanguage.Russian, "ru"}, {SystemLanguage.SerboCroatian, "sr"}, {SystemLanguage.Slovak, "sk"},
			{SystemLanguage.Slovenian, "sl"}, {SystemLanguage.Spanish, "es"}, {SystemLanguage.Swedish, "sv"},
			{SystemLanguage.Thai, "th"}, {SystemLanguage.Turkish, "tr"}, {SystemLanguage.Ukrainian, "uk"},
			{SystemLanguage.Unknown, "en"}
		};
        // Achtung bei SystemLanguage.English >>>> "us" 
        // Englisch wurde nochmal in UK und US unterschieden

        public static string GetSystemIsoCode() {
			return GetIsoCodeFor(Application.systemLanguage);
		}
		
		public static string GetDefaultIsoCode() {
			return map[SystemLanguage.Unknown];
        }

        public static string GetIsoCodeFor(SystemLanguage language) {
			string isoCode;

			if (map.TryGetValue(language, out isoCode))
				return isoCode;

			return GetDefaultIsoCode();
		}
	}
}