using NaughtyAttributes;
using StarCooperation.Export;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageController : MonoBehaviour
{

    public string language;
    public int index;
    public LocalizationSettings Settings;
    // Start is called before the first frame update
    void Start()
    {
        LocalizationSettings.SelectedLocaleChanged += LocaleChanged;
        FindObjectOfType<MessageReceiver>().LanguageChanged += OnLanguageChanged;
    //    LocalizationSettings.SelectedLocale.Identifier = LocalizationSettings.ProjectLocale.Identifier;
  //      AddLocale();
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales.Where(x => x.Identifier == "EN").FirstOrDefault();
    }

    [Button]
    public void SetLanguage()
    {
        OnLanguageChanged(language);
    }
    [Button]
    public void SetLanguageInt()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
    [Button]
    public void AddLocale()
    {

        var locale = new UnityEngine.Localization.Locale();
        locale.name = "Deutsch";
        locale.LocaleName = "German (DE)";
        locale.Identifier = "DE";

        LocalizationSettings.AvailableLocales.AddLocale(locale);
        //var localeEN = new UnityEngine.Localization.Locale();
        //locale.name = "English";
        //locale.LocaleName = "English (EN)";
        //locale.Identifier = "EN";

        //LocalizationSettings.AvailableLocales.AddLocale(localeEN);
    }

    [Button]
    public void CreateLocale()
    {
        Debug.Log($"{Settings.GetAvailableLocales().Locales.Count }locales available in creator settings");
        foreach (var item in Settings.GetAvailableLocales().Locales)
        {
            var locale = new Locale() { Identifier = item.Identifier, LocaleName = item.LocaleName, name = item.name };
            LocalizationSettings.AvailableLocales.AddLocale(locale);
        }
    }
    [Button]
    public void GetLocaleCount()
    {
        Debug.Log(LocalizationSettings.AvailableLocales.Locales.Count);
        foreach (var item in LocalizationSettings.AvailableLocales.Locales)
        {
            Debug.Log($"Locale Name: {item.LocaleName}, Locale :{item.Identifier}");

        }
    }
    private void OnLanguageChanged(string lang)
    {
        var locale = GetAvailableLocale(lang);
        Debug.Log($"Changing Language: {locale.LocaleName}");
        LocalizationSettings.SelectedLocale = locale;
    }

    private Locale GetAvailableLocale(string lang)
    {
        var locale = LocalizationSettings.AvailableLocales.Locales.Where(x => x.Identifier == lang).FirstOrDefault();

        if (locale is null)
        {
            Debug.Log($"Locale Identifier {lang} is not available, Select current active Locale {LocalizationSettings.SelectedLocale.Identifier} instead");
            return LocalizationSettings.SelectedLocale;
        }
        return locale;
    }

    private void LocaleChanged(Locale obj)
    {
        // throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
