using Paroxe.PdfRenderer;
using StarCooperation;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StepView : MonoBehaviour
{
    [Header("Component")]
    public StepContentItem Component;

    [SerializeField] private Toggle visibilityToggle, pdfToggle;
    public TextMeshProUGUI TitleText;

    private ComponentView ParentComponent;



    void Start()
    {
        ParentComponent = GetComponentInParent<ComponentView>();
        GetComponent<Toggle>().onValueChanged.AddListener(OnToggleClick);
        Component.Title.StringChanged += OnLanguageChange;
        Component.Highlighter.OnModelHighlighted.AddListener(OnHighlight);
        visibilityToggle?.onValueChanged.AddListener(OnVisibilityToggleClick);
        pdfToggle?.onValueChanged.AddListener(OnPDFToggleClick);


    }

    private void OnPDFToggleClick(bool arg0)
    {
        SceneManager.instance.OpenPDF(Component.PDFLink.GetComponent<PDFViewer>(), Component.pdfPage);

    }

    private void OnHighlight(bool arg0)
    {
        GetComponent<Toggle>().isOn = arg0;
    }

    private void OnVisibilityToggleClick(bool toggle)
    {
        Component.LowLighter.Highlight(toggle);
        //  transparentHighlighter.SetTransparent(toggle);

    }

    private void OnToggleClick(bool isOn)
    {
        //need to make sure here that the Component this step belongs to is now the active component
        if (isOn)
        {
            ParentComponent.SetActiveComponent(isOn);
            UpdateInfoBox(isOn);
            if (DeviceSwitcher.Instance.device != AppType.HL)
                SetCameraDefault(isOn);
        }

        Component.Highlighter.Highlight(isOn);
        Component.ToolTips.GetComponent<Tooltip>().HighlighterStateChanged(isOn);

        if (Component is StepContentItem)
            if ((Component).Scenario != null)
                Component.Scenario.ActivateScenario(isOn);
    }

    private void SetCameraDefault(bool arg0)
    {
        if ((arg0))
            FindObjectOfType<MoveCamera>().SetNewCameraLocation(Component.CameraDefault);
    }

    private void OnLanguageChange(string value)
    {
        TitleText.text = value;
    }

    //External Toggle Input
    public void ToggleItem(bool toggle)
    {
        GetComponent<Toggle>().isOn = toggle;
    }

    private void UpdateInfoBox(bool toggle)
    {
        if (toggle)
            FindObjectOfType<UIController_Infobox>()?.UpdateInfobox(Component.Information, Component.Diagram);
    }

    public void ToggleVisibility(bool arg0)
    {
        visibilityToggle.SetIsOnWithoutNotify(arg0);
        OnVisibilityToggleClick(arg0);
        visibilityToggle.GetComponent<ToggleInteractionDesign>().ToggleListener();
    }
}
