using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompendiumSlot : MonoBehaviour
{
    public QI_ItemData item;
    public Image icon;
    public QI_CraftingRecipe itemRecipe;
    public List<QI_ItemData.RecipeRevealObject> recipeReveals = new List<QI_ItemData.RecipeRevealObject>();

    public void AddItem(QI_ItemData newItem, QI_CraftingRecipe newRecipe = null)
    {
        item = newItem;
        icon.sprite = item.Icon;
        icon.enabled = true;
        itemRecipe = newRecipe;
    }
    public void AddRecipeReveal(List<QI_ItemData.RecipeRevealObject> revealObject)
    {
        recipeReveals =  revealObject;
    }


    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        recipeReveals.Clear();
        itemRecipe = null;
    }

    public void DisplayIformation()
    {
        CompendiumDisplayUI.instance.DisplayItemInformation(item, itemRecipe, recipeReveals);
    }
}
