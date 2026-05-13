using StarCooperation;
using StarCooperation.ExportCCP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StarCooperation.Interface;

public class GEHandler : MonoBehaviour
{
    public void HandleUIClick(string received)
    {
        Debug.Log(received);

        if (received == GetComponent<UIComponent>().Guid)
        {
            GetComponent<Toggle>().isOn = !GetComponent<Toggle>().isOn;
        }
    }

    public void HandleTooltipClick(string guid)
    {
        //We need to Tell the Viewer here that we want to activate the UI element with this guid
        Debug.Log("Clicked: " + guid);
        //DataInterface.DATAObject.ToggleState(DataInterface.DATAObject.ReturnsCorrespondingInteractor(guid), "Toggles");

        /*if (guid == GetComponent<UIComponent>().Guid)
        {
            //this is me
            GetComponent<Toggle>().isOn = true;
        }*/
    }
    public void HandleLupeClicked(string guid)
    {
        if (guid == GetComponent<UIComponent>().Guid)
        {
            //this is me
            GetComponent<LupenHandler>().OnLupeClick();
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
