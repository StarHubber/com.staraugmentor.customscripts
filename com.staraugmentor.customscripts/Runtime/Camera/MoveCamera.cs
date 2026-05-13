using STAR.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StarCooperation
{
    public class MoveCamera : MonoBehaviour
    {
        public enum CameraMode
        {
            Default,
            Orbit,
            Focus,
            Cinematic,
            GlobalCamera
        }

        public enum CameraType
        {
            Animated,
            Default,
        }

        public static MoveCamera instance;
        public bool allowMovement = true;
        public bool allowAnimation = true;
        public Camera cinematicCamera;
        public Camera normal;
        public float FreeCameraOffset;
        public GameObject cineCamBG;

        public MoveCameraSettings settings;

        [Header("Object References")]
        public GameObject rotationCenter;
        public GameObject buttonCloseFocus;

        public GameEvent CamEvent;

        // For click raycasting, description see Update()
        private GraphicRaycaster[] canvasRaycasters;

        // Working variables for Update loop
        private float touchDistanceDelta;
        private float currentTouchDistance;
        private float previousTouchDistance;
        public float distanceFactor;
        private float distOrbit;
        private Vector3 currentTouchMidPoint;
        private Vector3 previousTouchMidPoint;
        private Vector3 touchMidPointDelta;

        // Hit positions
        private Vector3 previousHitPos;
        private Vector2 hitPosDelta;
        private Vector2 rotation;
        private Vector2 previousTouchPoint0;
        private Vector2 previousTouchPoint1;

        // Camera mode variables
        private CameraMode currentMode;
        private bool isCameraModeChanging = false;
        private bool isZooming = false;
        private bool isDragging = false;
        private bool isDoubleTouch = false;
        private Vector3 defaultOrbitPosition;
        private Vector3 storedOrbitPosition;
        private Quaternion defaultOrbitRotation;
        private Quaternion storedOrbitRotation;
        private Vector3 defaultOrbitRotationCenter;
        private Vector3 lastOrbitRotationCenter;

        private Vector3 tempCamPos;

        private void Awake()
        {
            instance = this;
            currentMode = CameraMode.Cinematic;

            // Setup initial camera values = position and rotation
            transform.rotation.eulerAngles.Set(settings.initialRotX, settings.initialRotY, 0);

            // Initial camera values for camera reset
            defaultOrbitPosition = new Vector3(cinematicCamera.transform.position.x, cinematicCamera.transform.position.y, cinematicCamera.transform.position.z - FreeCameraOffset);
            defaultOrbitRotation = cinematicCamera.transform.rotation;
            defaultOrbitRotationCenter = rotationCenter.transform.position;
            lastOrbitRotationCenter = defaultOrbitRotationCenter;
        }

        // Use this for initialization
        private void Start()
        {
            buttonCloseFocus.SetActive(false);

            CalculateDistanceFactor();  // To set proper drag value at Start
            canvasRaycasters = FindObjectsOfType<GraphicRaycaster>();   // Get all Canvas raycasters in scene
            ModelHighlighter.OnModelHighlightChanged += MoveRotationCenterToHighlightedElements;

            SetCameraMode(CameraMode.Default);
        }

        private void OnDestroy()
        {
            ModelHighlighter.OnModelHighlightChanged -= MoveRotationCenterToHighlightedElements;
        }

        /// <summary>
        /// Move rotationCenter to center of highlighted bounds.
        /// </summary>
        /// <param name="highlighter"></param>
        private void MoveRotationCenterToHighlightedElements(ModelHighlighter highlighter)
        {
            if (!highlighter.changeRotationCenter)
            {
                return;
            }

            // Go through list of active High- or Lowlighters and set rotation center accordingly.
            // This enables more than 1 level of rotation center, e.g. Highlight -> Lupe with Auto Lowlight -> Highlight sub part -> Unhighlight sub part
            int idx = ModelHighlighter.activeHighOrLowlighers.Count - 1;
            if (ModelHighlighter.activeHighOrLowlighers.Count > 0)
            {
                while (idx >= 0)
                {
                    if (ModelHighlighter.activeHighOrLowlighers[idx].changeRotationCenter)
                    {
                        rotationCenter.transform.position = ModelHighlighter.activeHighOrLowlighers[idx].totalBounds.center;
                        break;
                    }
                    else
                    {
                        idx--;
                    }
                }
            }

            if (idx < 0)
            {
                rotationCenter.transform.position = defaultOrbitRotationCenter;
            }
            lastOrbitRotationCenter = rotationCenter.transform.position;
        }

        // Update is called once per frame
        //private void FixedUpdate()
        private void Update()
        {
            if (!allowMovement || isCameraModeChanging)
            {
                return;
            }

            // Camera GameObject has PhyscisRaycaster to allow OnPointerClick on 3D Models/Colliders.
            // Therefore, IsPointerOverGameObject cannot be used to check for click on UI. Instead, GraphicRaycaster on Canvas needs to be used.
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            foreach (var raycaster in canvasRaycasters)
            {
                raycaster.Raycast(eventData, results);
                foreach (var result in results)
                {
                    if (result.gameObject.layer == LayerMask.NameToLayer("UI"))
                    {
                        return;
                    }
                }
            }

            // One finger input: Rotate (via GetMouseButton-functions, works same for mouse and touch)
            if (Input.GetMouseButton(0))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    previousHitPos = Input.mousePosition;
                }
            }
            else
            {
                previousHitPos = Input.mousePosition;
            }

            hitPosDelta = (Input.mousePosition - previousHitPos) * Time.deltaTime * settings.speedRotate;
            previousHitPos = Input.mousePosition;

            // Two finger input: Zoom or Drag
            if (Input.touchCount == 2)
            {
                isDoubleTouch = true;

                currentTouchDistance = Vector3.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                currentTouchMidPoint = (Input.GetTouch(1).position + Input.GetTouch(0).position) / 2;

                if (Input.GetTouch(1).phase == TouchPhase.Began)
                {
                    previousTouchPoint0 = Input.GetTouch(0).position;
                    previousTouchPoint1 = Input.GetTouch(1).position;

                    previousTouchDistance = currentTouchDistance;
                    previousTouchMidPoint = currentTouchMidPoint;
                }

                else
                {
                    touchDistanceDelta = currentTouchDistance - previousTouchDistance;
                    touchMidPointDelta = previousTouchMidPoint - currentTouchMidPoint;

                    // Touch delta angle calculation
                    float touchVectorAngle = Vector2.Angle(Input.GetTouch(0).position - previousTouchPoint0, Input.GetTouch(1).position - previousTouchPoint1);
                    if (touchVectorAngle != 0)
                    {
                        // Zoom
                        if (!isDragging && Mathf.Abs(touchDistanceDelta) > settings.zoomThresholdDistance && touchVectorAngle >= settings.zoomDragThresholdAngle)
                        {
                            isZooming = true;
                        }

                        // Drag
                        float touchMidPointDistance = Vector3.Distance(currentTouchMidPoint, previousTouchMidPoint);
                        if (!isZooming && Mathf.Abs(touchMidPointDistance) > settings.dragThresholdDistance && touchVectorAngle < settings.zoomDragThresholdAngle)
                        {
                            isDragging = true;
                        }
                    }
                }
            }
            else
            {
                isZooming = false;
                isDragging = false;
            }

            // Reset double touch for rotating when all fingers released
            if (Input.touchCount == 0)
            {
                isDoubleTouch = false;
            }

#if UNITY_EDITOR || UNITY_STANDALONE
			if (Input.touchCount == 0)
			{
				// Mouse wheel = zoom
				if (Input.mouseScrollDelta.y != 0)
				{
					isZooming = true;
					touchDistanceDelta = Input.mouseScrollDelta.y * 20;
				}

				// Right click = drag
				if (Input.GetMouseButton(1))
				{
					isDragging = true;
					if (Input.GetMouseButtonDown(1))
					{
						previousTouchMidPoint = Input.mousePosition;
					}

					touchMidPointDelta = previousTouchMidPoint - Input.mousePosition;
					currentTouchMidPoint = Input.mousePosition;
				}
			}
#endif
            // Rotation
            if (!isDoubleTouch)
            {
                rotation = Vector2.Lerp(rotation, hitPosDelta, Time.deltaTime * settings.rotationDamping);

                // Rotation y-axis
                transform.RotateAround(rotationCenter.transform.position, Vector3.up, rotation.x);

                // Rotation x-axis with clamped pitch
                float rotX = -rotation.y;
                float curPitch = Vector3.SignedAngle(Vector3.up, transform.up, transform.right);    // More stable pitch calculation when some input frames are lost
                float newPitch = curPitch + rotX;
                if (newPitch > settings.maxPitch)
                {
                    rotX = settings.maxPitch - curPitch;
                }
                else if (newPitch < -settings.maxPitch)
                {
                    rotX = -settings.maxPitch - curPitch;
                }
                transform.RotateAround(rotationCenter.transform.position, transform.right, rotX);
            }

            // Zoom
            if (isZooming)
            {
                CalculateDistanceFactor();

                if (!(distOrbit <= settings.distOrbitMin && touchDistanceDelta > 0)
                 && !(distOrbit >= settings.distOrbitMax && touchDistanceDelta < 0))
                {
                    float speedZoom = Mathf.Lerp(settings.speedZoomMin, settings.speedZoomMax, distanceFactor);
                    float transZ = Mathf.Lerp(0, touchDistanceDelta, Time.deltaTime * speedZoom);
                    transform.Translate(Vector3.forward * transZ, Space.Self);
                }

                previousTouchDistance = currentTouchDistance;
            }

            // Drag
            if (isDragging)
            {
                // Compensate for zoom
                float speedDrag = Mathf.Lerp(settings.speedDragMin, settings.speedDragMax, distanceFactor);

                float transX = Mathf.Lerp(0, touchMidPointDelta.x, Time.deltaTime * speedDrag);
                float transY = Mathf.Lerp(0, touchMidPointDelta.y, Time.deltaTime * speedDrag);

                transform.Translate(Vector3.right * transX, Space.Self);
                transform.Translate(Vector3.up * transY, Space.Self);

                previousTouchMidPoint = currentTouchMidPoint;
            }
        }

        private void CalculateDistanceFactor()
        {
            Vector3 zoomDistVector = Vector3.Project(rotationCenter.transform.position - transform.position, transform.forward);
            distOrbit = zoomDistVector.magnitude; // Could be optimized using sqrMagnitude, but would need compared distances also as square
            distanceFactor = distOrbit / (settings.distOrbitMax - settings.distOrbitMin);
        }

        /// <summary>
        /// Focus on detail, that is, fly to detail and hide all other elements in scene.
        /// </summary>
        /// <param name="tooltip"></param>
        public void FocusDetail(Tooltip tooltip)
        {
            buttonCloseFocus.SetActive(true);
            SetCameraMode(CameraMode.Focus, tooltip);
        }

        /// <summary>
        /// Reset camera to default position.
        /// </summary>
        public void ResetCamera()
        {
            SetCameraMode(CameraMode.Cinematic);
            CamEvent.RaiseBool(false);
        }

        /// <summary>
        /// Reset camera to default position.
        /// </summary>
        public void ToggleCamera()
        {
            if (currentMode == CameraMode.Default)
            {
                SetCameraMode(CameraMode.Cinematic);
                CalculateDistanceFactor();
                CamEvent.RaiseBool(false);
            }
            else
            {
                defaultOrbitPosition = new Vector3(cinematicCamera.transform.position.x, cinematicCamera.transform.position.y, cinematicCamera.transform.position.z - FreeCameraOffset);
                defaultOrbitRotation = cinematicCamera.transform.rotation;
                SetStyRotationCenter();
                SetCameraMode(CameraMode.Default);
            }
        }


        private void SetStyRotationCenter()
        {
            rotationCenter.transform.localPosition = cinematicCamera.transform.localPosition;
        }

        public void ResetCameraToNewDefault(Transform target)
        {
            defaultOrbitPosition = new Vector3(target.position.x, target.position.y, target.position.z - FreeCameraOffset);
            defaultOrbitRotation = target.rotation;

            SetCameraMode(CameraMode.Default);
            SetStyRotationCenter();
            CamEvent.RaiseBool(true);
        }

        /// <summary>
        /// Reset camera from focussed postion to last position.
        /// </summary>
        public void ResetCameraToLastOrbitPosition()
        {
            SetCameraMode(CameraMode.Orbit);
        }

        /// <summary>
        /// Set new camera mode.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="tooltip"></param>
        public void SetCameraMode(CameraMode mode, Tooltip tooltip = null)
        {
            currentMode = mode;

            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            UpdateCameraMode(tooltip);
        }

        public void SetNewCameraLocation(Transform newLocation)
        {
            if (this.gameObject.activeInHierarchy)
            {
                StartCoroutine(DoFlyToDestination(newLocation.position, newLocation.rotation, lastOrbitRotationCenter, settings.timeCameraReset));
            }
        }

        /// <summary>
        /// Update camera mode depending on new mode, e.g. fly to detail or go back to orbital position.
        /// </summary>
        /// <param name="mode"></param>
        public void UpdateCameraMode(Tooltip tooltip = null)
        {
            isCameraModeChanging = true;
            StopAllCoroutines();

            switch (currentMode)
            {
                case CameraMode.Default:
                    EnableCam(CameraType.Default);
                    cineCamBG.SetActive(false);
                    StartCoroutine(DoFlyToDestination(defaultOrbitPosition, defaultOrbitRotation, defaultOrbitRotationCenter, settings.timeCameraReset));
                    break;

                case CameraMode.Orbit:
                    EnableCam(CameraType.Default);
                    StartCoroutine(DoFlyToDestination(storedOrbitPosition, storedOrbitRotation, lastOrbitRotationCenter, settings.timeCameraReset));
                    break;

                case CameraMode.Focus:
                    EnableCam(CameraType.Default);
                    cineCamBG.SetActive(false);
                    storedOrbitPosition = transform.position;
                    storedOrbitRotation = transform.rotation;
                    lastOrbitRotationCenter = rotationCenter.transform.position;

                    Vector3 targetPosition = tooltip.targetRotationCenter.position - tooltip.targetRotationCenter.forward * tooltip.cameraFocusDistance;
                    Quaternion targetRotation = tooltip.targetRotationCenter.rotation;
                    Vector3 targetRotationCenter = tooltip.targetRotationCenter.position;

                    StartCoroutine(DoFlyToDestination(targetPosition, targetRotation, targetRotationCenter, settings.timeFocusDetail));
                    break;

                case CameraMode.Cinematic:
                    cineCamBG.SetActive(true);
                    EnableCam(CameraType.Animated);
                    break;

                default:
                    break;
            }
        }

        public void EnableCam(CameraType cam)
        {
            switch (cam)
            {
                case CameraType.Animated:
                    normal.enabled = false;
                    cinematicCamera.enabled = true;
                    break;
                case CameraType.Default:
                    normal.enabled = true;
                    cinematicCamera.enabled = false;
                    break;
            }
        }

        /// <summary>
        /// Coroutine to fly camera to new destianation, usually focussed tooltip/detail.
        /// </summary>
        /// <param name="targetPos"></param>
        /// <param name="targetRot"></param>
        /// <param name="targetRotationCenterPos"></param>
        /// <returns></returns>
        private IEnumerator DoFlyToDestination(Vector3 targetPos, Quaternion targetRot, Vector3 targetRotationCenterPos, float flyTime)
        {
            if (currentMode != CameraMode.GlobalCamera)
                rotationCenter.transform.position = targetRotationCenterPos;

            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;

            float t = 0;
            tempCamPos = targetPos;
            while (t < 1)
            {
                if (targetPos != tempCamPos)
                    break;
                t += Time.deltaTime / flyTime;
                if (t > 1)
                {
                    t = 1;
                }

                if (transform.position == targetPos && transform.rotation == targetRot)
                {
                    break;
                }

                // If using animation curve, don't use slerp! Would be doppelt gemoppelt
                transform.position = Vector3.Lerp(startPos, targetPos, settings.speedCurve.Evaluate(t));
                transform.rotation = Quaternion.Lerp(startRot, targetRot, settings.speedCurve.Evaluate(t));

                yield return null;
            }

            isCameraModeChanging = false;
            CalculateDistanceFactor();
        }
    }
}