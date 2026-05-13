using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using HoloRepair.Core;
using System.Linq;
using System;
using System.Threading.Tasks;
using STAR.Utils;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace StarCooperation
{
    public class UIController_Infobox : MonoBehaviour
    {
        public Image DiagramImage;

        [Header("Main Content")]
        [SerializeField] private RectTransform viewPortRectTrafo;
        [SerializeField] private TextMeshProUGUI textStepNumber;
        [SerializeField] private TextMeshProUGUI textDescription;
        [SerializeField] private RectTransform mainContentPanel;

        [Header("Step Buttons")]
        [SerializeField] private bool onlyShowIfStepsHidden = true;
        [SerializeField] private GameObject stepButtonParent;
        [SerializeField] private Button buttonNextStep;
        [SerializeField] private Button buttonPrevStep;

        [Header("Window Control Buttons")]
        [SerializeField] private Button buttonShowContainer;
        [SerializeField] private Button buttonMinimizeContainer;
        [SerializeField] private Button buttonMaximizeContainer;
        [SerializeField] private Button buttonNormalizeContainer;

        [Header("Other Buttons")]
        [SerializeField] private Button buttonSubTutorial;

        [Header("Behaviour")]
        [SerializeField] private float movementTime = 1;
        [SerializeField] private AnimationCurve movementCurve;

        [Header("Unity Events")]
        [SerializeField] private UnityEvent onNextButtonClick;
        [SerializeField] private UnityEvent onPrevButtonClick;
        [SerializeField] private UnityEvent_String onSubTutorialOpenAttempt;

        public UnityEvent triggerAssetUpdate;

        private void OnEnable()
        {
            triggerAssetUpdate.Invoke();
        }

        // Privates
        private enum ViewState
        {
            Minimized,
            Normalized,
            Maximized
        }

        private ViewState viewState = ViewState.Normalized; // Default View State

        private float normalizedContainerPosY;
        private float maximizedContainerPosY;
        private bool coroutineRunning = false;

        private LocalizedString currentString;
        private LocalizedAsset<Sprite> currentSprite;

        private void Awake()
        {
            // Fetch heights from transform
            normalizedContainerPosY = mainContentPanel.anchoredPosition.y;
            maximizedContainerPosY = mainContentPanel.rect.height;

            // Hide inactive objects
            buttonShowContainer.gameObject.SetActive(false);
            buttonNextStep.gameObject.SetActive(false);
            buttonPrevStep.gameObject.SetActive(false);
            buttonSubTutorial.gameObject.SetActive(false);

            // Assign callbacks
            //
            buttonShowContainer.onClick.AddListener(() => UpdateViewState(ViewState.Normalized));
            buttonMinimizeContainer.onClick.AddListener(() => UpdateViewState(ViewState.Minimized));
            buttonNormalizeContainer.onClick.AddListener(() => UpdateViewState(ViewState.Normalized));
            buttonMaximizeContainer.onClick.AddListener(() => UpdateViewState(ViewState.Maximized));
            //
            // Prev/Next button throughput via UnityEvents to raise GameEvents
            buttonNextStep.onClick.AddListener(() => onNextButtonClick?.Invoke());
            buttonPrevStep.onClick.AddListener(() => onPrevButtonClick?.Invoke());

            // Default state: Normalized
            UpdateViewState(viewState);


        }
        private void Start()
        {
            currentString = null;
            currentSprite = null;
            UpdateInfobox("");
            UpdateAsset(null);
        }


        private void UpdateViewState(ViewState newViewState)
        {
            if (coroutineRunning)
                return;

            viewState = newViewState;

            buttonMinimizeContainer.gameObject.SetActive(newViewState == ViewState.Normalized);
            buttonNormalizeContainer.gameObject.SetActive(newViewState == ViewState.Maximized);
            buttonMaximizeContainer.gameObject.SetActive(newViewState == ViewState.Normalized);
            //   Tabs.ForEach(x => x.gameObject.SetActive(newViewState == ViewState.Normalized || newViewState == ViewState.Maximized));
            //buttonShowContainer.gameObject.SetActive(newViewState == ViewState.Minimized);	// new: after coroutine, looks better
            stepButtonParent.SetActive(!(newViewState == ViewState.Minimized));
            mainContentPanel.gameObject.SetActive(!(newViewState == ViewState.Minimized));
            StartCoroutine(CoResizeContainer(newViewState));
        }

        private IEnumerator CoResizeContainer(ViewState newViewState)
        {
            coroutineRunning = true;

            if (newViewState != ViewState.Minimized)
                buttonShowContainer.gameObject.SetActive(false);

            float targetPosY = 0;

            if (newViewState == ViewState.Minimized)
                targetPosY = 0;
            else if (newViewState == ViewState.Normalized)
                targetPosY = normalizedContainerPosY;
            else if (newViewState == ViewState.Maximized)
                targetPosY = maximizedContainerPosY;

            float startPosY = mainContentPanel.anchoredPosition.y;

            if (startPosY != targetPosY)
            {
                float t = 0;
                while (t < 1)
                {
                    t += Time.deltaTime / movementTime;
                    t = Mathf.Clamp01(t);

                    float curveT = movementCurve.Evaluate(t);
                    float newPosY = Mathf.Lerp(startPosY, targetPosY, curveT);

                    // Move panel
                    mainContentPanel.anchoredPosition = new Vector2(
                        mainContentPanel.anchoredPosition.x,
                        newPosY);

                    // Adjust viewport to visible area
                    viewPortRectTrafo.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newPosY + viewPortRectTrafo.anchoredPosition.y);

                    yield return null;
                }
            }

            if (newViewState == ViewState.Minimized)
                buttonShowContainer.gameObject.SetActive(true);

            coroutineRunning = false;
            yield return null;
        }

        public void UpdateInfobox(string textInfoString)
        {
            textDescription.text = textInfoString;

        }
        public void OnLanguageChange()
        {

            SetInfoText(currentString?.GetLocalizedString());
            var localizedAsset = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>(currentSprite.TableReference, currentSprite.TableEntryReference);
            SetDiagramPicture(localizedAsset);
        }
        public void UpdateInfobox(LocalizedString textInfoString, LocalizedAsset<Sprite> asset)
        {
            UpdateText(textInfoString);
            UpdateAsset(asset);
        }

        private void UpdateAsset(LocalizedAsset<Sprite> asset)
        {
            if (currentSprite is not null)
                currentSprite.AssetChanged -= SetDiagramPicture;

            if (asset == null)
            {
                SetDiagramPicture(null);
                return;
            }

            if (asset.IsEmpty || asset == null)
            {
                SetDiagramPicture(null);
                return;
            }

            var localizedAsset = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>(asset.TableReference, asset.TableEntryReference);


            SetDiagramPicture(localizedAsset);
            currentSprite = asset;
            currentSprite.AssetChanged += SetDiagramPicture;
        }

        private void UpdateText(LocalizedString textInfoString)
        {
            if (currentString is not null)
                currentString.StringChanged -= SetInfoText;



            if (textInfoString.IsEmpty)
            {
                SetInfoText("");
                return;
            }

            currentString = textInfoString;
            currentString.StringChanged += SetInfoText;
            SetInfoText(currentString.GetLocalizedString());
        }

        private void SetDiagramPicture(Sprite tex)
        {
            DiagramImage.sprite = tex;
        }

        private void SetInfoText(string text)
        {
            textDescription.text = text;
        }
        public void ToggleVisibilityInvert(bool enable)
        {
            mainContentPanel.gameObject.SetActive(!enable);
            buttonShowContainer.gameObject.SetActive(!enable);
        }
        public void ToggleVisibility(bool enable)
        {
            mainContentPanel.gameObject.SetActive(enable);
            buttonShowContainer.gameObject.SetActive(enable);
        }
    }
}