using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WorkshopModeManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text workshopModeText;
    public Toggle holdToggle;

    // Parkbremsbutton visuell gedrückt aussehen lassen
    public GameObject parkBrakeButtonHighlight;

    [Header("References")]
    public CounterManager counterManager;

    public bool IsWorkshopModeOn { get; private set; }

    private void Start()
    {
        IsWorkshopModeOn = false;
        UpdateDisplay();

        // Initialzustand des Highlights setzen
        UpdateHoldHighlight();
    }

    private void Update()
    {
        UpdateHoldHighlight();

        // Workshop Mode automatisch deaktivieren bei >30km/h
        if (IsWorkshopModeOn &&
            counterManager != null &&
            counterManager.currentValue > 30f)
        {
            IsWorkshopModeOn = false;
            UpdateDisplay();
        }
    }

    public void OnIgnitionSwitchedOff()
    {
        if (holdToggle != null && holdToggle.isOn)
        {
            IsWorkshopModeOn = true;

            // HOLD-Toggle automatisch zurücksetzen
            holdToggle.isOn = false;

            UpdateDisplay();
        }
    }
    private void UpdateDisplay()
    {
        if (workshopModeText != null)
        {
            workshopModeText.text = IsWorkshopModeOn ? "ACTIVE" : "INACTIVE";
        }
    }

    private void UpdateHoldHighlight()
    {
        if (parkBrakeButtonHighlight != null &&
            holdToggle != null)
        {
            parkBrakeButtonHighlight.SetActive(holdToggle.isOn);
        }
    }
}