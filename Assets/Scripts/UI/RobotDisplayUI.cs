using Klaxon.GOAD;
using Klaxon.Interactable;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class RobotDisplayUI : MonoBehaviour
{
    public static RobotDisplayUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    UIScreen screen;

    InteractableRobot currentRobot;


    public GameObject containerSlotHolder;
    public GameObject containerSlot;
    public List<ContainerDisplaySlot> containerSlots = new List<ContainerDisplaySlot>();

    [HideInInspector]
    public TutorialUI tutorial;

    [Serializable]
    public struct PriorityButton
    {
        public RobotPriorityTypes priorityType;
        public Button button;
        public Image buttonImage;
    }

    public List<PriorityButton> priorityButtons = new List<PriorityButton>();
    Color faded = new Color(1, 1, 1, 0.4f);
    public Color selectedButtonColor;

    public Slider activeToggle;
    public TextMeshProUGUI activeText;
    private void Start()
    {
        tutorial = GetComponent<TutorialUI>();
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.RobotUI);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GameEventManager.onInventoryUpdateEvent.AddListener(SetContainerUI);
    }
    private void OnDisable()
    {
        GameEventManager.onInventoryUpdateEvent.RemoveListener(SetContainerUI);
    }

    public void ShowUI(InteractableRobot robot)
    {
        currentRobot = robot;
        SetContainerUI();
        SetPriorityColor((RobotPriorityTypes)robot.currentPriority);
        SetActivateButton();
    }

    public void HideUI()
    {
        currentRobot = null;
    }
    void SetActivateButton() 
    {
        activeToggle.value = currentRobot.GetComponent<GOAD_Scheduler_Robot>().GetRobotActive() ? 1 : 0;
        activeText.text = activeToggle.value == 1 ? LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", "Deactivate") : LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", "Activate");
    }
    public void ActivateRobot()
    {
        bool active = activeToggle.value == 1 ? true : false;
        currentRobot.GetComponent<GOAD_Scheduler_Robot>().SetRobotActive(active);
        SetActivateButton();
    }

    public void SetRobotPriority(RobotPriorityTypes type)
    {
        currentRobot.SetCurrentPriority(type);
        SetPriorityColor(type);
        if (!tutorial.hasShownTutorial || tutorial.currentIndex > 0)
            tutorial.SetNextTutorialIndex(0);
    }

    private void SetPriorityColor(RobotPriorityTypes type)
    {
        
        foreach (var button in priorityButtons)
        {
            button.button.image.color = Color.white;
            button.buttonImage.color = faded;
            if (button.priorityType == type)
            {
                button.button.image.color = selectedButtonColor;
                button.buttonImage.color = Color.white;
            }
                
        }
    }

    public void SetRobotPriorityOres() => SetRobotPriority(RobotPriorityTypes.Ores);
    public void SetRobotPriorityWoods() => SetRobotPriority(RobotPriorityTypes.Woods);
    public void SetRobotPrioritySeeds() => SetRobotPriority(RobotPriorityTypes.Seeds);
    public void SetRobotPriorityFoods() => SetRobotPriority(RobotPriorityTypes.Foods);
    public void SetRobotPriorityMisc() => SetRobotPriority(RobotPriorityTypes.Misc);




    void SetContainerUI()
    {
        if (currentRobot.selfInventory == null)
            return;
        ClearSlots();
        for (int i = 0; i < currentRobot.selfInventory.MaxStacks; i++)
        {

            GameObject newSlot = Instantiate(containerSlot, containerSlotHolder.transform);
            var s = newSlot.GetComponent<ContainerDisplaySlot>();
            s.isContainerSlot = true;
            s.canTransfer = true;
            containerSlots.Add(s);

        }
        UpdateContainerInventoryUI();
        if (currentRobot.TryGetComponent(out GOAD_Scheduler_Robot robot))
            robot.robotLights.SetInventoryLights();
    }

    public void ClearSlots()
    {
        foreach (Transform child in containerSlotHolder.transform)
        {
            Destroy(child.gameObject);
        }


        containerSlots.Clear();
    }


    public void UpdateContainerInventoryUI()
    {
        if (currentRobot.selfInventory == null)
            return;
        foreach (ContainerDisplaySlot containerSlot in containerSlots)
        {
            containerSlot.ClearSlot();
            containerSlot.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        }

        for (int i = 0; i < currentRobot.selfInventory.Stacks.Count; i++)
        {
            var butt = containerSlots[i].GetComponentInChildren<Button>();

            if (currentRobot.selfInventory.Stacks[i].Item != null)
            {
                containerSlots[i].containerInventory = currentRobot.selfInventory;
                containerSlots[i].AddItem(currentRobot.selfInventory.Stacks[i].Item, currentRobot.selfInventory.Stacks[i].Amount);
                containerSlots[i].icon.enabled = true;
                containerSlots[i].isContainerSlot = true;
                butt.interactable = true;
            }
            else
            {
                butt.interactable = false;
            }
        }
        
    }
}
