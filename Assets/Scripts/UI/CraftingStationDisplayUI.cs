using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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



    private void Start()
    {
        craftingStationUI.SetActive(false);
        
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
        PlayerInformation.instance.uiScreenVisible = true;
        PlayerInformation.instance.TogglePlayerInput(false);

        EventSystem.current.SetSelectedGameObject(null);
        if (recipeButtons.Count > 0)
            EventSystem.current.SetSelectedGameObject(recipeButtons[0].GetComponentInChildren<Button>().gameObject);
        //craftingStationUI.SetActive(true);
    }

    public void HideUI()
    {
        PlayerInformation.instance.uiScreenVisible = false;
        PlayerInformation.instance.TogglePlayerInput(true);
        craftingHandler = null;
        //craftingStationUI.SetActive(false);
    }

    

    
    
    public void SetAvailableRecipes()
    {
        ClearRecipeSlots();

        for (int i = 0; i < recipeDatabase.CraftingRecipes.Count; i++)
        {
            GameObject newRecipe = Instantiate(recipeButton, recipeButtonHolder.transform);
            recipeButtons.Add(newRecipe.GetComponent<CraftingRecipeButton>());
        }
        UpdateCraftingUI();
        ClearCurrentRecipe();
    }

    public void UpdateCraftingUI()
    {
        for (int i = 0; i < recipeButtons.Count; i++)
        {
            recipeButtons[i].AddItem(recipeDatabase.CraftingRecipes[i]);
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
        while (recipeButtonHolder.transform.childCount > 0)
        {
            DestroyImmediate(recipeButtonHolder.transform.GetChild(0).gameObject);
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
        quantitySelectedText.text = ($"Quantity {quantityToCraftSlider.value}/{quantityToCraftSlider.maxValue}");
    }

    public void CraftItem()
    {
        if (craftableItem != null)
        {
            if (InteractCostReward())
            {
                for (int i = 0; i < (int)quantityToCraftSlider.value; i++)
                {
                    craftingHandler.Craft(craftableItem, 1, finalInventory);
                }
                
            }
                
        }
        SetCurrentRecipe(craftableItem);
        
    }

    bool InteractCostReward()
    {
        float agency = PlayerInformation.instance.playerStats.playerAttributes.GetAttributeValue("Agency");
        if (agency >= craftableItem.AgencyCost)
            return true;


        NotificationManager.instance.SetNewNotification($"{craftableItem.AgencyCost} Agency needed", NotificationManager.NotificationType.Warning);
        return false;
    }

}
