using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation.LegacyLocalization
{
	public class LocalizedText : LocalizedTextBase
	{
		[Tooltip("Assign text element. If none assigned, it is searched automatically in object and children.")]
		[SerializeField] private Text text;

		public override string GetText()
		{
			return text.text;
		}

		public override void UpdateText()
		{
			if (text == null)
			{
				text = GetComponentInChildren<Text>();
			}
			if (text == null)
			{
				Debug.LogError(nameof(LocalizedText) + " in " + gameObject.name + ": No text item could be found.");
			}

			text.text = Localizer.GetText(key, removeWordwrap);
		}

#if UNITY_EDITOR
		private void Reset()
		{
			text = GetComponentInChildren<Text>();
		}
#endif
	}
}