using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarCooperation
{
	public class HotspotController : MonoBehaviour
	{
		private GameObject[] allHotspots;

		private void Awake()
		{
			allHotspots = new GameObject[transform.childCount];
			for (int i = 0; i < allHotspots.Length; i++)
			{
				allHotspots[i] = transform.GetChild(i).gameObject;
			}
		}

		// Start is called before the first frame update
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}

		public void HideAllHotspots(bool hide)
		{
			foreach (var hotspot in allHotspots)
			{
				hotspot.SetActive(!hide);
			}
		}
	}
}
