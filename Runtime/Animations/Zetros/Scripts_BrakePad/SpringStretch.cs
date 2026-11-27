using UnityEngine;

public class SpringStretch : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    [Header("Anker")]
    public Transform anchorA;
    public Transform anchorB;

    [Header("Mesh-Einstellungen")]
    public Axis springAxis = Axis.X;
    public bool invertAxis = false;
    public bool pivotAtCenter = false;

    float startDistance;
    float startScale;

    Vector3 AxisVector()
    {
        Vector3 v;
        switch (springAxis)
        {
            case Axis.X: v = Vector3.right; break;
            case Axis.Y: v = Vector3.up; break;
            default:     v = Vector3.forward; break;
        }
        return invertAxis ? -v : v;
    }

    void Start()
    {
        startDistance = Vector3.Distance(anchorA.position, anchorB.position);
        startScale = transform.localScale[(int)springAxis];
    }

    void LateUpdate()
    {
        if (anchorA == null || anchorB == null || startDistance < 0.0001f) return;

        Vector3 dir = anchorB.position - anchorA.position;
        float dist = dir.magnitude;
        if (dist < 0.0001f) return;

        // Federachse auf die Verbindungslinie A -> B drehen
        Vector3 currentDir = transform.rotation * AxisVector();
        transform.rotation = Quaternion.FromToRotation(currentDir, dir) * transform.rotation;

        // Position
        transform.position = pivotAtCenter
            ? (anchorA.position + anchorB.position) * 0.5f
            : anchorA.position;

        // Skalierung relativ zum Startzustand
        Vector3 s = transform.localScale;
        s[(int)springAxis] = startScale * (dist / startDistance);
        transform.localScale = s;
    }
}