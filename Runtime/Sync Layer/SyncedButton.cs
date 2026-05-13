using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
	[RequireComponent(typeof(Button))]
	public class SyncedButton : SyncedElementBase<Button>
	{
		public static Dictionary<string, SyncedButton> buttonIDs = new Dictionary<string, SyncedButton>();

		[HideInInspector]
		public Button uiButton;

		private void Awake()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			uiButton = GetComponent<Button>();

			if (string.IsNullOrEmpty(id))
			{
				return;
			}

			if (buttonIDs.ContainsKey(id))
			{
				Debug.LogError(gameObject.name + ": ID '" + id + "' already exists.");
				return;
			}

			buttonIDs.Add(id, this);
		}

		private void OnDestroy()
		{
			buttonIDs.Remove(id);
		}

		private void Start()
		{
			if (string.IsNullOrEmpty(id))
			{
				Debug.LogError(gameObject.ToString() + ": This " + nameof(SyncedButton) + " is active, but no ID for sync. Please give an ID now.");
			}
		}

		public static void ClickButtonById(string id)
		{
			if (buttonIDs.ContainsKey(id))
			{
				buttonIDs[id].uiButton.onClick.Invoke();
			}
		}
	}
}