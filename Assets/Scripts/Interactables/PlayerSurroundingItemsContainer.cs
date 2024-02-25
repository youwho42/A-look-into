using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSurroundingItemsContainer : MonoBehaviour
{
    ContainerInventoryDisplayUI containerUI;
    public QI_Inventory inventory;
    public QI_Inventory crossCheckInventory;
    public float surroundingAreaRadius = 0.5f;
    public LayerMask detectableLayer;

    public void Start()
    {
        containerUI = ContainerInventoryDisplayUI.instance;
        GameEventManager.onSurroundingItemsEvent.AddListener(TryGetSurroundingtItems);
        //GameEventManager.onInventoryUpdateEvent.AddListener(CrossCheckCurrentItems);
    }
    public void OnDestroy()
    {
        GameEventManager.onSurroundingItemsEvent.RemoveListener(TryGetSurroundingtItems);
       //s GameEventManager.onInventoryUpdateEvent.RemoveListener(CrossCheckCurrentItems);

    }

    void TryGetSurroundingtItems()
    {
        inventory.RemoveAllItems();
        foreach (var item in GetSurroundingItems())
        {
            inventory.AddItem(item, 1, false);
        }

        if (inventory.Stacks.Count <= 0)
            return;
        
        //show or don't, depending...
        if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
            OpenContainer();
        else if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.ContainerUI)
            CloseContainer();
    }

    private void OpenContainer()
    {
        if (UIScreenManager.instance.DisplayIngameUI(UIScreenType.ContainerUI, true))
            containerUI.ShowContainerUI(inventory);
    }

    private void CloseContainer()
    {
        UIScreenManager.instance.HideScreenUI();
        containerUI.HideContainerUI();
    }

    List<QI_ItemData> GetSurroundingItems()
    {
        List<QI_ItemData> allItems = new List<QI_ItemData>();
        var pos = PlayerInformation.instance.player.position;
        var hits = Physics2D.OverlapCircleAll(pos, surroundingAreaRadius, detectableLayer, pos.z, pos.z);
        if (hits.Length > 0)
        {
            foreach (var surroundingObject in hits)
            {
                if (surroundingObject.TryGetComponent(out QI_Item item))
                {
                    if (item.Data.Type == ItemType.Animal || 
                        item.Data.Type == ItemType.Decoration || 
                        item.Data.Type == ItemType.Reading || 
                        item.Data.Type == ItemType.Utility)
                        continue;

                    allItems.Add(item.Data);
                }
                    
            }
        }
        return allItems;
    }

    void CrossCheckCurrentItems()
    {
        List<QI_ItemData> itemsToDestroy = new List<QI_ItemData>();
        crossCheckInventory.RemoveAllItems();
        foreach (var item in GetSurroundingItems())
        {
            crossCheckInventory.AddItem(item, 1, false);
        }
        foreach (var stack in inventory.Stacks)
        {
            foreach (var realWorldStack in crossCheckInventory.Stacks)
            {
                if(realWorldStack.Item == stack.Item)
                {
                    int diff = realWorldStack.Amount - stack.Amount;
                    for (int i = 0; i < diff; i++)
                    {
                        itemsToDestroy.Add(realWorldStack.Item);
                    }
                }
            }
        }
        var destroyItems = new List<GameObject>();
        var allItems = GetSurroundingGameObjects();
        foreach (var item in itemsToDestroy)
        {
            for (int i = 0; i < allItems.Count; i++)
            {
                if (allItems[i].Data == item)
                {
                    destroyItems.Add(allItems[i].gameObject);
                }
            }
        }

        foreach (var item in destroyItems)
        {
            Destroy(item);
        }

    }

    List<QI_Item> GetSurroundingGameObjects()
    {
        List<QI_Item> allItems = new List<QI_Item>();
        var pos = PlayerInformation.instance.player.position;
        var hits = Physics2D.OverlapCircleAll(pos, surroundingAreaRadius, detectableLayer, pos.z, pos.z);
        if (hits.Length > 0)
        {
            foreach (var surroundingObject in hits)
            {
                if (surroundingObject.TryGetComponent(out QI_Item item))
                    allItems.Add(item);

            }
        }
        return allItems;
    }

}
