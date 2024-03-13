using QuantumTek.QuantumInventory;
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
    public TextMeshProUGUI recipeAgencyText;

    public GameObject recipeButtonHolder;
    public GameObject recipeButton;

    QI_CraftingRecipe craftableItem;

    public Slider quantityToCraftSlider;
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

    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.CraftingStationUI);
    
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


    public void ShowUI(QI_CraftingHandler handler, QI_CraftingRecipeDatabase database, QI_Inventory inventory)
    {
        craftingHandler = handler;
        recipeDatabase = database;
        finalInventory = inventory;
        SetAvailableRecipes();
        quantityToCraftSlider.maxValue = 0;
        quantityToCraftSlider.value = 0;
        SetQuantityText();
        

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

    public void HideUI()
    {
        craftingHandler = null;
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
        recipeAgencyText.text = "";
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
        recipeAgencyText.text = craftableItem.AgencyCost.ToString();

        SetQuantitySlider();

        for (int i = 0; i < craftableItem.Ingredients.Count; i++)
        {
            ingredientSlots[i].AddItem(craftableItem.Ingredients[i].Item, craftableItem.Ingredients[i].Amount, PlayerInformation.instance.playerInventory.GetStock(craftableItem.Ingredients[i].Item.Name));
        }
        craftButton.interactable = true;
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
        
        quantitySelectedText.text = ($"{quantityToCraftSlider.value}/{quantityToCraftSlider.maxValue}");
    }

    public void CraftItem()
    {
        if (craftableItem != null)
        {
            if (InteractCostReward())
                craftingHandler.Craft(craftableItem, (int)quantityToCraftSlider.value, finalInventory);
        }
        SetCurrentRecipe(craftableItem);
        GetCraftingQueueItems();
    }

    bool InteractCostReward()
    {
        float agency = PlayerInformation.instance.statHandler.GetStatMaxModifiedValue("Agency");
        if (agency >= craftableItem.AgencyCost)
            return true;


        Notifications.instance.SetNewNotification($"{craftableItem.AgencyCost - agency} <sprite name=\"Agency\">", null, 0, NotificationsType.Warning);
        return false;
    }

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
