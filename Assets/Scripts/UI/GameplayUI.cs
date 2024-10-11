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
    [SerializeField]
    private Slider RIBDisplay;
    [HideInInspector]
    public int RIBBinary = 0;
    [SerializeField]
    private TextMeshProUGUI RIBText;
    [SerializeField]
    private Slider AutoSaveDisplay;
    [HideInInspector]
    public int AutoSaveBinary = 0;
    [SerializeField]
    private TextMeshProUGUI AutoSaveText;
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
        SetRIB(RIBBinary);
        SetAutoSave(autoZoomBinary);
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
    public void ToggleRIB()
    {
        RIBBinary = (int)RIBDisplay.value;
        SetRIBData();
    }
    public void SetRIB(int RIBBin)
    {
        RIBBinary = RIBBin;
        RIBDisplay.value = RIBBin;
        SetRIBData();
    }
    public void ToggleAutoSave()
    {
        AutoSaveBinary = (int)AutoSaveDisplay.value;
        SetHUDText();
    }
    public void SetAutoSave(int autoSave)
    {
        AutoSaveBinary = autoSave;
        AutoSaveDisplay.value = autoSave;
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

    void SetRIBData()
    {
        RIBText.text = RIBBinary == 1 ?
            LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", "RIB Display on") :
            LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", "RIB Display off");
        Application.runInBackground = RIBBinary == 1;
    }

    void SetAutoSaveData()
    {
        AutoSaveText.text = AutoSaveBinary == 1 ?
            LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", "Auto-save on") :
            LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", "Auto-save off");
        
    }

    public void SetFromSave(int locale, int autoZoom, int hud, int rib)
    {
        LocaleSelected(locale);
        SetAutoZoomReset(autoZoom);
        SetDisplayHUD(hud);
        SetRIB(rib);
        //SetAutoSave();
    }


}
