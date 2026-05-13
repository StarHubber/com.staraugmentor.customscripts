using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
	public class ToggleGroupInverse : MonoBehaviour
	{
		[Tooltip("Toggles to affect. If none set in inspector, all child toggles will be used.")]
		public Toggle[] toggles;

		// Start is called before the first frame update
		private void Start()
		{
			if (toggles.Length == 0)
			{
				toggles = GetComponentsInChildren<Toggle>();
				if (toggles == null)
				{
					Debug.LogWarning("No child toggles found.");
				}
			}

			foreach (var toggle in toggles)
			{
				toggle.onValueChanged.AddListener(delegate
				{
					EnsureAtLeastOneToggleIsOn(toggle);
				});
			}
		}

		public void EnsureAtLeastOneToggleIsOn(Toggle senderToggle)
		{
			if (!senderToggle.isOn)
			{
				bool allOff = true;
				foreach (var toggle in toggles)
				{
					if (toggle.isOn)
					{
						allOff = false;
						break;
					}
				}
				if (allOff)
				{
					// If all toggles are off, switch last toggle on again
					senderToggle.isOn = true;
				}
			}
		}
	}
}
