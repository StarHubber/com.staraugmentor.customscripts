using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarCooperation
{
	[CreateAssetMenu(menuName = "StarCooperation/MoveCamera Setting")]
	public class MoveCameraSettings : ScriptableObject
	{
		public int targetFrameRate = 30;

		[Header("Initial Camera View")]
		[Range(-90f, 90f)] public float initialRotX = 20;
		[Range(-180f, 180f)] public float initialRotY = -50;

		[Header("Camera Distances")]
		public float distOrbitMax = 10;
		public float distOrbitMin = 0.5f;
		public float distFocus = 3;
		public float distDetail = 2;

		[Header("Rotation")]
		public float speedRotate = 2.5f;
		public float rotationDamping = 15;

		[Header("Zoom")]
		public float speedZoomMax = 0.5f;
		public float speedZoomMin = 0.01f;

		[Header("Drag")]
		public float speedDragMax = 0.02f;
		public float speedDragMin = 0.002f;

		[Header("Thresholds")]
		public float zoomThresholdDistance = 10;
		public float dragThresholdDistance = 10;
		[Range(0, 180)] public float zoomDragThresholdAngle = 90;

		[Header("Constraints")]
		public float maxPitch = 90;

		[Header("Fly-to Settings")]
		public AnimationCurve speedCurve;
		public float timeFocusDetail;
		public float timeCameraReset;
	}
}