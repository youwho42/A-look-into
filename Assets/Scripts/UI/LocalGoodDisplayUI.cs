using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LocalGoodDisplayUI : MonoBehaviour
{
    public static LocalGoodDisplayUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


    public GameObject localGoodsSlotHolder;
    public GameObject playerSlotHolder;
    public GameObject containerSlot;

    public TextMeshProUGUI sparksDisplay;
    public TextMeshProUGUI playerName;
    QI_Inventory containerInventory;


    public List<ContainerDisplaySlot> containerSlots = new List<ContainerDisplaySlot>();
    public List<ContainerDisplaySlot> playerSlots = new List<ContainerDisplaySlot>();

    int totalSparks = 0;

    public void ShowGoodsUI(QI_Inventory container)
    {
        GameEventManager.onInventoryUpdateEvent.AddListener(UpdateGoodsUI);

        PlayerInformation.instance.uiScreenVisible = true;
        PlayerInformation.instance.TogglePlayerInput(false);
        containerInventory = container;
        playerName.text = $"{PlayerInformation.instance.playerName}'s inventory";
        UpdateGoodsUI();
    }
    public void HideGoodsUI()
    {
        GameEventManager.onInventoryUpdateEvent.RemoveListener(UpdateGoodsUI);

        PlayerInformation.instance.uiScreenVisible = false;
        PlayerInformation.instance.TogglePlayerInput(true);
        ClearSlots();
        

    }

    


    public void UpdateGoodsUI()
    {
        ClearSlots();

        for (int i = 0; i < PlayerInformation.instance.playerInventory.MaxStacks; i++)
        {

            GameObject newSlot = Instantiate(containerSlot, playerSlotHolder.transform);
            playerSlots.Add(newSlot.GetComponent<ContainerDisplaySlot>());
        }
        for (int i = 0; i < containerInventory.Stacks.Count; i++)
        {

            GameObject newSlot = Instantiate(containerSlot, localGoodsSlotHolder.transform);
            containerSlots.Add(newSlot.GetComponent<ContainerDisplaySlot>());
        }

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

        totalSparks = 0;

        for (int i = 0; i < containerInventory.Stacks.Count; i++)
        {
            var butt = containerSlots[i].GetComponentInChildren<Button>();

            if (containerInventory.Stacks[i].Item != null)
            {
                totalSparks += containerInventory.Stacks[i].Amount * containerInventory.Stacks[i].Item.Price;
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

        sparksDisplay.text = $"<sprite anim=\"3,5,12\"> {totalSparks}";
        EventSystem.current.SetSelectedGameObject(null);
        if (containerSlots.Count > 0)
            EventSystem.current.SetSelectedGameObject(containerSlots[0].GetComponentInChildren<Button>().gameObject);
    }

    public void TradeGoods()
    {
        PlayerInformation.instance.purse.AddToPurse(totalSparks);
        containerInventory.RemoveAllItems();
        UpdateGoodsUI();
    }

    public void ClearSlots()
    {
        foreach (Transform child in localGoodsSlotHolder.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in playerSlotHolder.transform)
        {
            Destroy(child.gameObject);
        }
        containerSlots.Clear();
        playerSlots.Clear();
    }
}