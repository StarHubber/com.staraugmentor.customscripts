using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation.LegacyLocalization
{
	[RequireComponent(typeof(Dropdown))]
    public class LanguageDropdown : MonoBehaviour
    {
		private Dropdown dropdown;

		private void Awake()
		{
			dropdown = GetComponent<Dropdown>();
		}

		private void Start()
		{
			// Read languages from Localizer and set options in Dropdown
			foreach (var language in Localizer.AvailableLanguages)
			{
				dropdown.options.Add(new Dropdown.OptionData(language));
			}

			// Set current language
			dropdown.value = Localizer.AvailableLanguages.IndexOf(HoloRepair.Core.ContentAppInterface.CurrentLanguageCode);
			dropdown.RefreshShownValue();

			// Add callback via code
			dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
		}

		/// <summary>
		/// The dropdown callback.
		/// </summary>
		/// <param name="value"></param>
		private void OnDropdownValueChanged(int value)
		{
			Localizer.instance.SetLanguage(dropdown.options[value].text);
		}
	}
}