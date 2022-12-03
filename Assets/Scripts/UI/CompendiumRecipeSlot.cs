using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompendiumRecipeSlot : MonoBehaviour
{
    public QI_ItemData item;
    public Image icon;
    public TextMeshProUGUI quantityText;
    

    public void AddItem(QI_ItemData newItem, int quantity)
    {
        item = newItem;
        icon.sprite = item.Icon;
        icon.enabled = true;
        quantityText.text = quantity.ToString();
    }


    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        quantityText.text = "";
    }

    
}
