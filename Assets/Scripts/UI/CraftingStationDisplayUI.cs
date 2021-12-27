using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    PlayerInformation playerInformation;
    PlayerCrafting playerRecipes;
    public GameObject craftingStationUI;
    


    QI_Inventory playerInventory;
    QI_CraftingHandler craftingHandler;
    public QI_ItemDatabase itemDatabase;
    


    public List<CraftingSlot> ingredientSlots = new List<CraftingSlot>();
    List<CraftingRecipeButton> recipeButtons = new List<CraftingRecipeButton>();


    public CraftingSlot recipeResultSlot;


    public GameObject recipeButtonHolder;
    public GameObject recipeButton;

    QI_CraftingRecipe craftableItem;

    






    private void Start()
    {
        craftingStationUI.SetActive(false);
        playerInformation = PlayerInformation.instance;
        playerInventory = playerInformation.playerInventory;
        playerRecipes = PlayerCrafting.instance;
        craftingHandler = GetComponent<QI_CraftingHandler>();
    }



    public void ShowUI()
    {
        SetAvailableRecipes();
        playerInformation.TogglePlayerInput(false);
        craftingStationUI.SetActive(true);
    }

    public void HideUI()
    {
        playerInformation.TogglePlayerInput(true);
        craftingStationUI.SetActive(false);
    }

    

    
    
    public void SetAvailableRecipes()
    {
        ClearRecipeSlots();

        for (int i = 0; i < playerRecipes.craftingRecipeDatabase.CraftingRecipes.Count; i++)
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
            recipeButtons[i].AddItem(playerRecipes.craftingRecipeDatabase.CraftingRecipes[i]);
        }

    }
    public void ClearCurrentRecipe()
    {
        craftableItem = null;
        recipeResultSlot.ClearSlot();

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



        for (int i = 0; i < craftableItem.Ingredients.Count; i++)
        {

            ingredientSlots[i].AddItem(craftableItem.Ingredients[i].Item, craftableItem.Ingredients[i].Amount, playerInventory.GetStock(craftableItem.Ingredients[i].Item.Name));
        }

    }

    public void CraftItem()
    {
        if (craftableItem != null)
        {
            if(InteractCostReward())
                craftingHandler.Craft(craftableItem, 1);
        }
        SetCurrentRecipe(craftableItem);
        
    }

    bool InteractCostReward()
    {
        if (playerInformation.playerStats.playerAttributes.GetAttributeValue("Bounce") >= craftableItem.PlayerEnergyCost)
        {
            
            PlayerInformation.instance.playerStats.RemovePlayerEnergy(craftableItem.PlayerEnergyCost);
            return true;
        }

        NotificationManager.instance.SetNewNotification("You are missing Yellow Bar stuff to craft this.");
        return false;
    }

}
