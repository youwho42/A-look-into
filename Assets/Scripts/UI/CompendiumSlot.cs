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

    public void AddItem(QI_ItemData newItem, QI_CraftingRecipe newRecipe = null)
    {
        item = newItem;
        icon.sprite = item.Icon;
        icon.enabled = true;
        itemRecipe = newRecipe;
    }


    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void DisplayIformation()
    {
        CompendiumDisplayUI.instance.DisplayItemInformation(item, itemRecipe);
    }
}
