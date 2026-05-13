using UnityEngine;

[ExecuteAlways] // wichtig, damit es auch im Editor bei Timeline funktioniert
public class MaterialTransparencyController : MonoBehaviour
{
    public Material targetMaterial;

    [Range(0f, 1f)]
    public float alpha = 1f;

    /*void Awake()
    {
        if (targetMaterial != null)
            targetMaterial = new Material(targetMaterial); // instance
    }*/

    private void Update()
    {
        if (targetMaterial != null)
        {
            Color color = targetMaterial.color;
            color.a = alpha;
            targetMaterial.color = color;
        }
    }
}
