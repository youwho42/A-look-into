using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingStationFuelInventorySlot : MonoBehaviour
{
    QI_ItemData currentItem;
    public Button iconImage;
    public TextMeshProUGUI amountText;
    QI_CraftingHandler craftingHandler;

    public void SetFuelInventorySlot(QI_ItemData itemData, int amount, QI_CraftingHandler handler)
    {
        currentItem = itemData;
        craftingHandler = handler;
        iconImage.image.sprite = itemData.Icon;
        amountText.text = amount.ToString();
        iconImage.interactable = amount > 0;
    }

    public void AddFuelToHandler()
    {
        if(craftingHandler.AddFuel(currentItem, 1))
        {
            PlayerInformation.instance.playerInventory.RemoveItem(currentItem, 1);
            GameEventManager.onInventoryUpdateEvent.Invoke();
        }
    }
}
