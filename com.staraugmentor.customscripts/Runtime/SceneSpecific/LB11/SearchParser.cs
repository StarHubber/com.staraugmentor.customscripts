using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using StarCooperation.Localization;

namespace StarCooperation
{

    public class SearchParser : MonoBehaviour
    {
        private void Update()
        {
#if !UNITY_STANDALONE
            //Fix for a Bug with TouchScreenKeyboard.Open where the native Keyboard wont open again after it is being disabled by the user once.
            if (TouchScreenKeyboard.visible == false)
            {
                CancelKeyboard();
            }
#endif
        }
        public static SearchParser Instance;

        [SerializeField] private GameObject menuContent;
        [SerializeField] private TMP_InputField searchInput;

        public TouchScreenKeyboard keyboard;
        private bool isActivated;
        private int toggleCount;

        private void OnEnable()
        {
            LegacyLocalization.Localizer.OnLanguageChanged += ResetMenu;

        }
        private void OnDisable()
        {
            LegacyLocalization.Localizer.OnLanguageChanged -= ResetMenu;

        }
        private void Awake()
        {
            Instance = this;
            searchInput.onValueChanged.AddListener(ParseSearchInput);
        }
        public void DeleteInputFieldText()
        {
            searchInput.text = string.Empty;
            ToggleActiveState(true);
        }
        public void ParseSearchInput(string inputValue)
        {
            string val = inputValue.ToLower();

            if (CheckInputValueForNulls(val))
                return;

            if (!SearchForID(val))
                SearchForBenennung(val);
        }

        private bool SearchForID(string inputVal)
        {
            bool val = false;
            ToggleActiveState(false);
            foreach (var item in ToggleEar.ToggleList)
            {
                if (item.Stecker == null)
                    continue;
                if (item.Stecker.SteckerInfo.id.ToLower().StartsWith(inputVal))
                {
                    item.gameObject.SetActive(true);
                    val = true;
                }
            }

            return val;
        }

        private bool SearchForBenennung(string inputVal)
        {
            bool val = false;
            ToggleActiveState(false);
            foreach (var item in ToggleEar.ToggleList)
            {
                if (item.Stecker == null)
                    continue;
                //  if (item.Stecker.SteckerInfo.benennung.ToLower().Contains(inputVal))
                if (item.Stecker.Toggle.GetComponent<ToggleInteractionDesign>().targetText.text.ToLower().Contains(inputVal))
                {
                    item.gameObject.SetActive(true);
                    val = true;
                }
            }

            return val;
        }

        private void ToggleActiveState(bool toggle)
        {
            foreach (var toggleGO in ToggleEar.ToggleList)
            {
                toggleGO.gameObject.SetActive(toggle);
            }
        }

        public void ResetMenu()
        {
            DeleteInputFieldText();
            foreach (var item in ToggleEar.ToggleList)
            {
                item.Highlighter.Highlight(false);
                item.Toggle.Set(false, false);
                item.ResetDelayed();
                item.GetComponent<ToggleInteractionDesign>().ToggleListener();
            }
            SteckerViewer.Instance.ToggleSteckerViewState(false);
            TabController.Instance.ClearAllTabs();
            SteckerHandler.Instance.RemoveActiveTooltip();
            LineDrawer.Instance.StopActivePS();
            StatsPanelSlider.Instance.SlideStatsPanel(null, false, Clicktype.Menu);
        }

        private bool CheckInputValueForNulls(string inputValue)
        {
            if (inputValue.Length == 0 || inputValue == "" || inputValue == null)
            {
               // ResetMenu();
                return true;
            }
            return false;
        }

        public void ActivateKeyboard()
        {
            keyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.Default, false, false, false, false, "Single-line title");
            TouchScreenKeyboard.hideInput = false;
        }

        public void CancelKeyboard()
        {
            if (keyboard != null) keyboard.active = false;

            keyboard = null;
        }
    }
}






