using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContainerDisplaySlot : MonoBehaviour
{

    public QI_ItemData item;
    public Image icon;

    public TextMeshProUGUI itemAmount;
    public QI_Inventory containerInventory;
    
    public bool isContainerSlot;


    private void Start()
    {
        if (item = null)
        {
            icon.enabled = false;
        }

    }

    public void TransferItem()
    {
        if(item != null)
        {
            if (isContainerSlot)
            {

                PlayerInformation.instance.playerInventory.AddItem(item, 1);
                RemoveItem();
            }
            else
            {
                containerInventory.AddItem(item, 1);
                RemoveItem();

            }
        }
        
    }

    public void AddItem(QI_ItemData newItem, int amount)
    {
        item = newItem;
        icon.sprite = item.Icon;

        itemAmount.text = amount.ToString();
        
    }
    public void RemoveItem()
    {
        if (item == null)
            return;
        if (isContainerSlot)
            containerInventory.RemoveItem(item, 1);
        else
            PlayerInformation.instance.playerInventory.RemoveItem(item, 1);
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        itemAmount.text = "";

    }
    

}
