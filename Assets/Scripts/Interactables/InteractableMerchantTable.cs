using Klaxon.SAP;
using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableMerchantTable : Interactable
{
    public bool isOpen;
    public SpriteRenderer itemIcon;
    [HideInInspector]
    public QI_ItemData item;
    [HideInInspector]
    public int amount;

    MerchantTableUI merchantTable;
    SAP_Scheduler_NPC scheduler_NPC;

    public override void Start()
    {
        base.Start();
        canInteract = false;
        merchantTable = MerchantTableUI.instance;
        
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        if (!isOpen)
        {
            var screen = LevelManager.instance.HUDBinary == 0 ? UIScreenType.None : UIScreenType.PlayerUI;
            if (UIScreenManager.instance.CurrentUIScreen() == screen)
            {
                OpenMerchantTable();
                isOpen = true;
            }
        }
        else
        {
            CloseMerchantTable();
            isOpen = false;
        }
    }

    

    private void OpenMerchantTable()
    {
        UIScreenManager.instance.DisplayScreen(UIScreenType.MerchantTableScreen);
        UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.PlayerUI);
        merchantTable.SetMerchantUI(item, amount, this);
    }

    private void CloseMerchantTable()
    {
        UIScreenManager.instance.HideAllScreens();
        if (LevelManager.instance.HUDBinary == 1)
            UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
    }

    public void RemoveItems(int quantity)
    {
        scheduler_NPC.agentInventory.RemoveItem(item, quantity);
        amount -= quantity;
        if(amount <= 0)
        {
            ClearTable();
        }
    }


    public void SetUpTable(QI_ItemData itemData, int _amount, SAP_Scheduler_NPC npc)
    {
        scheduler_NPC = npc;
        item = itemData;
        amount = _amount;
        itemIcon.sprite = item.Icon;
        canInteract = true;
        interactVerb = $"Buy {item.Name}";
    }

    public void ClearTable()
    {
        item = null;
        itemIcon.sprite = null;
        amount = 0;
        isOpen = false;
        canInteract = false;
    }

    public QI_ItemData GetTableItem()
    {
        return item;
    }
    public int GetAmount() 
    { 
        return amount; 
    }
}
