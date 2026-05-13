using Microsoft.MixedReality.Toolkit.Input;
using Paroxe.PdfRenderer;
using STAR.Utils;
using StarCooperation.ExportCCP;
using StarCooperation.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StarCooperation
{
    public enum TooltipState
    {
        Hotspot,
        Tooltip,
        Highlight,
        Focus,
        Hide,
        TooltipExtended
    }

    public class Tooltip : MonoBehaviour,
        IMixedRealityPointerHandler,
        IMixedRealityFocusHandler,
        IPointerClickHandler
    {
        public string Guid;
        public GameEvent TooltipClickedEvent;
        public UIComponent CorrespondingUIElement;

        public static Tooltip focussedTooltip;
        public static bool tooltipsEnabled = true;
        //public static bool allowInteraction = true;

        [Header("Settings")]
        public TooltipState startState = TooltipState.Hide;
        public bool alwaysKeepStartState = false;
        public bool zoomToDetail = false;
        public bool hasPdf = false;
        public PDFViewer pdfViewer;
        public bool showExtendedStats = false;
        public GameEvent zoomEvent;
        public explode_auto explosionComponent;
        [TextArea(0, 3)]
        public string text;
        public Color highlightColor = Color.yellow;
        public float cameraFocusDistance = 2;

        //[Header("Scaling")]
        //public bool scaleWithCameraDistance = true;

        [Header("Basic References")]
        public Transform hotspot;
        public Transform tooltip;

        [Header("Focus / Zoom")]
        public GameObject tooltipZoomPanel;
        public SpriteRenderer iconFocus;
        public GameObject closeFocusSphere;
        public bool copyCallbacksFromTAButton = false;
        public Button tabletButtonCloseDetail;

        [Header("PDF")]
        public GameObject tooltipPdfPanel;
        public SpriteRenderer iconPdf;

        [Header("Interaction Settings")]
        public Color colorInteractionDisabled = Color.gray;

        [Header("Highlight (deprecated)")]
        public GameObject tooltipHighlightTrigger;
        public TextMeshPro tooltipText;

        [Header("Extension")]
        public GameObject extendedPanel;
        public GameObject extendedStatsPanel;

        [Header("Lines")]
        public GameObject targetLines;
        public Transform targetRotationCenter;

        [Header("Geometry")]
        public Transform geometry;
        public List<GameObject> deactivateOnFocus;

        [Header("Highlighter")]
        public ModelHighlighter modelHighlighter;
        [SerializeField] private bool highlightingEnablesTooltip = true;
        [SerializeField] private bool highlightingEnablesExtendedTooltip = false;
        [SerializeField] private bool highlightingHighlightsTooltip = false;

        [HideInInspector] public Button connectedButtonLupe;
        [HideInInspector] public Button connectedButtonDoc;

        [HideInInspector] public bool isHighlighted = false;

        private static List<Tooltip> allTooltips = new List<Tooltip>();

        public TooltipState State { get; private set; }
        private TooltipState storedState;
        private Animator geometryAnimator;
        private Animator popUpAnimator;

        private float scaleFactorForCamera;
        private Color textBackgroundDefaultColor;

        // Hololens
        private GameObject hololensFocussedObject;

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(Guid))
                Guid = GUID.Generate().ToString();
#endif
        }

        private void OnEnable()
        {
            // Set interaction mode on startup = read interaction flag on startup of scene.
            // For more foolproofness, e.g. when Tooltips have been disabled before (or groups of Tooltips disabled),
            // read this flag on every Enable.
            UpdateInteractionMode();
        }

        private void Awake()
        {
            popUpAnimator = GetComponent<Animator>();

            // Get geometry animator, has to be in first child of geometry GameObject
            if (geometry.transform.childCount > 0)
            {
                geometryAnimator = geometry.transform.GetChild(0).GetComponent<Animator>();

                // Holy moly shitty manoly: Unity uses dummy nulls with a custom "==" operator.
                // But: ? and ?? operator (C#) can not be overloaded, so geometryAnimator? will not equal to null when GetComponent returned null.
                // Fix: if it equals to null, assign a "proper" null again, so ? will see a proper null.
                // See: https://blogs.unity3d.com/2014/05/16/custom-operator-should-we-keep-it/?_ga=2.186101353.1740129198.1571725668-436580837.1561610321
                // and: https://answers.unity.com/questions/1243356/getcomponent-returns-null-however-comparison-to-nu.html
                if (geometryAnimator == null)
                {
                    geometryAnimator = null;
                }
            }

            allTooltips.Add(this);

            textBackgroundDefaultColor = tooltipHighlightTrigger.GetComponent<MeshRenderer>().material.color;

            if (explosionComponent != null)
            {
                GameEventListener[] comps = this.GetComponents<GameEventListener>();
                GameEventListener comp = null;
                foreach (var c in comps)
                {
                    if (c.gameEvent.name == "GE_Tooltip_ZoomToDetail")
                        comp = c;
                }

                if (comp != null)
                {
                    comp.response_bool_false.AddListener((value) => explosionComponent.StartReset());
                    comp.response_string.AddListener((value) => explosionComponent.StartExplosion(value));
                }
            }
        }

        private void Start()
        {
            // ! Bugfix: Magnifier panel/extended panel etc. needs to be setup in Start(), because
            // connectedButtonLupe is set in Awake() of LupenHandler. To strongly avoid race condition in Awake() routines,
            // this setup needs to come in Start().
            // TODO (Optimization): Magnifier and Doc button activating/hiding should follow the same logic for clarity...
            if (!zoomToDetail && !showExtendedStats && connectedButtonLupe == null)
            {
                tooltipZoomPanel.SetActive(false);
            }
            tooltipPdfPanel.SetActive(hasPdf);

            if (showExtendedStats)
            {
                tooltipZoomPanel.SetActive(true);
            }

            SetState(startState);

            State = startState;
            // If localizer is not enabled or has no key, write text from Tooltip-Script into text field (otherwise don't, localizer will!)
            var localizer = GetComponent<LegacyLocalization.LocalizedTextBase>();
            if (localizer != null)
            {
                if (string.IsNullOrEmpty(localizer.key) || !localizer.enabled)
                {
                    tooltipText.text = text;
                    tooltipText.SetAllDirty();
                }
            }

            //scaleFactorForCamera = 1 / (MoveCamera.instance?.settings.distOrbitDefault ?? 1);
            //scaleWithCameraDistance = MoveCamera.instance != null ? scaleWithCameraDistance : false;
            if (modelHighlighter != null)
            {
                HighlighterStateChanged(modelHighlighter.isHighlighted); // Run once in Start() to highlight very first time after enabling
                try
                {
                    modelHighlighter.OnModelHighlighted.AddListener(HighlighterStateChanged);
                }
                catch (Exception e)
                {
                    Debug.LogError("Catched: " + e);
                }
            }

            InteractionControl.UserInteractionsDisabled += OnUserInteractionsDisabled;
        }

        private void OnDestroy()
        {
            allTooltips.Remove(this);
            InteractionControl.UserInteractionsDisabled -= OnUserInteractionsDisabled;
        }


        private void LateUpdate()
        {
            if (tooltip.gameObject.activeInHierarchy)   // Only look at Camera when hotspots visible, otherwise LineTargets will get false default position in Awake
            {
                // Rotate tooltip towards camera
                if (DeviceSwitcher.Instance.device == AppType.TA)
                {
                    tooltip.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
                }
                else if (DeviceSwitcher.Instance.device == AppType.HL)
                {
                    tooltip.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Vector3.up);
                }

                //hotspot.rotation = Quaternion.LookRotation(-Camera.main.transform.forward);	// No longer needed, is just a sphere (not plus-sign or Lupe)

                //if (scaleWithCameraDistance)
                //{
                //	hotspot.localScale = Vector3.one * Vector3.Distance(MoveCamera.instance.rotationCenter.transform.position, Camera.main.transform.position) * scaleFactorForCamera;
                //	tooltip.localScale = Vector3.one * Vector3.Distance(MoveCamera.instance.rotationCenter.transform.position, Camera.main.transform.position) * scaleFactorForCamera;
                //}
            }
        }

        public void HighlighterStateChanged(bool isHighlighted)
        {
            if (isHighlighted)
            {
                if (highlightingEnablesTooltip)
                {
                    SetState(TooltipState.Tooltip);
                }
                if (highlightingHighlightsTooltip)
                {
                    SetState(TooltipState.Highlight);
                }
                if (highlightingEnablesExtendedTooltip)
                {
                    SetState(TooltipState.TooltipExtended);
                }
            }
            else
            {
                SetState(TooltipState.Hotspot);
            }
        }


        public void Highlight(string guid)
        {
            bool doHighlight = false;
            if (!isHighlighted && CorrespondingUIElement.Guid == guid)
            {
                doHighlight = true;
            }
            Highlight(doHighlight);
            HighlighterStateChanged(doHighlight);
        }
        /// <summary>
        /// Highlight tooltip panel (text background) on click.
        /// </summary>
        public void Highlight(bool highlight)
        {
            isHighlighted = highlight;

            var meshRend = tooltipHighlightTrigger.GetComponent<MeshRenderer>();
            if (isHighlighted)
            {
                textBackgroundDefaultColor = meshRend.material.color;
                meshRend.material.color = highlightColor;
            }
            else
            {
                meshRend.material.color = textBackgroundDefaultColor;
            }
        }

        /// <summary>
        /// Set states of all tooltips in scene.
        /// </summary>
        /// <param name="state"></param>
        public static void SetAllTooltipStates(TooltipState state, Tooltip exception = null)
        {
            foreach (var tooltip in allTooltips)
            {
                if (exception != null)
                {
                    if (tooltip == exception)
                    {
                        continue;
                    }
                }
                tooltip.SetState(state);
            }
        }

        public static void ResetAllTooltipStates()
        {
            foreach (var tooltip in allTooltips)
            {
                tooltip.ResetState();
            }
        }

        /// <summary>
        /// Set state of this tooltip.
        /// </summary>
        /// <param name="state"></param>
        public void SetState(TooltipState state)
        {
            if (alwaysKeepStartState && state != startState)
            {
                return;
            }

            storedState = this.State;
            this.State = state;
            UpdateTooltipState();
        }

        /// <summary>
        /// Reset tooltip to state that it was before.
        /// </summary>
        public void ResetState()
        {
            if (alwaysKeepStartState)
            {
                return;
            }
            State = storedState;
            UpdateTooltipState();
        }

        /// <summary>
        /// Coroutine to run geometry animation backwards before showing rest of model and reset camera. Called from extern.
        /// </summary>
        /// <returns></returns>
        public IEnumerator DoUnfocus()
        {
            focussedTooltip.zoomEvent.RaiseBool(false);
            focussedTooltip = null;
            float waitTime = 0;
            if (geometryAnimator != null)
            {
                // Reverse animation method from: http://gyanendushekhar.com/2016/10/28/reverse-animation-play-unity3d/
                var currentState = geometryAnimator.GetCurrentAnimatorStateInfo(0);
                geometryAnimator.SetFloat("Direction", -1);
                geometryAnimator.Play(currentState.fullPathHash, 0, Mathf.Clamp01(currentState.normalizedTime));

                //Small Fix for LB10 - when Animations > 10 seconds cap them to 1.
                waitTime = currentState.length;
                if (waitTime > 9)
                {
                    waitTime = 1f;
                }
            }

            yield return new WaitForSeconds(waitTime);
        }

        public static void UpdateAllTooltipStates()
        {
            foreach (var tooltip in allTooltips)
            {
                tooltip.UpdateTooltipState();
            }
        }

        /// <summary>
        /// Update the detail elements depending on current state.
        /// </summary>
        public void UpdateTooltipState()
        {
            // Activate element (= tooltip) when tooltips are enabled, OR disabled but parent has tooltip.
            // The later means this tooltip is a "sub tooltip" within geometry that shall be activated = visible on focus.
            bool activateElement = tooltipsEnabled || transform.parent.GetComponentInParent<Tooltip>();

            switch (State)
            {
                case TooltipState.Hotspot:
                    //hotspot.gameObject.SetActive(activateElement && allowInteraction); // Interaction disabled also means hotspots are not shown
                    hotspot.gameObject.SetActive(activateElement && !InteractionControl.InteractionsDisabled); // Interaction disabled also means hotspots are not shown
                    tooltip.gameObject.SetActive(false);
                    targetLines.SetActive(false);
                    if (extendedPanel != null)
                    {
                        extendedStatsPanel.SetActive(false);
                    }

                    closeFocusSphere.SetActive(false);
                    geometry.gameObject.SetActive(false);

                    if (deactivateOnFocus != null)
                    {
                        foreach (var obj in deactivateOnFocus)
                        {
                            obj.SetActive(true);
                        }
                    }
                    popUpAnimator?.SetInteger("AniState", 1);
                    geometryAnimator?.SetInteger("AniState", 0);
                    break;

                case TooltipState.Tooltip:
                    hotspot.gameObject.SetActive(false);
                    if (extendedPanel != null)
                    {
                        extendedPanel.SetActive(false);
                    }

                    tooltip.gameObject.SetActive(activateElement);
                    targetLines.SetActive(activateElement);
                    closeFocusSphere.SetActive(false);
                    geometry.gameObject.SetActive(false);
                    if (deactivateOnFocus != null)
                    {
                        foreach (var obj in deactivateOnFocus)
                        {
                            obj.SetActive(true);
                        }
                    }
                    popUpAnimator?.SetInteger("AniState", 0);
                    geometryAnimator?.SetInteger("AniState", 0);
                    break;

                case TooltipState.Focus:
                    hotspot.gameObject.SetActive(false);
                    tooltip.gameObject.SetActive(false);
                    targetLines.SetActive(false);
                    if (DeviceSwitcher.Instance.device == AppType.HL)
                    {
                        //closeFocusSphere.SetActive(allowInteraction);
                        closeFocusSphere.SetActive(!InteractionControl.InteractionsDisabled);
                    }
                    geometry.gameObject.SetActive(true);
                    if (deactivateOnFocus != null)
                    {
                        foreach (var obj in deactivateOnFocus)
                        {
                            obj.SetActive(false);
                        }
                    }
                    geometryAnimator?.SetFloat("Direction", 1);

                    if (DeviceSwitcher.Instance.device == AppType.TA)
                    {
                        StartCoroutine(DoStartAnimationDelayed("AniState", 1));
                    }
                    else if (DeviceSwitcher.Instance.device == AppType.HL)
                    {
                        //geometry.gameObject.SetActive(true);
                        geometryAnimator?.SetInteger("AniState", 1);
                    }

                    // New: sync (only) focus state
                    break;

                case TooltipState.Highlight:
                    hotspot.gameObject.SetActive(false);
                    tooltip.gameObject.SetActive(activateElement);
                    targetLines.SetActive(false);
                    closeFocusSphere.SetActive(false);
                    geometry.gameObject.SetActive(false);
                    if (deactivateOnFocus != null)
                    {
                        foreach (var obj in deactivateOnFocus)
                        {
                            obj.SetActive(true);
                        }
                    }
                    geometryAnimator?.SetInteger("AniState", 0);
                    Highlight(true);
                    break;

                case TooltipState.Hide:
                    Highlight(false);
                    hotspot.gameObject.SetActive(false);
                    tooltip.gameObject.SetActive(false);
                    targetLines.SetActive(false);
                    closeFocusSphere.SetActive(false);
                    if (extendedPanel != null)
                    {
                        extendedStatsPanel.SetActive(false);
                    }

                    geometry.gameObject.SetActive(false);
                    if (deactivateOnFocus != null)
                    {
                        foreach (var obj in deactivateOnFocus)
                        {
                            obj.SetActive(true);
                        }
                    }
                    geometryAnimator?.SetInteger("AniState", 0);
                    break;

                case TooltipState.TooltipExtended:
                    hotspot.gameObject.SetActive(false);
                    tooltip.gameObject.SetActive(activateElement);
                    if (extendedPanel != null)
                    {
                        extendedPanel.SetActive(true);
                    }

                    targetLines.SetActive(activateElement);
                    closeFocusSphere.SetActive(false);
                    geometry.gameObject.SetActive(false);
                    if (deactivateOnFocus != null)
                    {
                        foreach (var obj in deactivateOnFocus)
                        {
                            obj.SetActive(true);
                        }
                    }
                    popUpAnimator?.SetInteger("AniState", 0);
                    geometryAnimator?.SetInteger("AniState", 0);
                    break;

                default:
                    break;
            }
        }

        private void OnUserInteractionsDisabled(bool disabled)
        {
            UpdateTooltipState();
            UpdateInteractionMode();
        }

        private void UpdateInteractionMode()
        {
            var iconColor = InteractionControl.InteractionsDisabled ? colorInteractionDisabled : Color.white;
            iconPdf.color = iconColor;
            iconFocus.color = iconColor;
        }

        /// <summary>
        /// Coroutine to wait for starting animation after model has been fade out and camera has focused the detail.
        /// </summary>
        /// <param name="animationName"></param>
        /// <param name="animationValue"></param>
        /// <returns></returns>
        private IEnumerator DoStartAnimationDelayed(string animationName, int animationValue)
        {
            yield return new WaitForSeconds(MoveCamera.instance.settings.timeFocusDetail);
            geometryAnimator?.SetInteger(animationName, animationValue);
        }

        /// <summary>
        /// Pointer click via MOUSE
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            //if (!allowInteraction)
            if (InteractionControl.InteractionsDisabled)
            {
                return;
            }

            if (eventData.button != PointerEventData.InputButton.Left)
            {
                Debug.Log("Right click on tooltip has no effect.");
                return;
            }

            // Hotspot clicked
            if (eventData.hovered.Contains(hotspot.gameObject))
            {
                if (!tooltip.gameObject.activeInHierarchy)
                {
                    Debug.Log(Guid);
                    TooltipClickedEvent.RaiseString(Guid);
                    modelHighlighter.Highlight(true);   // Setting new state hotspot -> tooltip via event callback (see above)
                }
            }

            // Magnifier clicked
            else if (eventData.hovered.Contains(tooltipZoomPanel.gameObject))
            {
                if (zoomToDetail == true)
                {
                    ZoomToDetail();
                }

                if (connectedButtonLupe != null)
                {
                    connectedButtonLupe.onClick.Invoke();
                }
            }

            // PDF icon clicked
            else if (eventData.hovered.Contains(tooltipPdfPanel.gameObject))
            {
                try
                {
                    SceneManager.instance.OpenPDF(pdfViewer.GetComponent<PDFViewer>(), pdfViewer.GetComponent<PDFFetcher>().page);
                } catch(Exception e)
                {
                    Debug.Log(e.Message + ": maybe Old PDF Verison used.");
                }

                if (connectedButtonDoc != null)
                {
                    connectedButtonDoc.onClick.Invoke();
                }
            }
        }

        /// <summary>
        /// Pointer clock via HOLOLENS
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            //if (!allowInteraction)
            if (InteractionControl.InteractionsDisabled)
            {
                return;
            }

            // Hotspot clicked
            if (hololensFocussedObject.FindGameObjectInParents(hotspot.gameObject))
            {
                if (!tooltip.gameObject.activeInHierarchy)
                {
                    modelHighlighter.Highlight(true);   // Setting new state hotspot -> tooltip via event callback (see above)
                }
            }

            // Closing sphere clicked
            else if (hololensFocussedObject.FindGameObjectInParents(closeFocusSphere))
            {
                CloseFocussedTooltip();
            }

            // Magnifier clicked
            else if (hololensFocussedObject.FindGameObjectInParents(tooltipZoomPanel.gameObject))
            {
                // Tooltip/Magnifier pressed
                if (zoomToDetail == true)
                {
                    ZoomToDetail();
                }

                if (connectedButtonLupe != null)
                {
                    connectedButtonLupe.onClick.Invoke();
                }
            }

            // PDF icon clicked
            else if (hololensFocussedObject.FindGameObjectInParents(tooltipPdfPanel.gameObject))
            {
                try
                {
                    SceneManager.instance.OpenPDF(pdfViewer.GetComponent<PDFViewer>(), pdfViewer.GetComponent<PDFFetcher>().page);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message + ": maybe Old PDF Verison used.");
                }

                if (connectedButtonDoc != null)
                {
                    connectedButtonDoc.onClick.Invoke();
                }
            }
        }

        /// <summary>
        /// Focus function for tooltip.
        /// </summary>
        public void ZoomToDetail()
        {
            zoomEvent.RaiseString(this.Guid);
            zoomEvent.RaiseBool(true);
            focussedTooltip = this;
            SetAllTooltipStates(TooltipState.Hide, this);
            SetState(TooltipState.Focus);
            //ModelControl.Instance.FadeModelOut(true);

            if (DeviceSwitcher.Instance.device == AppType.TA)
            {
                MoveCamera.instance.FocusDetail(this);
                SlidePanel.instance.DeactivatePanel(true);
            }
            else
            {
                DeviceSwitcher.Instance.canvasMenuHL?.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Opposite of ZoomToDetail. Static because it (also) needs to be called for all tooltips externally.
        /// </summary>
        public static void CloseFocussedTooltip()
        {
            focussedTooltip.zoomEvent.RaiseBool(false);
            if (focussedTooltip != null)
            {
                if (focussedTooltip.copyCallbacksFromTAButton && focussedTooltip.tabletButtonCloseDetail != null)
                {
                    focussedTooltip.tabletButtonCloseDetail.onClick.Invoke();
                }
                else
                {
                    SceneManager.instance.UnfocusDetail();
                }
            }
        }
        public static void CloseFocusedTooltipWithoutNotifyNetwork()
        {
            focussedTooltip.zoomEvent.RaiseBool(false);
            if (focussedTooltip != null)
            {
                if (focussedTooltip.copyCallbacksFromTAButton && focussedTooltip.tabletButtonCloseDetail != null)
                {
                    focussedTooltip.tabletButtonCloseDetail.onClick.Invoke();
                }
                else
                {
                    SceneManager.instance.UnfocusDetail();
                }
            }
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

        public void OnFocusEnter(FocusEventData eventData)
        {
            hololensFocussedObject = eventData.NewFocusedObject;
        }

        public void OnFocusExit(FocusEventData eventData)
        {
        }

        public static Tooltip GetTooltipByName(string name)
        {
            return allTooltips.Find(tooltip => tooltip.gameObject.name == name);
        }
    }
}