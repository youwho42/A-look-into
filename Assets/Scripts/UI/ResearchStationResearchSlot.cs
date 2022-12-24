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
            textToAdd += item.ResearchRecipes[i].Name + "\n";
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
        if (item != null && CheckForInventoryQuantity())
        {
            for (int i = 0; i < item.ResearchRecipes.Count; i++)
            {
                PlayerInformation.instance.playerStats.AddToAgency(item.AgencyReward);
                PlayerCrafting.instance.AddCraftingRecipe(item.ResearchRecipes[i]);
                NotificationManager.instance.SetNewNotification($"{item.ResearchRecipes[i].Name} recipe learned", NotificationManager.NotificationType.Compedium);
            }
            ResearchStationDisplayUI.instance.UpdateResearchDisplay();
        }
        ClearSlot();
    }

    public bool CheckForInventoryQuantity()
    {
        
            int t = PlayerInformation.instance.playerInventory.GetStock(item.Name);
            if (t < item.RecipeRevealAmount)
            {
            string plural = item.RecipeRevealAmount - t == 1 ? "" : "'s";
            NotificationManager.instance.SetNewNotification($"{item.RecipeRevealAmount - t} {item.Name}{plural} missing", NotificationManager.NotificationType.Warning);
                return false;
            }

        
        return true;
    }

}
