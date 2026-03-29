using Klaxon.GOAD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class MenuDisplayUI : MonoBehaviour
{

    bool inMenu;
    public Color selectedColor;
    public Color idleColor;
    public Button map, inventory, compendium, undertakings;
    public GameObject inventoryDisplaySection;
    public GameObject mapDisplaySection;
    public GameObject compendiumsDisplaySection;
    public GameObject undertakingsDisplaySection;
    public List<GameObject> controllerButtons = new List<GameObject>();
    UIScreen screen;

    public GOAD_ScriptableCondition mapActive;
    public GOAD_ScriptableCondition compendiumActive;
    public GOAD_ScriptableCondition undertakingsActive;

    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.TabbedMenuUI);


        GameEventManager.onGamepadBumpersButtonEvent.AddListener(ChangeUI);
        GameEventManager.onControlSchemeChangedEvent.AddListener(SetControllerButtons);
        GameEventManager.onWorldStateUpdateEvent.AddListener(SetButtonActive);
        maxButtons = System.Enum.GetValues(typeof(MenuButtons)).Length;
        HideAllTabbedUI();
        gameObject.SetActive(false);
        SetButtonActive();
        
    }

    private void SetButtonActive()
    {
        SetMainButtonActive(map, GOAD_WorldBeliefStates.instance.HasState(mapActive));
        SetMainButtonActive(compendium, GOAD_WorldBeliefStates.instance.HasState(compendiumActive));
        SetMainButtonActive(undertakings, GOAD_WorldBeliefStates.instance.HasState(undertakingsActive));

    }

    private void OnDestroy()
    {
        
        GameEventManager.onGamepadBumpersButtonEvent.RemoveListener(ChangeUI);
        GameEventManager.onControlSchemeChangedEvent.RemoveListener(SetControllerButtons);
        GameEventManager.onWorldStateUpdateEvent.RemoveListener(SetButtonActive);
    }
    public enum MenuButtons
    {
        Map,
        Compendium,
        Inventory, 
        Undertakings
    }
    MenuButtons currentButton;
    int currentButtonIndex;
    int maxButtons;

    //void DisplayMenuUI()
    //{
    //    if (MiniGameManager.instance.gameStarted)
    //        return;
    //    if (!inMenu)
    //    {
    //        if (PlayerInformation.instance.uiScreenVisible || LevelManager.instance.isInCutscene || PlayerInformation.instance.playerInput.isPaused)
    //            return;
    //        GameEventManager.onInventoryUpdateEvent.Invoke();
    //        SetInventoryUI();
    //        UIScreenManager.instance.DisplayPlayerHUD(true);
    //    }
        
    //}
    void SetControllerButtons(string scheme)
    {
        bool isController = scheme == "Gamepad";
        foreach (var button in controllerButtons)
        {
            button.SetActive(isController);
        }
    }
    //void DisplayMapUI()
    //{
    //    if (MiniGameManager.instance.gameStarted)
    //        return;
    //    if (!inMenu)
    //    {
    //        if (PlayerInformation.instance.uiScreenVisible || LevelManager.instance.isInCutscene || PlayerInformation.instance.playerInput.isPaused)
    //            return;
    //        SetMapUI();
    //        UIScreenManager.instance.DisplayPlayerHUD(true);
    //    }
    //}
    //void ToggleDisplayUI()
    //{
    //    if (!inMenu)
    //        DisplayMenuUI();
    //    else
    //        HideAllMenuUI();
            
        
    //}
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

        ChangeCurrentUI();
    }

    public void SetCurrentButton(MenuButtons button)
    {
        currentButton = button;
        ChangeCurrentUI();
    }
    private void ChangeCurrentUI()
    {
        switch (currentButton)
        {
            case MenuButtons.Map:
                SetMapUI();
                break;
            case MenuButtons.Inventory:
                SetInventoryUI();
                break;
            case MenuButtons.Compendium:
                SetCompendiumUI();
                break;
            case MenuButtons.Undertakings:
                SetUndertakingsUI();
                break;
        }
    }

    void HideAllTabbedUI()
    {
        inventoryDisplaySection.SetActive(false);
        mapDisplaySection.SetActive(false);
        compendiumsDisplaySection.SetActive(false);
        undertakingsDisplaySection.SetActive(false);
        UIScreenManager.instance.SetMapOpen(false);
    }

    public void SetInventoryUI()
    {
        UIScreenManager.instance.SetTipPanel(SetInventoryDisplayText());
        HideAllTabbedUI();
        inventoryDisplaySection.SetActive(true);
        
        SetButtonSelectedColor(map, false);
        SetButtonSelectedColor(inventory, true);
        SetButtonSelectedColor(compendium, false);
        SetButtonSelectedColor(undertakings, false);
        currentButtonIndex = (int)MenuButtons.Inventory;
        inMenu = true;
    }
    public void SetMapUI()
    {
        UIScreenManager.instance.SetTipPanel(SetCloseDisplayText());
        
        HideAllTabbedUI();
        mapDisplaySection.SetActive(true);
        UIScreenManager.instance.SetMapOpen(true);
        SetButtonSelectedColor(map, true);
        SetButtonSelectedColor(inventory, false);
        SetButtonSelectedColor(compendium, false);
        SetButtonSelectedColor(undertakings, false);
        currentButtonIndex = (int)MenuButtons.Map;
        inMenu = true;
    }
    public void SetCompendiumUI()
    {
        UIScreenManager.instance.SetTipPanel(SetCloseDisplayText());
        HideAllTabbedUI();
        compendiumsDisplaySection.SetActive(true);
        
        SetButtonSelectedColor(map, false);
        SetButtonSelectedColor(inventory, false);
        SetButtonSelectedColor(compendium, true);
        SetButtonSelectedColor(undertakings, false);
        currentButtonIndex = (int)MenuButtons.Compendium;
        inMenu = true;
    }
   
    public void SetUndertakingsUI()
    {
        UIScreenManager.instance.SetTipPanel(SetCloseDisplayText());
        HideAllTabbedUI();
        undertakingsDisplaySection.SetActive(true);
        
        SetButtonSelectedColor(map, false);
        SetButtonSelectedColor(inventory, false);
        SetButtonSelectedColor(compendium, false);
        SetButtonSelectedColor(undertakings, true);
        currentButtonIndex = (int)MenuButtons.Undertakings;
        inMenu = true;
        
    }

    void SetMainButtonActive(Button butt, bool active)
    {
        butt.gameObject.SetActive(active);
    }

    //void HideAllMenuUI()
    //{
        
    //    if (!inMenu || PlayerInformation.instance.isDragging)
    //        return;
    //    UIScreenManager.instance.HideScreenUI();
        
    //    SetButtonSelectedColor(map, false);
    //    SetButtonSelectedColor(inventory, false);
    //    SetButtonSelectedColor(compendium, false);
    //    SetButtonSelectedColor(undertakings, false);
    //    inMenu = false;
        
    //}

    

    void SetButtonSelectedColor(Button butt, bool selected)
    {
        var ac = butt.colors;
        ac.normalColor = ac.selectedColor = ac.highlightedColor = selected ? selectedColor : idleColor;
        butt.colors = ac;
    }

    string SetInventoryDisplayText()
    {
        string variant = PlayerInformation.instance.playerInput.currentControlScheme == "Gamepad" ? "Inventory Gamepad Display Text" : "Inventory Keyboard Display Text";
        
        return LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", variant);
    }

    string SetCloseDisplayText()
    {
        string variant = PlayerInformation.instance.playerInput.currentControlScheme == "Gamepad" ? "Close Gamepad Display Text" : "Close Keyboard Display Text";

        return LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", variant);
    }

}
