﻿using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class ContainerDisplaySlot : MonoBehaviour
{

    public QI_ItemData item;
    public Image icon;

    public TextMeshProUGUI itemAmount;
    public QI_Inventory containerInventory;
    
    public bool isContainerSlot;


    public bool canTransfer;

    DropAmountUI dropAmountUI;
    

    private void Start()
    {
        GameEventManager.onStackTransferGamepadEvent.AddListener(TransferStack);
    }
    private void OnDisable()
    {
        GameEventManager.onStackTransferGamepadEvent.RemoveListener(TransferStack);
    }

    

    public void TransferItem(bool isLeftButton)
    {
        if (item == null || !canTransfer)
            return;
        
        if (isContainerSlot)
        {
            if (isLeftButton)
                Transfer(item, 1, containerInventory, PlayerInformation.instance.playerInventory);
            else
                OpenDropAmountUI();
        }
        else
        {
            if (isLeftButton)
                Transfer(item, 1, PlayerInformation.instance.playerInventory, containerInventory);
            else
                OpenDropAmountUI();
        }

        //if (isContainerSlot)
        //{
        //    int amount = !isLeftButton ? containerInventory.GetStock(item.Name) : 1;
        //    Transfer(item, amount, containerInventory, PlayerInformation.instance.playerInventory);
        //}
        //else
        //{
        //    int amount = !isLeftButton ? PlayerInformation.instance.playerInventory.GetStock(item.Name) : 1;
        //    Transfer(item, amount, PlayerInformation.instance.playerInventory, containerInventory);
        //}


    }

    void TransferStack()
    {

        if (item == null || EventSystem.current.currentSelectedGameObject != icon.gameObject)
            return;

        if (isContainerSlot)
            Transfer(item, containerInventory.GetStock(item.Name), containerInventory, PlayerInformation.instance.playerInventory);
        else
            Transfer(item, PlayerInformation.instance.playerInventory.GetStock(item.Name), PlayerInformation.instance.playerInventory, containerInventory);
        
    }

    void OpenDropAmountUI()
    {

        int quantity = isContainerSlot ? containerInventory.GetStock(item.Name) : PlayerInformation.instance.playerInventory.GetStock(item.Name);
        if (quantity > 5)
            dropAmountUI = UIScreenManager.instance.DisplayDropAmountUI(this, quantity, Mouse.current.position.ReadValue());
        else
            TransferItem(true);

    }


    public void SetTransferAmount()
    {
        
        if (isContainerSlot)
            Transfer(item, dropAmountUI.CurrentAmount, containerInventory, PlayerInformation.instance.playerInventory);
        else
            Transfer(item, dropAmountUI.CurrentAmount, PlayerInformation.instance.playerInventory, containerInventory);
        UIScreenManager.instance.CloseDropAmountUI();
    }

    void Transfer(QI_ItemData item, int amount, QI_Inventory fromInventory, QI_Inventory toInventory)
    {
        int space = toInventory.CheckInventoryHasSpace(item, amount);
        int finalAmount = amount < space ? amount : space;
        if(toInventory.AddItem(item, finalAmount, false))
            fromInventory.RemoveItem(item, finalAmount);
        
    }

    public void AddItem(QI_ItemData newItem, int amount)
    {
        ClearSlot();
        item = newItem;
        icon.sprite = item.Icon;
        itemAmount.text = amount.ToString();
        icon.enabled = true;
    }
    

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        itemAmount.text = "";
        icon.enabled = false;
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

}
