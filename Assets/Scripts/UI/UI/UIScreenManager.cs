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
    public List<GameObject> screens = new List<GameObject>();
    public UIScreen playerHUD;
    public UIScreen tabbedMenu;

   
    private UIScreenType currentScreen;

    public bool canChangeUI;

    private void Start()
    {
        // using TAB to toggle tabbedMenu
        GameEventManager.onMenuToggleEvent.AddListener(ToggleTabbedMenu);
        // using gamepad UP to display tabbedMenu
        GameEventManager.onMenuDisplayEvent.AddListener(GamepadDisplayTabbedMenu);
        // using ESC to hide tabbedMenu
        GameEventManager.onMenuHideEvent.AddListener(HideTabbedMenu);
        // using gamepad B to hide tabbedMenu
        GameEventManager.onMapDisplayEvent.AddListener(HideTabbedMenu);


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
        GameEventManager.onMenuHideEvent.RemoveListener(HideTabbedMenu);
        GameEventManager.onMapDisplayEvent.RemoveListener(HideTabbedMenu);
    }

    void GamepadDisplayTabbedMenu()
    {
        DisplayTabbedMenu(true);
    }
    void HideTabbedMenu()
    {
        DisplayTabbedMenu(false);
    }

    void ToggleTabbedMenu()
    {
        DisplayTabbedMenu(!tabbedMenu.gameObject.activeInHierarchy);
    }

    public void DisplayTabbedMenu(bool state)
    {
        tabbedMenu.gameObject.SetActive(state);
    }

    public void DisplayPlayerHUD(bool state)
    {
        playerHUD.gameObject.SetActive(state);
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
    // dedicated tabbed menu display
    // dedicated ingame ui display
}
