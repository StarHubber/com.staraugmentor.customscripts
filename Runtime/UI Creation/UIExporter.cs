using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
namespace StarCooperation.ExportCCP
{

    public class UIExporter : MonoBehaviour
    {

        public List<UIComponent_Tab> TabList { get; set; }
        public UIExportData Data;

        public void OnValidate()
        {
            //ExportToClass(GetUIElements());
        }
        void Start()
        {
            ExportUI();
        }

        private void ExportUI()
        {
            var elementList = GetUIElements();
            var path = "C:/Users/mdraxl/OneDrive - Star Cooperation GmbH/Desktop/Tempfiles/Test.json";
            ExportToJson(elementList, path);
            //      ExportToClass(elementList);
        }

        public List<UITab> GetUIElements()
        {

            TabList = FindObjectsOfType<UIComponent_Tab>().ToList();
            var serialized = new List<UITab>();
            foreach (var tab in TabList)
            {
                var newStepList = new List<UIStep>();
                foreach (var step in tab.StepList)
                {
                    if (step is null)
                        continue;

                    var newSubStepList = new List<UIStep>();
                    step.SubSteps.ForEach(x => newSubStepList.Add(new UIStep(x.Guid, x.listPosition, new List<UIStep>(), x.GetTitleDictionary(), x.GetInfoDictionary(), x.StepShape)));

                    UIStep uiStep = new UIStep(step.Guid, step.listPosition, newSubStepList, step.GetTitleDictionary(), step.GetInfoDictionary(), step.StepShape);
                    newStepList.Add(uiStep);
                }

                UITab uiTab = new UITab(tab.Guid, tab.listPosition, newStepList, tab.GetTitleDictionary(), tab.StepShape);
                serialized.Add(uiTab);
            }

            return serialized;

        }

        private UIExportData ExportToClass(List<UITab> elementList)
        {
            Data = new UIExportData(elementList);
            return Data;
        }

        private void ExportToJson(List<UITab> serialized, string path)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(serialized);
            if (!File.Exists(path))
            {
                File.CreateText((path));
            }
            File.WriteAllText(path, json);

        }
    }
    [System.Serializable]
    public class UIExportData
    {
        public readonly List<UITab> elementList;

        public UIExportData(List<UITab> elementList)
        {
            this.elementList = elementList;
        }

        public List<UITab> TabList { get; set; }
    }
}