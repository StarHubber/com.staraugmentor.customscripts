using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.Toggle;

namespace StarCooperation
{
	public class MeshClickCallbacks : MonoBehaviour,
		IPointerDownHandler,
		IPointerUpHandler,
		IMixedRealityPointerHandler
	{
		[Header("Alternating True/False Callback")]
		public bool initialReturnValueTrue = true;
		[Tooltip("If true, return value true/false also changes when callback function is called from other scripts.")]
		public bool alsoToggleOnEventCall = true;
		public BooleanCallback AlternatingTrueFalseCallback;
		public ModelHighlighter ReturnValueChanger;	// todo: maybe generic?

		private bool returnValue;
		private Vector3 mouseHitPos;

		// Start is called before the first frame update
		private void Start()
		{
			returnValue = initialReturnValueTrue;

			// Todo
			if (alsoToggleOnEventCall)
			{
				//ReturnValueChanger.OnModelHighlighted.AddListener(ToggleReturnValue);
			}
		}

		private void ToggleReturnValue(bool isOn)
		{
			returnValue = !returnValue;
		}

		// Regular Mouse/Touch callbacks
		#region Mouse/Touch Callbacks

		/// <summary>
		/// Get hit position of mouse click/touch.
		/// </summary>
		/// <param name="eventData"></param>
		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				mouseHitPos = Input.mousePosition;
			}
		}

		/// <summary>
		/// Realize object clicked/touched by also checking mouse delta.
		/// </summary>
		/// <param name="eventData"></param>
		public void OnPointerUp(PointerEventData eventData)
		{
			// Call the UnityEvent with alternating return value true/false.
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				if (Input.mousePosition == mouseHitPos)
				{
					AlternatingTrueFalseCallback.Invoke(returnValue);
					//if (!alsoToggleOnEventCall)
					{
						// Only change return value when not changed in callback anyway
						returnValue = !returnValue;
					}
				}
			}
		}

		#endregion

		// MRTK Pointer Callbacks,
		// Necessary to click on mesh via Hololens cursor
		#region MRTK Pointer Callbacks

		public void OnPointerDown(MixedRealityPointerEventData eventData)
		{
			// not in use
		}

		public void OnPointerDragged(MixedRealityPointerEventData eventData)
		{
			// not in use
		}

		public void OnPointerUp(MixedRealityPointerEventData eventData)
		{
			// not in use
		}

		/// <summary>
		/// Realize click on object functionality without checking for cursor delta position, because HL cursor will never be fully steady like mouse/touch input.
		/// </summary>
		/// <param name="eventData"></param>
		public void OnPointerClicked(MixedRealityPointerEventData eventData)
		{
			AlternatingTrueFalseCallback.Invoke(returnValue);
			//if (!alsoToggleOnEventCall)
			{
				// Only change return value when not changed in callback anyway
				returnValue = !returnValue;
			}
		}

		#endregion
	}
}
