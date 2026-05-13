using StarCooperation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StarCooperation
{
    public struct AderInfo
    {
        public string ID { get; set; }
        public string Nr { get; set; }
        public string AderNr { get; set; }
        public string Benennung { get; set; }
        public string Steckkontakt { get; set; }
        public string Dichtungsteil { get; set; }
        public string Leitungsklasse { get; set; }
        public string Farbe { get; set; }
        public string Querschnitt { get; set; }
        public string Endstecker { get; set; }
        public string Endkammer { get; set; }
        public List<Color> AderColor { get; set; }

        public AderInfo(string[] cells, ColumnID column, string id)
        {
            this.AderColor = ParseStringToColor(cells[column.LeitungsFarbe]);
            this.Nr = cells[column.KammerNummer];
            this.AderNr = cells[column.AderNummer];
            this.Benennung = cells[column.TeileBeschreibung];
            this.Steckkontakt = cells[column.SteckKontaktNummer];
            this.Dichtungsteil = cells[column.DichtungsTeileNummer];
            this.Leitungsklasse = cells[column.LeitungsKlasse];
            this.Querschnitt = cells[column.Querschnitt];
            this.Farbe = cells[column.Farbe];
            this.Endstecker = cells[column.EndStecker];
            this.Endkammer = cells[column.EndKammer];
            this.ID = id;
        }
        private static List<Color> ParseStringToColor(object leitungsfarbe)
        {
            List<Color> colorContainer = new List<Color>();

            switch (leitungsfarbe)
            {
                case "SW/BR":
                    {
                        colorContainer.Add(new Color(0, 0, 0, 1));
                        colorContainer.Add(new Color(0.54f, 0.27f, 0.07f, 1));

                    }
                    break;
                case "GN/RT":
                    {
                        colorContainer.Add(Color.green);
                        colorContainer.Add(Color.red);

                    }
                    break;
                case "RT/GN":
                    {
                        colorContainer.Add(Color.red);
                        colorContainer.Add(Color.green);

                    }
                    break;
                case "GN/WS":
                    {
                        colorContainer.Add(Color.green);
                        colorContainer.Add(Color.white);

                    }
                    break;
                case "OR/GR":
                    {
                        Color orange = new Color(1, 0.65f, 0);
                        colorContainer.Add(orange);
                        colorContainer.Add(Color.grey);

                    }
                    break;
                case "BL/GE":
                    {

                        colorContainer.Add(Color.blue);
                        colorContainer.Add(Color.yellow);

                    }
                    break;
                case "BR/RT":
                    {

                        colorContainer.Add(new Color(0.54f, 0.27f, 0.07f, 1));
                        colorContainer.Add(Color.red);

                    }
                    break;
                case "GE/GR":
                    {

                        colorContainer.Add(Color.yellow);
                        colorContainer.Add(Color.grey);

                    }
                    break;
                case "VI/GE":
                    {

                        colorContainer.Add(new Color(.52f, 0, 1));
                        colorContainer.Add(Color.yellow);

                    }
                    break;
                case "BR/BL":
                    {

                        colorContainer.Add(new Color(0.54f, 0.27f, 0.07f, 1));
                        colorContainer.Add(Color.blue);

                    }
                    break;
                case "BL":
                    {

                        colorContainer.Add(Color.blue);
                        colorContainer.Add(Color.blue);

                    }
                    break;
                case "OR":
                    {
                        colorContainer.Add(new Color(1, 0.65f, 0));
                        colorContainer.Add(new Color(1, 0.65f, 0));


                    }
                    break;
                case "RT/GR":
                    {
                        colorContainer.Add(Color.red);
                        colorContainer.Add(Color.green);


                    }
                    break;
                case "SW/RS":
                    {
                        colorContainer.Add(Color.black);
                        colorContainer.Add(new Color(.9f, .5f, .6f));


                    }
                    break;
                case "SW/RT":
                    {
                        colorContainer.Add(Color.black);
                        colorContainer.Add(Color.red);


                    }
                    break;
                case "SW/GR":
                    {
                        colorContainer.Add(Color.black);
                        colorContainer.Add(Color.green);


                    }
                    break;
                case "BR":
                    {
                        colorContainer.Add(new Color(.54f, .2f, .07f));
                        colorContainer.Add(new Color(.54f, .2f, .07f));
                    }
                    break;
                case "SW/WS":
                    {
                        colorContainer.Add(Color.black);
                        colorContainer.Add(Color.white);
                    }
                    break;
                case "SW/BL":
                    {
                        colorContainer.Add(Color.black);
                        colorContainer.Add(Color.blue);
                    }
                    break;
                case "GR/BL":
                    {
                        colorContainer.Add(Color.grey);
                        colorContainer.Add(Color.blue);
                    }
                    break;
                case "VI/GN":
                    {
                        colorContainer.Add(new Color(.6f, .1f, .8f));
                        colorContainer.Add(Color.green);
                    }
                    break;
                case "WS":
                    {
                        colorContainer.Add(Color.white);
                        colorContainer.Add(Color.white);
                    }
                    break;
                case "BL/RT":
                    {
                        colorContainer.Add(Color.blue);
                        colorContainer.Add(Color.red);
                    }
                    break;
                case "BR/SW":
                    {
                        colorContainer.Add(new Color(.54f, .2f, .07f));
                        colorContainer.Add(Color.black);
                    }
                    break;
                case "GN/GE":
                    {
                        colorContainer.Add(Color.green);
                        colorContainer.Add(Color.yellow);
                    }
                    break;
                case "RS/BL":
                    {
                        colorContainer.Add(new Color(.9f, .5f, .6f));
                        colorContainer.Add(Color.blue);
                    }
                    break;
                case "GE/VI":
                    {
                        colorContainer.Add(Color.yellow);
                        colorContainer.Add(new Color(.6f, .1f, .8f));
                    }
                    break;
                case "RS/GR":
                    {
                        colorContainer.Add(new Color(.9f, .5f, .6f));
                        colorContainer.Add(Color.grey);
                    }
                    break;
                case "GE/GN":
                    {
                        colorContainer.Add(Color.yellow);
                        colorContainer.Add(Color.green);
                    }
                    break;
                case "RS/GE":
                    {
                        colorContainer.Add(new Color(.9f, .5f, .6f));
                        colorContainer.Add(Color.yellow);
                    }
                    break;
                case "WS/BR":
                    {
                        colorContainer.Add(Color.white);
                        colorContainer.Add(new Color(.54f, .2f, .07f));
                    }
                    break;
                case "GE/RT":
                    {
                        colorContainer.Add(Color.yellow);
                        colorContainer.Add(Color.red);
                    }
                    break;
                case "RT/BL":
                    {
                        colorContainer.Add(Color.red);
                        colorContainer.Add(Color.blue);
                    }
                    break;
                case "RS/SW":
                    {
                        colorContainer.Add(new Color(.9f, .5f, .6f));
                        colorContainer.Add(Color.black);
                    }
                    break;
                case "GN/VI":
                    {
                        colorContainer.Add(Color.green);
                        colorContainer.Add(new Color(.6f, .1f, .8f));
                    }
                    break;
                case "SW":
                    {
                        colorContainer.Add(Color.black);
                        colorContainer.Add(Color.black);
                    }
                    break;
                case "BL/RS":
                    {
                        colorContainer.Add(Color.blue);
                        colorContainer.Add(new Color(.9f, .5f, .6f));
                    }
                    break;
                case "WS/BL":
                    {
                        colorContainer.Add(Color.white);
                        colorContainer.Add(Color.blue);
                    }
                    break;

                case "BR/WS":
                    {
                        colorContainer.Add(new Color(.54f, .2f, .07f));
                        colorContainer.Add(Color.white);
                    }
                    break;
                case "RT/WS":
                    {
                        colorContainer.Add(Color.red);
                        colorContainer.Add(Color.white);
                    }
                    break;
                case "WS/GE":
                    {
                        colorContainer.Add(Color.white);
                        colorContainer.Add(Color.yellow);
                    }
                    break;
                case "WS/GN":
                    {
                        colorContainer.Add(Color.white);
                        colorContainer.Add(Color.green);
                    }
                    break;
                case "GR/RT":
                    {
                        colorContainer.Add(Color.grey);
                        colorContainer.Add(Color.red);
                    }
                    break;
                case "GE/WS":
                    {
                        colorContainer.Add(Color.yellow);
                        colorContainer.Add(Color.white);
                    }
                    break;
                case "GE/SW":
                    {
                        colorContainer.Add(Color.yellow);
                        colorContainer.Add(Color.black);
                    }
                    break;
                case "GE":
                    {
                        colorContainer.Add(Color.yellow);
                        colorContainer.Add(Color.yellow);
                    }
                    break;
                case "GN":
                    {
                        colorContainer.Add(Color.green);
                        colorContainer.Add(Color.green);
                    }
                    break;
                case "SW/GN":
                    {
                        colorContainer.Add(Color.black);
                        colorContainer.Add(Color.green);
                    }
                    break;
                case "GR/GN":
                    {
                        colorContainer.Add(Color.grey);
                        colorContainer.Add(Color.green);
                    }
                    break;
                case "SW/GE":
                    {
                        colorContainer.Add(Color.black);
                        colorContainer.Add(Color.yellow);
                    }
                    break;
                case "VI":
                    {
                        colorContainer.Add(new Color(.6f, .1f, .8f));
                        colorContainer.Add(new Color(.6f, .1f, .8f));
                    }
                    break;
                case "SW/VI":
                    {
                        colorContainer.Add(Color.black);
                        colorContainer.Add(new Color(.6f, .1f, .8f));
                    }
                    break;
                case "RT":
                    {
                        colorContainer.Add(Color.red);
                        colorContainer.Add(Color.red);
                    }
                    break;
                case "BL/VI":
                    {
                        colorContainer.Add(Color.blue);
                        colorContainer.Add(new Color(.6f, .1f, .8f));
                    }
                    break;
                case "BR/VI":
                    {
                        colorContainer.Add(new Color(.54f, .2f, .07f));
                        colorContainer.Add(new Color(.6f, .1f, .8f));
                    }
                    break;
                case "RT/SW":
                    {
                        colorContainer.Add(Color.red);
                        colorContainer.Add(Color.black);
                    }
                    break;
                case "BL/SW":
                    {
                        colorContainer.Add(Color.blue);
                        colorContainer.Add(Color.black);
                    }
                    break;


                default:
                    colorContainer.Add(new Color(0, 0, 0, 1));
                    colorContainer.Add(new Color(0, 0, 0, 1));
                    break;
            }
            return colorContainer;
        }
    }
    public class AderContainer
    {
        public ToggleEar ToggleEar { get; set; }
        public AderInfo AderInfo { get; set; }

        public AderContainer(AderInfo aderInfo, ToggleEar toggleEar)
        {
            if (toggleEar == null)
            {
               // aderInfo.Endstecker = "-";
            }
            AderInfo = aderInfo;
            ToggleEar = toggleEar;
        }

    }
}