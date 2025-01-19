using Klaxon.Interactable;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResearchStationDisplayUI : MonoBehaviour
{
    public static ResearchStationDisplayUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    //PlayerInformation playerInformation;
    PlayerCrafting playerRecipes;
    public GameObject researchStationUI;
    public ResearchStationInventorySlot inventoryItemDisplaySlot;
    public GameObject inventoryItemArea;
    public ResearchStationResearchSlot researchSlot;
    List<ResearchStationInventorySlot> researchStationInventorySlots = new List<ResearchStationInventorySlot>();
    [HideInInspector]
    public InteractableResearchStation currentResearchStation;
    UIScreen screen;

    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.ResearchStationUI);
     
        gameObject.SetActive(false);
        playerRecipes = PlayerCrafting.instance;
    }

    public void ShowUI(InteractableResearchStation researchStation)
    {
        UpdateResearchDisplay();
        PlayerInformation.instance.uiScreenVisible = true;
        PlayerInformation.instance.TogglePlayerInput(false);
        currentResearchStation = researchStation;
    }

    public void HideUI()
    {
        PlayerInformation.instance.uiScreenVisible = false;
        PlayerInformation.instance.TogglePlayerInput(true);
        currentResearchStation.HideResearchItem();
        currentResearchStation = null;
    }
    
    public void UpdateResearchDisplay()
    {
        ClearInventorySlots();


        List<QI_ItemData> itemsInStock = new List<QI_ItemData>();
        for (int i = 0; i < PlayerInformation.instance.playerInventory.Stacks.Count; i++)
        {

            QI_ItemData item = PlayerInformation.instance.playerInventory.Stacks[i].Item;
            if (itemsInStock.Contains(item))
                continue;
            itemsInStock.Add(item);
            if (item.ResearchRecipes.Count > 0)
            {
              
                
                foreach (var recipe in item.ResearchRecipes)
                {
                    if (!playerRecipes.craftingRecipeDatabase.CraftingRecipes.Contains(recipe.recipe))
                    {
                        ResearchStationInventorySlot newSlot = Instantiate(inventoryItemDisplaySlot, inventoryItemArea.transform);
                        newSlot.AddItem(item);
                        researchStationInventorySlots.Add(newSlot);
                        break;
                    }
                }

                EventSystem.current.SetSelectedGameObject(null);
                if (researchStationInventorySlots.Count > 0)
                    EventSystem.current.SetSelectedGameObject(researchStationInventorySlots[0].GetComponentInChildren<Button>().gameObject);

            }

        }
        
    }
    public void ClearInventorySlots()
    {
        researchStationInventorySlots.Clear();
        foreach (Transform child in inventoryItemArea.transform)
        {
            Destroy(child.gameObject);
        }
        
    }

    
}
