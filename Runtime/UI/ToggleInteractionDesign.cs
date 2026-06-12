using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace StarCooperation
{
	/// <summary>
	/// This class realizes Daimler styleguide on Unity UI toggles.
	/// </summary>
	[RequireComponent(typeof(Toggle))]
	[ExecuteAlways] // !!! Execution in EditMode enables sprite swapping on change of "isOn" in Editor
	public class ToggleInteractionDesign : MonoBehaviour
	{
		[Header("Text")]
		public TextMeshProUGUI targetText;
		public Color textHighlightColor = new Color32(0x00, 0xAD, 0xEF, 0xFF);
		public Color textDefaultColor = new Color32(0x08, 0x08, 0x08, 0xFF); // eingefügt von RW 26.11.24

		[Header("Sprite")]
		public Image targetImage;
		[Tooltip("Assignment of sprite in pressed and default condition is basically a workaround to properly unhighlight the toggle when isOn = false.")]
		public Color colorPressed = new Color32(0x00, 0xAD, 0xEF, 0xFF);
		public Color colorDefault = new Color32(0x00, 0xAD, 0xEF, 0xFF);

		private Toggle toggle;
		private bool imageHasDefaultUIMaterial = false;

		// Start is called before the first frame update
		private void Start()
		{
			// Workaround to actually assign instanced material on images when custom material applied in inspector
			imageHasDefaultUIMaterial = targetImage.material.shader == Shader.Find("UI/Default");
			if (!imageHasDefaultUIMaterial)
			{
				targetImage.material = new Material(targetImage.material);
			}

			toggle = GetComponent<Toggle>();
			toggle.onValueChanged.AddListener(delegate
			{
				ToggleListener();
			});

			ToggleListener();   // Run once to change sprite when isOn = true, ExecuteInEditMode should be activated
			//toggle.onValueChanged.Invoke(toggle.isOn);
		}

		public void ToggleListener()
		{
			if (toggle.isOn)
			{
				if (targetText != null)
				{
					targetText.color = textHighlightColor;
				}

				if (targetImage != null)
				{
					if (imageHasDefaultUIMaterial)
					{
						targetImage.color = colorPressed;
					}
					else
					{
						targetImage.material.color = colorPressed;
					}
				}
			}
			else
			{
				if (targetText != null)
				{
					targetText.color = textDefaultColor;   // angepasst von RW 26.11.24
				}

				if (targetImage != null)
				{
					if (imageHasDefaultUIMaterial)
					{
						targetImage.color = colorDefault;
					}
					else
					{
						targetImage.material.color = colorDefault;
					}
				}
			}
		}
	}
}