using StarCooperation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
	public class BannerCommunication : MonoBehaviour
	{
		public GameObject tooltipsToggle;
		public Dropdown dropdownLocalizer;

		private void Start()
		{
			// Set language dropdown to correct value on start
			var currentLanguage = HoloRepair.Core.ContentAppInterface.CurrentLanguageCode;
			if (currentLanguage == "DE")
			{
				dropdownLocalizer.value = 0;
			}
			else if (currentLanguage == "FR")
			{
				dropdownLocalizer.value = 1;
			}
			else if (currentLanguage == "IT")
			{
				dropdownLocalizer.value = 2;
			}
		}

		//public void HullToggle()
		//{
		//	ModelControl.instance.ToggleHull();
		//}

		//public void TooltipsToggle()
		//{
		//	SceneManager.instance.ToggleTooltips();
		//}

		//public void UnloadAssetBundle()
		//{
		//	SceneManager.instance.UnloadAssetBundle();
		//}

		//public void UnfocusDetail()
		//{
		//	SceneManager.instance.UnfocusDetail();
		//}

		public void HideTooltipsToggle(bool hide)
		{
			tooltipsToggle.SetActive(!hide);
		}
	}
}
