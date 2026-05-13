using UnityEngine;


[CreateAssetMenu(fileName = "LineDrawerSettings", menuName = "StarCooperation/LB11_LineDrawerSettings", order = 1)]
public class LineDrawerSettings : ScriptableObject
{
    public GameObject particlePrefab;
    public GameObject lineRendererPrefab;
    public Material PassiveMaterial, ActiveMaterial;
    public int TilingFactor;

}