using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarCooperation.Helpers;

namespace StarCooperation
{
	public class OnScriptableEventBool : MonoBehaviour
	{
		public ScriptableEventBool booleanEvent;
		public BooleanCallback onEventChanged;
		public BooleanCallback onEventChangedInverse;

		private void OnEnable()
		{
			booleanEvent.Raised += AssignEventChanged;
			booleanEvent.Raised += AssignEventChangedInverse;
		}

		private void OnDisable()
		{
			booleanEvent.Raised -= AssignEventChanged;
			booleanEvent.Raised -= AssignEventChangedInverse;
		}

		private void AssignEventChanged(bool value)
		{
			onEventChanged.Invoke(value);
		}

		private void AssignEventChangedInverse(bool value)
		{
			onEventChangedInverse.Invoke(!value);
		}
	}
}