using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StarCooperation
{
	public enum AppState
	{
		Orbit, Focus, Detail
	} //OVERVIEW, MASCHINE, PIVOT

	public class CameraRig : MonoBehaviour
	{
		public static CameraRig instance;

		private AppState appState;

		private SceneManager sceneManager;

		[Header("Initial Camera View")]
		[Range(-180f, 180f)]
		public float x = -50f; //initiale kamera ansicht
		[Range(-90f, 90f)]
		public float y = 20f; //initiale kamera ansicht

		[Header("Initial Orbit Position")]
		public Vector3 tposOrbitLast = new Vector3(-1.5f, 0f, 0f); //Orbit Target Position Save from last //is also initial orbit target pos

		[Header("Camera Angle Clamp Limits")]
		[Range(-90f, 0f)]
		public float yMinLimit = -20f;
		[Range(0f, 90f)]
		public float yMaxLimit = 20f;

		[Header("Camera Values")]
		public Transform tar;
		[Range(4f, 20f)]
		public float cameraSpeed = 5f;
		[Range(1f, 10f)]
		public float orbitSpeed = 5.0f;
		[Range(0f, 2f)]
		public float manualZoomOvershoot = 2f;
		[Range(100f, 200f)]
		public float maxContinuesRotation = 200f;
		[Range(25f, 75f)]
		public float zoomThreshold = 50f;
		[Range(75f, 125f)]
		public float dragThreshold = 100f;

		[Header("Camera Distances")]
		public float distance = 10.0f; //zooms in to distOrbit when app starts
		public float distOrbit = 4.0f; //TODO Relative to read out bounds 
		public float distFocus = 3.0f;
		public float distDetail = 2.0f;

		[Header("Bounding Box for Camera Movement")]
		public float camMoveBounds = 5f; //TODO Bounding box for move pivot = 5f -> make this read out from bounds

		//Raycast UI
		[Header("Canvas for Raycast")]
		public GraphicRaycaster m_Raycaster;
		private PointerEventData m_PointerEventData;
		private EventSystem m_EventSystem;

		//Camera Steuerung
		public Transform activeDetail;
		private float targetDistance;
		private float distanceInitial;
		//private float xMinLimit = -30f;
		//private float xMaxLimit = 30f;
		private Vector3 tpos; //Orbit Target Position
		private Quaternion rot; //Rotation
		private Vector3 pos; //Distanz Position (Kamera)
		private bool isEndPos = false;
		private bool isAlmostEndPos = false;
		private bool isFreeToRotate = false;
		private bool isUIhit = false;
		private float cameraSpeedInitial; //Before making this public fix with down below the camera++
		private float cameraStartZoom; //60f
		private float cameraZoom = 50f; //field of view ist standard 60f und geht bei zoom auf cameraStartZoom-cameraZoom=30f runter
		private float zoomPercent = 0f; //0 start bis 1 end

		//Maus Zeug
		private bool isMouseDown;
		private Vector2 mausStartPos;
		private float mausDeltaX;
		private float mausDeltaY;
		private int mouseCounter;

		//pinch zeug
		//private bool isPinch = false; //2 finger
		//public bool isMove = false; //3+ finger
		private Vector2 touch1 = Vector2.zero;
		private Vector2 touch2 = Vector2.zero;
		private float distanceStart;
		private float distanceNow;
		private float previousTouchDistance;
		private Vector3 previousTouchMidPoint;
		private bool isZooming = false;
		private bool isDragging = false;

		//Temp Zeug
		//public Slider zoomSlider;

		private void Awake()
		{
			instance = this;
		}

		private void Start()
		{
			sceneManager = SceneManager.instance;

			//Fetch the Event System from the Scene
			m_EventSystem = GetComponent<EventSystem>();

			this.appState = AppState.Orbit;
			UpdateAppState();
			cameraStartZoom = Camera.main.fieldOfView;
			cameraSpeedInitial = cameraSpeed;
		}

		private void FixedUpdate()
		{
			//start pinch
			if (Input.touchCount > 1)
			{
				//isPinch = true;
				for (int j = 0; j < Input.touchCount; j++)
				{
					if (j == 0 && touch1 == Vector2.zero)
						touch1 = Input.GetTouch(0).position;
					if (j == 1 && touch2 == Vector2.zero)
					{
						touch2 = Input.GetTouch(1).position;
						distanceStart = Vector2.Distance(touch1, touch2);
					}
				}

				float currentTouchDistance = Vector3.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
				Vector3 currentTouchMidPoint = (Input.GetTouch(1).position + Input.GetTouch(0).position) / 2;

				if (Input.GetTouch(1).phase == TouchPhase.Began)
				{
					previousTouchDistance = currentTouchDistance;
					previousTouchMidPoint = currentTouchMidPoint;
				}
				float touchDistanceDelta = previousTouchDistance - currentTouchDistance;
				float touchMidPointDelta = Vector3.Distance(currentTouchMidPoint, previousTouchMidPoint);

				// Zoom
				if (!isDragging && Mathf.Abs(touchDistanceDelta) > zoomThreshold)
				{
					isZooming = true;
				}
				if (isZooming)
				{
					//Zoom the Camera when pinch
					distanceNow = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
					Camera.main.fieldOfView = Camera.main.fieldOfView - ((distanceNow - distanceStart) / 120f);
					Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, cameraStartZoom - cameraZoom - manualZoomOvershoot, cameraStartZoom + manualZoomOvershoot);
					zoomPercent = (cameraStartZoom - Camera.main.fieldOfView) / cameraZoom;

					previousTouchDistance = currentTouchDistance;
				}
				// Drag
				if (!isZooming && Mathf.Abs(touchMidPointDelta) > dragThreshold)
				{
					isDragging = true;
				}
				if (isDragging)
				{
					Vector3 touchMidPointDistance = previousTouchMidPoint - currentTouchMidPoint;
					// Drag
					// Drag is happening down below in update

					previousTouchMidPoint = currentTouchMidPoint;
				}


			}
			else
			{
				//isPinch = false;
				isZooming = false;
				isDragging = false;
			}

			if (Input.touchCount < 1)
			{
				touch1 = Vector2.zero;
				touch2 = Vector2.zero;

				//tween back to max/min when overshoot zoom
				if (zoomPercent > 1f)
				{
					zoomPercent = Mathf.Lerp(zoomPercent, 1f, Time.deltaTime * cameraSpeed * 1f);
				}
				else if (zoomPercent < 0f)
				{
					zoomPercent = Mathf.Lerp(zoomPercent, 0f, Time.deltaTime * cameraSpeed * 1f);
				}
			}//end pinch

			if (Input.GetMouseButtonDown(0))
			{
				isMouseDown = true;
				mausStartPos = Input.mousePosition;
				cameraSpeed = cameraSpeedInitial;

				//Set up the new Pointer Event
				m_PointerEventData = new PointerEventData(m_EventSystem);
				//Set the Pointer Event Position to that of the mouse position
				m_PointerEventData.position = Input.mousePosition;
				//Create a list of Raycast Results
				List<RaycastResult> results = new List<RaycastResult>();
				//Raycast using the Graphics Raycaster and mouse click position
				m_Raycaster.Raycast(m_PointerEventData, results);
				//For every result returned, output the name of the GameObject on the Canvas hit by the Ray
				foreach (RaycastResult result in results)
				{
					if (result.gameObject.tag != "uiIgnore")
					{
						//Debug.Log("GRAPHICS - Hit " + result.gameObject.name);
						//Wenn UI getroffen dann kein Physics Raycast
						//Tag uiIgnore lässt den Raycast durch
						isUIhit = true;
						return;
					}
				}
			}

			if (Input.GetMouseButtonUp(0))
			{
				isMouseDown = false;

				//if ((appState == AppState.ORBIT || appState == AppState.FOCUS) && isFreeToRotate == true && isUIhit == false)
				//{
				//  //Debug.Log ("MouseDragDistance: "+Mathf.Abs(mausStartPos.x-Input.mousePosition.x));
				//  if (mouseCounter <= 10)
				//  {
				//      if (Mathf.Abs(mausStartPos.x - Input.mousePosition.x) <= 10)
				//      {
				//          Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				//          RaycastHit hit;
				//          if (Physics.Raycast(ray, out hit, 200))
				//          {
				//              switch (hit.collider.gameObject.tag)
				//              {
				//                  case "btnTooltip":
				//                      //TODO set activeDetail elegenter lösen
				//                      activeDetail = hit.transform.parent.parent.parent.transform;
				//                      appManager.FocusDetail(activeDetail.name);
				//                      break;
				//                  case "btnIsolate":
				//                      appManager.IsolateDetail();
				//                      break;
				//                  default:
				//                      break;
				//              }
				//          }
				//      }
				//  }
				//}
				mouseCounter = 0;
				isUIhit = false;
			}

			if (isMouseDown == true)
			{
				mouseCounter++;
			}

			if (Input.touchCount < 2 && isUIhit == false)
			{
				if (isMouseDown)
				{
					//kamera rotieren bei input
					if (isFreeToRotate == true && (appState == AppState.Detail || appState == AppState.Orbit || appState == AppState.Focus))
					{
						//TODO stop endless rotation by using decay
						mausDeltaX = Mathf.Clamp(Input.mousePosition.x - mausStartPos.x, -maxContinuesRotation, maxContinuesRotation);
						/*if (appState == AppState.ORBIT) x = Mathf.Clamp(Mathf.Lerp(x, x - mausDeltaX, Time.deltaTime * orbitSpeed / 2f), xMinLimit, xMaxLimit);
						else */
						x = Mathf.Lerp(x, x + mausDeltaX, Time.deltaTime * orbitSpeed / 4f);
						//if (appState == AppState.ORBIT) Mathf.Clamp(x, xMinLimit, xMaxLimit);

						mausDeltaY = Mathf.Clamp(Input.mousePosition.y - mausStartPos.y, -maxContinuesRotation, maxContinuesRotation);
						y = Mathf.Clamp(Mathf.Lerp(y, y - mausDeltaY, Time.deltaTime * orbitSpeed / 8f), yMinLimit, yMaxLimit);
						Mathf.Clamp(y, yMinLimit, yMaxLimit);
					}
				}
				else
				{
					//nachlaufen bei kamera rotation und kein input mehr
					mausDeltaX = Mathf.Lerp(mausDeltaX, 0.0f, Time.deltaTime * 2f);
					/*if (appState == AppState.ORBIT) x = Mathf.Clamp(Mathf.Lerp(x, x - mausDeltaX, Time.deltaTime * orbitSpeed / 4f), xMinLimit, xMaxLimit);
					else */
					x = Mathf.Lerp(x, x + mausDeltaX, Time.deltaTime * orbitSpeed / 4f);
					//if (appState == AppState.ORBIT) Mathf.Clamp(x, xMinLimit, xMaxLimit);

					mausDeltaY = Mathf.Lerp(mausDeltaY, 0.0f, Time.deltaTime * 2f);
					y = Mathf.Clamp(Mathf.Lerp(y, y - mausDeltaY, Time.deltaTime * orbitSpeed / 8f), yMinLimit, yMaxLimit);
					Mathf.Clamp(y, yMinLimit, yMaxLimit);
				}
			}
		}

		//Einmal updaten nachdem der appstate geändert wurde
		public void UpdateAppState()
		{
			switch (appState)
			{
				case AppState.Orbit:
					zoomPercent = 0f;
					targetDistance = distOrbit;
					tpos = tposOrbitLast;
					rot = Quaternion.Euler(y, x, 0f);
					break;
				case AppState.Focus:
					targetDistance = distFocus;
					tpos = new Vector3(activeDetail.position.x, activeDetail.position.y, activeDetail.position.z);
					rot = Quaternion.Euler(y, x, 0f);
					break;
				case AppState.Detail:
					zoomPercent = 0f;
					distanceInitial = distance;
					targetDistance = distDetail;
					tpos = new Vector3(activeDetail.position.x, activeDetail.position.y, activeDetail.position.z);
					rot = Quaternion.Euler(y, x, 0f);
					break;
				default:
					break;
			}
		}

		private void Update()
		{
			rot = Quaternion.Euler(y, x, 0f);

			//TODO xNorm kann dazu benutzt werden Tooltips ein und auszublenden je nach Drehung des Orbits
			//Debug.Log ("the x is: "+x);
			//x zwischen 90 und 270 einblenden
			//Debug.Log(x - Mathf.Floor(x/360f)*360f+"___"+x);
			/////float xNorm = x - Mathf.Floor(x / 360f) * 360f; //0-360 grad 
			//Debug.Log("the xNorm is: " + xNorm);

			pos = rot * new Vector3(0f, 0f, -distance) + tar.position;
			//Debug.Log("x: "+x);

			//Move the Camera Target
			if (appState == AppState.Orbit && isDragging == true)
			{
				Quaternion tempRot = Camera.main.transform.rotation;
				Vector3 tempVec = new Vector3(-Input.GetTouch(0).deltaPosition.x / 100f, -Input.GetTouch(0).deltaPosition.y / 100f, 0f);
				Vector3 rotateVec = tempRot * tempVec;
				tpos += rotateVec;
				tpos = Vector3.ClampMagnitude(tpos, camMoveBounds);
				tposOrbitLast = tpos;

			}

			//if (appState == AppState.DETAIL) distance = ((zoomPercent * (distanceInitial - 10f)) - distanceInitial) * -1f;
			distance = Mathf.Lerp(distance, targetDistance, Time.deltaTime * cameraSpeed);


			Camera.main.fieldOfView = ((zoomPercent * cameraZoom) - cameraStartZoom) * -1f;
			Vector3 tposZoom = tpos;
			if (appState == AppState.Detail)
			{
				tposZoom = tpos * (1f - zoomPercent) + activeDetail.Find("Target").position * (zoomPercent);
			}
			else if (appState == AppState.Orbit || appState == AppState.Focus)
			{
				tposZoom = tpos * (1f - zoomPercent) + tpos * (zoomPercent);
			}

			//Kamerabewegungen tweenen
			if (tar.transform.position != tposZoom)
			{
				tar.transform.position = Vector3.Lerp(tar.transform.position, tposZoom, Time.deltaTime * cameraSpeed);
				transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * cameraSpeed);
				transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * cameraSpeed);
				if (new Vector3(Mathf.Round(tar.transform.position.x * 10f) / 10f, Mathf.Round(tar.transform.position.y * 10f) / 10f, Mathf.Round(tar.transform.position.z * 10f) / 10f) == tposZoom)
				{
					isAlmostEndPos = true;
					cameraSpeed++;
				}
				isEndPos = false;
			}
			else
			{ //wenn fertig nicht mehr tweenen
				cameraSpeed = cameraSpeedInitial;
				tar.transform.position = tposZoom;
				transform.rotation = rot;
				transform.position = pos;
				isEndPos = true;
				if (appState == AppState.Orbit || appState == AppState.Detail || appState == AppState.Focus)
				{
					isFreeToRotate = true;
				}
			}

		}

		public void GoToState(AppState state)
		{
			appState = state;
			UpdateAppState();
		}

		/*
		public void SetZoomPercent(float arg)
		{
			//ZoomPercent muss per Pinch erfolgen...
			//Funktion und UI Buttons [0][1] und Slider sind nur zum Test
			zoomPercent = arg;
		}
		*/

		/*
		public void SlideZoomPercent()
		{
			//ZoomPercent muss per Pinch erfolgen...
			//Funktion und UI Buttons [0][1] und Slider sind nur zum Test
			zoomPercent = zoomSlider.value;
		}
		*/

	}
}