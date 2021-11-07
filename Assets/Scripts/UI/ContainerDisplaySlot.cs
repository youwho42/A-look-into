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


    

    public void TransferItem()
    {
        if(item != null)
        {

            if (isContainerSlot)
            {
                
                int a = Input.GetKey(KeyCode.LeftControl) ? containerInventory.GetStock(item.Name) : 1;
                
                PlayerInformation.instance.playerInventory.AddItem(item, a);
                containerInventory.RemoveItem(item, a);
                
                
            }
            else
            {
                int a = Input.GetKey(KeyCode.LeftControl) ? PlayerInformation.instance.playerInventory.GetStock(item.Name) : 1;
                
                containerInventory.AddItem(item, a);
                PlayerInformation.instance.playerInventory.RemoveItem(item, a);
               
            }
            ClearSlot();
        }
        
    }

    public void AddItem(QI_ItemData newItem, int amount)
    {
        item = newItem;
        icon.sprite = item.Icon;

        itemAmount.text = amount.ToString();
        icon.enabled = true;
    }
    

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        itemAmount.text = "";
        icon.enabled = false;
    }
    

}
