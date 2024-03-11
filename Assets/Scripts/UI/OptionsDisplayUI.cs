using Klaxon.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsDisplayUI : MonoBehaviour
{


    bool inMenu;
    public Color selectedColor;
    public Color idleColor;
    public Button audioControl, graphics, gameplay, controls;
    public GameObject audioDisplaySection;
    public GameObject graphicsDisplaySection;
    public GameObject gameplayDisplaySection;
    public GameObject controlsDisplaySection;
    UIScreenType backButtonScreen;

    UIScreen screen;

    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.OptionsUI);
    
        GameEventManager.onGamepadBumpersButtonEvent.AddListener(ChangeUI);
        maxButtons = System.Enum.GetValues(typeof(MenuButtons)).Length;
        HideAllOptionsUI();
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        SetAudioUI();
    }
    private void OnDestroy()
    {

        GameEventManager.onGamepadBumpersButtonEvent.RemoveListener(ChangeUI);
    }

    enum MenuButtons
    {
        Audio,
        Graphics,
        Gameplay,
        Controls
    }
    MenuButtons currentButton;
    int currentButtonIndex;
    int maxButtons;

    

    void HideAllOptionsUI()
    {
        audioDisplaySection.SetActive(false);
        graphicsDisplaySection.SetActive(false);
        gameplayDisplaySection.SetActive(false);
        controlsDisplaySection.SetActive(false);
    }



    void ChangeUI(int dir)
    {
        if (!inMenu)
            return;

        currentButtonIndex += dir;

        if (currentButtonIndex > maxButtons - 1)
            currentButtonIndex = 0;
        else if (currentButtonIndex < 0)
            currentButtonIndex = maxButtons - 1;
        currentButton = (MenuButtons)currentButtonIndex;

        inMenu = true;

        switch (currentButton)
        {
            case MenuButtons.Audio:
                SetAudioUI();
                break;
            case MenuButtons.Graphics:
                SetGraphicsUI();
                break;
            case MenuButtons.Gameplay:
                SetGameplayUI();
                break;
            case MenuButtons.Controls:
                SetControlsUI();
                break;
        }
    }

    public void SetAudioUI()
    {
        HideAllOptionsUI();
        audioDisplaySection.SetActive(true);
        SetButtonSelectedColor(audioControl, true);
        SetButtonSelectedColor(graphics, false);
        SetButtonSelectedColor(gameplay, false);
        SetButtonSelectedColor(controls, false);
        currentButtonIndex = (int)MenuButtons.Audio;
    }

    public void SetGraphicsUI()
    {
        HideAllOptionsUI();
        graphicsDisplaySection.SetActive(true);
        SetButtonSelectedColor(audioControl, false);
        SetButtonSelectedColor(graphics, true);
        SetButtonSelectedColor(gameplay, false);
        SetButtonSelectedColor(controls, false);
        currentButtonIndex = (int)MenuButtons.Graphics;
    }

    public void SetGameplayUI()
    {
        HideAllOptionsUI();
        gameplayDisplaySection.SetActive(true);
        SetButtonSelectedColor(audioControl, false);
        SetButtonSelectedColor(graphics, false);
        SetButtonSelectedColor(gameplay, true);
        SetButtonSelectedColor(controls, false);
        currentButtonIndex = (int)MenuButtons.Gameplay;
    }

    public void SetControlsUI()
    {
        HideAllOptionsUI();
        controlsDisplaySection.SetActive(true);
        SetButtonSelectedColor(audioControl, false);
        SetButtonSelectedColor(graphics, false);
        SetButtonSelectedColor(gameplay, false);
        SetButtonSelectedColor(controls, true);
        currentButtonIndex = (int)MenuButtons.Controls;
    }


    void SetButtonSelectedColor(Button butt, bool selected)
    {
        var ac = butt.colors;
        ac.normalColor = ac.selectedColor = ac.highlightedColor = selected ? selectedColor : idleColor;
        butt.colors = ac;
    }

    public void SetBackButton(UIScreenType screen)
    {
        backButtonScreen = screen;
    }
    public void Back()
    {
        SavingLoading.instance.SaveOptions();
        UIScreenManager.instance.HideScreenUI();
        UIScreenManager.instance.DisplayScreenUI(backButtonScreen, true);
    }
}
