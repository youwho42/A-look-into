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
    public GameObject inventoryDisplaySection;
    public GameObject mapDisplaySection;
    public GameObject compendiumsDisplaySection;
    public GameObject undertakingsDisplaySection;

    private void Start()
    {
        
        GameEventManager.onGamepadBumpersButtonEvent.AddListener(ChangeUI);
        maxButtons = System.Enum.GetValues(typeof(MenuButtons)).Length;
        HideAllTabbedUI();
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        
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
            UIScreenManager.instance.DisplayPlayerHUD(true);
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
            UIScreenManager.instance.DisplayPlayerHUD(true);
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
        ChangeControlTextInventory(PlayerInformation.instance.playerInput.currentControlScheme);
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
        UIScreenManager.instance.CloseTipPanel();
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
        UIScreenManager.instance.CloseTipPanel(); HideAllTabbedUI();
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
        UIScreenManager.instance.CloseTipPanel(); HideAllTabbedUI();
        undertakingsDisplaySection.SetActive(true);
        
        SetButtonSelectedColor(map, false);
        SetButtonSelectedColor(inventory, false);
        SetButtonSelectedColor(compendium, false);
        SetButtonSelectedColor(undertakings, true);
        currentButtonIndex = (int)MenuButtons.Undertakings;
        inMenu = true;
    }
    void HideAllMenuUI()
    {
        
        if (!inMenu || PlayerInformation.instance.isDragging)
            return;
        UIScreenManager.instance.HideScreenUI();
        
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

    void ChangeControlTextInventory(string text)
    {
        string displayText = "";
        string t0 = "";
        string t1 = "";
        string t2 = "";
        string t3 = "";
        if (text == "Gamepad")
        {
            t0 = "d-pad";
            t1 = "A";
            t2 = "R2 + RS";
            t3 = "B";
        }
        else
        {
            t0 = "A/D";
            t1 = "LMB";
            t2 = "LMB drag";
            t3 = "Tab";
        }

        displayText = $"{t0} select - {t1} > equip / unequip / consume - {t2} > place item in world - {t3} > close";
        UIScreenManager.instance.SetTipPanel(displayText);

    }
    
}
