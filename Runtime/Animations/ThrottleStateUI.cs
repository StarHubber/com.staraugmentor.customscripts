using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThrottleStateUI : MonoBehaviour
{
    public ConstantThrottleController throttleController; // Referenz
    public TextMeshProUGUI textDisplay;                   // UI Text

    private int lastState = -1;

    void Update()
    {
        int state = throttleController.CurrentState;

        // nur aktualisieren wenn geändert
        if (state != lastState)
        {
            lastState = state;
            UpdateText(state);
        }
    }

    void UpdateText(int state)
    {
        switch (state)
        {
            case 1:
                textDisplay.text = "Regular 4-Stroke";
                break;

            case 2:
                textDisplay.text = "Constant Throttle";
                break;

            case 3:
                textDisplay.text = "Constant Throttle + Exhaust Valve";
                break;

            default:
                textDisplay.text = "Unknown State";
                break;
        }
    }
}
