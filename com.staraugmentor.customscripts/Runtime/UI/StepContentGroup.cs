using Paroxe.PdfRenderer;
using StarCooperation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class TTTContent : MonoBehaviour
{
    [Header("Localization")]
    public LocalizedString Title;
    public LocalizedString Information;
    public LocalizedAsset<Sprite> Diagram;
    [Space(25)]
    public Transform CameraDefault;
    [Space(25)]
    public GameObject PDFLink,PDFInfoLink;
    public int pdfPage;
    
}
public class StepContentGroup : TTTContent
{
    public ModelHighlighter Highlighter;
    public GameObject ToolTips, Tooltip;
    public ParticleScenario ParticleScenario;
}