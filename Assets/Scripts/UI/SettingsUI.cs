using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;


public class SettingsUI : MonoBehaviour
{

    public TMP_Dropdown localeDropdown;
    IEnumerator Start()
    {
        // Wait for the localization system to initialize
        yield return LocalizationSettings.InitializationOperation;
        localeDropdown.ClearOptions();
        // Generate list of available Locales
        var options = new List<TMP_Dropdown.OptionData>();
        Debug.Log(options.Count);
        int selected = 0;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            if (LocalizationSettings.SelectedLocale == locale)
                selected = i;
            options.Add(new TMP_Dropdown.OptionData(locale.name));
        }
        localeDropdown.options = options;

        localeDropdown.value = selected;
        localeDropdown.onValueChanged.AddListener(LocaleSelected);
    }

    void LocaleSelected(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }

    //// Define an array of supported locales or languages.
    //private string[] supportedLocales = { "English", "French", "German" };

    //private void Start()
    //{
    //    // Initialize the dropdown with the supported locales.
    //    InitializeDropdown();
    //}

    //private void InitializeDropdown()
    //{

    //    // Clear existing options in the dropdown.
    //    localeDropdown.ClearOptions();

    //    // Add supported locales as options to the dropdown.
    //    localeDropdown.AddOptions(new List<string>(supportedLocales));

    //    // Set the initial value of the dropdown to the current locale.
    //    // You may want to save and load the selected locale from player preferences or elsewhere.
    //    // For this example, we set it to English as the default.
    //    localeDropdown.value = Array.IndexOf(supportedLocales, "English");

    //    // Add a listener to the dropdown to handle selection changes.
    //    localeDropdown.onValueChanged.AddListener(OnLocaleDropdownValueChanged);
    //    OnLocaleDropdownValueChanged(localeDropdown.value);
    //}

    //private void OnLocaleDropdownValueChanged(int index)
    //{
    //    // Handle the locale change here.

    //    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

    //}
}
