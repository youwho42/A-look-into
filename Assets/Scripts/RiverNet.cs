using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumInventory;

public class RiverNet : MonoBehaviour
{

    QI_Inventory inventory;
    public List<SpriteRenderer> items = new List<SpriteRenderer>();

    private void Start()
    {
        inventory = GetComponent<QI_Inventory>();

    }

    public void UpdateInventoryVisuals()
    {
        for (int i = 0; i < inventory.Stacks.Count; i++)
        {
            if (inventory.Stacks[i].Item != null)
            {
                items[i].sprite = inventory.Stacks[i].Item.Icon;
            }
            else
            {
                items[i].sprite = null;
            }

        }
        if(inventory.Stacks.Count == 0) 
        {
            foreach (var item in items)
            {
                item.sprite = null;
            }
        }
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("RiverDebris"))
        {
            if (collision.TryGetComponent(out QI_Item item))
            {
                if (inventory.AddItem(item.Data.pickUpItem, 1, false))
                {
                    Destroy(collision.gameObject);
                }
            }
        }
    }
}
