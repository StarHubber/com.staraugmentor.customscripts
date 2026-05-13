using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
	[RequireComponent(typeof(Toggle))]
	public class SyncedToggle : SyncedElementBase<Toggle>
	{
		public static Dictionary<string, SyncedToggle> toggleIDs = new Dictionary<string, SyncedToggle>();
		
		[HideInInspector]
		public Toggle uiToggle;

		private void Awake()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			uiToggle = GetComponent<Toggle>();

			if (string.IsNullOrEmpty(id) || toggleIDs.ContainsKey(id))	// On loading of AssetBundle, Awake gets called twice I think, so don't assign events twice etc.
			{
				return;
			}

			if (toggleIDs.ContainsKey(id))
			{
				Debug.LogError(gameObject.name + ": ID '" + id + "' already exists.");
				return;
			}
			toggleIDs.Add(id, this);


	
		}

		private void OnDestroy()
		{
			toggleIDs.Remove(id);
		}

		private void Start()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			if (uiToggle.onValueChanged.GetPersistentEventCount() > 0 && string.IsNullOrEmpty(id))
			{
				Debug.LogError(gameObject.ToString() + ": This " + nameof(SyncedToggle) + " has callbacks, but no ID for sync. Please give an ID now.");
			}
		}

		public static void SwitchToggleById(string id, bool state)
		{
			toggleIDs[id].uiToggle.isOn = state;
		}
	}
}