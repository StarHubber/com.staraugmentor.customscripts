using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
	/// <summary>
	/// This class enables a toggle to listen for other events or toggles ("MainToggle"). Several flags adjust how this or the main toggle interact with each other.
	/// </summary>
	[RequireComponent(typeof(Toggle))]
	public class ToggleListener : MonoBehaviour
	{
		public Toggle mainToggle;
		public ModelHighlighter highlighter;
        public GameObject camTrans;

        [Header("Change this Toggle State")]
		public bool switchToggleOffWithMain = true;
		public bool switchToggleOnWithMain = false;
		public bool switchToggleWithHighlighter = true;

		[Header("Change MainToggle State")]
		public bool switchMainOnWithThis = true;
		public bool switchMainOffWithThis = false;

		[Header("Change GameObject State by Toggle")]
		public bool deactivateWithMain = false;
		public bool activateWithMain = false;

		// Start is called before the first frame update
		private void Awake()
		{
			try {
				Debug.Log(mainToggle);
			} catch (Exception e)
            {
				Debug.Log("Preawake: " + e);
            }

			// Main Toggle callbacks
			if (mainToggle != null)
			{
				try
				{
					Debug.Log(mainToggle);
				}
				catch (Exception e)
				{
					Debug.Log("Awake: " + e);
				}

				// Affecting this toggle or GameObject
				mainToggle.onValueChanged.AddListener(isOn =>
				{
					if (switchToggleOffWithMain && !isOn)
					{
						GetComponent<Toggle>().isOn = false;
					}

					if (switchToggleOnWithMain && isOn)
					{
						GetComponent<Toggle>().isOn = true;
					}

					if (deactivateWithMain && !isOn)
					{
						gameObject.SetActive(false);
					}

					if (activateWithMain && isOn)
					{
						gameObject.SetActive(true);
					}
				});

				// Affecting MainToggle
				GetComponent<Toggle>().onValueChanged.AddListener(isOn =>
				{
					if (switchMainOnWithThis && isOn)
					{
						mainToggle.isOn = true;
					}

					if (switchMainOffWithThis && !isOn)
					{
						mainToggle.isOn = false;
					}
				});
			}

			if (camTrans != null && DeviceSwitcher.Instance?.device == AppType.TA)
			{
				MoveCamera comp = Camera.main.gameObject.transform.GetComponent<MoveCamera>();
				this.gameObject.GetComponent<UnityEngine.UI.Toggle>().onValueChanged.AddListener((value) => comp.SetNewCameraLocation(camTrans.transform));
			}

            // Highlighter callbacks
            if (switchToggleWithHighlighter)
			{
				if(highlighter != null)
					highlighter.OnModelHighlighted?.AddListener(isHighlighted =>
				{
					GetComponent<Toggle>().isOn = isHighlighted;
                });
			}
		}
	}
}