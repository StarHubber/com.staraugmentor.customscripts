using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using StarCooperation.Localization;
using System;
using UnityEngine.UI;

namespace StarCooperation
{

    public class CSVMain : MonoBehaviour
    {
        [SerializeField] private string steckerInfoPath;
        [SerializeField] private bool checkForUmlaute = false;
        public string[] descriptors, separators;
        private string line;
        private string[] cells;
        private int columnCount;
        private ColumnID column;
        private StreamReader reader;

        private void OnEnable()
        {
            LegacyLocalization.Localizer.OnLanguageChanged += ParseLang;

        }
        private void OnDisable() => LegacyLocalization.Localizer.OnLanguageChanged -= ParseLang;

        private void ParseLang()
        {
            //Parse(Application.streamingAssetsPath + "/StarCooperation/SteckerInformation" + Localizer.instance.GetLanguageIsoCode() + ".csv");
            Parse(Application.streamingAssetsPath + steckerInfoPath + LegacyLocalization.Localizer.instance.GetLanguageIsoCode() + ".csv");


        }
        private void Start()
        {
            Parse(Application.streamingAssetsPath + steckerInfoPath + LegacyLocalization.Localizer.instance.GetLanguageIsoCode() + ".csv");

        }
        public void ParseLanguage(Dropdown dropdown)
        {
            string langHolo = GetLanguageCode(dropdown);
            checkForUmlaute = langHolo == "DE" ? true : false;
            Stecker.SteckerList = null;
            Parse(Application.streamingAssetsPath + steckerInfoPath + langHolo + ".csv");

        }

        private void Parse(string filepath)
        {
            using (var readOnlyFs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (reader = new StreamReader(readOnlyFs, System.Text.Encoding.GetEncoding("ISO-8859-1")))
                {    //Get Column Indexes by Reading the first Line.
                    column = GetColumnIndex(ReadFirstLine());

                    while (CheckAndSplitNextLine())
                    {
                        if (!int.TryParse(cells[column.Polzahl], out int followingAderCount) && followingAderCount == 0) continue;

                        if (LegacyLocalization.Localizer.instance.GetLanguageIsoCode() == "DE") CheckAndSetUmlaute(cells);

                        CreateSteckerAndAddToSteckerList(followingAderCount);
                    }
                }
            }

        }
        private string[] ReadFirstLine()
        {
            line = reader.ReadLine();
            cells = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return cells;
        }
        private bool CheckAndSplitNextLine()
        {
            line = reader.ReadLine();
            if (line == null) return false;

            cells = line.Split(separators, StringSplitOptions.None);
            CheckAndSetEmptyCells();
            return true;
        }

        private void CreateSteckerAndAddToSteckerList(int polZahl)
        {
            Stecker tempStecker = CreateStecker();
            tempStecker.aderContainer = CreateAderDetails(polZahl);
            SteckerHandler.Instance.AddStecker(tempStecker);
        }
        private Stecker CreateStecker()
        {
            SteckerInfo steckerInfo = new SteckerInfo(cells[column.ID], cells[column.TeileBeschreibung], cells[column.Farbe], cells[column.TeileNummer], cells[column.BauteilBeschreibung], cells[column.Polzahl]);
            return new Stecker(steckerInfo);

        }
        private List<AderContainer> CreateAderDetails(int polZahl)
        {
            List<AderContainer> aderContainer = new List<AderContainer>();

            AddAderToContainer(aderContainer);

            for (int i = 1; i < polZahl; i++)
            {
                if (!CheckAndSplitNextLine()) continue;
                AddAderToContainer(aderContainer);
            }
            return aderContainer;
        }
        private void AddAderToContainer(List<AderContainer> aderContainer)
        {
            AderContainer aderRow = new AderContainer(new AderInfo(cells, column, cells[column.ID]), TryGetEndsteckerToggle(cells[column.EndStecker]));
            aderContainer.Add(aderRow);
        }

