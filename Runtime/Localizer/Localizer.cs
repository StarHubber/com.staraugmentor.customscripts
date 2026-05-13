using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;



namespace StarCooperation.LegacyLocalization
{
    /// <summary>
    /// This class realizes translation.
    /// Usage: Add LocalizeTextAuto component to element that contains (or one of its children) any kind of text component: TextMeshProUGUI, TextMeshPro, TextMesh, Text.
    /// LocalizeTextAuto automatically searches for any of these components.
    /// Alternatively, use LocalizeText/TextMesh/TM/TMP components on actual text component (children are not searched).
    /// </summary>
    /// 

    // Localizer needs to run even before DeviceSwitcher (exec order -1000), because elements get activated manually after switching to HoloLens,
    // which can result in unloaded localizer CSV. Result would be tabs without text!
    [DefaultExecutionOrder(-1001)]
    public class Localizer : MonoBehaviour
    {
        public static Localizer instance;

        public static List<string> AvailableLanguages => new List<string>(languageMap.Keys);

        [Tooltip("File path underneath Streaming Assets path.")]
        public string filePath;
        public string[] separators;

        private static Dictionary<string, Dictionary<string, string>> languageMap;
        private static string currentIsoCode;

        public static event Action OnLanguageChanged;
        public static event Action OnLocalizerLoaded;
        public static event Action OnTextsUpdated;

        private async void Awake()
        {
            instance = this;

            currentIsoCode = HoloRepair.Core.ContentAppInterface.CurrentLanguageCode;
            await LoadLanguageMapFile(filePath);

            //OnLanguageChanged += BroadcastLanguage;
        }

        private void OnDestroy()
        {
            //OnLanguageChanged -= BroadcastLanguage;
        }

