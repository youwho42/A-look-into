using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class InventoryDisplay : MonoBehaviour
{
    public QI_Inventory inventory;
    public EquipmentManager equipmentManager;
    public GameObject inventoryUI;
    public GameObject inventorySlot;
    public List<InventoryDisplaySlot> slots = new List<InventoryDisplaySlot>();
    

    private IEnumerator Start()
    {
        Vector3 offset = new Vector3(1000, 0, 0);

        inventoryUI.transform.position += offset;
        inventoryUI.SetActive(true);
        for (int i = 0; i < inventory.MaxStacks; i++)
        {
            
            GameObject newSlot = Instantiate(inventorySlot, inventoryUI.transform);
            slots.Add(newSlot.GetComponent<InventoryDisplaySlot>());
        }
        yield return new WaitForSeconds(2f);
        UpdateInventoryUI();
        inventoryUI.SetActive(false);
        inventoryUI.transform.position -= offset;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
    }
    public void UpdateInventoryUI()
    {
        foreach (InventoryDisplaySlot slot in slots)
        {
            slot.ClearSlot();
        }
        for (int i = 0; i < inventory.Stacks.Count; i++)
        {
            if(inventory.Stacks[i].Item != null)
            {
                slots[i].inventory = inventory;
                slots[i].equipmentManager = equipmentManager;
                slots[i].AddItem(inventory.Stacks[i].Item, inventory.Stacks[i].Amount);
                slots[i].icon.enabled = true;
            }
            
        }
    }


}
