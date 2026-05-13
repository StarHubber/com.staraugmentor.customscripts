using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace StarCooperation
{
	[RequireComponent(typeof(Toggle))]
	public class OnValueChangedAdvanced : MonoBehaviour
	{
		public BooleanCallback onValueChangedInverse;
		public BooleanCallback onValueChangedTrue;
		public BooleanCallback onValueChangedFalse;

		private void Awake()
		{
			var toggle = GetComponent<Toggle>();
			toggle.onValueChanged.AddListener(isOn => onValueChangedInverse.Invoke(!isOn));
			toggle.onValueChanged.AddListener(delegate
			{
				if (toggle.isOn)
				{
					onValueChangedTrue.Invoke(true);
				}
			});
			toggle.onValueChanged.AddListener(delegate
			{
				if (!toggle.isOn)
				{
					onValueChangedFalse.Invoke(false);
				}
			});
		}
	}
}