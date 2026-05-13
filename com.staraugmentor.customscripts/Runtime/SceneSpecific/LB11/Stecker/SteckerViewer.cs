using StarCooperation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace StarCooperation
{
    public class SteckerViewer : MonoBehaviour
    {
        public static Color ActiveEndsteckerColor = new Color(.9f, .6f, .3f);
        public Transform MenuContent, child;
        public static SteckerViewer Instance;
        private SteckerViewText _steckerViewer;
        private Stecker _activeStecker;

        private void Awake()
        {
            Instance = this;
            _steckerViewer = GetComponentInChildren<SteckerViewText>(true);
        }

        private void OnEnable() { SteckerHandler.OnActiveSteckerChange += RespondToActiveSteckerChange; }
        private void OnDisable() { SteckerHandler.OnActiveSteckerChange -= RespondToActiveSteckerChange; }


        public void ToggleSteckerViewState(bool activate) { child.gameObject.SetActive(activate); }

        private void RespondToActiveSteckerChange(Stecker steckerToActivate, bool active, Clicktype clicktype)
        {
            ToggleSteckerViewState(active);
            _steckerViewer.DisplayStecker(steckerToActivate);
        }

    }
}
