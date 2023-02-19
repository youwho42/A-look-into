using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using UnityEngine.EventSystems;

public class CompendiumDisplayUI : MonoBehaviour
{
    public static CompendiumDisplayUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public enum ItemType
    {
        Animal,
        Resource,
        Recipe,
        Note
    }

    ItemType itemType;
    int currentItemTypeIndex;
    int maxItemTypes;
    public GameObject compendiumUI;
    PlayerInformation playerInformation;

    public GameObject compendiumListHolder;
    public CompendiumSlot compendiumSlotObject;
    public List<CompendiumSlot> compendiumSlots = new List<CompendiumSlot>();

    public GameObject compendiumRecipeListHolder;
    public CompendiumRecipeSlot compendiumRecipeSlotObject;
    public List<CompendiumRecipeSlot> compendiumRecipeSlots = new List<CompendiumRecipeSlot>();

    public GameObject compendiumRecipeRevealListHolder;
    public List<CompendiumRecipeSlot> compendiumRecipeRevealSlots = new List<CompendiumRecipeSlot>();

    public GameObject recipeDisplay;
    public TextMeshProUGUI agencyText;

    public GameObject recipeRevealDisplay;
    public GameObject timesViewedDisplay;
    public TextMeshProUGUI timesViewedText;

    public Button animalButton, resourceButton, recipeButton, noteButton;
    public Color selectedColor;
    public Color idleColor;

    public TextMeshProUGUI informationDisplayItemName;
    public TextMeshProUGUI informationDisplayItemDescription;

    QI_ItemDatabase playerAnimalCompendium;
    QI_ItemDatabase playerResourceCompendium;
    QI_ItemDatabase playerNoteCompendium;
    QI_CraftingRecipeDatabase playerRecipeCompendium;


    public GameObject recipeRevealDescription;
    public GameObject animalRevealDescription;
    public GameObject resourceRevealDescription;
    public GameObject recipeDirectionsDescription;

    private void Start()
    {
        playerInformation = PlayerInformation.instance;
        playerAnimalCompendium = playerInformation.playerAnimalCompendiumDatabase;
        playerResourceCompendium = playerInformation.playerResourceCompendiumDatabase;
        playerRecipeCompendium = playerInformation.playerRecipeDatabase;
        playerNoteCompendium = playerInformation.playerNotesCompendiumDatabase;
        compendiumUI.SetActive(false);
        GameEventManager.onAnimalCompediumUpdateEvent.AddListener(UpdateCompendiumList);
        GameEventManager.onResourceCompediumUpdateEvent.AddListener(UpdateCompendiumList);
        GameEventManager.onRecipeCompediumUpdateEvent.AddListener(UpdateCompendiumList);
        GameEventManager.onNoteCompediumUpdateEvent.AddListener(UpdateCompendiumList);
        GameEventManager.onGamepadTriggersButtonEvent.AddListener(ChangeDisplayUI);

        maxItemTypes = System.Enum.GetValues(typeof(ItemType)).Length;

        SetCompendiumTypeAnimal();
        UpdateCompendiumList();
    }
    private void OnDestroy()
    {
        GameEventManager.onAnimalCompediumUpdateEvent.RemoveListener(UpdateCompendiumList);
        GameEventManager.onResourceCompediumUpdateEvent.RemoveListener(UpdateCompendiumList);
        GameEventManager.onRecipeCompediumUpdateEvent.RemoveListener(UpdateCompendiumList);
        GameEventManager.onNoteCompediumUpdateEvent.RemoveListener(UpdateCompendiumList);
        GameEventManager.onGamepadTriggersButtonEvent.RemoveListener(ChangeDisplayUI);

    }
    public void ShowUI()
    {
        PlayerInformation.instance.uiScreenVisible = true;
        ClearItemInformation();
        playerInformation.TogglePlayerInput(false);
    }

    public void HideUI()
    {
        PlayerInformation.instance.uiScreenVisible = false;

        playerInformation.TogglePlayerInput(true);
    }
    private void OnEnable()
    {
        ClearItemInformation();
    }

