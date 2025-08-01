﻿using QuantumTek.QuantumInventory;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Klaxon.SaveSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Klaxon.Interactable;

using System.Linq;

public class InventoryDisplaySlot : MonoBehaviour
{
    public QI_ItemData item;
    public Image icon;
    
    public TextMeshProUGUI itemAmount;
    public TextMeshProUGUI itemUse;
    public TextMeshProUGUI variantsDisplay;
    public TextMeshProUGUI researchDisplay;
    public QI_Inventory inventory;
    public EquipmentManager equipmentManager;
    GameObject itemToDrop;
    bool isDragged;
    public LayerMask waterLayer;
    string itemTypeName = "";
    public List<ItemTypeNames> itemTypes = new List<ItemTypeNames>();

    Button slotButton;
    
    bool buttonHeld;

    int decorationIndex = 0;

    int stackAmount;
    
    Bounds bounds;

    float dragTimer;

    DropAmountUI dropAmountUI;
    Vector3 dropPosition;
    bool mouseHover;
    [HideInInspector]
    public int inventoryStackIndex = -1;
    InventoryDisplayUI currentDisplay;

    [Serializable]
    public struct ItemTypeNames
    {
        public string itemDataName;
        public LocalizedString itemUseName;
    }
    private void Start()
    {
        slotButton = GetComponentInChildren<Button>();
    }

    
    private void OnEnable()
    {
        //GameEventManager.onInventoryDragEvent.AddListener(DragItem);
        GameEventManager.onInventoryRightClickEvent.AddListener(SetItemSelected);
        GameEventManager.onInventoryRightClickReleaseEvent.AddListener(EndDragItem);
        GameEventManager.onRotateDecoration.AddListener(RotateDecoration);
        
    }

    private void OnDisable()
    {
        //GameEventManager.onInventoryDragEvent.RemoveListener(DragItem);
        GameEventManager.onInventoryRightClickEvent.RemoveListener(SetItemSelected);
        GameEventManager.onInventoryRightClickReleaseEvent.RemoveListener(EndDragItem);
        GameEventManager.onRotateDecoration.RemoveListener(RotateDecoration);

        if (itemToDrop)
        {
            Destroy(itemToDrop);
            ResetDragging();
        }
        UIScreenManager.instance.CloseDropAmountUI();
    }
    
    public void SetMouseHover(bool state)
    {
        currentDisplay.currentHoverStack = state ? inventoryStackIndex : -1;
    }

    public void ShowTip(string tipName)
    {
        ItemInformationDisplayUI.instance.ShowTip(tipName, this.GetComponent<RectTransform>());
    }

    public void ShowInformation()
    {
        if (item == null)
            return;
        ItemInformationDisplayUI.instance.ShowItemName(item, this.GetComponent<RectTransform>());
    }
    public void HideInformation()
    {
        ItemInformationDisplayUI.instance.HideItemName();
    }
    
    public void UseItem()
    {
        if (item == null)
            return;
        item.UseItem();
        
    }

    void RotateDecoration()
    {

        if (EventSystem.current.currentSelectedGameObject != slotButton.gameObject || item == null)
            return;

        decorationIndex = (decorationIndex+1) % item.ItemPrefabVariants.Count;
        Destroy(itemToDrop.gameObject);
        var go = Instantiate(item.ItemPrefabVariants[decorationIndex], GetMousePosition(), Quaternion.identity);
        itemToDrop = go.gameObject;
        SetValidity();
    }
    public string GetItemType()
    {
        string n = item.GetType().Name;
        if (n == "QI_ItemData")
            return "";
        for (int i = 0; i < itemTypes.Count; i++)
        {
            if (n.Contains(itemTypes[i].itemDataName))
            {
                var word = LocalizationSettings.StringDatabase.GetLocalizedString("Variable-Texts", itemTypes[i].itemDataName);
            
                return word;
            }
        }

        return "";
    }

    public void ShowItemUse(bool active)
    {
        if (item != null)
            itemUse.text = active ? itemTypeName : "";
        
    }
    public void SetupSlot(int stackIndex, InventoryDisplayUI display)
    {
        inventoryStackIndex = stackIndex;
        currentDisplay = display;
    }
    public void AddItem(QI_ItemData newItem, int amount)
    {
        item = newItem;
        icon.sprite = item.Icon;
        stackAmount = amount;
        itemAmount.text = stackAmount.ToString();
        icon.enabled = true;
        itemTypeName = GetItemType();
        ShowItemUse(false);
        
        if (item.ItemPrefabVariants.Count > 1)
            variantsDisplay.gameObject.SetActive(true);
        UpdateRecipeRevealIcon();
    }