        /*private void BroadcastLanguage()
        {
            NetworkMessageController.instance.SendNetworkMessage("Localizer", instance.GetLanguageIsoCode(), "True");
        }*/
        /// <summary>
        /// /// Fetches a file from the given URI and Returns the bytes.
        /// </summary>
        /// <param name="filePath">The URI converted to a string.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<byte[]> ParseData(string filePath)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(filePath))
            {
                www.SendWebRequest();
                while (!www.isDone)
                    await Task.Yield();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Received");
                    return www.downloadHandler.data;
                }
                else
                {
                    //return www.downloadHandler.data;
                    throw new Exception("Download failed.");
                }
            }
        }
        private async Task LoadLanguageMapFile(string filePath)
        {
            filePath = Application.streamingAssetsPath + filePath;

            Uri uri = new Uri(filePath);

            byte[] file = await ParseData(uri.ToString());

            if (file != null)
            {
                languageMap = new Dictionary<string, Dictionary<string, string>>();

                /*using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            string[] values = line.Split(separators, StringSplitOptions.None);

                            if (string.IsNullOrEmpty(values[0]))
                            {
                                continue;
                            }

                            if (!headerParsed)
                            {
                                if (values[0] == "Key")
                                {

                                    // Only for header line, remove empty entries (recreate values via Split() is simplest)
                                    values = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                                    isoCodes = new string[values.Length - 1];
                                    for (int i = 0; i < isoCodes.Length; i++)
                                    {
                                        isoCodes[i] = values[i + 1];
                                        languageMap.Add(isoCodes[i], new Dictionary<string, string>());
                                    }

                                    headerParsed = true;
                                    continue;
                                }
                            }

                            for (int i = 0; i < isoCodes.Length; i++)
                            {
                                if (!languageMap[isoCodes[i]].ContainsKey(values[0]))
                                {
                                    try
                                    {
                                        languageMap[isoCodes[i]].Add(values[0], values[i + 1]);

                                    }
                                    catch (Exception)
                                    {

                                        //Debug.LogWarning("Values not set for " + values[0]);
                                    }
                                }
                                else
                                {
                                    Debug.LogWarning("Localizer key already exists: " + values[0]);
                                    break;
                                }
                            }
                        }
                    }
                }*/

                languageMap = await ParseLines(file);
                OnLocalizerLoaded?.Invoke();
                //return languageMap;

            }
            else
            {
                Debug.LogWarning("The file " + filePath + " does not exist in StreamingAssets (sub)folder.");
                //return null;
            }
        }

        private async Task<Dictionary<string, Dictionary<string, string>>> ParseLines(byte[] file)
        {
            languageMap = new Dictionary<string, Dictionary<string, string>>();
            string[] isoCodes = null;
            bool headerParsed = false;

            try
            {
                // Convert byte array to string
                string fileContent = System.Text.Encoding.UTF8.GetString(file);

                // Split the content by lines
                string[] lines = fileContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                // Check if there is any content
                //if (lines.Length == 0) return languageMap;

                // Get the languages from the first line
                string[] languages = lines[0].Split(';');

                for (int y = 0; y < lines.Length; y++)
                {
                    string line = lines[y];
                    string[] values = line.Split(';');
                    if (string.IsNullOrEmpty(values[0]))
                    {
                        continue;
                    }

                    if (!headerParsed)
                    {
                        //if (values[0] == "Key")
                        //{

                        // Only for header line, remove empty entries (recreate values via Split() is simplest)
                        values = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                        isoCodes = new string[values.Length - 1];
                        for (int i = 0; i < isoCodes.Length; i++)
                        {
                            isoCodes[i] = values[i + 1];
                            languageMap.Add(isoCodes[i], new Dictionary<string, string>());
                        }

                        headerParsed = true;
                        continue;
                        //}
                    }

                    for (int i = 0; i < isoCodes.Length; i++)
                    {
                        if (!languageMap[isoCodes[i]].ContainsKey(values[0]))
                        {
                            try
                            {
                                languageMap[isoCodes[i]].Add(values[0], values[i + 1]);

                            }
                            catch (Exception)
                            {

                                //Debug.LogWarning("Values not set for " + values[0]);
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Localizer key already exists: " + values[0]);
                            break;
                        }
                    }

                    /*if (values.Length == 0) continue;

                    string key = values[0].Trim();

                    if (!languageMap.ContainsKey(key))
                    {
                        languageMap[key] = new Dictionary<string, string>();
                    }

                    // Start from 1 to skip the key in values array
                    for (int j = 1; j < Mathf.Min(values.Length, languages.Length); j++)
                    {
                        string language = languages[j].Trim();
                        string localizedText = values[j].Trim();

                        languageMap[key][language] = localizedText;
                    }*/
                }
                return await Task.FromResult(languageMap);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                return null;
            }
        }

        public string GetLanguageIsoCode()
        {
            return currentIsoCode;
        }

        public void SetLanguage(string isoCode)
        {
            currentIsoCode = isoCode;

            OnLanguageChanged?.Invoke();
            OnTextsUpdated?.Invoke();   // Updated event after changed event, so all texts should be updated

            //// Change HoloRepair language in try-catch blog to avoid error when language not available in ARep
            //try
            //{
            //    HoloRepair.Core.ContentAppInterface.ChangeLanguage(isoCode);
            //}
            //catch (Exception e)
            //{
            //    Debug.LogWarning(e.Message);
            //}
        }

        public static string GetText(string key, bool removeWordwrap)
        {
            if (key == null || languageMap == null)
            {
                return null;
            }

            if (languageMap[currentIsoCode].TryGetValue(key, out var text) && !string.IsNullOrEmpty(text))
            {
                if (removeWordwrap)
                {
                    text = text.Replace("-\\", "");
                }
                else
                {
                    text = text.Replace("-\\", "-\n");
                }
                return text;
            }
            else
            {
                return key;
            }
        }

        ///// <summary>
        ///// Dropdown buttons listener
        ///// </summary>
        ///// <param name="value"></param>
        //public void LanguageValueChanged(Dropdown d)
        //{
        //    string langPhat = string.Empty;
        //    string langHolo = string.Empty;

        //    switch (d.value)
        //    {
        //        case 0: // DE
        //            langPhat = "de-CH";
        //            langHolo = "DE";
        //            break;

        //        case 1: // FR
        //            langPhat = "fr-CH";
        //            langHolo = "FR";
        //            break;

        //        case 2: // IT
        //            langPhat = "it-CH";
        //            langHolo = "IT";
        //            break;
        //        case 3: // EN
        //            langPhat = "en-CH";
        //            langHolo = "EN";
        //            break;
        //    }

        //    SetLanguage(langHolo);
        //}
    }
}