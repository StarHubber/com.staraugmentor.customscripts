using HoloRepair.Core;
using StarCooperation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CellCentric.h2gesamt
{
    public class PDF_ListController : MonoBehaviour
    {
        public string id;
        public GameObject button;
        public PDFFetcher fetcher;

        [SerializeField] private Color mHazardColor;
        [SerializeField] private Color mWarningColor;
        [SerializeField] private Color mCautionColor;
        [SerializeField] private Color mInformationColor;

        // Start is called before the first frame update
        void Awake()
        {
            ReadAndCreateButtons();
        }
        private void Start()
        {
            //set default
            if (fetcher.isList)
            {
                fetcher.filenameList[0].button.GetComponent<Toggle>().isOn = true;
                fetcher.SetupPdfFilePathList(fetcher.filenameList[0]);
            }
        }
        // Update is called once per frame
        void Update()
        {

        }

        public void ReadAndCreateButtons()
        {
            for (int i = 0; i < fetcher.PDFFileName.Count; i++)
            {
                GameObject listButton = Instantiate(button);
                listButton.transform.SetParent(this.transform, false);
                listButton.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
                listButton.GetComponentInChildren<TextMeshProUGUI>().text = fetcher.PDFButtonName[i];

                if(fetcher.PDFButtonPrio.Count != 0)
                {
                    listButton.transform.GetChild(1).gameObject.SetActive(true);
                    InformationType type = fetcher.PDFButtonPrio[i];
                    // Assign color depening on warning level
                    Color c = Color.clear;
                    switch (type)
                    {
                        case InformationType.Vorsicht:
                            c = mCautionColor;
                            break;
                        case InformationType.Warnung:
                            c = mWarningColor;
                            break;
                        case InformationType.Gefahr:
                            c = mHazardColor;
                            break;
                        case InformationType.AllgemeinerHinweis:
                            c = mInformationColor;
                            break;
                    }
                    listButton.transform.GetChild(1).GetComponent<Image>().color = c;
                }

                listButton.GetComponent<Toggle>().onValueChanged.AddListener(GetPDF);
                listButton.GetComponent<Toggle>().group = this.transform.GetComponent<ToggleGroup>();

                //create class element
                PDFFetcher.PDF e = new PDFFetcher.PDF();
                e.name = fetcher.PDFButtonName[i];
                e.PDFFilename = fetcher.PDFFileName[i];
                e.button = listButton;
                fetcher.filenameList.Add(e);
            }


        }

        public void GetPDF(bool b)
        {
            GameObject button = EventSystem.current.currentSelectedGameObject;

            foreach (PDFFetcher.PDF obj in fetcher.filenameList)
            {
                if (obj.button == button)
                    fetcher.SetupPdfFilePathList(obj);
            }
        }
    }
}
