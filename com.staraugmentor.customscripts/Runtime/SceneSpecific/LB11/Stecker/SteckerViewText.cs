using StarCooperation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace StarCooperation
{
    public class SteckerViewText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _steckerName, _ASNR, _description, _color, _pinCount;
        [SerializeField] private GameObject prefab, vertGroup;
        private List<Row_TextHandler> aderList;
        private List<Color> colorContainer;
        private List<Color> aderColor;
        private Coroutine _coR;
        private bool _isRunning = false;

        private void Awake()
        {
            aderList = new List<Row_TextHandler>();
        }
        public void DisplayStecker(Stecker stecker)
        {

            _steckerName?.SetText(stecker.Toggle.ToggleName.text);
            // _steckerName?.SetText(stecker.SteckerInfo.benennung);
            _ASNR?.SetText(stecker.SteckerInfo.teileNummer);
            _description?.SetText(stecker.SteckerInfo.description);
            _color?.SetText(stecker.SteckerInfo.farbe);
            _pinCount?.SetText(stecker.SteckerInfo.pins);
            DisplayAderStats(stecker.aderContainer);
        }
        private void DisplayAderStats(List<AderContainer> ader)
        {
            _isRunning = true;

            for (int currentAder = 0; currentAder < ader.Count; currentAder++)
            {
                InstantiateAndFillAderRows(ader, currentAder);

            }

            _isRunning = false;

        }

        private void InstantiateAndFillAderRows(List<AderContainer> info, int currentAder)
        {
            if (aderList.Count <= currentAder)
            {
                Row_TextHandler ader = Instantiate(prefab, vertGroup.transform).GetComponent<Row_TextHandler>();
                aderList.Add(ader);
                ader.SetRowText(info[currentAder]);
                // FillIn(info, currentAder, ader);

            }
            else
            {
                aderList[currentAder].gameObject.SetActive(true);
                Row_TextHandler ader = aderList[currentAder];
                ader.SetRowText(info[currentAder]);

            }
            for (int i = info.Count; i < aderList.Count; i++)
            {
                aderList[i].gameObject.SetActive(false);
            }
        }


    }
}