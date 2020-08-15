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
    CraftingTable craftingTable;
    public Button button;

    private void Start()
    {
        //craftingTable = GetComponentInParent<CraftingTable>();
        button.onClick.AddListener(SetCurrentRecipe);
    }

    private void SetCurrentRecipe()
    {
        craftingTable.SetCurrentRecipe(item);
        AudioManager.instance.PlaySound("Crafting_SetRecipe");
    }

    public void AddItem(QI_CraftingRecipe newItem, CraftingTable craftingTable)
    {
        item = newItem;
        recipeName.text = item.Name;
        this.craftingTable = craftingTable;
    }

    void ClearSlot()
    {
        item = null;
        recipeName.text = "";
    }
}
