using UnityEngine;

namespace StarCooperation
{
	public class CenteredPanel : MonoBehaviour
	{
		public RectTransform panelMenu;
		private float originalSize;
		private float lastMenuPos;
		private RectTransform rectTrafo;

		// Start is called before the first frame update
		void Start()
		{
			// On Tablet: center panel with and without menu panel shown
			if (DeviceSwitcher.Instance.device == AppType.TA)
			{
				rectTrafo = GetComponent<RectTransform>();
				originalSize = rectTrafo.rect.width;
				lastMenuPos = panelMenu.position.x;
			}
			// On Hololens: All children to parent, destroy this centered panel
			else
			{
				var childCount = transform.childCount;
				for (int i = childCount - 1; i >= 0; i--)
				{
					transform.GetChild(i).SetParent(transform.parent, true);
				}
				Destroy(this.gameObject);
			}
		}

		// Update is called once per frame
		void Update()
		{
			if (panelMenu.anchoredPosition.x != lastMenuPos)
			{
				rectTrafo.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize - panelMenu.anchoredPosition.x);
				lastMenuPos = panelMenu.anchoredPosition.x;
			}
		}
	}
}