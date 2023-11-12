using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class InventoryDisplayUI : MonoBehaviour
{
    
    public GameObject mainUI;
    public GameObject inventoryUI;
    public GameObject equipmentUI;
    public GameObject inventorySlot;
    public GameObject equipmentSlot;
    public TextMeshProUGUI playerName;

    public List<InventoryDisplaySlot> inventorySlots = new List<InventoryDisplaySlot>();
    public List<ISlot> equipmentSlots = new List<ISlot>();

    
    private void Start()
    {
        GameEventManager.onEquipmentUpdateEvent.AddListener(UpdateInventoryUI);
        GameEventManager.onInventoryUpdateEvent.AddListener(UpdateInventoryUI);

        SetInventoryUI();
        UpdateInventoryUI();

    }
    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        if (inventorySlots.Count > 0)
            EventSystem.current.SetSelectedGameObject(inventorySlots[0].GetComponentInChildren<Button>().gameObject);

        
    }
    private void OnDisable()
    {
        
        GameEventManager.onEquipmentUpdateEvent.RemoveListener(UpdateInventoryUI);
        GameEventManager.onInventoryUpdateEvent.RemoveListener(UpdateInventoryUI);
    }
    void SetInventoryUI()
    {
        for (int i = 0; i < PlayerInformation.instance.playerInventory.MaxStacks; i++)
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
    }

    public void UpdateInventoryUI()
    {
        //playerName.text = $"{PlayerInformation.instance.playerName}'s inventory";
        foreach (InventoryDisplaySlot slot in inventorySlots)
        {
            slot.ClearSlot();
        }
        for (int i = 0; i < PlayerInformation.instance.playerInventory.Stacks.Count; i++)
        {
            if(PlayerInformation.instance.playerInventory.Stacks[i].Item != null)
            {
                inventorySlots[i].inventory = PlayerInformation.instance.playerInventory;
                inventorySlots[i].equipmentManager = PlayerInformation.instance.equipmentManager;
                inventorySlots[i].AddItem(PlayerInformation.instance.playerInventory.Stacks[i].Item, PlayerInformation.instance.playerInventory.Stacks[i].Amount);
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
    
    public void SortInventory()
    {
        PlayerInformation.instance.playerInventory.SortInventory();
        UpdateInventoryUI();
    }
}
