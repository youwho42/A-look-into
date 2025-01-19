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
    
    public MainMenuUI mainMenu;
    public MenuDisplayUI tabbedMenu;
    public OptionsDisplayUI optionsMenu;
    public PauseUI pauseMenu;
    public LoadSelectionUI loadGame;
    public GameplayUI gameplay;
    public WarningUI warningUI;
    public TipPanelUI tipsPanelUI;
    public DropAmountUI dropAmountUI;

    bool mapIsOpen;
    public bool isSleeping;
    public bool inMainMenu;
    public List<GameObject> allScreens = new List<GameObject>();
    
    private UIScreenType currentUI;
    UIScreenType lastUI;
    float autoSaveTimer;
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
        GameEventManager.onMapUpdateEvent.AddListener(DisplayMapMenu);

        SavingLoading.instance.LoadOptions();

        inMainMenu = true;
        DisplayScreenUI(UIScreenType.MainMenuUI, true);
        mainMenu.GetComponent<SetButtonSelected>().SetSelectedButton();
    }

    private void OnDestroy()
    {
        GameEventManager.onMenuToggleEvent.RemoveListener(ToggleTabbedMenu);
        GameEventManager.onMenuDisplayEvent.RemoveListener(GamepadDisplayTabbedMenu);
        GameEventManager.onMenuHideEvent.RemoveListener(HideScreenUI);
        GameEventManager.onMapDisplayEvent.RemoveListener(DisplayMapMenu);
        GameEventManager.onMapUpdateEvent.RemoveListener(DisplayMapMenu);
    }

    void GamepadDisplayTabbedMenu()
    {
        if (inMainMenu)
            return;
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
        if (inMainMenu)
            return;
        if (GetCurrentUI() == UIScreenType.None)
        {
            tabbedMenu.SetMapUI();
            DisplayScreenUI(UIScreenType.TabbedMenuUI, true);
        }
        
    }
    void ToggleTabbedMenu()
    {
        if (inMainMenu)
            return;
        //var current = GetCurrentUI();
        //if (current == UIScreenType.TabbedMenuUI || current == UIScreenType.None)
        //{
        //    tabbedMenu.SetInventoryUI();
        //    DisplayScreenUI(UIScreenType.TabbedMenuUI, !tabbedMenu.gameObject.activeInHierarchy);
        //}
        //if (current == UIScreenType.CraftingStationUI || current == UIScreenType.ResearchStationUI || current == UIScreenType.ContainerUI)
        //    DisplayScreenUI(currentUI, false);
        switch (currentUI)
        {
            case UIScreenType.None:
                tabbedMenu.SetInventoryUI();
                DisplayScreenUI(UIScreenType.TabbedMenuUI, !tabbedMenu.gameObject.activeInHierarchy);
                break;
            case UIScreenType.PauseUI:
                DisplayScreenUI(currentUI, false);
                break;
            case UIScreenType.ContainerUI:
                DisplayScreenUI(currentUI, false);
                break;
            case UIScreenType.SleepUI:
                DisplayScreenUI(currentUI, false);
                break;
            case UIScreenType.ResearchStationUI:
                DisplayScreenUI(currentUI, false);
                break;
            case UIScreenType.CraftingStationUI:
                DisplayScreenUI(currentUI, false);
                break;
            case UIScreenType.TabbedMenuUI:
                tabbedMenu.SetInventoryUI();
                DisplayScreenUI(UIScreenType.TabbedMenuUI, !tabbedMenu.gameObject.activeInHierarchy);
                break;
            case UIScreenType.MerchantTableUI:
                DisplayScreenUI(currentUI, false);
                break;
            case UIScreenType.VillageDeskUI:
                DisplayScreenUI(currentUI, false);
                break;
            case UIScreenType.LocalGoodsUI:
                DisplayScreenUI(currentUI, false);
                break;
            case UIScreenType.GuiseUI:
                DisplayScreenUI(currentUI, false);
                break;
            case UIScreenType.RobotUI:
                DisplayScreenUI(currentUI, false);
                break;
            case UIScreenType.PaintingUI:
                DisplayScreenUI(currentUI, false);
                break;
            case UIScreenType.RestorePaintingUI:
                DisplayScreenUI(currentUI, false);
                break;
            
        }
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
            if(currentUI == UIScreenType.TabbedMenuUI)
                GameEventManager.onAutoSaveEvent.Invoke();
            currentUI = UIScreenType.None;
            DisplayPlayerHUD(gameplay.HUDBinary == 1);
            CloseTipPanel();
            ItemInformationDisplayUI.instance.HideItemName();
            pauseMenu.SetPause(false);
        }
        PreventPlayerInputs(state);
        SetTimeTickPause();
    }

    private void SetTimeTickPause()
    {


        RealTimeDayNightCycle.instance.isPaused = false;
        switch (currentUI)
        {
            case UIScreenType.MainMenuUI:
                RealTimeDayNightCycle.instance.isPaused = true;
                break;
            case UIScreenType.PauseUI:
                RealTimeDayNightCycle.instance.isPaused = true;
                break;
            case UIScreenType.PaintingUI:
                RealTimeDayNightCycle.instance.isPaused = true;
                break;
            case UIScreenType.LoadGameUI:
                RealTimeDayNightCycle.instance.isPaused = true;
                break;
            case UIScreenType.CharacterSelectUI:
                RealTimeDayNightCycle.instance.isPaused = true;
                break;
            case UIScreenType.OptionsUI:
                RealTimeDayNightCycle.instance.isPaused = true;
                break;
            case UIScreenType.CreditsUI:
                RealTimeDayNightCycle.instance.isPaused = true;
                break;
        }
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
                if (screen.TryGetComponent(out TutorialUI tutorial) && state)
                    tutorial.StartTutorial();
                break;
            }
        }
    }

    public void DisplayWarning(string warning, UIScreenType continueScreen, string continueButtonText, SetButtonSelected backSelect)
    {
        SetScreenUI(UIScreenType.WarningUI, true);
        warningUI.SetWarning(warning, continueScreen, continueButtonText, backSelect);
    }

    public void SetPauseScreen(bool state)
    {
        if (inMainMenu)
            return;
        if(state)
            lastUI = currentUI;
        DisplayScreenUI(UIScreenType.PauseUI, state);
        pauseMenu.SetPause(state);
        if (!state)
        {
            PlayerInformation.instance.playerInput.isPaused = false;
            currentUI = lastUI == UIScreenType.PauseUI ? UIScreenType.None : lastUI;
            if(currentUI != UIScreenType.None)
            {
                PreventPlayerInputs(true);
                foreach (var screen in allScreens)
                {
                    if (screen.TryGetComponent(out UIScreen ui))
                    {
                        if (ui.GetScreenType() == currentUI && screen.TryGetComponent(out SetButtonSelected setButton))
                            setButton.SetSelectedButton();
                    }
                        
                }
            }
            
        }
        
    }

    public void DisplayOptionsUI(bool state, UIScreenType screenType)
    {
        HideScreenUI();
        DisplayScreenUI(UIScreenType.OptionsUI, state);
        optionsMenu.SetBackButton(screenType);
        if(screenType == UIScreenType.PauseUI)
            pauseMenu.SetPause(true);
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


    public DropAmountUI DisplayDropAmountUI(InventoryDisplaySlot inventoryDisplaySlot, int maxAmount, int stackAmount, Vector3 position)
    {
        dropAmountUI.gameObject.SetActive(true);
        dropAmountUI.okButton.onClick.AddListener(inventoryDisplaySlot.SetDropAmount);
        dropAmountUI.SetAmount(maxAmount, stackAmount);
        dropAmountUI.SetupUI(maxAmount, stackAmount, position);
        return dropAmountUI;
    }

    public DropAmountUI DisplayDropAmountUI(ContainerDisplaySlot containerDisplaySlot, int maxAmount, int stackAmount, Vector3 position)
    {
        dropAmountUI.gameObject.SetActive(true);
        dropAmountUI.okButton.onClick.AddListener(containerDisplaySlot.SetTransferAmount);
        dropAmountUI.SetAmount(maxAmount, stackAmount);
        dropAmountUI.SetupUI(maxAmount, stackAmount, position);
        return dropAmountUI;
    }

    public void CloseDropAmountUI()
    {
        if (!dropAmountUI.isActiveAndEnabled)
            return;
        dropAmountUI.okButton.onClick.RemoveAllListeners();
        dropAmountUI.ResetUI();
        dropAmountUI.gameObject.SetActive(false);
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

//#if UNITY_STANDALONE && !UNITY_EDITOR
//        void OnApplicationFocus(bool hasFocus)
//        {
//            if (!hasFocus && !inMainMenu)
//                SetPauseScreen(true);
//        }
//#endif

}
