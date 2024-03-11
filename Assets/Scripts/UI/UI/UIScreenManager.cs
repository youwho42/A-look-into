using Klaxon.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreenManager : MonoBehaviour
{
    public static UIScreenManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    
    public MenuDisplayUI tabbedMenu;
    public OptionsDisplayUI optionsMenu;
    public PauseUI pauseMenu;
    public LoadSelectionUI loadGame;
    public GameplayUI gameplay;
    public WarningUI warningUI;
    public TipPanelUI tipsPanelUI;
    bool mapIsOpen;
    public bool isSleeping;
    public bool inMainMenu;
    public List<GameObject> allScreens = new List<GameObject>();
    
    private UIScreenType currentUI;

    private void Start()
    {
        // using TAB to toggle tabbedMenu
        GameEventManager.onMenuToggleEvent.AddListener(ToggleTabbedMenu);
        // using gamepad UP to display tabbedMenu
        GameEventManager.onMenuDisplayEvent.AddListener(GamepadDisplayTabbedMenu);
        // using ESC to hide tabbedMenu
        GameEventManager.onMenuHideEvent.AddListener(HideScreenUI);
        // using M to display tabbedMenu at Map
        GameEventManager.onMapDisplayEvent.AddListener(DisplayMapMenu);


        SavingLoading.instance.LoadOptions();

        inMainMenu = true;
        DisplayScreenUI(UIScreenType.MainMenuUI, true);
        
    }

    private void OnDestroy()
    {
        GameEventManager.onMenuToggleEvent.RemoveListener(ToggleTabbedMenu);
        GameEventManager.onMenuDisplayEvent.RemoveListener(GamepadDisplayTabbedMenu);
        GameEventManager.onMenuHideEvent.RemoveListener(HideScreenUI);
        GameEventManager.onMapDisplayEvent.RemoveListener(DisplayMapMenu);
    }

    void GamepadDisplayTabbedMenu()
    {
        if (currentUI == UIScreenType.None)
        {
            tabbedMenu.SetInventoryUI();
            DisplayScreenUI(UIScreenType.TabbedMenuUI, true);
        }
        
    }
    public void HideScreenUI()
    {
        if (GetCurrentUI() == UIScreenType.MiniGameUI)
            return;
        if (GetCurrentUI() != UIScreenType.None && !isSleeping)
            DisplayScreenUI(currentUI, false);
    }
    void DisplayMapMenu()
    {
        if (GetCurrentUI() == UIScreenType.None)
        {
            tabbedMenu.SetMapUI();
            DisplayScreenUI(UIScreenType.TabbedMenuUI, true);
        }
        
    }
    void ToggleTabbedMenu()
    {
        if (GetCurrentUI() == UIScreenType.TabbedMenuUI || GetCurrentUI() == UIScreenType.None)
        {
            tabbedMenu.SetInventoryUI();
            DisplayScreenUI(UIScreenType.TabbedMenuUI, !tabbedMenu.gameObject.activeInHierarchy);
        }
        if (GetCurrentUI() == UIScreenType.CraftingStationUI || GetCurrentUI() == UIScreenType.ResearchStationUI || GetCurrentUI() == UIScreenType.ContainerUI)
            DisplayScreenUI(currentUI, false);
    }

    public void DisplayScreenUI(UIScreenType screenType, bool state)
    {
        //if (MiniGameManager.instance.gameStarted /*|| LevelManager.instance.isInCutscene*/)
        //    return;
        SetScreenUI(screenType, state);
        
        if (state)
        {
            currentUI = screenType;
            DisplayPlayerHUD(!inMainMenu);
        }
        else
        {
            currentUI = UIScreenType.None;
            DisplayPlayerHUD(gameplay.HUDBinary == 1);
            CloseTipPanel();
            pauseMenu.SetPause(false);
        }
        PreventPlayerInputs(state);
       
    }
    /// <summary>
    /// Setting a screen here will just bypass all other functions and display the screen with any others that exist.
    /// </summary>
    /// <param name="screenType"></param>
    /// <param name="state"></param>
    void SetScreenUI(UIScreenType screenType, bool state)
    {
        foreach (var screen in allScreens)
        {
            if (screen.TryGetComponent(out UIScreen ui))
            {
                if (ui.GetScreenType() != screenType)
                    continue;

                screen.SetActive(state);

                if (screen.TryGetComponent(out SetButtonSelected selectedButton) && state)
                    selectedButton.SetSelectedButton();

                break;
            }
        }
    }

    public void DisplayWarning(string warning, UIScreenType continueScreen)
    {
        SetScreenUI(UIScreenType.WarningUI, true);
        warningUI.SetWarning(warning, continueScreen);
    }

    public void SetPauseScreen(bool state)
    {
        DisplayScreenUI(UIScreenType.PauseUI, state);
        pauseMenu.SetPause(state);
        if (!state)
            PlayerInformation.instance.playerInput.isPaused = false;
    }

    public void DisplayOptionsUI(bool state, UIScreenType screenType)
    {
        HideScreenUI();
        DisplayScreenUI(UIScreenType.OptionsUI, state);
        optionsMenu.SetBackButton(screenType);
        
    }

    public void DisplayLoadGameUI(bool state, UIScreenType screenType)
    {
        HideScreenUI();
        DisplayScreenUI(UIScreenType.LoadGameUI, state);
        loadGame.SetBackButton(screenType);

    }
    

    public void DisplayPlayerHUD(bool state)
    {
        SetScreenUI(UIScreenType.PlayerHUD, state);
    }

    /// <summary>
    /// This is used to display the Ingame UI, Crafting, merchants, etc...
    /// </summary>
    /// <param name="screenType"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public bool DisplayIngameUI(UIScreenType screenType, bool state)
    {
        if (currentUI == screenType || currentUI == UIScreenType.None)
        {
            DisplayScreenUI(screenType, state);
            return true;
        }
        return false;
    }

    public void SetCurrentUI(UIScreenType screenType)
    {
        currentUI = screenType;
    }
    /// <summary>
    /// Return the current UIScreenType
    /// </summary>
    /// <returns>UIScreenType</returns>
    public UIScreenType GetCurrentUI()
    {
        return currentUI;
    }
    /// <summary>
    /// Returns true/false if what you sent is is the currentUI
    /// </summary>
    /// <param name="screenType"></param>
    /// <returns>bool</returns>
    public bool GetIsCurrentUI(UIScreenType screenType)
    {
        return currentUI == screenType;
    }

    public void SetMiniGameUI(bool state)
    {
        SetCurrentUI(state ? UIScreenType.MiniGameUI : UIScreenType.None);
        PreventPlayerInputs(state);
    }



    public void SetTipPanel(string tiptText)
    {
        tipsPanelUI.SetTipPanel(tiptText);
        tipsPanelUI.gameObject.SetActive(true);
    }

    public void CloseTipPanel()
    {
        tipsPanelUI.gameObject.SetActive(false);
    }

    public bool GetMapOpen()
    {
        return mapIsOpen;
    }

    public void SetMapOpen(bool state)
    {
        mapIsOpen = state;
    }

    public void PreventPlayerInputs(bool state)
    {
        PlayerInformation.instance.playerInput.isInUI = state;
        PlayerInformation.instance.uiScreenVisible = state;
        PlayerInformation.instance.TogglePlayerInput(!state);
    }

#if UNITY_STANDALONE && !UNITY_EDITOR
        void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && !inMainMenu)
                SetPauseScreen(true);
        }
#endif

}
