using StarCooperation;
using StarCooperation.ExportCCP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public List<UIComponent_Step> ToggleList { get; set; }
    public Dictionary<string, UIComponent_Step> StepDict { get; set; }

    private void Awake()
    {
        ToggleList = FindObjectsOfType<UIComponent_Step>(true).ToList();
        StepDict = new Dictionary<string, UIComponent_Step>();
        ToggleList.ForEach(x => StepDict.Add(x.Guid, x));
        //Intercept Click on UI
        foreach (var item in StepDict)
        {
            bool state = item.Value.GetComponent<Toggle>().isOn;
            item.Value.GetComponent<Toggle>().onValueChanged.AddListener((state) => OnUiClick(item.Key, state));
            item.Value.TryGetComponent<LupenHandler>(out var lupeComp);
            if(lupeComp != null)
                lupeComp.buttonLupe.onClick.AddListener(() => OnLupeClick(item.Key));
            item.Value.GetComponent<DocButtonHandler>().buttonOpenDoc.onClick.AddListener(() => OnDocButtonClick(item.Key));
        }

    }

    private void OnDocButtonClick(string key)
    {
        throw new NotImplementedException();
    }

    public void OnLupeClick(string guid)
    {
        StepDict[guid].GetComponent<LupenHandler>().OnLupeClick();

    }
    public void OnUiClick(string guid, bool test)
    {
        StepDict[guid].GetComponent<Toggle>().isOn = test;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
