using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingStationFuelInventorySlot : MonoBehaviour
{
    [HideInInspector]
    public QI_ItemData currentItem;
    public Button iconImage;
    public TextMeshProUGUI amountText;
    [HideInInspector]
    public QI_CraftingHandler craftingHandler;

    public void ShowInformation()
    {
        if (currentItem == null)
            return;
        ItemInformationDisplayUI.instance.ShowItemName(currentItem, this.GetComponent<RectTransform>());
    }
    public void HideInformation()
    {
        ItemInformationDisplayUI.instance.HideItemName();
    }


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
    public void TransferStack()
    {
        if (currentItem == null || EventSystem.current.currentSelectedGameObject != iconImage.gameObject)
            return;
        int amount = PlayerInformation.instance.playerInventory.GetStock(currentItem.Name);
        if (craftingHandler.AddFuel(currentItem, amount))
        {
            PlayerInformation.instance.playerInventory.RemoveItem(currentItem, amount);
            GameEventManager.onInventoryUpdateEvent.Invoke();
        }
    }
}
