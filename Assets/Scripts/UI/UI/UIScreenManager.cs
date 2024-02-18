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
    // old
    public List<GameObject> screens = new List<GameObject>();
    private UIScreenType currentScreen;

    // new
    public MenuDisplayUI tabbedMenu;
    public TipPanelUI tipsPanelUI;
    bool mapIsOpen;
    public List<GameObject> allScreens = new List<GameObject>();
    
    private UIScreenType currentUI;

    public bool canChangeUI;

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


        canChangeUI = true;
        foreach (GameObject go in screens)
        {
           
            go.SetActive(false);
            
        }
        DisplayScreen(UIScreenType.StartScreen);

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
        if (currentUI != UIScreenType.None && !SleepDisplayUI.instance.isSleeping)
            DisplayScreenUI(currentUI, false);
    }
    void DisplayMapMenu()
    {
        if (currentUI == UIScreenType.None)
        {
            tabbedMenu.SetMapUI();
            DisplayScreenUI(UIScreenType.TabbedMenuUI, true);
        }
            
    }
    void ToggleTabbedMenu()
    {
        if (currentUI == UIScreenType.TabbedMenuUI || currentUI == UIScreenType.None)
        {
            tabbedMenu.SetInventoryUI();
            DisplayScreenUI(UIScreenType.TabbedMenuUI, !tabbedMenu.gameObject.activeInHierarchy);
        }
            
    }

    public void DisplayScreenUI(UIScreenType screenType, bool state)
    {
        if (MiniGameManager.instance.gameStarted || LevelManager.instance.isInCutscene)
            return;
        SetScreenUI(screenType, state);
        
        if (state)
        {
            currentUI = screenType;
            DisplayPlayerHUD(true);
        }
        else
        {
            currentUI = UIScreenType.None;
            DisplayPlayerHUD(LevelManager.instance.HUDBinary == 1);
            CloseTipPanel();
        }
        canChangeUI = !state;
        PlayerInformation.instance.playerInput.isInUI = state;
        PlayerInformation.instance.uiScreenVisible = state;
        PlayerInformation.instance.TogglePlayerInput(!state);
    }
    
    void SetScreenUI(UIScreenType screenType, bool state)
    {
        foreach (var screen in allScreens)
        {
            if (screen.TryGetComponent(out UIScreen ui))
            {
                if (ui.GetScreenType() == screenType)
                {
                    screen.SetActive(state);
                    break;
                }

            }
        }
    }

    public void DisplayPlayerHUD(bool state)
    {
        SetScreenUI(UIScreenType.PlayerHUD, state);
        //playerHUD.gameObject.SetActive(state);
    }

    public bool DisplayIngameUI(UIScreenType screenType, bool state)
    {
        if (currentUI == screenType || currentUI == UIScreenType.None)
        {
            DisplayScreenUI(screenType, state);
            return true;
        }
        return false;
    }

    public UIScreenType GetCurrentUI()
    {
        return currentUI;
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












    public void DisplayAdditionalUI(UIScreenType screenType)
    {
        foreach (var s in screens)
        {
            if (s.GetComponent<UIScreen>().GetScreenType() == screenType)
            {
                s.SetActive(true);
                
            }
        }
    }
    
    public void DisplayScreen(UIScreenType screen)
    {
        if (!canChangeUI)
            return;
        foreach (var s in screens)
        {
            if(s.GetComponent<UIScreen>().GetScreenType() == screen)
            {
                s.SetActive(true);
                currentScreen = screen;
                if (s.TryGetComponent(out SetButtonSelected button))
                    button.SetSelectedButton();
            }
            else
            {
                s.SetActive(false);
            }
        }
       
    }

    public void HideScreens(UIScreenType screenType)
    {
        foreach (var s in screens)
        {
            if (s.GetComponent<UIScreen>().GetScreenType() == screenType)
            {
                s.SetActive(false);
            }
        }
    }

    public void HideAllScreens()
    {
        foreach (var s in screens)
        {
            currentScreen = UIScreenType.None;
            s.SetActive(false);
        }
        PlayerInformation.instance.uiScreenVisible = false;
        PlayerInformation.instance.TogglePlayerInput(true);
    }

    public UIScreenType CurrentUIScreen()
    {
        return currentScreen;
    }


    // dedicated main menu display
    // dedicated pause menu display
    // dedicated options menu => needs to know which of the above it came from to go back to
        // try setting up a save system for the options

    // done - dedicated playerHUD display
    // done - dedicated tabbed menu display
    // done - dedicated ingame ui display
}
