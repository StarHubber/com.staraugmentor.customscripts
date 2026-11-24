using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ParkBrakeManager : MonoBehaviour
{
    [Header("Referenzen")]
    public CounterManager counterManager;
    public StateSelector stateSelector;
    public WorkshopModeManager workshopModeManager;

    [Header("UI")]
    public TextMeshProUGUI stateText;

    [SerializeField] private Button parkBrakeButton;
    [SerializeField] private GameObject imageLedPButton;

    [Header("Auto Release")]
    [SerializeField] private float autoReleaseDelay = 1f;

    private Coroutine autoReleaseCoroutine;

    public bool IsParkBrakeOn { get; private set; } = true; // Standard = ON

    void Awake()
    {
        if (workshopModeManager == null)
            workshopModeManager = FindObjectOfType<WorkshopModeManager>();

        if (parkBrakeButton != null)
        {
            parkBrakeButton.onClick.AddListener(ToggleParkBrake);
        }
    }

    void Start()
    {
        UpdateDisplay();
    }

    void OnDestroy()
    {
        if (parkBrakeButton != null)
        {
            parkBrakeButton.onClick.RemoveListener(ToggleParkBrake);
        }
    }

    private bool IsWorkshopModeEnabled()
    {
        if (workshopModeManager == null)
            workshopModeManager = FindObjectOfType<WorkshopModeManager>();

        return workshopModeManager != null &&
               workshopModeManager.IsWorkshopModeOn;
    }

    public void ToggleParkBrake()
    {
        if (counterManager == null)
            return;

        // Parkbremse ist aktuell EIN:
        // Lˆsen immer erlaubt
        if (IsParkBrakeOn)
        {
            IsParkBrakeOn = false;
            UpdateDisplay();
            return;
        }

        // Parkbremse ist aktuell AUS:
        // Aktivieren nur unter 5 km/h
        if (counterManager.currentValue >= 5f)
            return;

        IsParkBrakeOn = true;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (stateText != null)
        {
            stateText.text = IsParkBrakeOn ? "ON" : "OFF";
        }

        if (imageLedPButton != null)
        {
            imageLedPButton.SetActive(IsParkBrakeOn);
        }
    }

    public void OnStarterSwitchedToOff()
    {
        // Im Workshop Mode bleiben automatische Parkbremsen-Funktionen deaktiviert
        if (IsWorkshopModeEnabled())
        {
            return;
        }

        if (counterManager == null)
            return;

        // Fahrzeug <5 km/h und Parkbremse ist aktuell gelˆst
        if (counterManager.currentValue < 5f &&
            !IsParkBrakeOn)
        {
            IsParkBrakeOn = true;
            UpdateDisplay();
        }
    }

    public void ActivateByLever()
    {
        // Bereits aktiv -> nichts tun
        if (IsParkBrakeOn)
            return;

        IsParkBrakeOn = true;
        UpdateDisplay();
    }

    public void StartAutoRelease()
    {
        // Im Workshop Mode wird die Parkbremse nicht automatisch gel√∂st
        if (IsWorkshopModeEnabled())
        {
            return;
        }

        if (stateSelector == null || counterManager == null)
            return;

        // Nur im Zustand START
        if (stateSelector.currentState != StateSelector.DeviceState.Start)
            return;

        // Nur wenn Parkbremse aktiv
        if (!IsParkBrakeOn)
            return;

        // Nicht mehrfach starten
        if (autoReleaseCoroutine != null)
            return;

        autoReleaseCoroutine = StartCoroutine(AutoReleaseRoutine());
    }

    public void CancelAutoRelease()
    {
        if (autoReleaseCoroutine != null)
        {
            StopCoroutine(autoReleaseCoroutine);
            autoReleaseCoroutine = null;
        }
    }

    private IEnumerator AutoReleaseRoutine()
    {
        yield return new WaitForSeconds(autoReleaseDelay);

        // Workshop Mode kann w√§hrend der Wartezeit eingeschaltet werden
        if (IsWorkshopModeEnabled())
        {
            autoReleaseCoroutine = null;
            yield break;
        }

        if (stateSelector == null || counterManager == null)
        {
            autoReleaseCoroutine = null;
            yield break;
        }

        // Muss weiterhin auf START stehen
        if (stateSelector.currentState != StateSelector.DeviceState.Start)
        {
            autoReleaseCoroutine = null;
            yield break;
        }

        IsParkBrakeOn = false;
        UpdateDisplay();

        autoReleaseCoroutine = null;
    }
}