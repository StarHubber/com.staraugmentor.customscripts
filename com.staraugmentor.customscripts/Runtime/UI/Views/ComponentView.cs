using Paroxe.PdfRenderer;
using StarCooperation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ComponentView : MonoBehaviour
{
    [Header("Component")]
    public StepContentGroup Component;

    [Header("Items Parent")]
    public GameObject GroupItems;
    public GameObject Slider;
    private List<StepView> ContentItems;

    public TextMeshProUGUI TitleText;


    [Header("Toggles")]
    public Toggle ToggleShowItems;
    public Toggle ToggleVisibility;
    public Toggle ToggleCollapse;
    public Toggle TogglePlay;
    public Toggle PDFToggle;
    public GameObject PDFButton;

    [Header("Listening Settings")]
    public bool HighlightEnablesComponent = true;
    public bool ShowExplosionIcon = true;

    private bool doHighlight = true;
    private void Awake()
    {
        if (HighlightEnablesComponent)
            Component.Highlighter.OnModelHighlighted.AddListener((s) => OnHighlight(s));

        ToggleShowItems.onValueChanged.AddListener((s) => OnToggleClick(s));
        ToggleCollapse.onValueChanged.AddListener(OnToggleCollapse);
        ToggleVisibility.onValueChanged.AddListener(OnToggleVisibility);
        TogglePlay.onValueChanged.AddListener(OnTogglePlay);
        PDFToggle.onValueChanged.AddListener(OnPDFToggleClick);
        FindObjectOfType<UIController_Diagram>(true).triggerAssetUpdate.AddListener(UpdateAssetOnEnable);
        GetContentItems();
        CheckForChildren();
    }

    private void OnHighlight(bool arg0)
    {
        doHighlight = false;
        ToggleShowItems.isOn = arg0;

    }
    private void OnPDFToggleClick(bool arg0)
    {
        SceneManager.instance.OpenPDF(Component.PDFInfoLink.GetComponent<PDFViewer>(), Component.pdfPage);

    }

    private void OnToggleClick(bool isOn)
    {
        foreach (var item in Component.ToolTips.GetComponentsInChildren<Tooltip>(true))
        {
            item.gameObject.SetActive(isOn);
        }

        //if (PDFLink != null)
        UpdatePDFButton();
        UpdateInfoBox(isOn);
        if (DeviceSwitcher.Instance! != null)
        {
            if (DeviceSwitcher.Instance!.device != AppType.HL)
                MoveCamera(isOn);
        }
        ToggleSlider(isOn);

        if (doHighlight)
            Component.Highlighter.Highlight(isOn);

        if (isOn)
        {
            ToggleCollapse.isOn = isOn;
        }
        if (!isOn)
        {
            ContentItems.ForEach(x => x.ToggleItem(isOn));
        }
        doHighlight = true;
    }

    private void OnEnable()
    {
        Component.Title.StringChanged += UpdateText;
    }
    private void UpdateAssetOnEnable()
    {
        if (ToggleShowItems.isOn || TogglePlay.isOn)
            FindObjectOfType<UIController_Diagram>()?.UpdateInfobox(Component.Information, Component.Diagram);
    }

    private void OnTogglePlay(bool arg0)
    {
        if (ShowExplosionIcon)
        {
           Component.Tooltip.GetComponent<Tooltip>().ZoomToDetail();

        }
        else
        {
            Component.ParticleScenario.ActivateScenario(arg0);
            UpdateAssetOnEnable();
        }
    }

    public void ToggleSlider(bool arg0)
    {
        if (arg0)
            Slider.gameObject.SetActive(true);
        else
            Slider.gameObject.SetActive(false);
    }

    private void OnToggleCollapse(bool arg0)
    {
        OnToggleShowItems(arg0);
        //  Component.ToolTips.SetActive(arg0);
        if (!arg0)
            OnToggleVisibility(arg0);
    }

    public void SetActiveComponent(bool isOn)
    {
        ToggleShowItems.isOn = isOn;
        if (!isOn)
        {
            ContentItems.ForEach(x => x.ToggleItem(isOn));
        }
    }
    public void OnToggleVisibility(bool arg0)
    {
        foreach (var item in ContentItems)
        {
            item.ToggleVisibility(arg0);

        }
    }

    public void OnToggleShowItems(bool toggle)
    {
        foreach (var contentItem in ContentItems)
        {
            if (!toggle)
                contentItem.ToggleItem(toggle);

            contentItem.gameObject.SetActive(toggle);
        }
    }

    private void GetContentItems()
    {
        ContentItems = GroupItems.GetComponentsInChildren<StepView>().ToList();
    }
    public void MoveCamera(bool toggle)
    {
        if (toggle)
            FindObjectOfType<MoveCamera>().SetNewCameraLocation(Component.CameraDefault);
    }

    public void UpdateInfoBox(bool toggle)
    {
        if (toggle)
            FindObjectOfType<UIController_Diagram>()?.UpdateInfobox(Component.Information, Component.Diagram);
    }
    public void UpdatePDFButton()
    {
        if (Component.PDFLink == null)
        {
            PDFButton.SetActive(false);
            return;
        }

        PDFButton.SetActive(true);
        PDFButton.GetComponent<Button>().onClick.AddListener(() => SceneManager.instance.OpenPDF(Component.PDFLink.GetComponent<PDFViewer>()));
        PDFButton.GetComponentInChildren<TextMeshProUGUI>().text = Component.PDFLink.GetComponent<PDFFetcher>().PDFFileName.Count.ToString();


    }

    private void UpdateText(string value)
    {
        TitleText.text = value;
    }


    public void CheckForChildren()
    {
        var groupItems = transform.GetChild(1);
        if (groupItems.childCount > 0)
        {
            ToggleShowItems.isOn = true;
            // manually set bool to true as otherwise it will be false, even if group is opened
        }
    }


}
