using StarCooperation.Export;
using StarCooperation.ExportCCP;
using StarCooperation.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
    //[RequireComponent(typeof(Toggle))]
    public class LupenHandler : MonoBehaviour, IDataInterface
    {
        public string syncID;

        [Space(5)]
        public bool autoLowlight = false;
        public bool zoomToDetail = false;
        public Button buttonLupe;
        public Transform[] connectedTooltips;

        [Space(5)]
        public PanelGroupMember panelToOpen;
        public TooltipGroupMember tooltipsToShow;

        private Toggle toggle;

        public List<InteractorData> Data { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Dictionary<string, List<System.Delegate>> InterfaceMethods
        {
            get => interfaceMethods;
            set
            {
                interfaceMethods = value;
            }
        }

        private Dictionary<string, List<System.Delegate>> interfaceMethods = new Dictionary<string, List<Delegate>>();
        public delegate void DelegateElement();

        private List<Delegate> delList = new List<Delegate>();
        private DelegateElement del/* = OnLupeClick*/;
        private void Awake()
        {
            toggle = GetComponent<Toggle>();

            // Assign button ID before activating button
            buttonLupe.GetComponent<SyncedButton>().id = syncID;

            // Make Lupe visible with toggle if tooltips connected OR panel/tooltips assigned
            if (IsLupeVisible())
            {
                toggle.onValueChanged.AddListener(isOn => buttonLupe.gameObject.SetActive(isOn));
                panelToOpen.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => SceneManager.instance.GetComponent<PanelAndHotspotHandler>().GotoPanel(this.transform.parent.GetComponent<PanelGroupMember>()));
                panelToOpen.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => SceneManager.instance.GetComponent<PanelAndHotspotHandler>().ShowTooltips(this.GetComponent<UIComponent_Step>().transform.parent.GetComponent<TooltipGroupMember>()));
                panelToOpen.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => this.GetComponent<ToggleListener>().highlighter.ResetHighlightingAll());
            }

            // Copy Lupen-callbacks to Tooltip Lupen-callbacks
            foreach (var tooltip in connectedTooltips)
            {
                tooltip.GetComponent<Tooltip>().connectedButtonLupe = buttonLupe;
            }

            // Assign Lupen callbacks
            //    buttonLupe.onClick.AddListener(OnLupeClick);

            // Auto lowlight all other model parts when clicking on Lupe
            if (autoLowlight)
            {
                buttonLupe.onClick.AddListener(delegate
                {
                    GetComponent<ToggleListener>().highlighter.Highlight(false);
                    GetComponent<ToggleListener>().highlighter.Lowlight(true);
                });
            }


            //fill data for the CustomContent Interface
            //delList.Add(del);
            //interfaceMethods.Add("LupenHandler", delList);
        }

        public void OnLupeClick()
        {
            if (IsZoomToDetail())
            {
                var tooltip = connectedTooltips[0].GetComponent<Tooltip>();
                if (tooltip.State != TooltipState.Focus)
                {
                    tooltip.ZoomToDetail();
                }
            }
            else
            {
                PanelAndHotspotHandler.instance.GotoPanel(panelToOpen);
                PanelAndHotspotHandler.instance.ShowTooltips(tooltipsToShow);
            }

        }

        public bool IsZoomToDetail()
        {
            return zoomToDetail && connectedTooltips.Length == 1;
        }

        public bool IsLupeVisible()
        {
            return connectedTooltips.Length != 0 || panelToOpen != null || tooltipsToShow != null;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            if (string.IsNullOrEmpty(syncID))
            {
                UpdateNameFromGameObject();
            }
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(syncID))
            {
                UpdateNameFromGameObject();
            }
        }

        private void UpdateNameFromGameObject()
        {
            syncID = "SyncedButtonLupe_" + gameObject.name;
        }
#endif
    }
}