using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace StarCooperation
{
    public class Row_TextHandler : MonoBehaviour
    {
        public ToggleEar stecker;
        [SerializeField] private TextMeshProUGUI Nr, aderNr, benennung, steckkontakt, dichtungsteil, leitungsklasse, leitungsfarbe, leitungsquerschnitt, endstecker, endkammer;
        [SerializeField] private Image colorTop, colorBottom;
        [SerializeField] private GameObject disabledTransformPosition, enabledTransformPosition;
        [SerializeField] private Button endsteckerButton;

        void Start()
        {
            endsteckerButton.onClick.AddListener(delegate { InvokeTooltip(); });

        }


        public void SetRowText(AderContainer ader)
        {
            colorTop.color = ader.AderInfo.AderColor[0];
            colorBottom.color = ader.AderInfo.AderColor[1];

            stecker = ader.ToggleEar;

            Nr.SetText(ader.AderInfo.Nr);
            aderNr.SetText(ader.AderInfo.AderNr);
            benennung.SetText(ader.AderInfo.Benennung);
            steckkontakt.SetText(ader.AderInfo.Steckkontakt);
            dichtungsteil.SetText(ader.AderInfo.Dichtungsteil);
            leitungsklasse.SetText(ader.AderInfo.Leitungsklasse);
            leitungsfarbe.SetText(ader.AderInfo.Farbe);
            leitungsquerschnitt.SetText(ader.AderInfo.Querschnitt);
            endstecker.SetText(ader.AderInfo.Endstecker);
            endkammer.SetText(ader.AderInfo.Endkammer);

            SetEndsteckerButtonColor(ader);
        }
        private void InvokeTooltip()
        {
            SteckerHandler.Instance.RegisterEndsteckerClick(stecker);
        }
        private void SetEndsteckerButtonColor(AderContainer ader)
        {
            if (ader.ToggleEar == null)
            {
                endstecker.color = Color.grey;
                endsteckerButton.enabled = false;
            }
            else
            {
                endstecker.color = SteckerViewer.ActiveEndsteckerColor;
                endsteckerButton.enabled = true;
            }
        }

    }
}
