using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarCooperation
{
    public struct SteckerInfo
    {
        public string id, description;
        public string farbe;
        public string teileNummer;
        public string benennung;
        public string pins;

        public SteckerInfo(string id, string teileBeschreibung, string farbe, string teileNummer, string bauteilBeschreibung, string polzahl)
        {
            this.id = id;
            this.description = teileBeschreibung;
            this.farbe = farbe;
            this.teileNummer = teileNummer;
            this.benennung = bauteilBeschreibung;
            this.pins = polzahl;

        }


    }

    public class Stecker
    {     //Global List of Steckers. Every Instance of Stecker will add itself to the list on creation
        public static List<Stecker> SteckerList;
        public List<AderContainer> aderContainer;
        public ModelHighlighter Highlighter { get; set; }
        public ToggleEar Toggle { get; set; }
        public SteckerInfo SteckerInfo { get; set; }

        public Stecker(SteckerInfo steckerInfo)
        {
            SteckerInfo = steckerInfo;
            SetHighlighter();

        }

        private void SetHighlighter()
        {
            foreach (var item in ToggleEar.ToggleList)
            {
                string temp = item.GetComponent< LegacyLocalization.LocalizedTextAuto >().key;
                string[] split = temp.Split('_');
                if (split[1] == SteckerInfo.id)
                {
                    this.Highlighter = item.Highlighter;
                    this.Toggle = item;
                    item.Stecker = this;
                }
            }
        }
        public override string ToString() => SteckerInfo.id;
    }
}