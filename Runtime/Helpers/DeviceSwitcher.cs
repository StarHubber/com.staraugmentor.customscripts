using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

namespace StarCooperation
{
    public enum AppType
    {
        TA,
        HL,
        None
    }

    [DefaultExecutionOrder(-1000)]
    public class DeviceSwitcher : MonoBehaviour
    {
        public static DeviceSwitcher Instance { get; private set; }

        public AppType device;

        [Header("General")]
        public GameObject eventSystemTA;

        [Header("MRTK")]
        public GameObject[] mrtkObjects;
        public GameObject appBar;

        [Header("Scene Holders")]
        public GameObject mainHolderTA;
        public GameObject mainHolderHL;

        [Header("Objects Following 3D Model")]
        public Transform tooltipHolder;
        public Transform particleHolder;
        public Transform carFollowHolderHL;

        [Header("Model, e.g. LBx")]
        public Transform modelHullHolder;
        public Transform modelHolderTA;
        public Transform modelHolderHL;
        public Transform modelScalerHL;

        [Header("Canvas stuff")]
        public RectTransform canvasMenuTA;
        public RectTransform canvasMenuHL;
        public RectTransform panelMenuTA;

        public RectTransform panelHolderSchaltbildTA;
        public RectTransform panelHolderSchaltbildHL;

        public RectTransform bannerTA;
        public TextMeshProUGUI bannerTitleTA;
        public Button buttonArepMenuTA;
        public Button buttonArepBackTA;

        public RectTransform panelSideIconsTA;
        public Button buttonResetCameraTA;
        public Toggle toggleHull;

        private bool arepSceneFound = false;

        private void Awake()
        {
            Instance = this;
            //#if UNITY_STANDALONE && !UNITY_EDITOR
            //			device = AppType.TA;

            // For AREP 1.5
            eventSystemTA.SetActive(false);

            //if (DIService.Resolve<IARAnchorManager>() != null)
            if (UnityEngine.SceneManagement.SceneManager.GetSceneByName("ARep_Main_HoloLens").IsValid() || UnityEngine.SceneManagement.SceneManager.GetSceneByName("ARep_Main_Android").IsValid())  // For Unity Editor ARep
            {
                device = AppType.HL;
                //arepSceneFound = true;
            }
            else if (XRSettings.isDeviceActive)
            {
                device = AppType.HL;
            }
            else //if (UnityEngine.SceneManagement.SceneManager.GetSceneByName("Tablet").IsValid())  // For Unity Editor ARep
            {
                device = AppType.TA;
                //arepSceneFound = true;
            }

            if (device == AppType.TA)
            {
                mainHolderHL.SetActive(false);

                foreach (var obj in mrtkObjects)
                {
                    obj.SetActive(false);
                }
            }
            else if (device == AppType.HL)
            {
                mainHolderTA.SetActive(false);

                // MRTK objects and DontDestroyOnLoad
                var dontDestroyOnLoadNames = DontDestroyOnLoadAccessor.instance.GetDontDestroyOnLoadNames();
                {
                    foreach (var obj in mrtkObjects)
                    {
                        // When using AREP and HL, kick off MRTK
                        if (arepSceneFound)
                        {
                            obj.SetActive(false); // Just to be sure
                            DestroyImmediate(obj);
                        }
                        else
                        {
                            if (!DontDestroyOnLoadAccessor.instance.dontDontDestroyOnLoad)
                            {
                                // Regular behaviour
                                if (!dontDestroyOnLoadNames.Contains(obj.name))
                                {
                                    obj.SetActive(true);
                                    DontDestroyOnLoad(obj);
                                }
                                else
                                {
                                    obj.SetActive(false);
                                    DestroyImmediate(obj);
                                }
                            }
                            // Only when dontDontDestroyOnLoad flag active for Play Mode Saving using HL scene
                            else
                            {
                                obj.SetActive(true);
                            }
                        }
                    }
                }

                // Camera: Display 1
                if (!arepSceneFound)
                {
                    var mrktInputModule = FindObjectOfType<MixedRealityInputModule>();
                    if (mrktInputModule != null)
                        mrktInputModule.GetComponent<Camera>().targetDisplay = 0;
                }

                // General
                eventSystemTA.SetActive(false);

                // MRTK
                appBar.SetActive(false);

                // Model
                modelHolderTA.SetParent(modelHolderHL, true);
                modelHullHolder.SetParent(modelHolderHL, true);

                // Tooltips
                tooltipHolder.SetParent(carFollowHolderHL);

                // Particles
                particleHolder.SetParent(carFollowHolderHL);

                // Canvas stuff

                // First, move panelMenu to HL canvas
                panelMenuTA.SetParent(canvasMenuHL, false);
                foreach (RectTransform child in canvasMenuHL)
                {
                    child.rect.Set(0, 0, child.rect.width, child.rect.height);
                    child.localScale = Vector3.one;
                }

                // After panelMenu has moved, move all other panels (and BlockDiagram panels of couse) to movable canvas
                canvasMenuTA.MoveAllChildrenToNewParent(panelHolderSchaltbildHL, false);
                panelHolderSchaltbildTA.MoveAllChildrenToNewParent(panelHolderSchaltbildHL, false);
                foreach (RectTransform child in panelHolderSchaltbildHL)
                {
                    child.rect.Set(0, 0, child.rect.width, child.rect.height);
                    child.localScale = Vector3.one;
                }

                // Add banner to HL canvas
                bannerTA.SetParent(canvasMenuHL, false);
                bannerTitleTA.text = "AREP";

                // Deactivate Banner buttons, only active on TA
                buttonArepMenuTA.gameObject.SetActive(false);
                if (arepSceneFound) // Not in AiO
                {
                    buttonArepBackTA.gameObject.SetActive(false);
                }

                // Add side icons panel to HL canvas (needs repositioning)
                panelSideIconsTA.SetParent(canvasMenuHL, false);
                panelSideIconsTA.anchoredPosition = Vector2.zero;
            }
        }

