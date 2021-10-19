using System;
using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using UnityEngine;
using UnityEngine.Events;

public class CraftingTable : MonoBehaviour
{
    public QI_Inventory inventory;
    public QI_CraftingHandler craftingHandler;
    public QI_ItemDatabase itemDatabase;
    public QI_CraftingRecipeDatabase recipeDatabase;

    
    public List<CraftingSlot> ingredientSlots = new List<CraftingSlot>();
    List<CraftingRecipeButton> recipeButtons = new List<CraftingRecipeButton>();
    

    public CraftingSlot craftedSlot;
    

    public GameObject recipeButtonHolder;
    public GameObject recipeButton;

    QI_CraftingRecipe craftableItem;

    public UnityEvent EventUIUpdateInventory;
    
    public void Start()
    {
        
        /*for (int i = 0; i < recipeDatabase.CraftingRecipes.Count; i++)
        {
            GameObject newRecipe = Instantiate(recipeButton, recipeButtonHolder.transform);
            recipeButtons.Add(newRecipe.GetComponent<CraftingRecipeButton>());
        }
        UpdateCraftingUI();
        ClearCurrentRecipe();*/
    }
    public void SetAvailableRecipes()
    {
        
        
        for (int i = 0; i < recipeDatabase.CraftingRecipes.Count; i++)
        {
            GameObject newRecipe = Instantiate(recipeButton, recipeButtonHolder.transform);
            recipeButtons.Add(newRecipe.GetComponent<CraftingRecipeButton>());
        }
        UpdateCraftingUI();
        ClearCurrentRecipe();
    }

    public void UpdateCraftingUI()
    {
        for (int i = 0; i < recipeButtons.Count; i++)
        {
            recipeButtons[i].AddItem(recipeDatabase.CraftingRecipes[i], this);
        }
        
    }

    public void SetCurrentRecipe(QI_CraftingRecipe itemToCraft)
    {
        ClearCurrentRecipe();
        craftableItem = itemToCraft;
        craftedSlot.AddItem(craftableItem.Product.Item, craftableItem.Product.Amount, this);

        

        for (int i = 0; i < craftableItem.Ingredients.Count; i++)
        {
            
            ingredientSlots[i].AddItem(craftableItem.Ingredients[i].Item, craftableItem.Ingredients[i].Amount, inventory.GetStock(craftableItem.Ingredients[i].Item.Name));
        }

    }

    public void ClearCurrentRecipe()
    {
        craftableItem = null;
        craftedSlot.ClearSlot();
        
        foreach (CraftingSlot slot in ingredientSlots)
        {
            slot.ClearSlot();
        }
    }

    public void CraftItem()
    {
        if (craftableItem != null)
        {
            craftingHandler.Craft(craftableItem, 1);
        }
        SetCurrentRecipe(craftableItem);
        EventUIUpdateInventory.Invoke();
    }
    public void ResetButtons()
    {
        CraftingRecipeButton[] currentRecipeButtons = recipeButtonHolder.GetComponentsInChildren<CraftingRecipeButton>();
        if (currentRecipeButtons.Length > 0)
        {
            for (int i = 0; i < currentRecipeButtons.Length; i++)
            {
                Destroy(currentRecipeButtons[i].gameObject);
            }
        }
        recipeButtons.Clear();
    }
}
