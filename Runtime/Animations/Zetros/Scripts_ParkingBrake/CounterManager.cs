using UnityEngine;
using UnityEngine.Serialization;
using TMPro;

public class CounterManager : MonoBehaviour
{
    [System.Serializable]
    public class WheelRotationConfig
    {
        public Transform wheel;
        [FormerlySerializedAs("localAxis")]
        public Vector3 worldAxis = Vector3.up;
    }

    [Header("Abhängigkeiten")]
    public StateSelector stateSelector;
    public ParkBrakeManager parkBrakeManager;

    [Header("Werte & Grenzen")]
    public float currentValue = 0f;
    public float minValue = 0f;
    public float maxValue = 50f;
    public float accelerationFactor = 1.0f;
    public float brakeFactor = 1.0f;

    [Header("Parkbremse")]
    public float parkBrakeBrakeFactor = 0.35f;

    [Header("UI Anzeige")]
    public TextMeshProUGUI counterText;

    [Header("Radrotation")]
    public WheelRotationConfig[] wheels;
    [Min(0.01f)] public float wheelRadiusMeters = 0.4f;

    void Start()
    {
        UpdateTextDisplay();
    }

    public void UpdateSpeedDisplay()
    {
        UpdateTextDisplay();
    }

    void Update()
    {
        if (parkBrakeManager != null &&
            parkBrakeManager.IsParkBrakeOn &&
            currentValue > 0f)
        {
            // Exponentielle Verzögerung durch Parkbremse
            float brakeAmount =
                currentValue *
                parkBrakeBrakeFactor *
                Time.deltaTime;

            currentValue -= brakeAmount;

            if (currentValue < 0.1f)
            {
                currentValue = 0f;
            }

            UpdateTextDisplay();
        }

        RotateWheels();
    }

    // Wird vom Plus-Button aufgerufen
    public void IncreaseValue()
    {
        // Beschleunigen nur im Zustand START
        if (stateSelector == null ||
            stateSelector.currentState != StateSelector.DeviceState.START)
        {
            return;
        }

        // Kein Gas bei aktiver Parkbremse
        if (parkBrakeManager != null && parkBrakeManager.IsParkBrakeOn)
            return;

        // Exponentielle Beschleunigung
        float accelerationAmount =
            (currentValue + 1f) *
            accelerationFactor *
            Time.deltaTime;

        currentValue += accelerationAmount;
        currentValue = Mathf.Clamp(currentValue, minValue, maxValue);

        UpdateTextDisplay();
    }

    // Wird vom Minus-Button aufgerufen
    public void DecreaseValue()
    {
        // Exponentielle Verzögerung
        float brakeAmount =
            currentValue *
            brakeFactor *
            Time.deltaTime;

        currentValue -= brakeAmount;

        if (currentValue < 0.1f)
        {
            currentValue = 0f;
        }

        currentValue = Mathf.Clamp(currentValue, minValue, maxValue);

        UpdateTextDisplay();
    }

    void UpdateTextDisplay()
    {
        if (counterText != null)
        {
            counterText.text = currentValue.ToString("F1") + " km/h";
        }
    }

    private void RotateWheels()
    {
        if (wheels == null || wheelRadiusMeters <= 0f)
            return;

        // currentValue wird als km/h behandelt.
        float speedMetersPerSecond = Mathf.Max(0f, currentValue) / 3.6f;
        float rotationDegrees =
            speedMetersPerSecond / wheelRadiusMeters *
            Mathf.Rad2Deg *
            Time.deltaTime;

        foreach (WheelRotationConfig wheelConfig in wheels)
        {
            if (wheelConfig == null || wheelConfig.wheel == null)
                continue;

            wheelConfig.wheel.Rotate(
                wheelConfig.worldAxis,
                rotationDegrees,
                Space.World);
        }
    }
}