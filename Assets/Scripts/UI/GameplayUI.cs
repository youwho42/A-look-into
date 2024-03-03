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
    private Slider autoZoomReset;
    [HideInInspector]
    public int autoZoomBinary = 1;
    [SerializeField]
    private TextMeshProUGUI autoZoomText;
    [SerializeField]
    private Slider HUDDisplay;
    [HideInInspector]
    public int HUDBinary = 1;
    [SerializeField]
    private TextMeshProUGUI HUDText;
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
        autoZoomBinary = (int)autoZoomReset.value;
        SetAutoZoomText();
    }

    public void SetAutoZoomReset(int zoomBinary)
    {
        autoZoomBinary = zoomBinary;
        autoZoomReset.value = zoomBinary;
        SetAutoZoomText();
    }

    public void ToggleDisplayHUD()
    {
        HUDBinary = (int)HUDDisplay.value;
        SetHUDText();
    }
    public void SetDisplayHUD(int displayHUD)
    {
        HUDBinary = displayHUD;
        HUDDisplay.value = displayHUD;
        SetHUDText();
    }

    void SetAutoZoomText()
    {
        autoZoomText.text = autoZoomBinary == 1 ?
            LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", "Reset Zoom On") :
            LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", "Reset Zoom Off");
    }

    void SetHUDText()
    {
        HUDText.text = HUDBinary == 1 ?
            LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", "HUD Display on") :
            LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", "HUD Display off");
    }

    public void SetFromSave(int locale, int autoZoom, int hud)
    {
        LocaleSelected(locale);
        SetAutoZoomReset(autoZoom);
        SetDisplayHUD(hud);
    }


}
