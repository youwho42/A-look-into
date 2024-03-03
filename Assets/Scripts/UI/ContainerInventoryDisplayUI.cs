using Klaxon.StatSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
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
    string tipText;

    private void Start()
    {
        gameObject.SetActive(false);
    }
    void ChangeControlText(string text)
    {
        
        string t1 = "";
        string t2 = "";
        string t3 = "";
        if (text == "Gamepad")
        {
            t1 = "A";
            t2 = "R2 + RS";
            t3 = "X";
        }
        else
        {
            t1 = "LMB";
            t2 = "CTRL+LMB";
            t3 = "E";
        }

        tipText = $"{t1} > transfer item - {t2} > transfer stack / {t3} > close";
        UIScreenManager.instance.SetTipPanel(tipText);

    }


    public void ShowContainerUI(QI_Inventory container)
    {
        ChangeControlText(PlayerInformation.instance.playerInput.currentControlScheme);
        
        
        containerInventory = container;
        containerName.text = LocalizationSettings.StringDatabase.GetLocalizedString("Items-Utility", container.Name);
        //playerName.text = $"{PlayerInformation.instance.playerName}'s inventory";
        SetContainerUI();
    }
    public void HideContainerUI()
    {
        
        ClearSlots();
        containerInventory = null;
    }

    void SetContainerUI()
    {
        ClearSlots();
        for (int i = 0; i < containerInventory.MaxStacks; i++)
        {

            GameObject newSlot = Instantiate(containerSlot, containerSlotHolder.transform);
            var s = newSlot.GetComponent<ContainerDisplaySlot>();
            s.isContainerSlot = true;
            s.canTransfer = true;
            containerSlots.Add(s);

        }
        for (int i = 0; i < PlayerInformation.instance.playerInventory.MaxStacks; i++)
        {

            GameObject newSlot = Instantiate(containerSlot, playerSlotHolder.transform);
            var s = newSlot.GetComponent<ContainerDisplaySlot>();
            s.isContainerSlot = false;
            s.canTransfer = containerInventory.PlayerCanAddToInventory;
            playerSlots.Add(s);
            
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
        
        EventSystem.current.SetSelectedGameObject(null);
        if (containerSlots.Count > 0)
            EventSystem.current.SetSelectedGameObject(containerSlots[0].GetComponentInChildren<Button>().gameObject);
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

    public void SortInventory()
    {
        containerInventory.SortInventory();
        UpdateContainerInventoryUI();
    }

    public void PlaceAllSimilarItems()
    {
        if (!containerInventory.PlayerCanAddToInventory)
            return;
        List<QI_ItemData> items = new List<QI_ItemData>();
        List<int> amounts = new List<int>();
        foreach (var stack in containerInventory.Stacks)
        {
            int amount = PlayerInformation.instance.playerInventory.GetStock(stack.Item.Name);
            if (amount > 0)
            {
                items.Add(stack.Item);
                amounts.Add(amount);
                
            }

        }
        for (int i = 0; i < items.Count; i++)
        {
            int space = containerInventory.CheckInventoryHasSpace(items[i], amounts[i]);
            int finalAmount = amounts[i] < space ? amounts[i] : space;
            containerInventory.AddItem(items[i], finalAmount, false);
            PlayerInformation.instance.playerInventory.RemoveItem(items[i], finalAmount);
        }
    }
}
