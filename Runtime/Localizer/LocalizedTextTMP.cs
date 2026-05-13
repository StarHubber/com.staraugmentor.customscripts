using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation.LegacyLocalization
{
	public class LocalizedTextTMP : LocalizedTextBase
	{
		[Tooltip("Assign text element. If none assigned, it is searched automatically in object and children.")]
		[SerializeField] private TextMeshPro text;

		public override string GetText()
		{
			return text.text;
		}

		public override void UpdateText()
		{
			if (text == null)
			{
				text = GetComponentInChildren<TextMeshPro>();
			}
			if (text == null)
			{
				Debug.LogError(nameof(LocalizedTextTMP) + " in " + gameObject.name + ": No text item could be found.");
			}

			text.text = Localizer.GetText(key, removeWordwrap);
		}

#if UNITY_EDITOR
		private void Reset()
		{
			text = GetComponentInChildren<TextMeshPro>();
		}
#endif
	}
}