using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarCooperation
{
	[RequireComponent(typeof(Camera))]
	[ExecuteAlways]
	public class AdjustViewportToUI : MonoBehaviour
	{
		public static AdjustViewportToUI instance;

		// As public and non-static to setup camera in edit mode
		public bool doAdjust = false;

		public RectTransform uiBorderLeft;
		public RectTransform uiBorderTop;

		private Camera cam;

		private void Awake()
		{
			instance = this;
		}

		// Start is called before the first frame update
		void Start()
		{
			cam = GetComponent<Camera>();
			AdjustCameraRect();
		}

		// Update is called once per frame
		void LateUpdate()
		{
			if (doAdjust)
			{
				AdjustCameraRect();
			}
		}

		private void AdjustCameraRect()
		{
			float xMin = (uiBorderLeft.anchoredPosition.x + uiBorderLeft.rect.width) / Screen.width;
			float height = 1 - uiBorderTop.rect.height / Screen.height;
			Rect newCamRect = new Rect(xMin, cam.rect.y, cam.rect.width, height);
			cam.rect = newCamRect;
		}
	}
}