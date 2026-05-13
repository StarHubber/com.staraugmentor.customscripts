using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace StarCooperation
{
    public class ToggleEar : MonoBehaviour
    {
        public static List<ToggleEar> ToggleList = new List<ToggleEar>();

        [SerializeField] private ModelHighlighter _highlighter;

        public TextMeshProUGUI ToggleName;
        public ModelHighlighter Highlighter { get => _highlighter; set => _highlighter = value; }
        public Toggle Toggle { get; set; }
        public Stecker Stecker { get; set; }
        public bool IsDelayed { get; set; }

        public static float _toggleDisableTime = 2f;

        private void Awake()
        {
            ToggleName = GetComponentInChildren<TextMeshProUGUI>();
            Toggle = GetComponent<Toggle>();
            AddToToggleList();
        }
        public void ResetDelayed()
        {
            SetInteractableState(false);
        }
        private void Start()
        {
            AddStecker();
            SetupToggleListener();

        }
        private void OnEnable()
        {
            Toggle = GetComponent<Toggle>();
            AddToToggleList();
        }
        private void OnDisable()
        {
        }


        private void AddToToggleList()
        {
            if (ToggleList == null) ToggleList = new List<ToggleEar>();
            if (ToggleList.Contains(this))
                return;

            ToggleList.Add(this);
        }


        private void OnDestroy()
        {
            ToggleList.Remove(this);
            if (ToggleList.Count == 0) ToggleList = null;
            Stecker.SteckerList = null;
        }

        private void SetupToggleListener()
        {

            Toggle.onValueChanged.AddListener((value) => { SendMenuClick(value); });
        }


        private void SetInteractableState(bool disableInteraction)
        {
            if (disableInteraction)
            {
                Toggle.interactable = false;

            }
            else
            {
                Toggle.interactable = true;
            }
        }

        private void SendMenuClick(bool isOn)
        {
            if (IsDelayed) return;
            SteckerHandler.Instance.RegisterMenuClick(Stecker, isOn);
            foreach (var item in SteckerHandler.SteckerList)
            {
                item.Toggle?.gameObject.SetActive(true);
            }
        }

        public void AddStecker()
        {
            string[] split = this.GetComponent<LegacyLocalization.LocalizedTextAuto>().key.Split('_');
            foreach (var stecker in SteckerHandler.SteckerList)
            {
                if (split[1] == stecker.SteckerInfo.id)
                {
                    Stecker = stecker;
                }
            }

        }

    }
}