using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace StarCooperation
{
	public class DiagramCanvasControl : MonoBehaviour
	{
		public bool moveBackWhenUnused = true;
		public bool keepAlignedToMenu = true;
		public RectTransform panelMenu;

		private Transform originalParent;
		private Vector3 originalPos;
		//private Vector3 originalScale;
		private bool isNextToMenu = true;
		private bool doAlignToPanelMenu = false;

		private void Awake()
		{
			originalParent = transform.parent;
			originalPos = transform.localPosition;
			//originalScale = transform.localScale;
		}

		private void Update()
		{
			if (transform.parent != originalParent)
			{
				isNextToMenu = false;
			}

			if (moveBackWhenUnused && !isNextToMenu)
			{
				bool allInactive = true;
				for (int i = 0; i < transform.childCount; i++)
				{
					if (transform.GetChild(i).gameObject.activeSelf)
					{
						allInactive = false;
						break;
					}
				}
				if (allInactive)
				{
					GetComponent<Billboard>().enabled = false;
					transform.SetParent(originalParent, false);
					transform.localPosition = originalPos;
					//transform.localScale = originalScale;
					transform.rotation = default;
				}
			}

			if (doAlignToPanelMenu)
			{
				// HL diagram canvas has pivot 0.5/0.5, because Billboard is directly on Canvas (in constrast to panelMenu) and needs pivot centered
				//originalPos = new Vector3(panelMenu.transform.localPosition.x, panelMenu.transform.localPosition.y, 0);

				var thisRect = GetComponent<RectTransform>();
				originalPos = new Vector3(
					panelMenu.transform.localPosition.x + (thisRect.rect.width / 2) * transform.localScale.x,
					panelMenu.transform.localPosition.y + (thisRect.rect.height / 2 * transform.localScale.y),
					0);
				if (isNextToMenu)
				{
					transform.localPosition = originalPos;
					//transform.localScale = originalScale;
				}
			}
		}

		public void StartAlignToPanelMenu()
		{
			if (keepAlignedToMenu)
			{
				doAlignToPanelMenu = true;
			}
		}

		public void StopAlignToPanelMenu()
		{
			doAlignToPanelMenu = false;
		}
	}
}