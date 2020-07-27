using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryDisplaySlot : MonoBehaviour
{
    QI_ItemData item;
    public Image icon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemAmount;
    public QI_Inventory inventory;
    public EquipmentManager equipmentManager;
    
    private void Start()
    {
        if(item = null)
        {
            icon.enabled = false;
        }
        
    }
    
    public void UseItem()
    {
        if (item == null)
            return;
        item.UseItem();
    }

    public void AddItem(QI_ItemData newItem, int amount)
    {
        item = newItem;
        icon.sprite = item.Icon;
        
        itemAmount.text = amount.ToString();
        itemName.text = item.Name;
    }

    public void RemoveItem()
    {
        if (item == null)
            return;
        Instantiate(item.ItemPrefab, inventory.transform.position + ItemDropOffset(), Quaternion.identity);
        inventory.RemoveItem(item, 1);
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        itemAmount.text = "";
        itemName.text = "";
    }
    Vector3 ItemDropOffset()
    {
        return new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f),0);
    }
}
