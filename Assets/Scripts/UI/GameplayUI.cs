using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    public TMP_Dropdown localeDropdown;

    [SerializeField]
    private Toggle autoZoomReset;
    //[HideInInspector]
    public int autoZoomBinary = 1;
    [SerializeField]
    private Toggle HUDDisplay;
    //[HideInInspector]
    public int HUDBinary = 1;
    public int languageSelected;

    void Start()
    {
        
        localeDropdown.ClearOptions();

        var options = new List<TMP_Dropdown.OptionData>();
        int selected = 0;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            if (LocalizationSettings.SelectedLocale == locale)
                selected = i;
            options.Add(new TMP_Dropdown.OptionData(locale.name));
        }
        localeDropdown.options = options;
        languageSelected = selected;
        localeDropdown.value = selected;
        localeDropdown.onValueChanged.AddListener(LocaleSelected);
        SetDisplayHUD(HUDBinary);
        SetAutoZoomReset(autoZoomBinary);
    }

    void LocaleSelected(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        languageSelected = index;
    }


    public void ToggleAutoZoomReset()
    {
        autoZoomBinary = autoZoomReset.isOn ? 1 : 0;
    }

    public void SetAutoZoomReset(int zoomBinary)
    {
        autoZoomBinary = zoomBinary;
        autoZoomReset.isOn = autoZoomBinary == 0 ? false : true;
    }
    public void ToggleDisplayHUD()
    {
        HUDBinary = HUDDisplay.isOn ? 1 : 0;
    }
    public void SetDisplayHUD(int displayHUD)
    {
        HUDBinary = displayHUD;
        HUDDisplay.isOn = HUDBinary == 0 ? false : true;

    }

    public void SetFromSave(int locale, int autoZoom, int hud)
    {
        LocaleSelected(locale);
        SetAutoZoomReset(autoZoom);
        SetDisplayHUD(hud);
    }


}
