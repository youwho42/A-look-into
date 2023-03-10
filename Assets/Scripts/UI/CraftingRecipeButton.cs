using System;
using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using TMPro;
using UnityEngine;
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
    }

    private void SetCurrentRecipe()
    {
        craftingStation.SetCurrentRecipe(item);
        AudioManager.instance.PlaySound("Crafting_SetRecipe");
    }

    public void AddItem(QI_CraftingRecipe newItem)
    {
        item = newItem;
        recipeName.text = item.Name;
        
    }

    void ClearSlot()
    {
        item = null;
        recipeName.text = "";
    }
}
