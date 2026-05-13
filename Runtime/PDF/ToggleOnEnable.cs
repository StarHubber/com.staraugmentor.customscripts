using Paroxe.PdfRenderer.Internal.Viewer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleOnEnable : MonoBehaviour
{

    private void OnEnable()
    {
        GetComponent<PDFViewerLeftPanel>().SetOpened(false);
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
