using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarCooperation.LegacyLocalization
{
	public abstract class LocalizedTextBase : MonoBehaviour
	{
		public bool removeWordwrap = false;
		public string key;

		private void OnEnable()
		{
			UpdateText();
		}

		private void Awake()
		{
			// Update text once in Awake, before SortChildren runs ins Start - Localizer got -100 execution order, so this is OK.
			// Needs to run in Awake, because OnEnable runs before Localizer is actually set up.
			// Needs to run in OnEnable as well for tooltips.
			UpdateText();
			Localizer.OnLanguageChanged += UpdateText;
		}

		private void OnDestroy()
		{
			Localizer.OnLanguageChanged -= UpdateText;
		}

		public abstract string GetText();

		public void SetKey(string key)
		{
			this.key = key;
			UpdateText();
		}

		public abstract void UpdateText();
	}
}
