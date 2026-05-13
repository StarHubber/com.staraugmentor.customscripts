using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarCooperation
{
	public class FramerateQuality : MonoBehaviour
	{
		public int targetFrameRate = 60;

		private void Awake()
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = targetFrameRate;
		}

		private void Update()
		{
			QualitySettings.vSyncCount = 0;
			if (Application.targetFrameRate != targetFrameRate)
			{
				Application.targetFrameRate = targetFrameRate;
			}
		}
	}
}