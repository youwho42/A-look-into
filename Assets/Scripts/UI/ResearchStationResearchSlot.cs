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
                PlayerCrafting.instance.AddCraftingRecipe(item.ResearchRecipes[i]);
                NotificationManager.instance.SetNewNotification("You learned the " + item.ResearchRecipes[i].Name + " recipe.");
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
                NotificationManager.instance.SetNewNotification("You are missing " + (item.RecipeRevealAmount - t) + " " + item.Name + " to reveal its recipes.");
                return false;
            }

        
        return true;
    }

}
