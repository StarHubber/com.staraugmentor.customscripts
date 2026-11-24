using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StateSelector : MonoBehaviour
{
    public enum DeviceState
    {
        OFF = 0,
        ON = 1,
        START = 2
    }

    [Header("UI")]
    public Button leftButton;
    public Button rightButton;
    public TMP_Text stateText;

    [Header("Indicator")]
    public RectTransform indicatorImage;

    [Header("Current State")]
    public DeviceState currentState = DeviceState.OFF;

    [Header("Angles")]
    public float offAngle = -30f;
    public float onAngle = 0f;
    public float startTurnAngle = 30f;

    [Header("Animation")]
    public float stateChangeDuration = 0.2f;
    public float turnToStartDuration = 0.15f;
    public float holdStartDuration = 0.3f;
    public float returnDuration = 0.15f;

    [Header("Park Brake State")]
    public ParkBrakeManager parkBrakeManager;

    [Header("Workshop Mode")]
    public WorkshopModeManager workshopModeManager;

    private Coroutine animationRoutine;

    private void Start()
    {
        leftButton.onClick.AddListener(PreviousState);
        rightButton.onClick.AddListener(NextState);

        UpdateUI(false);
    }

    private void NextState()
    {
        if (currentState == DeviceState.OFF)
        {
            currentState = DeviceState.ON;
            UpdateUI(false);
        }
        else if (currentState == DeviceState.ON)
        {
            currentState = DeviceState.START;
            UpdateUI(true);
        }
    }

    private void PreviousState()
    {
        if (currentState == DeviceState.START ||
            currentState == DeviceState.ON)
        {
            currentState = DeviceState.OFF;
            UpdateUI(false);

            if (workshopModeManager == null)
                workshopModeManager = FindObjectOfType<WorkshopModeManager>();

            if (workshopModeManager != null)
                workshopModeManager.OnIgnitionSwitchedOff();

            if (parkBrakeManager != null &&
                (workshopModeManager == null ||
                 !workshopModeManager.IsWorkshopModeOn))
            {
                parkBrakeManager.OnStarterSwitchedToOff();
            }
        }
    }

    private void UpdateUI(bool playStartAnimation)
{
    // Links nur aktiv, wenn wir nicht im OFF-Zustand sind
    leftButton.interactable = currentState != DeviceState.OFF;

    // Rechts nur aktiv, wenn wir nicht im START-Zustand sind
    rightButton.interactable = currentState != DeviceState.START;

    if (stateText != null)
        stateText.text = currentState.ToString();

    if (animationRoutine != null)
        StopCoroutine(animationRoutine);

    if (playStartAnimation)
    {
        animationRoutine = StartCoroutine(StartKeyAnimation());
    }
    else
    {
        animationRoutine = StartCoroutine(RotateToCurrentState());
    }
}

    private void SetStateAngle()
    {
        float angle = offAngle;

        switch (currentState)
        {
            case DeviceState.OFF:
                angle = offAngle;
                break;

            case DeviceState.ON:
                angle = onAngle;
                break;

            case DeviceState.START:
                // Motor luft -> Schlssel steht wieder bei ON
                angle = onAngle;
                break;
        }

        indicatorImage.localRotation =
            Quaternion.Euler(0, 0, angle);
    }

    private IEnumerator StartKeyAnimation()
    {
        yield return RotateToAngle(onAngle, startTurnAngle, turnToStartDuration);

        yield return new WaitForSeconds(holdStartDuration);

        yield return RotateToAngle(startTurnAngle, onAngle, returnDuration);

        // Zustand bleibt START
        currentState = DeviceState.START;

        if (stateText != null)
            stateText.text = currentState.ToString();
    }

    private IEnumerator RotateToAngle(float from, float to, float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float angle = Mathf.Lerp(from, to, t / duration);

            indicatorImage.localRotation =
                Quaternion.Euler(0, 0, angle);

            yield return null;
        }

        indicatorImage.localRotation =
            Quaternion.Euler(0, 0, to);
    }

    private IEnumerator RotateToCurrentState()
    {
        float targetAngle = offAngle;

        switch (currentState)
        {
            case DeviceState.OFF:
                targetAngle = offAngle;
                break;

            case DeviceState.ON:
                targetAngle = onAngle;
                break;

            case DeviceState.START:
                targetAngle = onAngle;
                break;
        }

        float currentAngle = indicatorImage.localEulerAngles.z;

        // Bereich auf -180� bis +180� normieren
        if (currentAngle > 180f)
            currentAngle -= 360f;

        yield return RotateToAngle(
            currentAngle,
            targetAngle,
            stateChangeDuration);
    }
}