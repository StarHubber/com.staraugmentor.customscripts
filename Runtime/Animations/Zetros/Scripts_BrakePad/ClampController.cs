using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClampController : MonoBehaviour
{
    [Header("Klammern")]
    public Transform pivotLeft;
    public Transform pivotRight;
    public Vector3 rotationAxis = Vector3.forward;
    public float maxAngle = 30f;

    [Header("Lager (festes Maximum, festes Minimum)")]
    public Transform bearing;
    public Vector3 bearingRotationAxis = Vector3.right;
    public float bearingMaxAngle = 90f;

    [Header("Welle (Kind des Lagers, Maximum und Minimum wachsen)")]
    public Transform shaft;
    public Vector3 shaftRotationAxis = Vector3.right;
    public float shaftMaxAngle = 90f;

    [Header("Bremspedal")]
    [Tooltip("UI-Objekt mit PedalButton-Skript hier hineinziehen")]
    public PedalButton pedal;
    [Tooltip("Öffnungsgeschwindigkeit bei gedrücktem Pedal (0.5 = 2 s bis zum Maximum)")]
    public float openSpeed = 0.5f;
    [Tooltip("Rücklaufgeschwindigkeit nach dem Loslassen (1 = 1 s bis zum Minimum)")]
    public float closeSpeed = 1f;

    [Tooltip("Wird vom Pedal gesteuert (nur zur Anzeige)")]
    [Range(0f, 1f)]
    public float openAmount = 0f;

    [Header("Reset")]
    public Button resetButton;

    [Header("Automatisches Wachstum am Maximum (Klammern und Welle)")]
    public bool growAtMax = true;
    [Range(0.5f, 1f)]
    public float maxThreshold = 0.999f;
    [Tooltip("Kurze Pause in Sekunden zwischen Erreichen des Maximums und Beginn des Wachstums")]
    public float growDelay = 0.3f;
    public float armGrowthPerSecond = 0.5f;
    public float armLimit = 60f;
    public float shaftGrowthPerSecond = 2f;
    public float shaftLimit = 180f;

    [Header("Einfärbung der Klammern")]
    public Renderer[] clampRenderers;
    public Color darkColor = new Color(0.1f, 0.1f, 0.1f, 1f);

    [Header("Prozentanzeige")]
    [Tooltip("TextMeshPro-Text (UI) per Drag & Drop hineinziehen")]
    public TMP_Text percentText;

    // Ausgangszustand
    Quaternion startLeft, startRight, startShaft, startBearing;
    float startMaxAngle, startShaftMaxAngle;
    float timeAtMax;

    // Einfärbung
    Color[] originalColors;
    MaterialPropertyBlock block;
    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor"); // URP/HDRP
    static readonly int ColorId     = Shader.PropertyToID("_Color");     // Built-in
    int[] colorPropIds;

    // Aktuelle Minima (ergeben sich aus dem Zuwachs)
    public float ArmMinAngle   => maxAngle      - startMaxAngle;
    public float ShaftMinAngle => shaftMaxAngle - startShaftMaxAngle;

    // Zuwachs des Maximums in Prozent (0 = Start, 100 = Limit)
    public float GrowthPercent
    {
        get
        {
            float range = shaftLimit - startShaftMaxAngle;
            if (range <= 0.0001f) return 0f;
            return Mathf.Clamp01((shaftMaxAngle - startShaftMaxAngle) / range) * 100f;
        }
    }

    void Start()
    {
        startLeft  = pivotLeft.localRotation;
        startRight = pivotRight.localRotation;
        if (shaft != null)   startShaft   = shaft.localRotation;
        if (bearing != null) startBearing = bearing.localRotation;

        startMaxAngle      = maxAngle;
        startShaftMaxAngle = shaftMaxAngle;
        openAmount = 0f;

        InitColors();
        ApplyPercentText();
    }

    void InitColors()
    {
        block = new MaterialPropertyBlock();
        int n = clampRenderers != null ? clampRenderers.Length : 0;
        originalColors = new Color[n];
        colorPropIds = new int[n];

        for (int i = 0; i < n; i++)
        {
            if (clampRenderers[i] == null) continue;
            Material m = clampRenderers[i].sharedMaterial;
            if (m == null) continue;

            if (m.HasProperty(BaseColorId))  colorPropIds[i] = BaseColorId;
            else if (m.HasProperty(ColorId)) colorPropIds[i] = ColorId;
            else                             colorPropIds[i] = 0;

            originalColors[i] = colorPropIds[i] != 0 ? m.GetColor(colorPropIds[i]) : Color.white;
        }
    }

    void OnEnable()
    {
        if (resetButton != null) resetButton.onClick.AddListener(ResetState);
    }

    void OnDisable()
    {
        if (resetButton != null) resetButton.onClick.RemoveListener(ResetState);
    }

    void Update()
    {
        bool pressed = pedal != null && pedal.IsPressed;

        // Pedal: langsam öffnen, beim Loslassen zurück auf 0 (= aktuelles Minimum)
        float speed = pressed ? openSpeed : closeSpeed;
        openAmount = Mathf.MoveTowards(openAmount, pressed ? 1f : 0f, speed * Time.deltaTime);

        // Verzögerung: Timer läuft nur, solange gedrückt UND Maximum erreicht
        if (pressed && openAmount >= maxThreshold)
            timeAtMax += Time.deltaTime;
        else
            timeAtMax = 0f;

        if (growAtMax && timeAtMax >= growDelay)
        {
            maxAngle      = Mathf.MoveTowards(maxAngle,      armLimit,   armGrowthPerSecond   * Time.deltaTime);
            shaftMaxAngle = Mathf.MoveTowards(shaftMaxAngle, shaftLimit, shaftGrowthPerSecond * Time.deltaTime);
        }

        ApplyRotations();
        ApplyColor();
        ApplyPercentText();
    }

    void ApplyRotations()
    {
        // Klammern: Minimum wächst mit dem Maximum
        float angle = Mathf.Lerp(ArmMinAngle, maxAngle, openAmount);
        pivotLeft.localRotation  = startLeft  * Quaternion.AngleAxis( angle, rotationAxis);
        pivotRight.localRotation = startRight * Quaternion.AngleAxis(-angle, rotationAxis);

        // Lager: festes Minimum (0) und festes Maximum
        float bearingAngle = openAmount * bearingMaxAngle;
        if (bearing != null)
            bearing.localRotation = startBearing * Quaternion.AngleAxis(bearingAngle, bearingRotationAxis);

        // Welle: Weltwinkel mit wachsendem Minimum, abzüglich des geerbten Lagerwinkels
        if (shaft != null)
        {
            float shaftWorldAngle = Mathf.Lerp(ShaftMinAngle, shaftMaxAngle, openAmount);
            float extraAngle = shaftWorldAngle - bearingAngle;
            shaft.localRotation = startShaft * Quaternion.AngleAxis(extraAngle, shaftRotationAxis);
        }
    }

    void ApplyColor()
    {
        if (clampRenderers == null || originalColors == null) return;

        float t = GrowthPercent / 100f;

        for (int i = 0; i < clampRenderers.Length; i++)
        {
            if (clampRenderers[i] == null || colorPropIds[i] == 0) continue;

            clampRenderers[i].GetPropertyBlock(block);
            block.SetColor(colorPropIds[i], Color.Lerp(originalColors[i], darkColor, t));
            clampRenderers[i].SetPropertyBlock(block);
        }
    }

    void ApplyPercentText()
    {
        if (percentText == null) return;
        percentText.text = Mathf.RoundToInt(GrowthPercent) + " %";
    }

    public void ResetState()
    {
        maxAngle      = startMaxAngle;       // damit ist auch das Minimum wieder 0
        shaftMaxAngle = startShaftMaxAngle;
        openAmount    = 0f;
        timeAtMax     = 0f;

        ApplyRotations();
        ApplyColor();
        ApplyPercentText();
    }
}