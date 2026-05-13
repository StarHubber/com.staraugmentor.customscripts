using StarCooperation.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace StarCooperation.ExportCCP
{
    public enum StepDetail
    {
        Default,
        Lupe,
        Explosion,
        Information,
        Animation
    }
    public abstract class UIComponent : MonoBehaviour
    {
        public string Guid;
        public int listPosition;
        public GameObject[] Toggles;
        public Dictionary<string, string> NamesDictionary = new Dictionary<string, string>();
        public StepDetail StepShape = StepDetail.Default;
        public LocalizedString TitleString,InfoString;

        private void OnValidate()
        {
            listPosition = this.transform.GetSiblingIndex();
            SetStepShape();

#if UNITY_EDITOR
            if (string.IsNullOrEmpty(Guid))
                Guid = GUID.Generate().ToString();
#endif
        }



        private void SetStepShape()
        {
            var lupenHandler = GetComponent<LupenHandler>();
            if (lupenHandler is null) return;
            if (lupenHandler.IsLupeVisible())
            {
                StepShape = StepDetail.Lupe;

                if (lupenHandler.IsZoomToDetail())
                {
                    StepShape = StepDetail.Explosion;
                }
                else
                {

                    StepShape = StepDetail.Lupe;
                }

                if (GetComponent<DocButtonHandler>().connectedTooltip != null)
                {
                    StepShape = StepDetail.Information;
                }

            }
            else
            {
                StepShape = StepDetail.Default;

            }

        }


        protected void Awake()
        {
            GetNames();
        }
        public virtual void GetNames()
        {


        }
        public Dictionary<string, string> GetTitleDictionary()
        {
            var dict = new Dictionary<string, string>();
            if (TitleString.IsEmpty || TitleString is null)
            {
                Debug.Log("No Information");
                return dict;
            }

            foreach (var item in LocalizationSettings.AvailableLocales.Locales)
            {
                var operation = LocalizationSettings.StringDatabase.GetLocalizedString(TitleString.TableReference, TitleString.TableEntryReference, item);

                if (!string.IsNullOrEmpty(operation))
                    dict.Add(item.LocaleName, operation);
            }
            return dict;
        }
    }
}