        private ToggleEar TryGetEndsteckerToggle(string endsteckerString)
        {
            if (endsteckerString == null || endsteckerString == "")
                return null;

            //Parse String and look for an existing Tooltip.
            string[] endstecker = endsteckerString.Split('-', ' ');
            endstecker[0].Replace(" ", string.Empty);

            foreach (var item in ToggleEar.ToggleList)
            {
                string temp = item.GetComponent< LegacyLocalization.LocalizedTextAuto >().key;
                string[] split = temp.Split('_');
                if (split[1] == endstecker[0])
                    return item;

            }
            return null;

        }
        private ColumnID GetColumnIndex(string[] cells)
        {
            ColumnID Column = new ColumnID();
            columnCount = cells.Length;
            for (int i = 0; i < cells.Length; i++)
            {
                if (descriptors[i].Contains("ID"))
                {
                    Column.ID = i;
                }
                else if (descriptors[i].Contains("Teilenummer"))
                {
                    Column.TeileNummer = i;
                }
                else if (descriptors[i] == "Teilebeschreibung")
                {
                    Column.TeileBeschreibung = i;
                }
                else if (descriptors[i].Contains("Bauteil-Beschreibung"))
                {
                    Column.BauteilBeschreibung = i;
                }
                else if (descriptors[i].Contains("Abkürzung"))
                {
                    Column.Abkuerzung = i;
                }
                else if (descriptors[i].Contains("Polzahl"))
                {
                    Column.Polzahl = i;
                }
                else if (descriptors[i].Contains("Gehäusefarbe"))
                {
                    Column.Farbe = i;
                }
                else if (descriptors[i].Contains("Kammernummer"))
                {
                    Column.KammerNummer = i;
                }
                else if (descriptors[i].Contains("Draht/Ader-Nummer"))
                {
                    Column.AderNummer = i;
                }
                else if (descriptors[i].Contains("Steckkontakt-Beschreibung"))
                {
                    Column.SteckKontakt = i;
                }
                else if (descriptors[i].Contains("Steckkontaktteilenummer"))
                {
                    Column.SteckKontaktNummer = i;
                }
                else if (descriptors[i].Contains("Dichtungsteilenummer"))
                {
                    Column.DichtungsTeileNummer = i;
                }
                else if (descriptors[i].Contains("Leitungsklasse"))
                {
                    Column.LeitungsKlasse = i;
                }
                else if (descriptors[i].Contains("Drahtnummer"))
                {
                    Column.DrahtNummer = i;
                }
                else if (descriptors[i].Contains("Leitungsquerschnitt"))
                {
                    Column.Querschnitt = i;
                }
                else if (descriptors[i].Contains("Leitungsfarbe"))
                {
                    Column.LeitungsFarbe = i;
                }
                else if (descriptors[i].Contains("Anfangsstecker"))
                {
                    Column.AnfangsStecker = i;
                }
                else if (descriptors[i].Contains("Anfangskammer"))
                {
                    Column.AnfangsKammer = i;
                }
                else if (descriptors[i].Contains("Endstecker"))
                {
                    Column.EndStecker = i;
                }
                else if (descriptors[i].Contains("Endkammer"))
                {
                    Column.EndKammer = i;
                }
            }
            return Column;
        }
        private string GetLanguageCode(Dropdown languageDropdown)
        {
            string langPhat = string.Empty;
            string langHolo = string.Empty;
            switch (languageDropdown.value)
            {
                case 0: // DE
                    langPhat = "de-CH";
                    langHolo = "DE";
                    break;
                case 1: // FR
                    langPhat = "fr-CH";
                    langHolo = "FR";
                    break;
                case 2: // IT
                    langPhat = "it-CH";
                    langHolo = "IT";
                    break;
            }

            return langHolo;
        }
        private void CheckAndSetEmptyCells()
        {
            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i].Length == 0 || cells[i] == "")
                {
                    if (column.ID == i)
                    {
                        continue;

                    }
                    cells[i] = "-";
                }
            }
        }
        private void CheckAndSetUmlaute(string[] cells)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = replaceGermanUmlauts(cells[i]);
            }

        }
        private String replaceGermanUmlauts(String s)
        {
            String t = s;
            t = t.Replace("ae", "ä");
            t = t.Replace("oe", "ö");
            // t = t.Replace("ue", "ü");
            t = t.Replace("Ae", "Ä");
            t = t.Replace("Oe", "Ö");
            t = t.Replace("Ue", "Ü");
            //   t = t.Replace("ss", "ß");
            return t;
        }
    }

    public struct ColumnID
    {
        public int ID, TeileNummer, TeileBeschreibung, BauteilBeschreibung, Abkuerzung, Polzahl, Farbe, KammerNummer, AderNummer,
            SteckKontakt, SteckKontaktNummer, DichtungsTeileNummer, Module, LeitungsKlasse, DrahtNummer, Querschnitt, LeitungsFarbe,
            AnfangsStecker, AnfangsKammer, EndStecker, EndKammer, Line;

    }

}



