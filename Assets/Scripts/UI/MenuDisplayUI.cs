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
        GameEventManager.onMenuDisplayEvent.AddListener(DisplayMenuUI);
        GameEventManager.onMapDisplayEvent.AddListener(DisplayMapUI);
    }
    private void OnDisable()
    {
        GameEventManager.onMenuDisplayEvent.RemoveListener(DisplayMenuUI);
        GameEventManager.onMapDisplayEvent.RemoveListener(DisplayMapUI);
    }
    //private void Update()
    //{
    //    if (MiniGameManager.instance.gameStarted)
    //        return;

    //    if (!inMenu)
    //    {
    //        if (PlayerInformation.instance.uiScreenVisible || LevelManager.instance.isInCutscene || PlayerInformation.instance.playerInput.isPaused)
    //            return;
    //        if (Input.GetKeyDown(KeyCode.Tab))
    //        {
    //            SetInventoryUI();
    //        }
    //        else if (Input.GetKeyDown(KeyCode.C))
    //        {
    //            SetCompendiumUI();
    //        }
    //        else if (Input.GetKeyDown(KeyCode.M))
    //        {
    //            SetMapUI();
    //        }
    //    } 
    //    else
    //    {
    //        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.M))
    //            HideAllMenuUI();
    //    }
    //}

    void DisplayMenuUI()
    {
        if (MiniGameManager.instance.gameStarted)
            return;
        if (!inMenu)
        {
            if (PlayerInformation.instance.uiScreenVisible || LevelManager.instance.isInCutscene || PlayerInformation.instance.playerInput.isPaused)
                return;
            SetInventoryUI();
        }
        else
        {
                HideAllMenuUI();
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
        else
        {
            HideAllMenuUI();
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
        inMenu = true;
    }
    public void SetInventoryUI()
    {
        UIScreenManager.instance.DisplayScreen(UIScreenType.InventoryScreen);
        UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.PlayerUI);
        UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.MenuScreen);
        PlayerInformation.instance.uiScreenVisible = true;
        PlayerInformation.instance.TogglePlayerInput(true);
        SetButtonSelectedColor(map, false);
        SetButtonSelectedColor(inventory, true);
        SetButtonSelectedColor(compendium, false);
        SetButtonSelectedColor(undertakings, false);
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
        inMenu = true;
    }
    void HideAllMenuUI()
    {
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
