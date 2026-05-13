using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
    //[RequireComponent(typeof(Toggle))]
    public class Synced_LB11 : SyncedElementBase<Toggle>
    {
        public static Dictionary<string, Synced_LB11> toggleIDs = new Dictionary<string, Synced_LB11>();

        [HideInInspector]
        public Toggle uiToggle;

        private void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            uiToggle = GetComponent<Toggle>();

            if (string.IsNullOrEmpty(id) || toggleIDs.ContainsKey(id))  // On loading of AssetBundle, Awake gets called twice I think, so don't assign events twice etc.
            {
                return;
            }

            if (toggleIDs.ContainsKey(id))
            {
                Debug.LogError(gameObject.name + ": ID '" + id + "' already exists.");
                return;
            }
            toggleIDs.Add(id, this);

            //uiToggle.onValueChanged.AddListener(isOn =>
            //{
            //    NetworkMessageController.instance.SendNetworkMessage(nameof(Synced_LB11), id.ToString(), isOn.ToString());
            //});

        }

        private void OnDestroy()
        {
            toggleIDs.Remove(id);
        }
        private void OnEnable() { SteckerHandler.OnActiveSteckerChange += SendNMessage; }
        private void OnDisable() { SteckerHandler.OnActiveSteckerChange -= SendNMessage; }

        private void Start()
        {

            if (!Application.isPlaying)
            {
                return;
            }

            if (uiToggle.onValueChanged.GetPersistentEventCount() > 0 && string.IsNullOrEmpty(id))
            {
                Debug.LogError(gameObject.ToString() + ": This " + nameof(SyncedToggle) + " has callbacks, but no ID for sync. Please give an ID now.");
            }
        }

        private void SendNMessage(Stecker activeTooltip, bool active, Clicktype clicktype)
        {
            if (activeTooltip == GetComponent<ToggleEar>().Stecker)
                NetworkMessageController.instance.SendNetworkMessage(nameof(Synced_LB11), id.ToString(), active.ToString(), clicktype.ToString());
            if (uiToggle.isOn || !active && clicktype == Clicktype.Menu) ;

        }

        public static void SwitchToggleById(string id, bool state)
        {
            toggleIDs[id].uiToggle.isOn = state;
        }
        public static void SwitchAllToggles(bool state)
        {
            foreach (var item in toggleIDs.Values)
            {
                item.uiToggle.isOn = state;
            }


        }
    }
}