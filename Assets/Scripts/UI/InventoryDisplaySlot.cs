using QuantumTek.QuantumInventory;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class InventoryDisplaySlot : MonoBehaviour
{
    public QI_ItemData item;
    public Image icon;
    
    public TextMeshProUGUI itemAmount;
    public QI_Inventory inventory;
    public EquipmentManager equipmentManager;
    GameObject itemToDrop;
    bool isDragged;
    public LayerMask waterLayer;

    public void ShowInformation()
    {
        if (item == null)
            return;
        ItemInformationDisplayUI.instance.ShowInformationDisplay(item);
    }
    public void HideInformation()
    {
        ItemInformationDisplayUI.instance.HideInformationDisplay();
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
        icon.enabled = true;
    }
    public void RemoveItem()
    {
        if (item == null)
            return;
        inventory.RemoveItem(item, 1);
    }
    public void DropItem(Vector3 position)
    {
        if (item == null)
            return;
        
        
        if (itemToDrop.TryGetComponent(out SaveableItemEntity itemDrop))
        {
            itemDrop.GenerateId();
        }
        if (itemToDrop.TryGetComponent(out ReplaceObjectOnItemDrop obj))
        {
            obj.CheckForObjects();
        }

        inventory.RemoveItem(item, 1);
    }



    public void DragItem()
    {
        if (!isDragged)
        {
            var go = Instantiate(item.ItemPrefab, GetMousePosition(), Quaternion.identity);
            itemToDrop = go.gameObject;
            isDragged = true;
        }
        
        
        itemToDrop.transform.position = GetMousePosition();
    }

    public void EndDragItem()
    {
        //Check if in player vicinity :)
        if (CheckPlayerVicinity() && CheckForGameObjects())
            DropItem(GetMousePosition());
        else
            Destroy(itemToDrop);
          
        isDragged = false;
    }
    
    

    bool CheckForGameObjects()
    {
        Collider2D coll = itemToDrop.GetComponent<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        List<Collider2D> results = new List<Collider2D>();
        if (Physics2D.OverlapCollider(coll, filter, results) > 0)
        {
            foreach (var hit in results)
            {
                if (hit.gameObject == itemToDrop || hit.transform.parent.gameObject == itemToDrop)
                    continue;
                if(hit.CompareTag("Water") && itemToDrop.CompareTag("RiverItem"))
                    return true;
                if (!itemToDrop.CompareTag("RiverItem"))
                {
                    if (hit.CompareTag("Grass") || hit.CompareTag("Path"))
                        return true;
                }
                NotificationManager.instance.SetNewNotification($"You can't place {item.Name} on {hit.transform.parent.name}.");
                return false;
            }
            
        }

        return true;

        
    }

    bool CheckPlayerVicinity()
    {
        Vector3 playerPos = PlayerInformation.instance.player.position;
        
        float dist = Vector2.Distance(playerPos, GetMousePosition());
        if (dist <= 0.5f)
            return true;

        NotificationManager.instance.SetNewNotification("Try placing the " + item.Name + " closer to you.");
        return false;
    }
   
    Vector3 GetMousePosition()
    {
        Vector3 movePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        movePos.z = PlayerInformation.instance.player.position.z;
        

        return movePos;
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        itemAmount.text = "";
        icon.enabled = false;
    }
    
}
