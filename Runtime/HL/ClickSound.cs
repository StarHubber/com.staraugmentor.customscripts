using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

namespace StarCooperation
{
	[RequireComponent(typeof(AudioSource))]
	public class ClickSound : MonoBehaviour, IMixedRealityPointerHandler
	{
		private AudioSource audioSource;

		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
		}

		public void OnPointerClicked(MixedRealityPointerEventData eventData)
		{
			audioSource.Play();
		}

		public void OnPointerDown(MixedRealityPointerEventData eventData)
		{
		}

		public void OnPointerDragged(MixedRealityPointerEventData eventData)
		{
		}

		public void OnPointerUp(MixedRealityPointerEventData eventData)
		{
		}
	}
}