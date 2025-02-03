using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class CompendiumSlot : MonoBehaviour
{
    public QI_ItemData item;
    public Image icon;
    public QI_CraftingRecipe itemRecipe;
    public List<QI_ItemData.RecipeRevealObject> recipeReveals = new List<QI_ItemData.RecipeRevealObject>();
    public TextMeshProUGUI itemName;
    LocalizedString localizedName;
    bool longDescription;
    public void AddItem(QI_ItemData newItem, QI_CraftingRecipe newRecipe = null)
    {
        item = newItem;
        icon.sprite = item.Icon;
        icon.enabled = true;
        itemRecipe = newRecipe;
        localizedName = newItem.localizedName;
        localizedName.StringChanged += UpdateName;
        string amt = "";
        if (newRecipe != null)
            amt = $"x{itemRecipe.Product.Amount}";
        itemName.text = $"{newItem.localizedName.GetLocalizedString(newItem.Name)} {amt}";
        longDescription = false;
        if(item.Type == ItemType.Animal)
        {
            int index = PlayerInformation.instance.animalCompendiumInformation.animalNames.IndexOf(item.Name);
            int amount = PlayerInformation.instance.animalCompendiumInformation.animalTimesViewed[index];
            if(amount >= item.ResearchRecipes[0].RecipeRevealAmount)
                longDescription = true;
        }
    }
    public void AddRecipeReveal(List<QI_ItemData.RecipeRevealObject> revealObject)
    {
        recipeReveals =  revealObject;
    }

    void UpdateName(string value)
    {
        itemName.text = value;
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        recipeReveals.Clear();
        itemRecipe = null;
        localizedName.StringChanged -= UpdateName;
    }

    public void DisplayIformation()
    {
        CompendiumDisplayUI.instance.DisplayItemInformation(item, itemRecipe, recipeReveals, longDescription);
    }
}
