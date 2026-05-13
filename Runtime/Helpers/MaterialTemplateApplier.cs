using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StarCooperation.Helpers
{
	[DefaultExecutionOrder(-100)]
	//[RequireComponent(typeof(MeshRenderer))]
	public class MaterialTemplateApplier : MonoBehaviour
	{
		private enum ApplicationSpot
		{
			Disabled,
			Awake,
			Start
		}

		[SerializeField] private RendererMaterialTemplate template;
		[SerializeField] private bool autoApplyOnRuntimeChange = false;
		[SerializeField] private ApplicationSpot autoApplyAtStartup = ApplicationSpot.Awake;
		//private MeshRenderer targetRenderer;

		public bool applyToChildren = true;

		public bool AutoApplyOnRuntimeChange
		{
			get => autoApplyOnRuntimeChange;
			set => autoApplyOnRuntimeChange = value;
		}

		public RendererMaterialTemplate Template
		{
			get => template;
			set
			{
				var changed = template != value;
				template = value;
				if (changed)
				{
					OnTemplateChanged();
				}
			}
		}

		private void Awake()
		{
			if (autoApplyAtStartup == ApplicationSpot.Awake)
			{
				ApplyTemplate();
			}
		}

		private void Start()
		{
			if (autoApplyAtStartup == ApplicationSpot.Start)
			{
				ApplyTemplate();
			}
		}

		private void OnTemplateChanged()
		{
			if (!autoApplyOnRuntimeChange || !Application.isPlaying)
			{
				return;
			}

			ApplyTemplate();
		}

		//private void EnsureRendererAssigned()
		//{
		//	if (targetRenderer == null)
		//	{
		//		targetRenderer = GetComponent<MeshRenderer>();
		//	}
		//}

		public void ApplyTemplate()
		{
			if (template == null)
			{
				Debug.LogWarning("Material template missing", this);
				return;
			}
			//EnsureRendererAssigned();
			//if (targetRenderer == null)
			//{
			//	Debug.LogWarning("Missing Renderer for material template assignment, ignoring", this);
			//	return;
			//}

			var targetRends = GetComponentsInChildren<MeshRenderer>();

			for (int k = 0; k < targetRends.Length; k++)
			{
				if (k > 0 && !applyToChildren)
				{
					break;
				}
				var targetRenderer = targetRends[k];
				//var mats = targetRenderer.materials;
				var mats = targetRenderer.sharedMaterials;
				var ordereredMats = template.OrdereredMaterials;
				for (var i = 0; i < mats.Length; i++)
				{
					if (i >= ordereredMats.Count)
					{
						mats[i] = template.SlotFillMaterial;
					}
					else
					{
						mats[i] = ordereredMats[i];
					}
				}
				//targetRenderer.materials = mats;
				targetRenderer.sharedMaterials = mats;
			}
		}
	}
}
