using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;

public class ResearchStationResearchSlot : MonoBehaviour
{
    QI_ItemData item;
    public Image icon;
    public TextMeshProUGUI recipesText;
    public Button researchButton;
    float gameEnergyReward = 1;

    private void Start()
    {
        ClearSlot();
    }
    public void AddItem(QI_ItemData newItem)
    {
        researchButton.interactable = true;
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
            if (!PlayerInformation.instance.playerRecipeDatabase.CraftingRecipes.Contains(item.ResearchRecipes[i].recipe))
            {
                var n = LocalizationSettings.StringDatabase.GetLocalizedString($"Items-{item.ResearchRecipes[i].recipe.Product.Item.Type.ToString()}", item.ResearchRecipes[i].recipe.Product.Item.Name);
                var c = PlayerInformation.instance.playerInventory.GetStock(item.Name) >= item.ResearchRecipes[i].RecipeRevealAmount ? "" : "<color=#FF0000>";
                textToAdd += $"{c} {item.ResearchRecipes[i].RecipeRevealAmount} - {n}\n";
            }
                
        }
        recipesText.text = textToAdd;
    }


    public void ClearSlot()
    {
        researchButton.interactable = false;
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        recipesText.text = "The recipes you will from researching items will show here alongside the amount of items needed to research the recipe.";
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
                PlayerInformation.instance.statHandler.ChangeStat(item.ResearchRecipes[i].agencyStatChanger);
                PlayerCrafting.instance.AddCraftingRecipe(item.ResearchRecipes[i].recipe);
                Notifications.instance.SetNewNotification($"{item.ResearchRecipes[i].recipe.Product.Item.localizedName.GetLocalizedString()}", null, 0, NotificationsType.Compendium);

                //NotificationManager.instance.SetNewNotification($"{item.ResearchRecipes[i].recipe.Name} recipe learned", NotificationManager.NotificationType.Compendium);
            }
            //ResearchStationDisplayUI.instance.UpdateResearchDisplay();

        }
        ResearchStationDisplayUI.instance.UpdateResearchDisplay();
        ClearSlot();
        
    }

    public bool CheckForInventoryQuantity(int index)
    {
        
            int t = PlayerInformation.instance.playerInventory.GetStock(item.Name);
            if (t < item.ResearchRecipes[index].RecipeRevealAmount)
            {
                //string plural = item.ResearchRecipes[index].RecipeRevealAmount - t == 1 ? "" : "'s";
                Notifications.instance.SetNewNotification($"{item.ResearchRecipes[index].RecipeRevealAmount - t} {item.localizedName.GetLocalizedString()}", null, 0, NotificationsType.Warning);

                //NotificationManager.instance.SetNewNotification($"{item.ResearchRecipes[index].RecipeRevealAmount - t} {item.Name}{plural} missing", NotificationManager.NotificationType.Warning);
                return false;
            }

        
        return true;
    }

}
