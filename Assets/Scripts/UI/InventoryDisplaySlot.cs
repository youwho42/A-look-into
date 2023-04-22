using QuantumTek.QuantumInventory;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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
        //var stick = PlayerInformation.instance.playerInput.rightStickPos;
        //Vector2 currentPosition = Mouse.current.position.ReadValue();
        //stick = Gamepad.current.rightStick.ReadValue();
        //for (var passedTime = 0f; passedTime < 1; passedTime += Time.deltaTime)
        //{
        //    currentPosition += stick * 15 * Time.deltaTime;
        //}
        //Mouse.current.WarpCursorPosition(currentPosition);

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
        if (EventSystem.current.currentSelectedGameObject != slotButton.gameObject || itemToDrop == null)
            return;
        //Check if in player vicinity :)
        if (CheckPlayerVicinity() && CheckForGameObjects())
            DropItem(itemToDrop.transform.position);
        else
            Destroy(itemToDrop);
        EventSystem.current.SetSelectedGameObject(null);
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

    bool CheckForGameObjects()
    {
        Collider2D coll = itemToDrop.GetComponent<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        List<Collider2D> results = new List<Collider2D>();
        if (Physics2D.OverlapCollider(coll, filter, results) > 0)
        {
            foreach (var hit in results)
            {
                if (hit.gameObject == itemToDrop /*|| hit.transform.parent.gameObject == itemToDrop*/)
                    continue;
                if(hit.CompareTag("Water") && itemToDrop.CompareTag("RiverItem"))
                    return true;
                if (!itemToDrop.CompareTag("RiverItem"))
                {
                    if (hit.CompareTag("Grass") || hit.CompareTag("Path") || hit.CompareTag("House"))
                        return true;
                }
                NotificationManager.instance.SetNewNotification("Invalid placement", NotificationManager.NotificationType.Warning);
                return false;
            }
            
        }

        return true;

        
    }

    bool CheckPlayerVicinity()
    {
        Vector3 playerPos = PlayerInformation.instance.player.position;
        
        float dist = Vector2.Distance(playerPos, itemToDrop.transform.position);
        if (dist <= 0.5f)
            return true;

        NotificationManager.instance.SetNewNotification("Too far", NotificationManager.NotificationType.Warning);
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
