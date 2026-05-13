using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
	/// <summary>
	/// Makes scroll view check for children at Start and jump to item when selected/switched on.
	/// </summary>
	[RequireComponent(typeof(ScrollRect))]
	public class CheckScrollViewFocus : MonoBehaviour
	{
		private ScrollRect scrollRect;

        public ScrollRect ScrollRect { get => scrollRect; set => scrollRect = value; }

        private void Awake()
		{
			ScrollRect = GetComponent<ScrollRect>();
		}

		private void Start()
		{
			// Check for pivot, x needs to be zero to work properly!
			var rt = GetComponent<RectTransform>();
			rt.pivot = new Vector2(0, rt.pivot.y);

			foreach (var child in GetComponentsInChildren<Toggle>(true))
			{
				child.onValueChanged.AddListener(isOn =>
				{
					if (isOn)
					{
						ScrollRect.ScrollToItem(child.GetComponent<RectTransform>());
					}
				});
			}
		}
	}
}