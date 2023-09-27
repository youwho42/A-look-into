using QuantumTek.QuantumInventory;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Klaxon.SaveSystem;
using Unity.Collections.LowLevel.Unsafe;

public class InventoryDisplaySlot : MonoBehaviour
{
    public QI_ItemData item;
    public Image icon;
    
    public TextMeshProUGUI itemAmount;
    public TextMeshProUGUI itemUse;
    public QI_Inventory inventory;
    public EquipmentManager equipmentManager;
    GameObject itemToDrop;
    bool isDragged;
    public LayerMask waterLayer;
    string itemTypeName = "";
    public List<ItemTypeNames> itemTypes = new List<ItemTypeNames>();

    Button slotButton;
    bool buttonHeld;

    //Vector3Int currentTilePos;
    //List<TileDirectionInfo> tileBlockInfo = new List<TileDirectionInfo>();

    [Serializable]
    public struct ItemTypeNames
    {
        public string itemDataName;
        public string itemUseName;
    }
    private void Start()
    {
        slotButton = GetComponentInChildren<Button>();
    }
    private void OnEnable()
    {
        GameEventManager.onInventoryDragEvent.AddListener(DragItem);
        GameEventManager.onInventoryRightClickEvent.AddListener(SetItemSelected);
        GameEventManager.onInventoryRightClickReleaseEvent.AddListener(EndDragItem);
    }

    private void OnDisable()
    {
        GameEventManager.onInventoryDragEvent.RemoveListener(DragItem);
        GameEventManager.onInventoryRightClickEvent.RemoveListener(SetItemSelected);
        GameEventManager.onInventoryRightClickReleaseEvent.RemoveListener(EndDragItem);
    }
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

    public string GetItemType()
    {
        string n = item.GetType().Name;
        if (n == "QI_ItemData")
            return "";
        for (int i = 0; i < itemTypes.Count; i++)
        {
            if (n.Contains(itemTypes[i].itemDataName))
                return itemTypes[i].itemUseName;
        }

        return "";
    }

    public void ShowItemUse(bool active)
    {
        if (item != null)
            itemUse.text = active ? itemTypeName : "";
        
    }

    public void AddItem(QI_ItemData newItem, int amount)
    {
        item = newItem;
        icon.sprite = item.Icon;
        
        itemAmount.text = amount.ToString();
        icon.enabled = true;
        itemTypeName = GetItemType();
        ShowItemUse(false);
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
        if (EventSystem.current.currentSelectedGameObject != slotButton.gameObject)
            return;
        

        if (!isDragged)
        {
            var go = Instantiate(item.ItemPrefab, GetMousePosition(), Quaternion.identity);
            itemToDrop = go.gameObject;
            isDragged = true;
        }

        
        if (!CheckPlayerVicinity() || CheckForObstacles() || !CheckTileValid())
            TurnObjectColor(Color.red);
        else
            TurnObjectColor(Color.white);
        
        itemToDrop.transform.position = GetMousePosition();
    }

    void TurnObjectColor(Color color)
    {
        var sprites = itemToDrop.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sprite in sprites)
        {
            sprite.color = color;
        }
    }
    public void EndDragItem()
    {
        if (EventSystem.current.currentSelectedGameObject != slotButton.gameObject || itemToDrop == null)
            return;
        //Check if in player vicinity :)

        if (!CheckPlayerVicinity())
        {
            NotificationManager.instance.SetNewNotification($"Too far from {PlayerInformation.instance.playerName}", NotificationManager.NotificationType.Warning);
            
            Destroy(itemToDrop);
            ResetDragging();
            return;
        }
        
        if (CheckForObstacles() || !CheckTileValid())
        {
            NotificationManager.instance.SetNewNotification($"Invalid spot for {item.Name}", NotificationManager.NotificationType.Warning);
            Destroy(itemToDrop);
            ResetDragging();
            return;
        }
        DropItem(itemToDrop.transform.position);
        
        EventSystem.current.SetSelectedGameObject(null);
        ResetDragging();

    }

    void ResetDragging()
    {
        isDragged = false;
        itemToDrop = null;
    }
    
    void SetItemSelected()
    {

        if (RectTransformUtility.RectangleContainsScreenPoint(slotButton.GetComponent<RectTransform>(), Mouse.current.position.ReadValue()))
        {
            EventSystem.current.SetSelectedGameObject(slotButton.gameObject);
            //buttonHeld = true;
        }
    }

    bool CheckForObstacles()
    {
        Collider2D coll = itemToDrop.GetComponentInChildren<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = LayerMask.NameToLayer("Obstacle");
        List<Collider2D> results = new List<Collider2D>();
        coll.OverlapCollider(filter, results);
        
        if (results.Count > 0)
        {
            foreach (var hit in results)
            {
                if (hit.transform.IsChildOf(itemToDrop.transform))
                    continue;

                
                return true;
            }
        }
            
        return false;
    }





    bool CheckTileValid()
    {
        var gManager = GridManager.instance;
        var pos = itemToDrop.transform.position;
        pos.z -= 1;
        Vector3Int posDown = gManager.grid.WorldToCell(pos);
        
        Vector3Int posUp = new Vector3Int(posDown.x, posDown.y, posDown.z + 1);
        
        if (gManager.groundMap.HasTile(posDown) && !gManager.groundMap.HasTile(posUp)) 
            return true;

        return false;
        
    }

    bool CheckPlayerVicinity()
    {
        
        Vector3 playerPos = PlayerInformation.instance.player.position;
        
        float dist = Vector2.Distance(playerPos, itemToDrop.transform.position);
        if (dist <= 0.5f)
            return true;

        return false;
    }
   
    Vector3 GetMousePosition()
    {
        
        Vector3 movePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        movePos.z = PlayerInformation.instance.player.position.z;
        
        return movePos;
    }


    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        itemAmount.text = "";
        icon.enabled = false;
        itemUse.text = "";
    }
    
}
