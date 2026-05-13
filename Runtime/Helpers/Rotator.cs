using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
	public class Rotator : MonoBehaviour
	{
		public Slider slider;
		public float rotMin = 90;
		public float rotMax = 180;
		[HideInInspector] public bool rotationEnabled = false;

		private float rotationSpeed;

		// Update is called once per frame
		private void Update()
		{
			if (rotationEnabled)
			{
				this.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
			}
		}

		public void SetRotationSpeed()
		{
			// Map slider value to min/max
			if (slider.value != 0)
			{
				rotationSpeed = Mathf.Lerp(rotMin, rotMax, (slider.value - slider.minValue) / (slider.maxValue - slider.minValue));
			}
			else
			{
				rotationSpeed = 0;
			}
		}

		public void EnableRotation(bool enable)
		{
			SetRotationSpeed();
			rotationEnabled = enable;
		}
	}
}