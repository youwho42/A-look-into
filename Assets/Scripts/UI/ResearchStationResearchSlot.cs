using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResearchStationResearchSlot : MonoBehaviour
{
    QI_ItemData item;
    public Image icon;
    public TextMeshProUGUI recipesText;
    float gameEnergyReward = 1;

    private void Start()
    {
        ClearSlot();
    }
    public void AddItem(QI_ItemData newItem)
    {
        item = newItem;
        icon.sprite = item.Icon;
        icon.enabled = true;
        AddRecipes();
    }

    void AddRecipes()
    {
        string textToAdd = "";
        for (int i = 0; i < item.ResearchRecipes.Count; i++)
        {
            if(!PlayerInformation.instance.playerRecipeDatabase.CraftingRecipes.Contains(item.ResearchRecipes[i].recipe))
                textToAdd += item.ResearchRecipes[i].recipe.Name + "\n";
        }
        recipesText.text = textToAdd;
    }


    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        recipesText.text = "...";
    }

    public void LearnRecipes()
    {
        // loop through all  recipeReveals 
        for (int i = 0; i < item.ResearchRecipes.Count; i++)
        {
            if (PlayerCrafting.instance.craftingRecipeDatabase.CraftingRecipes.Contains(item.ResearchRecipes[i].recipe))
                continue;
            if (CheckForInventoryQuantity(i))
            {
                PlayerInformation.instance.playerStats.AddToAgency(item.ResearchRecipes[i].AgencyReward);
                PlayerCrafting.instance.AddCraftingRecipe(item.ResearchRecipes[i].recipe);
                NotificationManager.instance.SetNewNotification($"{item.ResearchRecipes[i].recipe.Name} recipe learned", NotificationManager.NotificationType.Compedium);
            }
            ResearchStationDisplayUI.instance.UpdateResearchDisplay();

        }
        
        ClearSlot();
    }

    public bool CheckForInventoryQuantity(int index)
    {
        
            int t = PlayerInformation.instance.playerInventory.GetStock(item.Name);
            if (t < item.ResearchRecipes[index].RecipeRevealAmount)
            {
            string plural = item.ResearchRecipes[index].RecipeRevealAmount - t == 1 ? "" : "'s";
            NotificationManager.instance.SetNewNotification($"{item.ResearchRecipes[index].RecipeRevealAmount - t} {item.Name}{plural} missing", NotificationManager.NotificationType.Warning);
                return false;
            }

        
        return true;
    }

}