    void ChangeDisplayUI(int dir)
    {
        if (!compendiumUI.activeInHierarchy)
            return;

        currentItemTypeIndex += dir;

        if (currentItemTypeIndex > maxItemTypes - 1)
            currentItemTypeIndex = 0;
        else if (currentItemTypeIndex < 0)
            currentItemTypeIndex = maxItemTypes - 1;
        itemType = (ItemType)currentItemTypeIndex;

        switch (itemType)
        {
            case ItemType.Animal:
                SetCompendiumTypeAnimal();
                break;
            case ItemType.Resource:
                SetCompendiumTypeResource();
                break;
            case ItemType.Recipe:
                SetCompendiumTypeRecipe();
                break;
            case ItemType.Note:
                SetCompendiumTypeNote();
                break;
        }
    }

    void SetButtonSelectedColor(Button butt, bool selected)
    {
        var ac = butt.colors;
        ac.normalColor = selected ? selectedColor : idleColor;
        butt.colors = ac;
    }
    public void SetCompendiumTypeAnimal()
    {
        SetButtonSelectedColor(animalButton, true);
        SetButtonSelectedColor(resourceButton, false);
        SetButtonSelectedColor(recipeButton, false);
        SetButtonSelectedColor(noteButton, false);

        itemType = ItemType.Animal;
        UpdateCompendiumList();
    }
    public void SetCompendiumTypeResource()
    {
        SetButtonSelectedColor(animalButton, false);
        SetButtonSelectedColor(resourceButton, true);
        SetButtonSelectedColor(recipeButton, false);
        SetButtonSelectedColor(noteButton, false);

        itemType = ItemType.Resource;
        UpdateCompendiumList();
    }
    public void SetCompendiumTypeRecipe()
    {
        SetButtonSelectedColor(animalButton, false);
        SetButtonSelectedColor(resourceButton, false);
        SetButtonSelectedColor(recipeButton, true);
        SetButtonSelectedColor(noteButton, false);

        itemType = ItemType.Recipe;
        UpdateCompendiumList();
    }
    public void SetCompendiumTypeNote()
    {
        SetButtonSelectedColor(animalButton, false);
        SetButtonSelectedColor(resourceButton, false);
        SetButtonSelectedColor(recipeButton, false);
        SetButtonSelectedColor(noteButton, true);

        itemType = ItemType.Note;
        UpdateCompendiumList();
    }

    void UpdateCompendiumList()
    {
        
        ClearItemInformation();
        ClearCompendiumSlot();
        ClearCompendiumRecipeSlot();
        recipeDisplay.SetActive(false);
        recipeRevealDisplay.SetActive(false);
        agencyText.text = "";
        switch (itemType)
        {
            
            case ItemType.Animal:
                for (int i = 0; i < playerAnimalCompendium.Items.Count; i++)
                {
                    CompendiumSlot newSlot = Instantiate(compendiumSlotObject, compendiumListHolder.transform);
                    newSlot.AddItem(playerAnimalCompendium.Items[i]);
                    newSlot.AddRecipeReveal(playerAnimalCompendium.Items[i].ResearchRecipes);
                    compendiumSlots.Add(newSlot);
                }
                
                recipeRevealDescription.SetActive(true);
                animalRevealDescription.SetActive(true);
                resourceRevealDescription.SetActive(false);
                recipeDirectionsDescription.SetActive(false);
                break;


            case ItemType.Resource:
                for (int i = 0; i < playerResourceCompendium.Items.Count; i++)
                {
                    CompendiumSlot newSlot = Instantiate(compendiumSlotObject, compendiumListHolder.transform);
                    newSlot.AddItem(playerResourceCompendium.Items[i]);
                    newSlot.AddRecipeReveal(playerResourceCompendium.Items[i].ResearchRecipes);
                    compendiumSlots.Add(newSlot);
                }
                
                recipeRevealDescription.SetActive(true);
                animalRevealDescription.SetActive(false);
                resourceRevealDescription.SetActive(true);
                recipeDirectionsDescription.SetActive(false);
                break;


            case ItemType.Recipe:
                for (int i = 0; i < playerRecipeCompendium.CraftingRecipes.Count; i++)
                {
                    CompendiumSlot newSlot = Instantiate(compendiumSlotObject, compendiumListHolder.transform);
                    newSlot.AddItem(playerRecipeCompendium.CraftingRecipes[i].Product.Item, playerRecipeCompendium.CraftingRecipes[i]);
                    compendiumSlots.Add(newSlot);
                }
                
                recipeRevealDescription.SetActive(true);
                animalRevealDescription.SetActive(false);
                resourceRevealDescription.SetActive(false);
                recipeDirectionsDescription.SetActive(true);
                break;


            case ItemType.Note:
                for (int i = 0; i < playerNoteCompendium.Items.Count; i++)
                {
                    CompendiumSlot newSlot = Instantiate(compendiumSlotObject, compendiumListHolder.transform);
                    newSlot.AddItem(playerNoteCompendium.Items[i]);
                    compendiumSlots.Add(newSlot);
                }
                
                recipeRevealDescription.SetActive(false);
                break;
        }

        EventSystem.current.SetSelectedGameObject(null);
        if (compendiumSlots.Count > 0)
            EventSystem.current.SetSelectedGameObject(compendiumSlots[0].GetComponentInChildren<Button>().gameObject);
    }


