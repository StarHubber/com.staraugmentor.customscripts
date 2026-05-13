using Newtonsoft.Json;
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
    public class UIComponent_Step : UIComponent
    {
        public List<UIComponent_Step> SubSteps = new List<UIComponent_Step>();
        private void Awake()
        {
            base.Awake();
        }

        public Dictionary<string, string> GetInfoDictionary()
        {
            var dict = new Dictionary<string, string>();
            if (InfoString.IsEmpty)
            {
                Debug.Log("No Information");
                return dict;
            }
            foreach (var item in LocalizationSettings.AvailableLocales.Locales)
            {
                var operation = LocalizationSettings.StringDatabase.GetLocalizedString(InfoString.TableReference, InfoString.TableEntryReference, item);

                if (!string.IsNullOrEmpty(operation))
                    dict.Add(item.LocaleName, operation);
            }
            return dict;
        }
    }
}