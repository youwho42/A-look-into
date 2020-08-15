using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InventoryDisplaySlot : MonoBehaviour
{
    public QI_ItemData item;
    public Image icon;
    
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
        
    }
    public void RemoveItem()
    {
        if (item == null)
            return;
        inventory.RemoveItem(item, 1);
    }
    public void DropItem()
    {
        if (item == null)
            return;
        var go = Instantiate(item.ItemPrefab, PlayerInformation.instance.player.transform.position + ItemDropOffset(), Quaternion.identity);
        go.GetComponent<SaveableEntity>().GenerateId();
        inventory.RemoveItem(item, 1);
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        itemAmount.text = "";
        
    }
    Vector3 ItemDropOffset()
    {
        return new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f),0);
    }
}
