using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MerchantTableUI : MonoBehaviour
{
    public static MerchantTableUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    QI_ItemData item;
    int itemQuantity;
    public Image icon;

    public TextMeshProUGUI sparks;
    public TextMeshProUGUI stock;
    public TextMeshProUGUI purchaseQuantity;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;

    public Slider purchaseSlider;
    InteractableMerchantTable merchantTable;

    public void Purchase()
    {
        PlayerInformation.instance.playerInventory.AddItem(item, (int)purchaseSlider.value, false);
        PlayerInformation.instance.purse.RemoveFromPurse((int)purchaseSlider.value * item.Price);
        merchantTable.RemoveItems((int)purchaseSlider.value);
        SetQuantityText();
        HideUI();
    }


    public void SetMerchantUI(QI_ItemData item, int amount, InteractableMerchantTable table)
    {
        this.item = item;
        itemQuantity = amount;
        icon.sprite = item.Icon;
        sparks.text = $"Sparks<sprite anim=\"3,5,12\"> {item.Price}";
        stock.text = $"Stock {amount}";
        itemName.text = item.Name;
        itemDescription.text = item.Description;
        merchantTable = table;
        SetQuantitySlider();
    }

    void SetQuantitySlider()
    {

        int amount = PlayerInformation.instance.purse.GetPurseAmount() / item.Price;
        amount = Mathf.Clamp(amount, 0, itemQuantity);
        purchaseSlider.maxValue = amount;
        purchaseSlider.value = amount == 0 ? 0 : 1;
        SetQuantityText();

    }

    public void SetQuantityText()
    {
        purchaseQuantity.text = $"Quantity {purchaseSlider.value} / {purchaseSlider.maxValue}";
    }

    public void HideUI()
    {
        UIScreenManager.instance.HideAllScreens();
        merchantTable.isOpen = false;
        //UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
    }
}
