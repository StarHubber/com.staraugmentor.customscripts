using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightController : MonoBehaviour
{
    [System.Serializable]
    public class ColorGroup
    {
        public Color highlightColor = Color.yellow;

        [Tooltip("Alle Bauteile dieser Gruppe erhalten die oben gewählte Farbe")]
        public Renderer[] targetRenderers;
    }

    [Header("Toggle des Panels")]
    public Toggle toggle;

    [Header("Farbgruppen")]
    public List<ColorGroup> colorGroups = new();

    private Dictionary<Renderer, Color> originalColors = new();

    private void Start()
    {
        // Ursprungsfarben speichern
        foreach (var group in colorGroups)
        {
            foreach (var rend in group.targetRenderers)
            {
                if (rend == null)
                    continue;

                if (!originalColors.ContainsKey(rend))
                {
                    originalColors.Add(rend, GetColor(rend));
                }
            }
        }

        toggle.onValueChanged.AddListener(OnToggleChanged);

        OnToggleChanged(toggle.isOn);
    }

    private void OnDestroy()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleChanged);
        }
    }

    private void OnToggleChanged(bool isOn)
    {
        foreach (var group in colorGroups)
        {
            foreach (var rend in group.targetRenderers)
            {
                if (rend == null)
                    continue;

                if (isOn)
                {
                    SetColor(rend, group.highlightColor);
                }
                else
                {
                    if (originalColors.TryGetValue(rend, out Color original))
                    {
                        SetColor(rend, original);
                    }
                }
            }
        }
    }

    private Color GetColor(Renderer renderer)
    {
        Material mat = renderer.material;

        if (mat.HasProperty("_BaseColor"))
            return mat.GetColor("_BaseColor");

        if (mat.HasProperty("_Color"))
            return mat.GetColor("_Color");

        return Color.white;
    }

    private void SetColor(Renderer renderer, Color color)
    {
        Material mat = renderer.material;

        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", color);

        if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", color);
    }
}