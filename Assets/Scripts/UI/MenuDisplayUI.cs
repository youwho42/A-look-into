using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuDisplayUI : MonoBehaviour
{

    bool inMenu;
    public Color selectedColor;
    public Color idleColor;
    public Button map, inventory, compendium, undertakings;

    private void Start()
    {
        GameEventManager.onMenuToggleEvent.AddListener(ToggleDisplayUI);
        GameEventManager.onMenuDisplayEvent.AddListener(DisplayMenuUI);
        GameEventManager.onMenuHideEvent.AddListener(HideAllMenuUI);
        GameEventManager.onMapDisplayEvent.AddListener(DisplayMapUI);
        GameEventManager.onGamepadBumpersButtonEvent.AddListener(ChangeUI);
        maxButtons = System.Enum.GetValues(typeof(MenuButtons)).Length;
    }
    private void OnDisable()
    {
        GameEventManager.onMenuToggleEvent.RemoveListener(ToggleDisplayUI);
        GameEventManager.onMenuDisplayEvent.RemoveListener(DisplayMenuUI);
        GameEventManager.onMenuHideEvent.RemoveListener(HideAllMenuUI);
        GameEventManager.onMapDisplayEvent.RemoveListener(DisplayMapUI);
        GameEventManager.onGamepadBumpersButtonEvent.RemoveListener(ChangeUI);
    }
    enum MenuButtons
    {
        Map,
        Compendium,
        Inventory, 
        Undertakings
    }
    MenuButtons currentButton;
    int currentButtonIndex;
    int maxButtons;
    void DisplayMenuUI()
    {
        if (MiniGameManager.instance.gameStarted)
            return;
        if (!inMenu)
        {
            if (PlayerInformation.instance.uiScreenVisible || LevelManager.instance.isInCutscene || PlayerInformation.instance.playerInput.isPaused)
                return;
            GameEventManager.onInventoryUpdateEvent.Invoke();
            SetInventoryUI();
            
        }
        
    }
    void DisplayMapUI()
    {
        if (MiniGameManager.instance.gameStarted)
            return;
        if (!inMenu)
        {
            if (PlayerInformation.instance.uiScreenVisible || LevelManager.instance.isInCutscene || PlayerInformation.instance.playerInput.isPaused)
                return;
            SetMapUI();
        }
    }
    void ToggleDisplayUI()
    {
        if (!inMenu)
            DisplayMenuUI();
        else
            HideAllMenuUI();
    }
    void ChangeUI(int dir)
    {
        if (!inMenu)
            return;

        currentButtonIndex += dir;

        if (currentButtonIndex > maxButtons - 1)
            currentButtonIndex = 0;
        else if(currentButtonIndex<0)
            currentButtonIndex = maxButtons - 1;
        currentButton = (MenuButtons)currentButtonIndex;

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

    public void SetMapUI()
    {
        UIScreenManager.instance.DisplayScreen(UIScreenType.MapScreen);
        UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.PlayerUI);
        UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.MenuScreen);
        PlayerInformation.instance.uiScreenVisible = true;
        PlayerInformation.instance.TogglePlayerInput(false);
        SetButtonSelectedColor(map, true);
        SetButtonSelectedColor(inventory, false);
        SetButtonSelectedColor(compendium, false);
        SetButtonSelectedColor(undertakings, false);
        currentButtonIndex = (int)MenuButtons.Map;
        inMenu = true;
    }
    public void SetCompendiumUI()
    {
        UIScreenManager.instance.DisplayScreen(UIScreenType.CompendiumScreen);
        UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.PlayerUI);
        UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.MenuScreen);
        PlayerInformation.instance.uiScreenVisible = true;
        PlayerInformation.instance.TogglePlayerInput(false);
        SetButtonSelectedColor(map, false);
        SetButtonSelectedColor(inventory, false);
        SetButtonSelectedColor(compendium, true);
        SetButtonSelectedColor(undertakings, false);
        currentButtonIndex = (int)MenuButtons.Compendium;
        inMenu = true;
    }
    public void SetInventoryUI()
    {
        UIScreenManager.instance.DisplayScreen(UIScreenType.InventoryScreen);
        UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.PlayerUI);
        UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.MenuScreen);
        PlayerInformation.instance.uiScreenVisible = true;
        PlayerInformation.instance.TogglePlayerInput(false);
        SetButtonSelectedColor(map, false);
        SetButtonSelectedColor(inventory, true);
        SetButtonSelectedColor(compendium, false);
        SetButtonSelectedColor(undertakings, false);
        currentButtonIndex = (int)MenuButtons.Inventory;
        inMenu = true;
    }
    public void SetUndertakingsUI()
    {
        UIScreenManager.instance.DisplayScreen(UIScreenType.Undertakings);
        UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.PlayerUI);
        UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.MenuScreen);
        PlayerInformation.instance.uiScreenVisible = true;
        PlayerInformation.instance.TogglePlayerInput(false);
        SetButtonSelectedColor(map, false);
        SetButtonSelectedColor(inventory, false);
        SetButtonSelectedColor(compendium, false);
        SetButtonSelectedColor(undertakings, true);
        currentButtonIndex = (int)MenuButtons.Undertakings;
        inMenu = true;
    }
    void HideAllMenuUI()
    {
        if (!inMenu)
            return;
        UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
        PlayerInformation.instance.TogglePlayerInput(true);
        PlayerInformation.instance.uiScreenVisible = false;
        SetButtonSelectedColor(map, false);
        SetButtonSelectedColor(inventory, false);
        SetButtonSelectedColor(compendium, false);
        SetButtonSelectedColor(undertakings, false);
        inMenu = false;
        
    }

    void SetButtonSelectedColor(Button butt, bool selected)
    {
        var ac = butt.colors;
        ac.normalColor = ac.selectedColor = ac.highlightedColor = selected ? selectedColor : idleColor;
        butt.colors = ac;
    }
}