    void UpdateRecipeRevealIcon()
    {
        researchDisplay.gameObject.SetActive(HasRecipeToReveal());
    }
    bool HasRecipeToReveal()
    {
        
        foreach (var recipe in item.ResearchRecipes)
        {
            if (!PlayerInformation.instance.playerRecipeDatabase.CraftingRecipes.Contains(recipe.recipe))
                return true;
        }
        
        return false;
    }

    public void RemoveItem()
    {
        if (item == null)
            return;
        inventory.RemoveItem(item, 1);
    }

    void OpenDropAmountUI()
    {
        if (itemToDrop.GetComponent<InteractablePickUp>() == null)
        {
            DropItem(1);
            return;
        }
            


        int quantity = inventory.GetStock(item.Name);
        if (quantity > 1)
            dropAmountUI = UIScreenManager.instance.DisplayDropAmountUI(this, quantity, stackAmount, Mouse.current.position.ReadValue());
        else
            DropItem(1);
        
    }
    public void SetDropAmount()
    {
        DropItem(dropAmountUI.CurrentAmount);
        UIScreenManager.instance.CloseDropAmountUI();
    }
    public void DropItem(int quantity)
    {
        if (item == null)
            return;

        if(quantity == 0)
        {
            Destroy(itemToDrop);
            ResetDragging();
            return;
        }

        itemToDrop.transform.position = dropPosition;

        if (itemToDrop.TryGetComponent(out SaveableItemEntity itemDrop))
            itemDrop.GenerateId();

        if (itemToDrop.TryGetComponent(out QI_Item i))
            i.itemVariantIndex = decorationIndex;

        if (itemToDrop.TryGetComponent(out Interactable interactable))
        {
            var replace = interactable.replaceObjectOnDrop;
            if (replace != null)
                replace.CheckForObjects();
            interactable.isBeingDragged = false;
        }
        

        if (itemToDrop.TryGetComponent(out InteractablePickUp pickUpItem))
            pickUpItem.pickupQuantity = quantity;

        if (item.placementGumption != null)
        PlayerInformation.instance.statHandler.AddModifiableModifier(item.placementGumption);

        if (itemToDrop.TryGetComponent(out ScarekernutInteractable scarekernut))
            scarekernut.visibleRadius.SetActive(false);
        // get interactablePickUp and add said amount to drop
        inventory.RemoveItem(item, quantity);
        
        EventSystem.current.SetSelectedGameObject(null);
        ResetDragging();
    }


    public void PointerRelease()
    {
        currentDisplay.ResetStackImage();
        if (EventSystem.current.currentSelectedGameObject != slotButton.gameObject || item == null)
            return;
        if (currentDisplay.currentHoverStack == -1 || currentDisplay.currentHoverStack == inventoryStackIndex)
            return;
        var inventory = PlayerInformation.instance.playerInventory;
        inventory.SwapStacks(inventoryStackIndex, currentDisplay.currentHoverStack);
        EventSystem.current.SetSelectedGameObject(currentDisplay.inventorySlots[currentDisplay.currentHoverStack].slotButton.gameObject);
        currentDisplay.UpdateInventoryUI();

    }

    public void DragItem()
    {
        if (item == null)
            return;
        if (PlayerInformation.instance.inventorySlot != null && PlayerInformation.instance.inventorySlot != this)
            return;

        if (EventSystem.current.IsPointerOverGameObject())
        {
            if(itemToDrop != null)
            {
                Destroy(itemToDrop);
                ResetDragging();
                
            }
            currentDisplay.dragableStack.color = new Color(1, 1, 1, 1);
            currentDisplay.dragableStack.sprite = item.Icon;
            currentDisplay.dragableStack.rectTransform.position = Mouse.current.position.ReadValue();
            return;
        }

        currentDisplay.ClearStack();
        dragTimer += Time.deltaTime;

        if (!isDragged/* && mouseHover*/)
        {

            var prefab = item.ItemPrefabVariants[0];
            if (item.Type == ItemType.Decoration)
            {

                prefab = item.ItemPrefabVariants[decorationIndex];
            }

            var go = Instantiate(prefab, GetMousePosition(), Quaternion.identity);
            itemToDrop = go.gameObject;
            isDragged = true;
            
            if (itemToDrop.TryGetComponent(out Interactable interactable))
                interactable.isBeingDragged = true;
            

            PlayerInformation.instance.inventorySlot = this;
            PlayerInformation.instance.isDragging = true;
            if(go.TryGetComponent(out ScarekernutInteractable scarekernut))
                scarekernut.visibleRadius.SetActive(true);

        }

        if (itemToDrop != null)
        {
            itemToDrop.transform.position = GetMousePosition();
            SetValidity();
            
        }
        
    }
    void SetValidity()
    {
        if (!CheckPlayerVicinity() || !CheckTileValid() || CheckForObstacles())
            TurnObjectColor(Color.red);
        else
            TurnObjectColor(Color.white);
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
        if (dragTimer < 0.3f)
        {
            Destroy(itemToDrop);
            ResetDragging();
            return;
        }
            

        

        if (!CheckPlayerVicinity())
        {
            Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Too far"), null, 0, NotificationsType.Warning);

            Destroy(itemToDrop);
            ResetDragging();
            return;
        }
        
        
        if (CheckForObstacles() || !CheckTileValid())
        {
            Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Invalid Spot"), null, 0, NotificationsType.Warning);

            Destroy(itemToDrop);
            ResetDragging();
            return;
        }

        dropPosition = itemToDrop.transform.position;
        OpenDropAmountUI();
        

    }

