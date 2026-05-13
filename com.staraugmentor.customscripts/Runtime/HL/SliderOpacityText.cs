using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StarCooperation
{
	public class SliderOpacityText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
	{
		public TextMeshProUGUI textMesh;

		private bool pointerDown = false;
		private bool pointerEnter = false;

		// Start is called before the first frame update
		private void Start()
		{

		}

		public void WriteOpacity(float value)
		{
			textMesh.text = "Opacity: " + (value * 100f).ToString("F0") + " %";
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			pointerEnter = true;
			textMesh.gameObject.SetActive(true);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			pointerEnter = false;
			if (!pointerDown)
			{
				textMesh.gameObject.SetActive(false);
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			pointerDown = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			pointerDown = false;
			if (!pointerEnter)
			{
				textMesh.gameObject.SetActive(false);
			}
		}
	}
}