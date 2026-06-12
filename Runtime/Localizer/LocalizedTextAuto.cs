using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation.LegacyLocalization
{
    public class LocalizedTextAuto : LocalizedTextBase
    {
        private Text text;
        private TextMesh textMesh;
        private TextMeshPro textMeshPro;
        private TextMeshProUGUI textMeshProUI;

        public override string GetText()
        {
            string text = "";
            if (textMeshProUI != null)
            {
                text = textMeshProUI.text;
            }
            else if (textMeshPro != null)
            {
                text = textMeshPro.text;
            }
            else if (textMesh != null)
            {
                text = textMesh.text;
            }
            else if (this.text != null)
            {
                text = this.text.text;  // lol
            }
            return text;
        }

        private bool TryGetComponentInChildren<T>(bool includeInactive, out T component)
        where T : Component
        {
            component = GetComponentInChildren<T>(includeInactive);
            return component != null;
        }

        public override void UpdateText()
        {
            if (this == null || gameObject == null)
                return;

            string localizedText = Localizer.GetText(key, removeWordwrap);

            if (TryGetComponentInChildren(true, out TextMeshProUGUI tmpUi))
            {
                tmpUi.text = localizedText;
                return;
            }

            if (TryGetComponentInChildren(true, out TextMeshPro tmp))
            {
                tmp.text = localizedText;
                return;
            }

            if (TryGetComponentInChildren(true, out TextMesh textMesh))
            {
                textMesh.text = localizedText;
                return;
            }

            if (TryGetComponentInChildren(true, out Text text))
            {
                text.text = localizedText;
                return;
            }

            Debug.LogWarning("No text element available on this GameObject or children.", this);
        }
    }
}