    public void ClearCompendiumSlot()
    {
        compendiumSlots.Clear();
        while (compendiumListHolder.transform.childCount > 0)
        {
            DestroyImmediate(compendiumListHolder.transform.GetChild(0).gameObject);
        }
    }
    public void ClearCompendiumRecipeSlot()
    {
        compendiumRecipeSlots.Clear();
        while (compendiumRecipeListHolder.transform.childCount > 0)
        {
            DestroyImmediate(compendiumRecipeListHolder.transform.GetChild(0).gameObject);
        }
    }

    public void ClearCompendiumRecipeRevealSlot()
    {
        compendiumRecipeRevealSlots.Clear();
        while (compendiumRecipeRevealListHolder.transform.childCount > 0)
        {
            DestroyImmediate(compendiumRecipeRevealListHolder.transform.GetChild(0).gameObject);
        }
    }

    public void DisplayItemInformation(QI_ItemData item, QI_CraftingRecipe recipe, List<QI_ItemData.RecipeRevealObject> recipeReveals)
    {
        recipeRevealDescription.SetActive(false);
        informationDisplayItemName.text = item.Name;
        informationDisplayItemDescription.text = item.Description;
        recipeDisplay.SetActive(false);
        recipeRevealDisplay.SetActive(false);
        if (recipe!= null)
        {
            ClearCompendiumRecipeSlot();
            for (int i = 0; i < recipe.Ingredients.Count; i++)
            {
                CompendiumRecipeSlot newRecipeSlot = Instantiate(compendiumRecipeSlotObject, compendiumRecipeListHolder.transform);
                newRecipeSlot.AddItem(recipe.Ingredients[i].Item, recipe.Ingredients[i].Amount);
                compendiumRecipeSlots.Add(newRecipeSlot);
                agencyText.text = recipe.AgencyCost.ToString();
            }
            recipeDisplay.SetActive(true);
        }
        if (recipeReveals.Count > 0)
        {
            ClearCompendiumRecipeRevealSlot();
            for (int i = 0; i < recipeReveals.Count; i++)
            {
                CompendiumRecipeSlot newRecipeSlot = Instantiate(compendiumRecipeSlotObject, compendiumRecipeRevealListHolder.transform);
                newRecipeSlot.AddItem(recipeReveals[i].recipe.Product.Item, recipeReveals[i].RecipeRevealAmount);
                compendiumRecipeSlots.Add(newRecipeSlot);
                int times = PlayerInformation.instance.animalCompendiumInformation.TimesViewed(item.Name);
                if(times > 0)
                {
                    timesViewedText.text = times.ToString();
                    timesViewedDisplay.SetActive(true);
                }
                else
                {
                    timesViewedDisplay.SetActive(false);
                }
                    
                
            }
            recipeRevealDisplay.SetActive(true);

        }
        
    }

    public void ClearItemInformation()
    {
        
        informationDisplayItemName.text = "";
        informationDisplayItemDescription.text = "";
        agencyText.text = "";
        timesViewedText.text = "";
        ClearCompendiumRecipeSlot();
        ClearCompendiumRecipeRevealSlot();
    }
}
