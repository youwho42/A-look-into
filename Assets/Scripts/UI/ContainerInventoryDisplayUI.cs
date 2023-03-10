using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    public TextMeshProUGUI containerName;
    public TextMeshProUGUI playerName;
    QI_Inventory containerInventory;
    //PlayerInformation playerInformation;
    public List<ContainerDisplaySlot> containerSlots = new List<ContainerDisplaySlot>();
    public List<ContainerDisplaySlot> playerSlots = new List<ContainerDisplaySlot>();

    

    public void ShowContainerUI(QI_Inventory container)
    {
        PlayerInformation.instance.uiScreenVisible = true;
        PlayerInformation.instance.TogglePlayerInput(false);
        containerInventory = container;
        containerName.text = container.Name;
        playerName.text = $"{PlayerInformation.instance.playerName}'s inventory";
        SetContainerUI();
    }
    public void HideContainerUI()
    {
        PlayerInformation.instance.uiScreenVisible = false;
        ClearSlots();
        containerInventory = null;
        PlayerInformation.instance.TogglePlayerInput(true);
    }

    void SetContainerUI()
    {
        ClearSlots();
        for (int i = 0; i < containerInventory.MaxStacks; i++)
        {

            GameObject newSlot = Instantiate(containerSlot, containerSlotHolder.transform);
            containerSlots.Add(newSlot.GetComponent<ContainerDisplaySlot>());
        }
        for (int i = 0; i < PlayerInformation.instance.playerInventory.MaxStacks; i++)
        {

            GameObject newSlot = Instantiate(containerSlot, playerSlotHolder.transform);
            playerSlots.Add(newSlot.GetComponent<ContainerDisplaySlot>());
            GameEventManager.onInventoryUpdateEvent.AddListener(UpdateContainerInventoryUI);
        }
        EventSystem.current.SetSelectedGameObject(null);
        if (containerSlots.Count > 0)
            EventSystem.current.SetSelectedGameObject(containerSlots[0].GetComponentInChildren<Button>().gameObject);
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
            var butt = containerSlots[i].GetComponentInChildren<Button>();
            
            if (containerInventory.Stacks[i].Item != null)
            {
                containerSlots[i].containerInventory = containerInventory;
                containerSlots[i].AddItem(containerInventory.Stacks[i].Item, containerInventory.Stacks[i].Amount);
                containerSlots[i].icon.enabled = true;
                containerSlots[i].isContainerSlot = true;
                butt.interactable = true;
            }
            else
            {
                butt.interactable = false;
            }
        }
        
        for (int i = 0; i < PlayerInformation.instance.playerInventory.Stacks.Count; i++)
        {
            var butt = playerSlots[i].GetComponentInChildren<Button>();
            
            if (PlayerInformation.instance.playerInventory.Stacks[i].Item != null)
            {
                playerSlots[i].containerInventory = containerInventory;
                playerSlots[i].AddItem(PlayerInformation.instance.playerInventory.Stacks[i].Item, PlayerInformation.instance.playerInventory.Stacks[i].Amount);
                playerSlots[i].icon.enabled = true;
                butt.interactable = true;
            }
            else
            {
                butt.interactable = false;
            }
        }
        
    }

    public void ClearSlots()
    {
        foreach (Transform child in containerSlotHolder.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in playerSlotHolder.transform)
        {
            Destroy(child.gameObject);
        }
        
        GameEventManager.onInventoryUpdateEvent.RemoveListener(UpdateContainerInventoryUI);
        containerSlots.Clear();
        playerSlots.Clear();
    }

    
}
