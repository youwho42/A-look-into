using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSlot : MonoBehaviour
{
    QI_ItemData item;
    public Image icon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI amount;
    public bool isIngredientSlot;
    public Button craftButton;

    

    public void AddItem(QI_ItemData newItem, int recipeQuantity, CraftingTable craftingTable)
    {
        item = newItem;
        icon.sprite = item.Icon;
        itemName.text = item.Name;
        craftButton.onClick.AddListener(craftingTable.CraftItem);
        if (!isIngredientSlot)
        {
            amount.text = recipeQuantity.ToString();
        }
        icon.enabled = true;

    }

    public void AddItem(QI_ItemData newItem, int recipeQuantity, int inventoryQuantity)
    {
        item = newItem;
        icon.sprite = item.Icon;
        itemName.text = item.Name;
        
        if (isIngredientSlot)
        {
            amount.text = inventoryQuantity.ToString() + "/" + recipeQuantity.ToString();
            
        }

        icon.enabled = true;

    }

    public void RemoveItem()
    {

    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        itemName.text = "";
        amount.text = "";
        icon.enabled = false;
    }
}
