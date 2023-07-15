using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableMerchantTable : Interactable
{
    bool isOpen;
    public SpriteRenderer itemIcon;
    QI_ItemData item;
    [HideInInspector]
    public int amount;

    MerchantTableUI merchantTable;

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
            if (UIScreenManager.instance.CurrentUIScreen() == UIScreenType.PlayerUI)
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
        UIScreenManager.instance.HideScreens(UIScreenType.MerchantTableScreen);
        UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
    }

    public void RemoveItems(int quantity)
    {
        amount -= quantity;
        if(amount <=0)
        {
            ClearTable();
        }
    }


    public void SetUpTable(QI_ItemData itemData, int _amount)
    {
        item = itemData;
        amount = _amount;
        itemIcon.sprite = item.Icon;
        canInteract = true;
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
