using Klaxon.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [HideInInspector]
    public bool isInWarning;

    public NotificationLargeDisplayObject largeNotificationObject;
    [SerializeField] private InputActionReference holdButton;
    bool fromHold;
    float currentHoldTime = 0f;
    float maxHoldTime = 1f;
    public Slider interactSlider;
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
    private void OnEnable()
    {
        holdButton.action.started += OnHoldButtonPerformed;
        holdButton.action.canceled += OnHoldButtonCanceled;
    }

    private void OnDisable()
    {
        holdButton.action.started -= OnHoldButtonPerformed;
        holdButton.action.canceled -= OnHoldButtonCanceled;
    }

    private void OnDestroy()
    {
        GameEventManager.onMenuToggleEvent.RemoveListener(ToggleTabbedMenu);
        GameEventManager.onMenuDisplayEvent.RemoveListener(GamepadDisplayTabbedMenu);
        GameEventManager.onMenuHideEvent.RemoveListener(HideScreenUI);
        GameEventManager.onMapDisplayEvent.RemoveListener(DisplayMapMenu);
        GameEventManager.onMapUpdateEvent.RemoveListener(DisplayMapMenu);
    }

    private void OnHoldButtonPerformed(InputAction.CallbackContext context)
    {
        if(largeNotificationObject.gameObject.activeInHierarchy)
            StartCoroutine("StartHoldButtonCo");
    }
    private void OnHoldButtonCanceled(InputAction.CallbackContext context)
    {
        StopCoroutine("StartHoldButtonCo");
        currentHoldTime = 0.0f;
        interactSlider.value = 0;
        fromHold = false;
    }


    IEnumerator StartHoldButtonCo()
    {
        fromHold = false;
        while (currentHoldTime < maxHoldTime)
        {
            currentHoldTime += Time.deltaTime;
            interactSlider.value = NumberFunctions.RemapNumber(currentHoldTime, 0.0f, maxHoldTime, 0.0f, 1.0f);
            yield return null;
        }
        currentHoldTime = 0.0f;
        interactSlider.value = 0;
        fromHold = true;
        
        ToggleTabbedMenu();
       
        yield return null;
        Notifications.instance.ClearLargeNotifications();
        largeNotificationObject.gameObject.SetActive(false);
        
    }

    void GamepadDisplayTabbedMenu()
    {
        
        if (inMainMenu)
            return;
        if (currentUI == UIScreenType.None)
        {
            if (largeNotificationObject.gameObject.activeInHierarchy)
            {
                if (largeNotificationObject.notification.type == NotificationsType.UndertakingStart || largeNotificationObject.notification.type == NotificationsType.UndertakingComplete)
                    tabbedMenu.SetCurrentButton(MenuDisplayUI.MenuButtons.Undertakings);
                if (largeNotificationObject.notification.type == NotificationsType.Compendium)
                    tabbedMenu.SetCurrentButton(MenuDisplayUI.MenuButtons.Compendium);
            }
            else
                tabbedMenu.SetCurrentButton(MenuDisplayUI.MenuButtons.Inventory);
            DisplayScreenUI(UIScreenType.TabbedMenuUI, !tabbedMenu.gameObject.activeInHierarchy);
        }
        
    }
    public void HideScreenUI()
    {
        
        if (GetCurrentUI() == UIScreenType.MiniGameUI || GetCurrentUI() == UIScreenType.PauseUI && isInWarning)
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
        


        switch (currentUI)
        {
            case UIScreenType.None:
                if (fromHold)
                {
                    if (largeNotificationObject.gameObject.activeInHierarchy)
                    {
                        if (largeNotificationObject.notification.type == NotificationsType.UndertakingStart || largeNotificationObject.notification.type == NotificationsType.UndertakingComplete)
                            tabbedMenu.SetCurrentButton(MenuDisplayUI.MenuButtons.Undertakings);
                        if (largeNotificationObject.notification.type == NotificationsType.Compendium)
                            tabbedMenu.SetCurrentButton(MenuDisplayUI.MenuButtons.Compendium);
                    }
                }
                else
                    tabbedMenu.SetCurrentButton(MenuDisplayUI.MenuButtons.Inventory);
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

        bool paused = false;
        
        switch (currentUI)
        {
            case UIScreenType.MainMenuUI:
                paused = true;
                break;
            case UIScreenType.PauseUI:
                paused = true; 
                break;
            case UIScreenType.PaintingUI:
                paused = true;
                break;
            case UIScreenType.LoadGameUI:
                paused = true;
                break;
            case UIScreenType.CharacterSelectUI:
                paused = true;
                break;
            case UIScreenType.OptionsUI:
                paused = true;
                break;
            case UIScreenType.CreditsUI:
                paused = true;
                break;
            case UIScreenType.WarningUI:
                paused = true;
                break;
        }
        RealTimeDayNightCycle.instance.isPaused = paused;
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
        SetButtonsActive(false);
        
        SetScreenUI(UIScreenType.WarningUI, true);
        warningUI.SetWarning(warning, continueScreen, continueButtonText, backSelect);
        isInWarning = true;
    }

    public void SetButtonsActive(bool active)
    {
        foreach (var button in mainMenu.allButtons)
        {
            button.interactable = active;
        }
        foreach (var button in pauseMenu.allButtons)
        {
            button.interactable = active;
        }
    }
    public void SetPauseScreen(bool state)
    {
        if (inMainMenu || isInWarning)
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
