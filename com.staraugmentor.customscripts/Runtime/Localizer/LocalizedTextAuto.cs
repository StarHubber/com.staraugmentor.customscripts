using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation.LegacyLocalization
{
	public class LocalizedTextAuto : LocalizedTextBase
	{
		private Text text;
		private TextMesh textMesh;
		private TextMeshPro textMeshPro;
		private TextMeshProUGUI textMeshProUI;

		public override string GetText()
		{
			string text = "";
			if (textMeshProUI != null)
			{
				text = textMeshProUI.text;
			}
			else if (textMeshPro != null)
			{
				text = textMeshPro.text;
			}
			else if (textMesh != null)
			{
				text = textMesh.text;
			}
			else if (this.text != null)
			{
				text = this.text.text;	// lol
			}
			return text;
		}

		public override void UpdateText()
		{
			// Todo: Could be optimized for speed if necessary
			textMeshProUI = GetComponentInChildren<TextMeshProUGUI>(true);
			if (textMeshProUI != null)
			{
				textMeshProUI.text = Localizer.GetText(key, removeWordwrap);
			}
			else
			{
				textMeshPro = GetComponentInChildren<TextMeshPro>(true);
				if (textMeshPro != null)
				{
					textMeshPro.text = Localizer.GetText(key, removeWordwrap);
				}
				else
				{
					textMesh = GetComponentInChildren<TextMesh>(true);
					if (textMesh != null)
					{
						textMesh.text = Localizer.GetText(key, removeWordwrap);
					}
					else
					{
						text = GetComponentInChildren<Text>(true);
						if (text != null)
						{
							text.text = Localizer.GetText(key, removeWordwrap);
						}
						else
						{
							Debug.LogWarning("No text element available on this GameObject or children.");
						}
					}
				}
			}
		}
	}
}