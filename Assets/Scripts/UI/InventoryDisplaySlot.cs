using QuantumTek.QuantumInventory;
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

[Serializable]
public class NavNodeQuadTree
{
    public Bounds bounds;
    public int capacity;
    public List<NavigationNode> allSpots = new List<NavigationNode>();
    public bool divided;
    public NavNodeQuadTree northWest;
    public NavNodeQuadTree northEast;
    public NavNodeQuadTree southWest;
    public NavNodeQuadTree southEast;

    public NavNodeQuadTree(Bounds bounds, int capacity)
    {
        this.bounds = bounds;
        this.capacity = capacity;
    }

    public void Insert(NavigationNode spot)
    {
        if (!bounds.Contains(spot.transform.position))
            return;

        if (allSpots.Count < capacity)
            allSpots.Add(spot);
        else
        {
            if (!divided)
                Subdivide();


            northEast.Insert(spot);
            northWest.Insert(spot);
            southWest.Insert(spot);
            southEast.Insert(spot);
        }

    }



    public void Subdivide()
    {
        Bounds nw = new Bounds(new Vector3(bounds.center.x - bounds.extents.x / 2, bounds.center.y + bounds.extents.y / 2), bounds.extents);
        northWest = new NavNodeQuadTree(nw, capacity);
        Bounds ne = new Bounds(new Vector3(bounds.center.x + bounds.extents.x / 2, bounds.center.y + bounds.extents.y / 2), bounds.extents);
        northEast = new NavNodeQuadTree(ne, capacity);
        Bounds sw = new Bounds(new Vector3(bounds.center.x - bounds.extents.x / 2, bounds.center.y - bounds.extents.y / 2), bounds.extents);
        southWest = new NavNodeQuadTree(sw, capacity);
        Bounds se = new Bounds(new Vector3(bounds.center.x + bounds.extents.x / 2, bounds.center.y - bounds.extents.y / 2), bounds.extents);
        southEast = new NavNodeQuadTree(se, capacity);

        divided = true;
    }


    public List<NavigationNode> QueryTree(Bounds boundry)
    {

        List<NavigationNode> spots = new List<NavigationNode>();

        if (!bounds.Intersects(boundry))
            return spots;

        foreach (var spot in allSpots)
        {
            if (boundry.Contains(spot.transform.position))
                spots.Add(spot);

        }

        if (divided)
        {
            spots.AddRange(northWest.QueryTree(boundry));
            spots.AddRange(northEast.QueryTree(boundry));
            spots.AddRange(southWest.QueryTree(boundry));
            spots.AddRange(southEast.QueryTree(boundry));
        }

        return spots;
    }



}




public class InventoryDisplaySlot : MonoBehaviour
{
    public QI_ItemData item;
    public Image icon;
    
    public TextMeshProUGUI itemAmount;
    public TextMeshProUGUI itemUse;
    public TextMeshProUGUI variantsDisplay;
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

    public List<NavigationNode> allAreas = new List<NavigationNode>();
    public Bounds baseBounds = new Bounds(new Vector3(13, -10, 0), new Vector3(128, 128, 20));
    NavNodeQuadTree quadTree;
    Bounds bounds;

    float dragTimer;
    


    [Serializable]
    public struct ItemTypeNames
    {
        public string itemDataName;
        public LocalizedString itemUseName;
    }
    private void Start()
    {
        allAreas.Clear();
        allAreas = FindObjectsOfType<NavigationNode>().ToList();
        

        quadTree = new NavNodeQuadTree(baseBounds, 10);

        foreach (var spot in allAreas)
        {
            quadTree.Insert(spot);
        }



        slotButton = GetComponentInChildren<Button>();
    }

    public List<NavigationNode> QueryQuadTree(Bounds boundry)
    {
        return quadTree.QueryTree(boundry);
    }

    private void OnEnable()
    {
        GameEventManager.onInventoryDragEvent.AddListener(DragItem);
        GameEventManager.onInventoryRightClickEvent.AddListener(SetItemSelected);
        GameEventManager.onInventoryRightClickReleaseEvent.AddListener(EndDragItem);
        GameEventManager.onRotateDecoration.AddListener(RotateDecoration);
    }

    private void OnDisable()
    {
        GameEventManager.onInventoryDragEvent.RemoveListener(DragItem);
        GameEventManager.onInventoryRightClickEvent.RemoveListener(SetItemSelected);
        GameEventManager.onInventoryRightClickReleaseEvent.RemoveListener(EndDragItem);
        GameEventManager.onRotateDecoration.RemoveListener(RotateDecoration);

        if (itemToDrop)
        {
            Destroy(itemToDrop);
            ResetDragging();
        }
            
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

    public void AddItem(QI_ItemData newItem, int amount)
    {
        item = newItem;
        icon.sprite = item.Icon;
        
        itemAmount.text = amount.ToString();
        icon.enabled = true;
        itemTypeName = GetItemType();
        ShowItemUse(false);
        if (item.ItemPrefabVariants.Count > 1)
            variantsDisplay.gameObject.SetActive(true);
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
            itemDrop.GenerateId();

        if (itemToDrop.TryGetComponent(out QI_Item i))
            i.itemVariantIndex = decorationIndex;

        var replace = itemToDrop.GetComponent<Interactable>().replaceObjectOnDrop;
        if (replace != null)
            replace.CheckForObjects();
        
        
        if(item.placementGumption != null)
            PlayerInformation.instance.statHandler.AddModifiableModifier(item.placementGumption);

        inventory.RemoveItem(item, 1);
    }



    public void DragItem()
    {
        if (EventSystem.current.currentSelectedGameObject != slotButton.gameObject || item == null)
            return;

        PlayerInformation.instance.isDragging = true;

        dragTimer += Time.deltaTime;
        
        if (!isDragged)
        {
            
            var prefab = item.ItemPrefabVariants[0];
            if (item.Type == ItemType.Decoration)
            {
                
                prefab = item.ItemPrefabVariants[decorationIndex];
            }
                
            var go = Instantiate(prefab, GetMousePosition(), Quaternion.identity);
            itemToDrop = go.gameObject;
            isDragged = true;
        }

        SetValidity();
        

        itemToDrop.transform.position = GetMousePosition();
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
        
        DropItem(itemToDrop.transform.position);
        EventSystem.current.SetSelectedGameObject(null);
        ResetDragging();

    }

    void ResetDragging()
    {
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

        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = LayerMask.NameToLayer("Obstacle");
        List<Collider2D> results = new List<Collider2D>();
        coll.OverlapCollider(filter, results);
        
        
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
                    if (interactable.canPlaceOnOther)
                    {
                        interactable.visualItem.localPosition = obj.displacedPosition;
                        if (obj.isDecorationSurface)
                            return false;
                    }
                }
                
                return true;
            }
        }
            
        return false;
    }


    bool PlacedOnNavigationNodes(Collider2D coll)
    {
        bounds = new Bounds(itemToDrop.transform.position, new Vector3(1, 1, 1));
        List<NavigationNode> closestSpots = quadTree.QueryTree(bounds);
        foreach (var spot in closestSpots)
        {
            if (coll.OverlapPoint(spot.transform.position))
                return true;
        }
        return false;
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
        decorationIndex = 0;
        variantsDisplay.gameObject.SetActive(false);
    }
    
}
