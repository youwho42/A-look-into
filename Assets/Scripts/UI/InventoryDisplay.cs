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
    public GameObject mainUI;
    public GameObject inventoryUI;
    public GameObject equipmentUI;
    public GameObject inventorySlot;
    public GameObject equipmentSlot;
    public List<InventoryDisplaySlot> inventorySlots = new List<InventoryDisplaySlot>();
    public List<ISlot> equipmentSlots = new List<ISlot>();



    private IEnumerator Start()
    {
        

        Vector3 offset = new Vector3(3000, 0, 0);

        mainUI.transform.position += offset;
        mainUI.SetActive(true);
        for (int i = 0; i < inventory.MaxStacks; i++)
        {
            
            GameObject newSlot = Instantiate(inventorySlot, inventoryUI.transform);
            inventorySlots.Add(newSlot.GetComponent<InventoryDisplaySlot>());
        }
        for (int i = 0; i < System.Enum.GetNames(typeof(EquipmentSlot)).Length; i++)
        {

            GameObject newSlot = Instantiate(equipmentSlot, equipmentUI.transform);
            equipmentSlots.Add(newSlot.GetComponent<ISlot>());
            for (int x = 0; x < equipmentSlots.Count; x++)
            {
                equipmentSlots[x].SetIndex(x);
            }
        }
        yield return new WaitForSeconds(2f);
        UpdateInventoryUI();
        mainUI.SetActive(false);
        mainUI.transform.position -= offset;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            mainUI.SetActive(!mainUI.activeSelf);
            PlayerInformation.instance.uiScreenVisible = !PlayerInformation.instance.uiScreenVisible;
        }
    }
    public void UpdateInventoryUI()
    {
        foreach (InventoryDisplaySlot slot in inventorySlots)
        {
            slot.ClearSlot();
        }
        for (int i = 0; i < inventory.Stacks.Count; i++)
        {
            if(inventory.Stacks[i].Item != null)
            {
                inventorySlots[i].inventory = inventory;
                inventorySlots[i].equipmentManager = equipmentManager;
                inventorySlots[i].AddItem(inventory.Stacks[i].Item, inventory.Stacks[i].Amount);
                inventorySlots[i].icon.enabled = true;
            }
            
        }

        foreach (ISlot slot in equipmentSlots)
        {
            slot.ClearSlot();
        }
        for (int i = 0; i < System.Enum.GetNames(typeof(EquipmentSlot)).Length; i++)
        {
            if (EquipmentManager.instance.currentEquipment[i] != null)
            {
                
                equipmentSlots[i].AddItem(EquipmentManager.instance.currentEquipment[i], 1);
                
                
            }

        }
    }


}
