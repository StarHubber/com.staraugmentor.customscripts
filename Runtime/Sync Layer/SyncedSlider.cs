using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
	[RequireComponent(typeof(Slider))]
	public class SyncedSlider : SyncedElementBase<Slider>
	{
		public static Dictionary<string, SyncedSlider> sliderIDs = new Dictionary<string, SyncedSlider>();

		[HideInInspector]
		public Slider uiSlider;

		private void Awake()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			uiSlider = GetComponent<Slider>();

			if (string.IsNullOrEmpty(id) || sliderIDs.ContainsKey(id))  // On loading of AssetBundle, Awake gets called twice I think, so don't assign events twice etc.
			{
				return;
			}

			if (sliderIDs.ContainsKey(id))
			{
				Debug.LogError(gameObject.name + ": ID '" + id + "' already exists.");
				return;
			}
			sliderIDs.Add(id, this);


		}

		private void OnDestroy()
		{
			sliderIDs.Remove(id);
		}

		private void Start()
		{
			if (uiSlider.onValueChanged.GetPersistentEventCount() > 0 && string.IsNullOrEmpty(id))
			{
				Debug.LogError(gameObject.ToString() + ": This " + nameof(SyncedSlider) + " has callbacks, but no ID for sync. Please give an ID now.");
			}
		}

		public static void SetSliderValueById(string id, float value)
		{
			sliderIDs[id].uiSlider.value = value;
		}
	}
}
