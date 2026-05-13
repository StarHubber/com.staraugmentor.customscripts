using System.Collections.Generic;
using UnityEngine;

namespace StarCooperation.Helpers
{
    [CreateAssetMenu(menuName = "StarCooperation/Renderer Material Template")]
    public class RendererMaterialTemplate : ScriptableObject
    {
        [Tooltip("Applied in order to the renderer, followed by "+ nameof(SlotFillMaterial)+" if depleted")]
        [SerializeField] List<Material> ordereredMaterials = new List<Material>();

        [Tooltip("Fill all available slots with this or rest if "+nameof(OrdereredMaterials)+" is depleted")]
        [SerializeField] Material slotFillMaterial;

        public List<Material> OrdereredMaterials => ordereredMaterials;
        public Material SlotFillMaterial => slotFillMaterial;
    }
}