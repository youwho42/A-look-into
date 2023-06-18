using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableMerchantTable : Interactable
{
    public SpriteRenderer itemIcon;
    QI_ItemData item;
    int amount;

    public override void Start()
    {
        base.Start();
        canInteract = false;
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        Debug.Log("should see the item in ui screen to be able to buy");
    }

    public void SetUpTable(QI_ItemData itemData, int _amount)
    {
        item = itemData;
        amount = _amount;
        itemIcon.sprite = item.Icon;
    }

    public void ClearTable()
    {
        item = null;
        itemIcon.sprite = null;
        amount = 0;
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
