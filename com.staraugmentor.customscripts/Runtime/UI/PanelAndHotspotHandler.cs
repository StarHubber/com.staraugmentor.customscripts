using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
	public class PanelAndHotspotHandler : MonoBehaviour
	{
		public static PanelAndHotspotHandler instance;

		private void Awake()
		{
			instance = this;
		}

		private void Start()
		{
			HideAllPanels();
			HideAllTooltips();
		}

		public void HideAllPanels()
		{
			foreach (var panelGroup in PanelGroupMember.allPanelGroupMembers)
			{
				panelGroup.gameObject.SetActive(false);
			}
		}

		public void HideAllTooltips()
		{
			foreach (var tooltipGroup in TooltipGroupMember.allTooltipGroupMembers)
			{
				tooltipGroup.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Go to new panel and switch all toggles off.
		/// </summary>
		/// <param name="panel"></param>
		public void GotoPanel(PanelGroupMember panel)
		{
			HideAllPanels();
			if (panel != null)
			{
				panel.gameObject.SetActive(true);
				if (panel.transform.parent.GetComponent<PanelGroupMember>())
				{
					panel.transform.parent.gameObject.SetActive(true);
				}

				// Switch toggles off when entering new panel (could be on from previous selection).
				// Don't switch toggles off when panel goes inactive, as this could deactivate particles etc. that should stay on when e.g. clicking a Lupe.
				foreach (var toggle in panel.GetComponentsInChildren<Toggle>())
				{
					toggle.isOn = false;
				}
			}
		}

		public void ShowTooltips(TooltipGroupMember tooltips)
		{
			HideAllTooltips();
			if (tooltips != null)
			{
				tooltips.gameObject.SetActive(true);
				if (tooltips.transform.parent.GetComponent<TooltipGroupMember>())   // To avoid activation of deactivated tooltip holder
				{
					tooltips.transform.parent.gameObject.SetActive(true);
				}
			}
		}
	}
}
