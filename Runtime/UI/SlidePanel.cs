using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
	[RequireComponent(typeof(Toggle))]
	public class SlidePanel : MonoBehaviour
	{
		public static SlidePanel instance;

		public static bool isIn = true;
		public static bool isDeactivated = false;

		public RectTransform panelToSlide;
		[Space(5)]
		public GameObject iconClose;
		public GameObject iconOpen;
		[Space(5)]
		public float toggleAlpha = 0.2f;
		public float slideTime = 0.15f;
		[Space(5)]
        public GameObject hullPanel;
		public RectTransform panelBlockDiagramWorld;

		private Vector2 startPos;
		private Toggle toggle;

		private float panelBlockDiagramDefaultWidth;

		private void Awake()
		{
			instance = this;
			toggle = GetComponent<Toggle>();
		}

		// Start is called before the first frame update
		private void Start()
		{
			startPos = panelToSlide.position;
			if (hullPanel == null && DeviceSwitcher.Instance.device == AppType.TA)	// Missing reference on HL okay
			{
				Debug.LogError("Hull panel not assigned. Please do now.");
			}
			panelBlockDiagramDefaultWidth = panelBlockDiagramWorld.rect.width;
		}

		public void SlidePanelIn(bool slideIn)
		{
			StartCoroutine(DoSlidePanelIn(slideIn));
		}

		public void DeactivatePanel(bool deactivate)
		{
			toggle.isOn = !deactivate;

			isDeactivated = deactivate;
			toggle.interactable = !deactivate;
            hullPanel?.gameObject.SetActive(!deactivate);	// Reference is checked for TA, but on HL not existing

			var iconText = iconOpen.GetComponent<Text>();
            if (isDeactivated == true)
			{
				iconText.color = new Color(iconText.color.r, iconText.color.g, iconText.color.b, toggleAlpha);
			}
			else
			{
				iconText.color = new Color(iconText.color.r, iconText.color.g, iconText.color.b, 1);
			}
		}

		private IEnumerator DoSlidePanelIn(bool open)
		{
			toggle.interactable = false;

			if (!open)
			{
				//foreach (var layout in panelToSlide.gameObject.GetComponentsInChildren<LayoutGroup>())
				//{
				//	layout.enabled = false;
				//}

				//foreach (var layout in panelToSlide.gameObject.GetComponentsInChildren<LayoutElement>())
				//{
				//	layout.enabled = false;
				//}
				iconClose.SetActive(false);
				iconOpen.SetActive(true);
            }

			float t = 0;
			float startPosX = panelToSlide.anchoredPosition.x;
			float endPosX = open ? startPosX + panelToSlide.rect.width : startPosX - panelToSlide.rect.width;
			float newPosX;
			var thisRect = GetComponent<RectTransform>();
			var thisRectXOffset = thisRect.anchoredPosition.x - panelToSlide.anchoredPosition.x;

			AdjustViewportToUI.instance.doAdjust = true;

			while (t < 1)
			{
				t += Time.deltaTime / slideTime;
				if (t > 1)
				{
					t = 1;
				}

				newPosX = Mathf.Lerp(startPosX, endPosX, t);
				panelToSlide.anchoredPosition = new Vector2(newPosX, panelToSlide.anchoredPosition.y);
				panelBlockDiagramWorld.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelBlockDiagramDefaultWidth - newPosX);
				thisRect.anchoredPosition = new Vector2(newPosX + thisRectXOffset, thisRect.anchoredPosition.y);

				yield return null;
			}

			AdjustViewportToUI.instance.doAdjust = false;

			if (open)
			{
				//foreach (var layout in panelToSlide.gameObject.GetComponentsInChildren<LayoutGroup>())
				//{
				//	layout.enabled = true;
				//}

				//foreach (var layout in panelToSlide.gameObject.GetComponentsInChildren<LayoutElement>())
				//{
				//	layout.enabled = true;
				//}
				iconOpen.SetActive(false);
				iconClose.SetActive(true);
			}

			if (isDeactivated == false)
			{
				toggle.interactable = true;
			}

			isIn = open;
		}
	}
}
