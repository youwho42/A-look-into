using System;
using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class CraftingRecipeButton : MonoBehaviour
{

    QI_CraftingRecipe item;
    public TextMeshProUGUI recipeName;
    CraftingStationDisplayUI craftingStation;
    public Button button;

    private void Start()
    {
        craftingStation = CraftingStationDisplayUI.instance;
        button.onClick.AddListener(SetCurrentRecipe);
        button.onClick.AddListener(SetTutorial);
    }

    private void SetCurrentRecipe()
    {
        craftingStation.SetCurrentRecipe(item);
        AudioManager.instance.PlaySound("Crafting_SetRecipe");
    }

    private void SetTutorial()
    {
        craftingStation.tutorial.SetNextTutorialIndex(1);
    }

    public void AddItem(QI_CraftingRecipe newItem)
    {
        item = newItem;
        var n = item.Product.Item.localizedName.GetLocalizedString();
        recipeName.text = item.Product.Amount > 1 ? $"{n} ({item.Product.Amount})" : n;
        
    }

    void ClearSlot()
    {
        item = null;
        recipeName.text = "";
    }
}