    void ResetDragging()
    {
        PlayerInformation.instance.inventorySlot = null;
        PlayerInformation.instance.isDragging = false;
        isDragged = false;
        itemToDrop = null;
        dragTimer = 0;
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
        
        Collider2D coll = itemToDrop.GetComponentInChildren<PolygonCollider2D>();
        if(coll == null)
            coll = itemToDrop.GetComponentInChildren<Collider2D>();
        if (coll == null)
            coll = itemToDrop.GetComponent<Collider2D>();

        if (PlacedOnNavigationNodes(coll))
            return true;

        if (item.onlyPlacedInPlayerHouse)
        {
            if (!PlayerHouseCollision(coll))
                return true;
        }
            

        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = LayerMask.NameToLayer("Obstacle");
        
        List<Collider2D> results = new List<Collider2D>();
        coll.Overlap(filter, results);
        
        
        var interactable = itemToDrop.GetComponent<Interactable>();
        if(interactable!=null)
            interactable.visualItem.localPosition = Vector3.zero;

        if (results.Count > 0)
        {
            foreach (var hit in results)
            {
                if (hit.transform == itemToDrop.transform || hit.transform.IsChildOf(itemToDrop.transform))
                    continue;
                
                
                if (hit.TryGetComponent(out DrawZasYDisplacement obj))
                {
                    if (obj.positionZ == 0)
                        return false;
                    if (interactable != null)
                    {
                        if (interactable.canPlaceOnOther)
                        {
                            interactable.visualItem.localPosition = obj.displacedPosition;
                            if (obj.isDecorationSurface)
                                return false;
                        }
                    }
                        
                }
                return true;
            }
        }
            
        return false;
    }

    bool PlayerHouseCollision(Collider2D coll)
    {
        if (!item.onlyPlacedInPlayerHouse)
            return false;
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        filter.useDepth = true;
        filter.minDepth = coll.transform.position.z;
        filter.maxDepth = coll.transform.position.z;
        List<Collider2D> results = new List<Collider2D>();
        coll.Overlap(filter, results);
        foreach (var c in results)
        {
            if (c.gameObject.CompareTag("PlayerHouse"))
                return true;
        }
        return false;
    }

    bool PlacedOnNavigationNodes(Collider2D coll)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("NavNode"));
        filter.useTriggers = true;
        filter.useDepth = true;
        filter.minDepth = coll.transform.position.z;
        filter.maxDepth = coll.transform.position.z;
        List <Collider2D> results = new List<Collider2D>();
        coll.Overlap(filter, results);
        
        return results.Count > 0;
    }

    bool CheckTileValid()
    {
        var gManager = GridManager.instance;
        var pos = itemToDrop.transform.position;
        pos.z -= 1;
        Vector3Int posDown = gManager.Grid.WorldToCell(pos);
        
        Vector3Int posUp = new Vector3Int(posDown.x, posDown.y, posDown.z + 1);
        
        if (gManager.groundMap.HasTile(posDown) && !gManager.groundMap.HasTile(posUp)) 
            return true;
        
        return false;
        
    }

    bool CheckPlayerVicinity()
    {
        
        Vector3 playerPos = PlayerInformation.instance.player.position;
        
        float dist = Vector2.Distance(playerPos, itemToDrop.transform.position);
        if (dist <= 1.5f)
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
        icon.color = new Color(1, 1, 1, 0);
        itemUse.text = "";
        decorationIndex = 0;
        variantsDisplay.gameObject.SetActive(false);
        researchDisplay.gameObject.SetActive(false);
    }

    
}
