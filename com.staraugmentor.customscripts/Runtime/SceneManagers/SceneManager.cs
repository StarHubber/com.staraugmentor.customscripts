using Paroxe.PdfRenderer;
using StarCooperation.ExportCCP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
    public class SceneManager : MonoBehaviour, IAutoConfigurableSceneManager
    {
        public static SceneManager instance;

        [Header("Scene References")]
        [SerializeField] private GameObject tooltipsHolder;
        [SerializeField] public GameObject viewerInterface;
        [SerializeField] public GameObject komponentenMain;
        [SerializeField] private GameObject particlesHolder;
        [SerializeField] private GameObject canvasAREP;
        [SerializeField] private GameObject panelSideIcons;

        [Header("Tab Toggles - for startup and callbacks")]
        [SerializeField] private RectTransform tabHolder;
        [SerializeField] private RectTransform toggleHolder;
        [SerializeField] private List<GameObject> MainToggles;

        [Header("Start scene name for All-in-One")]
        [SerializeField] private string aioStartSceneName;

        private int highlighterMessageRoute = -1;

        private List<ToggleListener> highlighterToggles = new List<ToggleListener>();
        List<ToggleListener> IAutoConfigurableSceneManager.highlighterToggles => highlighterToggles;

        private ParticleSystem[] particleSystems;

        private bool tooltipsEnabled = true;

        private PDFViewer currentlyOpenedPDFViewer;

        /// <summary>
        /// This Scene Manager does not only manage the Scene it provides the data for the CustomContent interface
        /// the Data Interface Instance is on the game object "Interface" as a dll component. The interface project itself
        /// is an separate visual studio .NET Project.
        /// </summary>
        protected virtual void Awake()  // Todo: remove virtual when other SceneManagers finally removed.
        {
            //fetch all objects with guid for custom content.
            foreach (Transform komp in tabHolder.GetComponentsInChildren<Transform>(true))
            {
                komp.TryGetComponent<UIComponent_Tab>(out var comp);
                if (comp != null) MainToggles.Add(komp.gameObject);
            }
            foreach (Transform komp in toggleHolder.GetComponentsInChildren<Transform>(true))
            {
                komp.TryGetComponent<UIComponent_Step>(out var comp);
                if (comp != null) MainToggles.Add(komp.gameObject);
            }

            instance = this;

            highlighterToggles.AddRange(toggleHolder.GetComponentsInChildren<ToggleListener>());
            highlighterToggles.RemoveAll(t => t.highlighter == null);

            for (var i = 0; i < highlighterToggles.Count; i++)
            {
                var idx = i;
                highlighterToggles[i].GetComponent<Toggle>().onValueChanged.AddListener(isOn =>
                {
                    SetHighlighterState(new IndexedToggleState { index = idx, state = isOn });
                });
            }

            //Generates the data for interface.
            Interface.DataInterface.DATAObject = viewerInterface.transform.GetComponent<Interface.DataInterface>();
            foreach (var komp in MainToggles)
            {
                komp.transform.TryGetComponent<UIComponent_Step>(out var stepChild);
                komp.transform.TryGetComponent<UIComponent_Tab>(out var stepChildTab);
                if (stepChildTab != null)
                {
                    Interface.InteractorData actor = new Interface.InteractorData(komp.transform.GetComponent<UIComponent_Tab>().Guid, "Toggles", komp.transform.gameObject);
                    foreach (var tool in stepChildTab.Toggles)
                        actor.AddObj(tool.GetComponent<Tooltip>().Guid, tool.transform.gameObject);
                    Interface.DataInterface.DATAObject.Data.Add(actor);

                }

                if (stepChild != null)
                {
                    Interface.InteractorData actor = new Interface.InteractorData(komp.transform.GetComponent<UIComponent_Step>().Guid, "Toggles", komp.transform.gameObject);
                    /*komp.transform.TryGetComponent<LupenHandler>(out var tooltipObj);
					if (tooltipObj != null)
					{*/
                    foreach (var tool in stepChild.Toggles)
                    {
                        if (tool.TryGetComponent<Tooltip>(out var toolTip))
                        {

                            actor.AddObj(tool.GetComponent<Tooltip>().Guid, tool.transform.gameObject);
                        }
                        //else
                        //{
                        //    actor.AddObj(tool.GetComponent<GuidComponent>().Guid, tool.transform.gameObject);
                        //}
                    }
                    //}
                    Interface.DataInterface.DATAObject.Data.Add(actor);

                }
            }
        }

        // Start is called before the first frame update
        public virtual void Start()
        {
            // Multiplayer setup

            particleSystems = particlesHolder.GetComponentsInChildren<ParticleSystem>(true);
            StartCoroutine(DoDelayedStart());
        }

        private IEnumerator DoDelayedStart()
        {
            yield return null;

            // Run Awake-routines in all-controlled panels (activate and deactivate). Awake() will run without any frame in between.
            //foreach (var tab in tabToggles)
            Toggle[] tabs = tabHolder.gameObject.GetComponentsInChildren<Toggle>();
            foreach (Toggle tab in tabs)
            {
                tab.isOn = true;     // To run Awake() in all tab's panels' scripts
                tab.isOn = false;    // To switch all tabs off

                // March 2020: Dedicated camera start points for each tab, assigned via Inspector
                //if (DeviceSwitcher.instance.device == AppType.TA)
                //{
                //    tab.onValueChanged.AddListener(delegate
                //    {
                //        MoveCamera.instance.ResetCamera();
                //    });
                //}
            }
            //tabToggles[0].isOn = true;  // First panel has to be activated by default
            tabs[0].isOn = true;  // First panel has to be activated by default
        }

        /// <summary>
        /// Actual highlighting routine.
        /// </summary>
        /// <param name="doHighlight"></param>
        /// <param name="container"></param>
        public virtual void HighlightModelPart(bool doHighlight, ModelHighlighter highlighter)
        {
            highlighter.Highlight(doHighlight);
        }

        /// <summary>
        /// Home Button functionality = Unload current AssetBundle.
        /// </summary>
        public void UnloadAssetBundle()
        {
            if (DontDestroyOnLoadAccessor.instance.startSceneName != aioStartSceneName)
            {
                Debug.Log("Unloading AssetBundle...");
                var dontDestroyOnLoadObjects = DontDestroyOnLoadAccessor.instance.GetDontDestroyOnLoadObjects();
                for (int i = dontDestroyOnLoadObjects.Length - 1; i >= 0; i--)
                {
                    DestroyImmediate(dontDestroyOnLoadObjects[i]);
                }

                HoloRepair.Core.ContentAppInterface.ExitContentScene();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(aioStartSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
        }

        public void ToggleTooltips()
        {
            Tooltip.tooltipsEnabled = !Tooltip.tooltipsEnabled;
            Tooltip.UpdateAllTooltipStates();
        }

        public void OpenPDF(PDFViewer pdfViewer)
        {
            ClosePDF();
            OpenPDF(pdfViewer, true);
        }
        public void OpenPDF(PDFViewer pdfViewer, int page)
        {
            ClosePDF();
            OpenPDF(pdfViewer, true);
            pdfViewer.GetComponent<PDFFetcher>().Setup();        
            //pdfViewer.CurrentPageIndex = page;
            pdfViewer.GoToPage(page);
        }


        public void ClosePDF()
        {
            //// Close PDF is synced via Code here, not via SyncedButton component, as this is very prone to naming errors. Could be changed using GUIDs for syncing buttons etc. in future release.
            //NetworkMessageController.instance.SendNetworkMessage("SceneManager", System.Reflection.MethodBase.GetCurrentMethod().Name);

            if (currentlyOpenedPDFViewer != null)
            {
                OpenPDF(currentlyOpenedPDFViewer, false);
            }
        }

        public void OpenPDF(PDFViewer pdfViewer, bool open)
        {
            pdfViewer.gameObject.SetActive(open);

            if (DeviceSwitcher.Instance.device == AppType.TA)
            {
                canvasAREP.SetActive(!open);
                //Camera.main.GetComponent<OrientationChecker>().EnablePortraitOrientation(open);
            }
            else if (DeviceSwitcher.Instance.device == AppType.HL)
            {
                panelSideIcons.SetActive(!open);
            }

            currentlyOpenedPDFViewer = open ? pdfViewer : null;
        }

        public void UpdateParticleSystemScaling(bool doUpdate)
        {
            foreach (var ps in particleSystems)
            {
                var pathFlow = ps.GetComponent<BLINDED_AM_ME.ParticlePathFlow>();
                if (pathFlow != null)
                {
                    pathFlow.isPathUpdating = doUpdate;
                }
            }
        }

        public void UnfocusDetail()
        {
            StartCoroutine(DoUnfocusDetail());
        }

        private IEnumerator DoUnfocusDetail()
        {
            yield return StartCoroutine(Tooltip.focussedTooltip.DoUnfocus());

            Tooltip.ResetAllTooltipStates();
            ModelControl.Instance.FadeModelOut(false);

            if (DeviceSwitcher.Instance.device == AppType.TA)
            {
                yield return new WaitForSeconds(MoveCamera.instance.settings.timeCameraReset / 2);
                MoveCamera.instance.ResetCameraToLastOrbitPosition();
                SlidePanel.instance.DeactivatePanel(false);
            }
            else if (DeviceSwitcher.Instance.device == AppType.HL)
            {
                DeviceSwitcher.Instance.canvasMenuHL.gameObject.SetActive(true);
            }
        }

        public void LoadSceneForStandalone(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        // Datenflug
        #region From Datenflug

        private ModelHighlighter GetHighlighterFromIndex(int index)
        {
            if (index < 0 || index >= highlighterToggles.Count)
            {
                Debug.LogError($"Highlighter index out of bounds ({index})");
                return null;
            }
            return highlighterToggles[index].highlighter;
        }

        public void SetHighlighterState(IndexedToggleState highlighterState)
        {
            var target = GetHighlighterFromIndex(highlighterState.index);
            if (target == null)
            {
                Debug.LogError("Highlighter missing, cannot set state", this);
                return;
            }
            HighlightModelPart(highlighterState.state, target);
        }

        public void SetHighlighterStateByTooltip(ModelHighlighter highlighter)
        {
            for (int index = 0; index < highlighterToggles.Count; index++)
            {
                if (highlighter == highlighterToggles[index].highlighter)
                {
                    SetHighlighterState(new IndexedToggleState { index = index, state = true });
                    return;
                }
            }
        }

        #endregion

        // AREP Settings Menu
        #region AREP Settings Menu

        public void OnSettingsButtonHome()
        {
            HoloRepair.Core.ContentAppInterface.BackToStartPage();
        }

        public void OnSettingsButtonQuit()
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
			HoloRepair.Core.ContentAppInterface.QuitApplication();
#else
            Application.Quit();
#endif
        }

        public void OnButtonConnectionDialog()
        {
            HoloRepair.Core.ContentAppInterface.OpenConnectionDialog(canvasAREP.transform);
        }

        #endregion
    }

}