        // Start is called before the first frame update
        private void Start()
        {
            // Activate in Start, not in Awake, so all other Awake routines can run proper order
            if (device == AppType.TA)
            {
                mainHolderTA.SetActive(true);
                //NetworkMessageController.instance.isReceiver = false;
                //NetworkMessageController.instance.isTransmitter = true;
            }
            else if (device == AppType.HL)
            {
                mainHolderHL.SetActive(true);
                //NetworkMessageController.instance.isReceiver = true;
                //NetworkMessageController.instance.isTransmitter = false;

                // Toggle sidepanel icons - after Awake for instances to exist
                toggleHull.isOn = false;
                buttonResetCameraTA.gameObject.SetActive(false);

                // Disable Diagram Canvas movement when pointer down on interactive component = UI Selectable
                foreach (var selectable in panelHolderSchaltbildHL.GetComponentsInChildren<Selectable>(true))
                {
                    var manipulationHandler = panelHolderSchaltbildHL.GetComponent<Microsoft.MixedReality.Toolkit.UI.ManipulationHandler>();
                    var pointerHandler = selectable.gameObject.AddComponent<PointerHandler>();

                    // Need to init events when added by code, because MRTK doesn't check for non-assigned events (maybe in newer version)
                    pointerHandler.OnPointerDown = new PointerUnityEvent();
                    pointerHandler.OnPointerUp = new PointerUnityEvent();
                    pointerHandler.OnPointerClicked = new PointerUnityEvent();
                    pointerHandler.OnPointerDragged = new PointerUnityEvent();

                    // Add actual events
                    pointerHandler.OnPointerDown.AddListener(delegate
                    {
                        manipulationHandler.enabled = false;
                    });

                    pointerHandler.OnPointerUp.AddListener(delegate
                    {
                        manipulationHandler.enabled = true;
                    });
                }

                // Put MRTK-created UIRaycastCamera to DontDestroyOnLoad
                if (!arepSceneFound)
                {
                    IMixedRealityInputSystem inputSystem = null;
                    if (MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem))
                        DontDestroyOnLoad(inputSystem?.FocusProvider?.UIRaycastCamera);
                }
            }

            // Workaround: For some reason, AppBar needs to be deactivated and re-activated after everything else is set up.
            appBar.SetActive(true);
        }
    }
}
