using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StarCooperation
{
    public class PanelAndHotspotHandler : MonoBehaviour
    {
        public static PanelAndHotspotHandler instance;

        private bool initialized = false;
        private int _tooltipRequestId;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            HideAllPanels();
            Task hide = HideAllTooltips();
            initialized = true;
        }

        public void HideAllPanels()
        {
            foreach (var panelGroup in PanelGroupMember.allPanelGroupMembers)
            {
                panelGroup.gameObject.SetActive(false);
            }
        }

        public async Task HideAllTooltips()
        {
            foreach (var tooltipGroup in TooltipGroupMember.allTooltipGroupMembers.ToList())
            {
                tooltipGroup.gameObject.SetActive(false);
                await Task.Yield();
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

        public async void ShowTooltips(TooltipGroupMember tooltips)
        {
            if (initialized)
            {
                int requestId = ++_tooltipRequestId;

                bool wasActive = tooltips != null && tooltips.gameObject.activeSelf;

                if (tooltips == null)
                    return;

                await HideAllTooltips();

                // Falls während await ein neuerer Aufruf kam: abbrechen
                if (requestId != _tooltipRequestId && tooltips.name != "TooltipsKomponentenMain")
                    return;

                if (!wasActive)
                {
                    Transform parent = tooltips.transform.parent;

                    if (parent != null && parent.GetComponent<TooltipGroupMember>() != null)
                    {
                        parent.gameObject.SetActive(true);
                    }

                    tooltips.gameObject.SetActive(true);
                }
            }
        }
    }
}
