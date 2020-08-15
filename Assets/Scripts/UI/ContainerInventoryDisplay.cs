using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContainerInventoryDisplay : MonoBehaviour
{
    public GameObject transferUI;
    public GameObject containerUI;
    public GameObject playerUI;
    public GameObject inventorySlot;
    public QI_Inventory containerInventory;
    QI_Inventory playerInventory;
    public List<ContainerDisplaySlot> containerSlots = new List<ContainerDisplaySlot>();
    public List<ContainerDisplaySlot> playerSlots = new List<ContainerDisplaySlot>();
    public List<QI_ItemData> itemsInContainer = new List<QI_ItemData>();

    private IEnumerator Start()
    {
        Vector3 offset = new Vector3(1000, 0, 0);
        transferUI.transform.position += offset;
        //yield return new WaitForSeconds(2.1f);
        //playerInventory = PlayerInformation.instance.playerInventory;
        

        
        transferUI.SetActive(true);
        for (int i = 0; i < containerInventory.MaxStacks; i++)
        {

            GameObject newSlot = Instantiate(inventorySlot, containerUI.transform);
            containerSlots.Add(newSlot.GetComponent<ContainerDisplaySlot>());
        }
        for (int i = 0; i < PlayerInformation.instance.playerInventory.MaxStacks; i++)
        {

            GameObject newSlot = Instantiate(inventorySlot, playerUI.transform);
            playerSlots.Add(newSlot.GetComponent<ContainerDisplaySlot>());
            PlayerInformation.instance.playerInventory.EventUIUpdateInventory.AddListener(UpdateInventoryUI);
        }
        yield return new WaitForSeconds(2.5f);
        foreach (var item in itemsInContainer)
        {
            containerInventory.AddItem(item, 1);
            
        }
        UpdateInventoryUI();
        transferUI.SetActive(false);
        transferUI.transform.position -= offset;
    }

    public void ShowContainerUI()
    {
        transferUI.SetActive(true);
    }
    public void HideContainerUI()
    {
        transferUI.SetActive(false);
    }

    public void UpdateInventoryUI()
    {
        
        foreach (ContainerDisplaySlot containerSlot in containerSlots)
        {
            containerSlot.ClearSlot();
            containerSlot.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        }
        
        
        for (int i = 0; i < containerInventory.Stacks.Count; i++)
        {
            
            if (containerInventory.Stacks[i].Item != null)
            {
                containerSlots[i].containerInventory = containerInventory;
                containerSlots[i].AddItem(containerInventory.Stacks[i].Item, containerInventory.Stacks[i].Amount);
                containerSlots[i].GetComponentInChildren<Button>().onClick.AddListener(UpdateInventoryUI);
                containerSlots[i].icon.enabled = true;
                containerSlots[i].isContainerSlot = true;
            }

        }
        foreach (ContainerDisplaySlot playerSlot in playerSlots)
        {
            playerSlot.ClearSlot();
            playerSlot.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        }
        for (int i = 0; i < PlayerInformation.instance.playerInventory.Stacks.Count; i++)
        {
            
            if (PlayerInformation.instance.playerInventory.Stacks[i].Item != null)
            {
                playerSlots[i].containerInventory = containerInventory;
                playerSlots[i].AddItem(PlayerInformation.instance.playerInventory.Stacks[i].Item, PlayerInformation.instance.playerInventory.Stacks[i].Amount);
                playerSlots[i].GetComponentInChildren<Button>().onClick.AddListener(UpdateInventoryUI);
                playerSlots[i].icon.enabled = true;
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            transferUI.SetActive(false);
            PlayerInformation.instance.uiScreenVisible = !PlayerInformation.instance.uiScreenVisible;
        }

    }
}
