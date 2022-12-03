using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContainerInventoryDisplayUI : MonoBehaviour
{

    public static ContainerInventoryDisplayUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }


    public GameObject containerDisplayUI;
    public GameObject containerSlotHolder;
    public GameObject playerSlotHolder;
    public GameObject containerSlot;
    QI_Inventory containerInventory;
    PlayerInformation playerInformation;
    public List<ContainerDisplaySlot> containerSlots = new List<ContainerDisplaySlot>();
    public List<ContainerDisplaySlot> playerSlots = new List<ContainerDisplaySlot>();

    private void Start()
    {
        playerInformation = PlayerInformation.instance;
    }


    public void ShowContainerUI(QI_Inventory container)
    {
        playerInformation.TogglePlayerInput(false);
        containerInventory = container;
        SetContainerUI();
        //containerDisplayUI.SetActive(true);

    }
    public void HideContainerUI()
    {
        ClearSlots();
        containerInventory = null;
        playerInformation.TogglePlayerInput(true);
        //containerDisplayUI.SetActive(false);
    }

    void SetContainerUI()
    {
        ClearSlots();
        for (int i = 0; i < containerInventory.MaxStacks; i++)
        {

            GameObject newSlot = Instantiate(containerSlot, containerSlotHolder.transform);
            containerSlots.Add(newSlot.GetComponent<ContainerDisplaySlot>());
        }
        for (int i = 0; i < playerInformation.playerInventory.MaxStacks; i++)
        {

            GameObject newSlot = Instantiate(containerSlot, playerSlotHolder.transform);
            playerSlots.Add(newSlot.GetComponent<ContainerDisplaySlot>());
            GameEventManager.onInventoryUpdateEvent.AddListener(UpdateContainerInventoryUI);
        }

        UpdateContainerInventoryUI();
    }

    

    public void UpdateContainerInventoryUI()
    {
        if (containerInventory == null)
            return;
        foreach (ContainerDisplaySlot containerSlot in containerSlots)
        {
            containerSlot.ClearSlot();
            containerSlot.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        }

        foreach (ContainerDisplaySlot playerSlot in playerSlots)
        {
            playerSlot.ClearSlot();
            playerSlot.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        }


        for (int i = 0; i < containerInventory.Stacks.Count; i++)
        {

            if (containerInventory.Stacks[i].Item != null)
            {
                containerSlots[i].containerInventory = containerInventory;
                containerSlots[i].AddItem(containerInventory.Stacks[i].Item, containerInventory.Stacks[i].Amount);
                containerSlots[i].GetComponentInChildren<Button>().onClick.AddListener(UpdateContainerInventoryUI);
                containerSlots[i].icon.enabled = true;
                containerSlots[i].isContainerSlot = true;
            }

        }
        
        for (int i = 0; i < playerInformation.playerInventory.Stacks.Count; i++)
        {

            if (playerInformation.playerInventory.Stacks[i].Item != null)
            {
                playerSlots[i].containerInventory = containerInventory;
                playerSlots[i].AddItem(playerInformation.playerInventory.Stacks[i].Item, playerInformation.playerInventory.Stacks[i].Amount);
                playerSlots[i].GetComponentInChildren<Button>().onClick.AddListener(UpdateContainerInventoryUI);
                playerSlots[i].icon.enabled = true;
            }

        }
    }

    public void ClearSlots()
    {
        while (containerSlotHolder.transform.childCount > 0)
        {
            DestroyImmediate(containerSlotHolder.transform.GetChild(0).gameObject);
        }
        while (playerSlotHolder.transform.childCount > 0)
        {
            DestroyImmediate(playerSlotHolder.transform.GetChild(0).gameObject);
        }
        GameEventManager.onInventoryUpdateEvent.RemoveListener(UpdateContainerInventoryUI);
        containerSlots.Clear();
        playerSlots.Clear();
    }

    
}
