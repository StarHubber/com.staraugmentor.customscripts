using HoloRepair.Core;
using Paroxe.PdfRenderer;
using StarCooperation.LegacyLocalization;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
    [RequireComponent(typeof(PDFViewer))]
    public class PDFFetcher : MonoBehaviour
    {
        public string subFolderPathPdf = "StarCooperation/PDF";

        public class PDF
        {
            public string name;
            public string PDFFilename;
            public GameObject button;
        }

        [Space(5)]
        public bool isList = false;
        public string filename;
        public int page;
        public List<string> PDFButtonName;
        public List<InformationType> PDFButtonPrio;
        public List<string> PDFFileName;
        public List<PDF> filenameList = new List<PDF>();

        private PDFViewer viewer;
        [SerializeField]
        private Button closeButton;

        // Start is called before the first frame update

        private void Awake()
        {
            viewer = GetComponent<PDFViewer>();
            closeButton.onClick.AddListener(SceneManager.instance.ClosePDF);
            if (!isList)
            {
                SetupPdfFilePath();
                viewer.LoadDocument();
            }
        }
        private void OnEnable()
        {
            Localizer.OnLanguageChanged += SetupPdfFilePath;

        }
        private void OnDisable()
        {
            Localizer.OnLanguageChanged += SetupPdfFilePath;

        }
        public void Setup()
        {
            SetupPdfFilePath();
            viewer.LoadDocument();
        }


        private void SetupPdfFilePath()
        {
            var languageIsoCode = Localizer.instance.GetLanguageIsoCode();

            if (!filename.EndsWith(".pdf"))
            {
                filename += ".pdf";
            }

            var subFolderPathPdfLanguage = subFolderPathPdf + "/" + languageIsoCode;
            viewer.Folder = subFolderPathPdfLanguage;   // PDF viewer is already set up to look in SteamingAssets folder, so only sub folder needed

            if (!File.Exists(Path.Combine(Application.streamingAssetsPath, subFolderPathPdfLanguage, filename)))
            {
                viewer.FileName = "Error.pdf";
            }
            else
            {
                viewer.FileName = filename;
            }
        }

        public void SetupPdfFilePathList(PDF id)
        {
            var languageIsoCode = Localizer.instance.GetLanguageIsoCode();

            filename = id.PDFFilename;

            if (!filename.EndsWith(".pdf"))
            {
                filename += ".pdf";
            }

            var subFolderPathPdfLanguage = subFolderPathPdf + "/" + languageIsoCode;
            viewer.Folder = subFolderPathPdfLanguage;   // PDF viewer is already set up to look in SteamingAssets folder, so only sub folder needed

            if (!File.Exists(Path.Combine(Application.streamingAssetsPath, subFolderPathPdfLanguage, filename)))
            {
                viewer.FileName = "Error.pdf";
            }
            else
            {
                viewer.FileName = filename;
            }

            viewer.LoadDocument();
        }

        /// <summary>
        /// Load Document with current settings from PDF Viewer.
        /// </summary>
        private void LoadPdf()
        {
            viewer.LoadDocument();
        }
    }
}