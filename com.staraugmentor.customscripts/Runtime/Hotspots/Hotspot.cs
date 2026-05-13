using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace StarCooperation
{
	public class Hotspot : MonoBehaviour, IPointerClickHandler
	{
		public static List<Hotspot> allHotspots = new List<Hotspot>();

		public ModelHighlighter highlighter;

		public UnityEvent Onclick;

		private void Awake()
		{
			allHotspots.Add(this);
		}

		private void OnDestroy()
		{
			allHotspots.Remove(this);
		}

		// Start is called before the first frame update
		private void Start()
		{
			// todo: not here and highlighter not as field
			highlighter.OnModelHighlighted.AddListener(isHighlighted =>
			{
				DisableHotspot(isHighlighted);
			});
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			Onclick.Invoke();
		}

		private void DisableHotspot(bool disable)
		{
			gameObject.SetActive(!disable);

			//if (SceneManagerBase.isHighlightingExclusive)
			//{
			//	foreach (var hotspot in allHotspots)
			//	{
			//		hotspot.gameObject.SetActive(!disable);
			//	}
			//}
		}
	}
}
