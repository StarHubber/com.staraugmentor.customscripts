using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
	public static class Extensions
	{
		/// <summary>
		/// Makes a scroll view jump to an item that was previously out of focus.
		/// </summary>
		/// <param name="scrollRect"></param>
		/// <param name="child"></param>
		public static void ScrollToItem(this ScrollRect scrollRect, RectTransform child)
		{
			// Basic concept from: https://stackoverflow.com/questions/30766020/how-to-scroll-to-a-specific-element-in-scrollrect-with-unity-ui
			Vector2 childLocalPosition = child.localPosition;

			var scrollRectTrafo = scrollRect.GetComponent<RectTransform>();
			var childLocalMin = scrollRectTrafo.InverseTransformPoint(child.TransformPoint(child.rect.min));
			var childLocalMax = scrollRectTrafo.InverseTransformPoint(child.TransformPoint(child.rect.max));
			if (scrollRectTrafo.rect.Contains(childLocalMin) && scrollRectTrafo.rect.Contains(childLocalMax))
			{
				return;
			}

			Canvas.ForceUpdateCanvases();
			Vector2 viewportLocalPosition = scrollRect.viewport.localPosition;
			Vector2 newScrollRectLocalPosition = new Vector2(
				0 - (viewportLocalPosition.x + childLocalPosition.x),
				0 - (viewportLocalPosition.y + childLocalPosition.y)
			);

			scrollRect.content.localPosition = newScrollRectLocalPosition;
		}

		/// <summary>
		/// Iterate through all parents to find certain GameObject.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		public static GameObject FindGameObjectInParents(this GameObject start, GameObject target)
		{
			while (start != target)
			{
				if (start.transform.parent != null)
				{
					start = start.transform.parent.gameObject;
				}
				else
				{
					return null;
				}
			}
			return start;
		}

		public static void MoveAllChildrenToNewParent(this Transform oldParent, Transform newParent, bool worldPositionStays = true)
		{
			for (int i = oldParent.childCount - 1; i >= 0; i--)
			{
				var child = oldParent.GetChild(i);
				child.SetParent(newParent, worldPositionStays);
			}
		}
	}
}
