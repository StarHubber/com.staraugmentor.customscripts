using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation.LegacyLocalization
{
	public class LocalizedTextTM : LocalizedTextBase
	{
		[Tooltip("Assign text element. If none assigned, it is searched automatically in object and children.")]
		[SerializeField] private TextMesh text;

		public override string GetText()
		{
			return text.text;
		}

		public override void UpdateText()
		{
			if (text == null)
			{
				text = GetComponentInChildren<TextMesh>();
			}
			if (text == null)
			{
				Debug.LogError(nameof(LocalizedTextTM) + " in " + gameObject.name + ": No text item could be found.");
			}

			text.text = Localizer.GetText(key, removeWordwrap);
		}

#if UNITY_EDITOR
		private void Reset()
		{
			text = GetComponentInChildren<TextMesh>();
		}
#endif
	}
}