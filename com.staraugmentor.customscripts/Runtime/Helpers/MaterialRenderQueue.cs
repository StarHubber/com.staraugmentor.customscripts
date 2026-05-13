using UnityEngine;

[ExecuteAlways] // wichtig, damit es auch im Editor bei Timeline funktioniert
public class MaterialRenderQueue : MonoBehaviour
{
    public Material targetMaterial;
    public int RenderQueue = 3021;

    private void Update()
    {
        if (targetMaterial != null)
        {
            targetMaterial.renderQueue = RenderQueue;
        }
    }
}
