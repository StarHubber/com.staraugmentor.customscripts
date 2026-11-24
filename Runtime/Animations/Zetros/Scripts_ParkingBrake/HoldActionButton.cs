using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum ButtonDirection { Increase, Decrease }

    [Header("Button-Einstellung")]
    public ButtonDirection direction;      // Im Inspector wählen: Increase oder Decrease
    public CounterManager counterManager;  // Verknüpfung zum Manager

    private bool isHolding = false;

    public ParkBrakeManager parkBrakeManager; // Für Autorelease bei Gas geben
    public WorkshopModeManager workshopModeManager;

    void Update()
    {
        if (isHolding && counterManager != null)
        {
            if (direction == ButtonDirection.Increase)
            {
                counterManager.IncreaseValue();
            }
            else if (direction == ButtonDirection.Decrease)
            {
                counterManager.DecreaseValue();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;

        if (direction == ButtonDirection.Increase &&
            parkBrakeManager != null)
        {
            if (workshopModeManager == null)
                workshopModeManager = FindObjectOfType<WorkshopModeManager>();

            if (workshopModeManager == null ||
                !workshopModeManager.IsWorkshopModeOn)
            {
                parkBrakeManager.StartAutoRelease();
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;

        if (direction == ButtonDirection.Increase &&
            parkBrakeManager != null)
        {
            parkBrakeManager.CancelAutoRelease();
        }
    }
}
