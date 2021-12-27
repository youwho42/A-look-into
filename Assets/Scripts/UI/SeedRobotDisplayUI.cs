using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SeedRobotDisplayUI : MonoBehaviour
{
    public static SeedRobotDisplayUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    BasicAI robotAI;

    public GameObject seedRobotDisplayUI;
    public GameObject containerSlotHolder;
   
    public GameObject seedRobotSlot;
    QI_Inventory seedRobotInventory;
    PlayerInformation playerInformation;
    public List<ContainerDisplaySlot> seedRobotSlots = new List<ContainerDisplaySlot>();
    
    public QI_ItemDatabase seedDatabase;
    public Image activeImage;
    public TextMeshProUGUI activeText;
    private void Start()
    {
        playerInformation = PlayerInformation.instance;
    }


    public void ShowContainerUI(QI_Inventory container)
    {
        playerInformation.TogglePlayerInput(false);
        seedRobotInventory = container;
        SetContainerUI();
        seedRobotDisplayUI.SetActive(true);
        robotAI = container.GetComponent<BasicAI>();
        activeImage.color = robotAI.isActivated ? Color.green : Color.red;
        activeText.text = robotAI.isActivated ? "Deactivate" : "Activate";
    }
    public void HideContainerUI()
    {
        ClearSlots();
        seedRobotInventory = null;
        playerInformation.TogglePlayerInput(true);
        seedRobotDisplayUI.SetActive(false);
        robotAI = null;
    }

    void SetContainerUI()
    {
        ClearSlots();
        for (int i = 0; i < seedRobotInventory.MaxStacks; i++)
        {

            GameObject newSlot = Instantiate(seedRobotSlot, containerSlotHolder.transform);
            seedRobotSlots.Add(newSlot.GetComponent<ContainerDisplaySlot>());
        }
        
        UpdateContainerInventoryUI();
    }

    public void UpdateContainerInventoryUI()
    {
        if (seedRobotInventory == null)
            return;

        foreach (ContainerDisplaySlot containerSlot in seedRobotSlots)
        {
            containerSlot.ClearSlot();
            containerSlot.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        }

        

        for (int i = 0; i < seedRobotInventory.Stacks.Count; i++)
        {

            if (seedRobotInventory.Stacks[i].Item != null)
            {
                seedRobotSlots[i].containerInventory = seedRobotInventory;
                seedRobotSlots[i].AddItem(seedRobotInventory.Stacks[i].Item, seedRobotInventory.Stacks[i].Amount);
                seedRobotSlots[i].GetComponentInChildren<Button>().onClick.AddListener(UpdateContainerInventoryUI);
                seedRobotSlots[i].icon.enabled = true;
                seedRobotSlots[i].isContainerSlot = true;
            }

        }

        

    }

        public void ClearSlots()
    {
        while (containerSlotHolder.transform.childCount > 0)
        {
            DestroyImmediate(containerSlotHolder.transform.GetChild(0).gameObject);
        }
        
        playerInformation.playerInventory.EventUIUpdateInventory.RemoveListener(UpdateContainerInventoryUI);
        seedRobotSlots.Clear();
        
    }

    public void ActivateRobot()
    {
        robotAI.ActivateRobotToggle();
        activeImage.color = robotAI.isActivated ? Color.green : Color.red;
        activeText.text = robotAI.isActivated ? "Deactivate" : "Activate";
    }
}
