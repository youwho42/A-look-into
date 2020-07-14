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

    private void Start()
    {
        if (item = null)
        {
            icon.enabled = false;
        }

    }

    public void AddItem(QI_ItemData newItem, int recipeQuantity)
    {
        item = newItem;
        icon.sprite = item.Icon;
        itemName.text = item.Name;
        if (!isIngredientSlot)
        {
            amount.text = recipeQuantity.ToString();
        }

        
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
    }
}
