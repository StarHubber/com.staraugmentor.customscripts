using Microsoft.MixedReality.Toolkit.Input;
using StarCooperation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HLToggleClick : MonoBehaviour, IMixedRealityPointerHandler
{
    public Toggle toggle;

    /// <summary>
    /// Pointer clock via HOLOLENS
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        //if (!allowInteraction)
        if (InteractionControl.InteractionsDisabled)
        {
            return;
        }

        if(toggle.isOn)
            toggle.isOn = false;
        else
            toggle.isOn = true;
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
