using UnityEngine;
using UnityEngine.EventSystems;

public class LeverAngleDrag : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("References")]
    public RectTransform leverPivot;

    [Header("Rotation Limits")]
    public float minAngle = -35f;
    public float maxAngle = 0f;

    [Header("Return")]
    public float returnSpeed = 200f;

    [Header("Park Brake")]
    public ParkBrakeManager parkBrakeManager;

    [Header("Counter")]
    public CounterManager counterManager;

    [Header("Brake Effect")]
    public float maxBrakeForce = 15f; // km/h pro Sekunde


    private bool dragging;

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }

    private void Update()
    {
        if (dragging)
        {
            Vector2 screenPivot =
                RectTransformUtility.WorldToScreenPoint(
                    null,
                    leverPivot.position);

            Vector2 direction =
                (Vector2)Input.mousePosition - screenPivot;

            float angle =
                Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            angle = Mathf.Clamp(angle, minAngle, maxAngle);

            leverPivot.localRotation =
                Quaternion.Euler(0, 0, angle);

            if (counterManager != null)
            {
                // Hebelweg 0..1 berechnen
                float pullAmount =
                    Mathf.InverseLerp(maxAngle, minAngle, angle);

                // Geschwindigkeit > 5 km/h:
                // Hebel wirkt wie Bremspedal
                if (counterManager.currentValue > 5f)
                {
                    float brakeAmount =
                        maxBrakeForce *
                        pullAmount *
                        Time.deltaTime;

                    counterManager.currentValue =
                        Mathf.Max(
                            0f,
                            counterManager.currentValue - brakeAmount);

                    counterManager.UpdateSpeedDisplay();
                }
            }

            // Hebel am Anschlag -> Parkbremse aktivieren
            if (angle <= minAngle + 1f)
            {
                if (parkBrakeManager != null)
                {
                    parkBrakeManager.ActivateByLever();
                }
            }

        }
        else
        {
            float currentAngle = leverPivot.localEulerAngles.z;

            if (currentAngle > 180)
                currentAngle -= 360;

            float newAngle = Mathf.MoveTowards(
                currentAngle,
                0f,
                returnSpeed * Time.deltaTime);

            leverPivot.localRotation =
                Quaternion.Euler(0, 0, newAngle);
        }
    }
}