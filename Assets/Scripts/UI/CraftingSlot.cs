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
    
    public TextMeshProUGUI amount;
    public bool isIngredientSlot;
    public Button craftButton;
    bool addedToListener;

   
    public void ShowInformation()
    {
        if (item == null || !isIngredientSlot)
            return;
        ItemInformationDisplayUI.instance.ShowItemName(item, this.GetComponent<RectTransform>());
    }
    public void HideInformation()
    {
        ItemInformationDisplayUI.instance.HideItemName();
    }

    // Recipe Result 
    public void AddItem(QI_ItemData newItem, int recipeQuantity)
    {

        item = newItem;
        icon.sprite = item.Icon;
        
        
        
        if (!isIngredientSlot)
        {
            amount.text = recipeQuantity.ToString();
        }
        icon.enabled = true;

    }

    // Recipe Ingredients
    public void AddItem(QI_ItemData newItem, int recipeQuantity, int inventoryQuantity)
    {
        item = newItem;
        icon.sprite = item.Icon;
        Color c = inventoryQuantity < recipeQuantity? Color.red:Color.black;
        
        if (isIngredientSlot)
        {
            amount.text = inventoryQuantity.ToString() + "/" + recipeQuantity.ToString();
            amount.color = c;
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
        
        amount.text = "";
        icon.enabled = false;
        
        
    }
}
