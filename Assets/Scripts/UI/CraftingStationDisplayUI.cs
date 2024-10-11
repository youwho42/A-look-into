using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class CraftingStationDisplayUI : MonoBehaviour
{
    public static CraftingStationDisplayUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }


    //PlayerInformation playerInformation;
    QI_CraftingRecipeDatabase recipeDatabase;
    public GameObject craftingStationUI;
    


    //QI_Inventory playerInventory;
    QI_CraftingHandler craftingHandler;
    QI_Inventory finalInventory;
    public QI_ItemDatabase itemDatabase;
    


    public List<CraftingSlot> ingredientSlots = new List<CraftingSlot>();
    List<CraftingRecipeButton> recipeButtons = new List<CraftingRecipeButton>();


    public CraftingSlot recipeResultSlot;
    public TextMeshProUGUI recipeTicksToCraftText;

    public GameObject recipeButtonHolder;
    public GameObject recipeButton;

    QI_CraftingRecipe craftableItem;

    public Slider quantityToCraftSlider;
    UISelectHandler sliderSelectHandler;
    public TextMeshProUGUI quantitySelectedText;

    public Button craftButton;

    [Space]
    [Header("Output Container")]
    public GameObject finalContainer;
    public GameObject containerSlotHolder;
    public GameObject containerSlot;
    public List<ContainerDisplaySlot> containerSlots = new List<ContainerDisplaySlot>();

    [Space]
    [Header("Crafting Queue")]
    public TextMeshProUGUI craftingQueue;
    public GameObject craftingQueueObject;
    UIScreen screen;
    [HideInInspector]
    public TutorialUI tutorial;
    public TextMeshProUGUI queueTotalTime;
    [Space]
    [Header("Energy")]
    public GameObject energyContainer;
    public GameObject inventoryEnergy;
    public Image energyImage;
    public TextMeshProUGUI energyAmountText;
    public Image energyUseImage;
    public Image energyBGImage;
    public CraftingStationFuelInventorySlot craftingStationFuelInventorySlot;
    public Sprite emptySprite;
    bool pulsingEnergy;
    public Color energyFullColor;
    public Color energyEmptyColor;
    public TextMeshProUGUI energyTotalTime;
    private void Start()
    {
        tutorial = GetComponent<TutorialUI>();
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.CraftingStationUI);
        sliderSelectHandler = quantityToCraftSlider.GetComponent<UISelectHandler>();
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        GameEventManager.onInventoryUpdateEvent.AddListener(SetContainerUI);
        GameEventManager.onTimeTickEvent.AddListener(SetEnergyPercent);
    }
    private void OnDisable()
    {
        GameEventManager.onInventoryUpdateEvent.RemoveListener(SetContainerUI);
        GameEventManager.onTimeTickEvent.RemoveListener(SetEnergyPercent);
    }
    private void OnDestroy()
    {
        StopCoroutine(PulseEmptyEnergy());
    }


    public void ShowUI(QI_CraftingHandler handler, QI_CraftingRecipeDatabase database, QI_Inventory inventory)
    {
        craftingHandler = handler;
        recipeDatabase = database;
        finalInventory = inventory;
        SetAvailableRecipes();
        quantityToCraftSlider.maxValue = 0;
        quantityToCraftSlider.value = 0;
        SetQuantityText();
        inventoryEnergy.SetActive(false);
        SetEnergyItems();
        
        energyContainer.SetActive(craftingHandler.craftingFuels.Count > 0);

        finalContainer.SetActive(false);
        craftingQueueObject.SetActive(false);
        if(finalInventory.Name != "MainPlayerInventory")
        {
            finalContainer.SetActive(true);
            craftingQueueObject.SetActive(true);
            SetContainerUI();
        }
        
        EventSystem.current.SetSelectedGameObject(null);
        if (recipeButtons.Count > 0)
            EventSystem.current.SetSelectedGameObject(recipeButtons[0].GetComponentInChildren<Button>().gameObject);
        
    }

    private void SetEnergyItems()
    {
        
        energyImage.sprite = emptySprite;
        energyAmountText.text = "0";
        energyUseImage.fillAmount = 0;
        if (craftingHandler.currentFuel != null)
        {
            energyImage.sprite = craftingHandler.currentFuel.Icon;
            energyAmountText.text = craftingHandler.currentFuelAmount.ToString();
        }
        
        SetEnergyPercent(0);
    }
    public void SetEnergyPercent(int tick)
    {
        if (craftingHandler.currentFuelAmount > 0)
        {
            energyUseImage.fillAmount = craftingHandler.GetCurrentFuelPercent();
            if (pulsingEnergy)
            {
                pulsingEnergy = false;
                StopCoroutine(PulseEmptyEnergy());
                ResetEnergyPulse();
            }
            int amount = craftingHandler.currentFuelTick;
            amount += (craftingHandler.currentFuelAmount - 1) * craftingHandler.currentFuel.FuelTicks;
            energyTotalTime.text = NumberFunctions.GetTimeAsString(amount);    
        }
        else
        {
            energyTotalTime.text = "0:00";
            if (!pulsingEnergy)
            {
                pulsingEnergy = true;
                StartCoroutine(PulseEmptyEnergy());
            }
        }

        int totalTime = 0;
        foreach (var item in craftingHandler.Queues)
        {
            totalTime += (int)item.Timer;
        }
        queueTotalTime.text = NumberFunctions.GetTimeAsString(totalTime);

    }

    void ResetEnergyPulse()
    {
        energyBGImage.color = energyFullColor;
    }
    IEnumerator PulseEmptyEnergy()
    {
        bool pulseIn = true;
        float waitTime = 1.0f;
        float timer = 0;
        Color c = energyFullColor;
        while (pulsingEnergy)
        {
            
            timer += Time.deltaTime;
            
            if (pulseIn)
                c = Color.Lerp(energyFullColor, energyEmptyColor, timer / waitTime);
            else
                c = Color.Lerp(energyEmptyColor, energyFullColor, timer / waitTime);
            energyBGImage.color = c;
            if (timer >= waitTime)
            {
                pulseIn = !pulseIn;
                timer = 0;
            }
            yield return null;
        }

        
    }

    public void HideUI()
    {
        craftingHandler = null;
    }

    public void ToggleInventoryEnergy()
    {
        if (inventoryEnergy.activeInHierarchy)
        {
            inventoryEnergy.SetActive(false);
            return;
        }
        
        tutorial.SetNextTutorialIndex(0);
        SetFuelSlots();
        inventoryEnergy.SetActive(true);
    }

    private void SetFuelSlots()
    {
        ClearFuelSlots();
        foreach (var fuelType in craftingHandler.craftingFuels)
        {
            var slot = Instantiate(craftingStationFuelInventorySlot, inventoryEnergy.gameObject.transform);
            slot.SetFuelInventorySlot(fuelType.item, PlayerInformation.instance.playerInventory.GetStock(fuelType.item.Name), craftingHandler);

        }
    }

    public void ClearFuelSlots()
    {
        foreach (Transform child in inventoryEnergy.transform)
        {
            Destroy(child.gameObject);
        }

    }

    void SetContainerUI()
    {
        if (finalInventory == null)
            return;
        ClearSlots();
        for (int i = 0; i < finalInventory.MaxStacks; i++)
        {

            GameObject newSlot = Instantiate(containerSlot, containerSlotHolder.transform);
            var s = newSlot.GetComponent<ContainerDisplaySlot>();
            s.isContainerSlot = true;
            s.canTransfer = true;
            containerSlots.Add(s);

        }
        UpdateContainerInventoryUI();
        SetEnergyItems();
        SetFuelSlots();
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
        if (finalInventory == null)
            return;
        foreach (ContainerDisplaySlot containerSlot in containerSlots)
        {
            containerSlot.ClearSlot();
            containerSlot.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        }

        for (int i = 0; i < finalInventory.Stacks.Count; i++)
        {
            var butt = containerSlots[i].GetComponentInChildren<Button>();

            if (finalInventory.Stacks[i].Item != null)
            {
                containerSlots[i].containerInventory = finalInventory;
                containerSlots[i].AddItem(finalInventory.Stacks[i].Item, finalInventory.Stacks[i].Amount);
                containerSlots[i].icon.enabled = true;
                containerSlots[i].isContainerSlot = true;
                butt.interactable = true;
            }
            else
            {
                butt.interactable = false;
            }
        }
        GetCraftingQueueItems();
    }

    public void SetAvailableRecipes()
    {
        ClearRecipeSlots();
        QI_CraftingRecipeDatabase playerRecipes = PlayerInformation.instance.playerRecipeDatabase;
        List<QI_CraftingRecipe> recipes = new List<QI_CraftingRecipe>();
        for (int i = 0; i < recipeDatabase.CraftingRecipes.Count; i++)
        {
            if (playerRecipes.CraftingRecipes.Contains(recipeDatabase.CraftingRecipes[i]))
            {
                GameObject newRecipe = Instantiate(recipeButton, recipeButtonHolder.transform);
                recipeButtons.Add(newRecipe.GetComponent<CraftingRecipeButton>());
                recipes.Add(recipeDatabase.CraftingRecipes[i]);
            }
            
        }
        UpdateCraftingUI(recipes);
        ClearCurrentRecipe();
    }

    public void UpdateCraftingUI(List<QI_CraftingRecipe> recipes)
    {
        for (int i = 0; i < recipeButtons.Count; i++)
        {
            recipeButtons[i].AddItem(recipes[i]);
        }

    }

    public void ClearCurrentRecipe()
    {
        craftButton.interactable = false;
        craftableItem = null;
        recipeResultSlot.ClearSlot();
        recipeTicksToCraftText.text = "";
        foreach (CraftingSlot slot in ingredientSlots)
        {
            slot.ClearSlot();
        }
    }

    public void ClearRecipeSlots()
    {
        foreach (Transform child in recipeButtonHolder.transform)
        {
            Destroy(child.gameObject);
        }
        
        recipeButtons.Clear();
    }

    public void SetCurrentRecipe(QI_CraftingRecipe itemToCraft)
    {
        ClearCurrentRecipe();
        craftableItem = itemToCraft;
        recipeResultSlot.AddItem(craftableItem.Product.Item, craftableItem.Product.Amount);
        recipeTicksToCraftText.text = NumberFunctions.GetTimeAsString((int)craftableItem.CraftingTime);

        SetQuantitySlider();
       
            
        for (int i = 0; i < craftableItem.Ingredients.Count; i++)
        {
            ingredientSlots[i].AddItem(craftableItem.Ingredients[i].Item, craftableItem.Ingredients[i].Amount, PlayerInformation.instance.playerInventory.GetStock(craftableItem.Ingredients[i].Item.Name));
        }
        
    }
    void SetQuantitySlider()
    {
        
        int max = 1000;
        for (int i = 0; i < craftableItem.Ingredients.Count; i++)
        {
            int amount =  PlayerInformation.instance.playerInventory.GetStock(craftableItem.Ingredients[i].Item.Name) / craftableItem.Ingredients[i].Amount;
            if(amount < max)
                max = amount;
        }
        quantityToCraftSlider.maxValue = max;
        quantityToCraftSlider.value = max == 0 ? 0 : 1;
        SetQuantityText();
        
    }

    public void SetQuantityText()
    {
        if(sliderSelectHandler.IsSelected)
            tutorial.SetNextTutorialIndex(2);
        quantitySelectedText.text = $"{quantityToCraftSlider.value}/{quantityToCraftSlider.maxValue}";
        craftButton.interactable = quantityToCraftSlider.value > 0;
        quantityToCraftSlider.interactable = quantityToCraftSlider.maxValue > 0;
    }

    public void CraftItem()
    {
        if (craftableItem != null)
        {
            
            //if (InteractCostReward())
                craftingHandler.Craft(craftableItem, (int)quantityToCraftSlider.value, finalInventory);
        }
        SetCurrentRecipe(craftableItem);
        GetCraftingQueueItems();
    }

    //bool InteractCostReward()
    //{
    //    float agency = PlayerInformation.instance.statHandler.GetStatMaxModifiedValue("Agency");
    //    if (agency >= craftableItem.AgencyCost)
    //        return true;


    //    Notifications.instance.SetNewNotification($"{craftableItem.AgencyCost - agency} <sprite name=\"Agency\">", null, 0, NotificationsType.Warning);
    //    return false;
    //}

    void GetCraftingQueueItems()
    {
        
        

        QI_ItemData lastItem = null;
        

        string craftingText = "";

       
        int finalAmount = 0;
        List<string> lines = new List<string>();
        int lineIndex = 0;
        for (int i = 0; i < craftingHandler.Queues.Count; i++)
        {
            if (lastItem == null)
            {
                lastItem = craftingHandler.Queues[i].Item;
                lines.Add("");
            }
              
            
            if (craftingHandler.Queues[i].Item == lastItem)
            {
                
                finalAmount += craftingHandler.Queues[i].Amount;
                lines[lineIndex] = $"{lastItem.localizedName.GetLocalizedString()} x {finalAmount}\n";
            }
            else
            {
                lastItem = craftingHandler.Queues[i].Item;
               
                finalAmount = craftingHandler.Queues[i].Amount;
                lines.Add($"{lastItem.localizedName.GetLocalizedString()} x {finalAmount}\n");
                lineIndex++;
                
            }
        }
        foreach (var line in lines)
        {
            craftingText += line;
        }
        craftingQueue.text = craftingText;
        
        //string craftingText = "";
        //foreach (var item in craftingHandler.craftingQueue)
        //{
        //    craftingText += $"{item.Key.localizedName.GetLocalizedString()} x {item.Value} \n";
        //}
        //craftingQueue.text = craftingText;
    }
}
