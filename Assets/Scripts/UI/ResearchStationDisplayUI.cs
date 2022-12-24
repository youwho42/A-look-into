using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private void Start()
    {
        researchStationUI.SetActive(false);
        //playerInformation = PlayerInformation.instance;
        playerRecipes = PlayerCrafting.instance;
    }

    public void ShowUI()
    {
        UpdateResearchDisplay();
        PlayerInformation.instance.uiScreenVisible = true;
        PlayerInformation.instance.TogglePlayerInput(false);
        //researchStationUI.SetActive(true);
    }

    public void HideUI()
    {
        PlayerInformation.instance.uiScreenVisible = false;
        PlayerInformation.instance.TogglePlayerInput(true);
        //researchStationUI.SetActive(false);
    }
    
    public void UpdateResearchDisplay()
    {
        ClearInventorySlots();

        
        
        for (int i = 0; i < PlayerInformation.instance.playerInventory.Stacks.Count; i++)
        {

            QI_ItemData item = PlayerInformation.instance.playerInventory.Stacks[i].Item;
            if (item.ResearchRecipes.Count > 0)
            {
              
                
                    foreach (var recipe in item.ResearchRecipes)
                    {
                        if (!playerRecipes.craftingRecipeDatabase.CraftingRecipes.Contains(recipe))
                        {
                            ResearchStationInventorySlot newSlot = Instantiate(inventoryItemDisplaySlot, inventoryItemArea.transform);
                            newSlot.AddItem(item);
                        }
                    }
                

                
            }

        }
        
    }
    public void ClearInventorySlots()
    {
        while (inventoryItemArea.transform.childCount > 0)
        {
            DestroyImmediate(inventoryItemArea.transform.GetChild(0).gameObject);
        }
    }

    
}
