using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using UnityEngine.EventSystems;
using UnityEngine.Localization;

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
        Note, 
        Guide
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

    public Button animalButton, resourceButton, recipeButton, noteButton, guideButton;
    public Color selectedColor;
    public Color idleColor;

    public TextMeshProUGUI informationDisplayItemName;
    public TextMeshProUGUI informationDisplayItemDescription;
    LocalizedString localizedDisplayName;
    LocalizedString localizedDisplayDescription;


    QI_ItemDatabase playerAnimalCompendium;
    QI_ItemDatabase playerResourceCompendium;
    QI_ItemDatabase playerNoteCompendium;
    QI_ItemDatabase playerGuideCompendium;
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
        playerGuideCompendium = playerInformation.playerGuidesCompendiumDatabase;

        //compendiumUI.SetActive(false);
        
        maxItemTypes = System.Enum.GetValues(typeof(ItemType)).Length;

        SetCompendiumTypeAnimal();
    }

    private void OnEnable()
    {
        ClearItemInformation();
        GameEventManager.onAnimalCompediumUpdateEvent.AddListener(UpdateCompendiumList);
        GameEventManager.onResourceCompediumUpdateEvent.AddListener(UpdateCompendiumList);
        GameEventManager.onRecipeCompediumUpdateEvent.AddListener(UpdateCompendiumList);
        GameEventManager.onNoteCompediumUpdateEvent.AddListener(UpdateCompendiumList);
        GameEventManager.onGuideCompediumUpdateEvent.AddListener(UpdateCompendiumList);
        GameEventManager.onGamepadTriggersButtonEvent.AddListener(ChangeDisplayUI);
        UpdateCompendiumList();
    }


    private void OnDisable()
    {
        GameEventManager.onAnimalCompediumUpdateEvent.RemoveListener(UpdateCompendiumList);
        GameEventManager.onResourceCompediumUpdateEvent.RemoveListener(UpdateCompendiumList);
        GameEventManager.onRecipeCompediumUpdateEvent.RemoveListener(UpdateCompendiumList);
        GameEventManager.onNoteCompediumUpdateEvent.RemoveListener(UpdateCompendiumList);
        GameEventManager.onGuideCompediumUpdateEvent.RemoveListener(UpdateCompendiumList);
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
            case ItemType.Guide:
                SetCompendiumTypeGuide();
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
        SetButtonSelectedColor(guideButton, false);

        itemType = ItemType.Animal;
        UpdateCompendiumList();
    }
    public void SetCompendiumTypeResource()
    {
        SetButtonSelectedColor(animalButton, false);
        SetButtonSelectedColor(resourceButton, true);
        SetButtonSelectedColor(recipeButton, false);
        SetButtonSelectedColor(noteButton, false);
        SetButtonSelectedColor(guideButton, false);

        itemType = ItemType.Resource;
        UpdateCompendiumList();
    }
    public void SetCompendiumTypeRecipe()
    {
        SetButtonSelectedColor(animalButton, false);
        SetButtonSelectedColor(resourceButton, false);
        SetButtonSelectedColor(recipeButton, true);
        SetButtonSelectedColor(noteButton, false);
        SetButtonSelectedColor(guideButton, false);

        itemType = ItemType.Recipe;
        UpdateCompendiumList();
    }
    public void SetCompendiumTypeNote()
    {
        SetButtonSelectedColor(animalButton, false);
        SetButtonSelectedColor(resourceButton, false);
        SetButtonSelectedColor(recipeButton, false);
        SetButtonSelectedColor(noteButton, true);
        SetButtonSelectedColor(guideButton, false);

        itemType = ItemType.Note;
        UpdateCompendiumList();
    }

    public void SetCompendiumTypeGuide()
    {
        SetButtonSelectedColor(animalButton, false);
        SetButtonSelectedColor(resourceButton, false);
        SetButtonSelectedColor(recipeButton, false);
        SetButtonSelectedColor(noteButton, false);
        SetButtonSelectedColor(guideButton, true);

        itemType = ItemType.Guide;
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
        if (playerAnimalCompendium == null)
            return;
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

            case ItemType.Guide:
                for (int i = 0; i < playerGuideCompendium.Items.Count; i++)
                {
                    CompendiumSlot newSlot = Instantiate(compendiumSlotObject, compendiumListHolder.transform);
                    newSlot.AddItem(playerGuideCompendium.Items[i]);
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
        foreach (Transform child in compendiumListHolder.transform)
        {
            Destroy(child.gameObject);
        }
        
    }
    public void ClearCompendiumRecipeSlot()
    {
        compendiumRecipeSlots.Clear();
        foreach (Transform child in compendiumRecipeListHolder.transform)
        {
            Destroy(child.gameObject);
        }
        
    }

    public void ClearCompendiumRecipeRevealSlot()
    {
        compendiumRecipeRevealSlots.Clear();
        foreach (Transform child in compendiumRecipeRevealListHolder.transform)
        {
            Destroy(child.gameObject);
        }
        
    }

    void UpdateName(string value)
    {
        informationDisplayItemName.text = value;
    }
    void UpdateDescription(string value)
    {
        informationDisplayItemDescription.text = value;
    }

    public void DisplayItemInformation(QI_ItemData item, QI_CraftingRecipe recipe, List<QI_ItemData.RecipeRevealObject> recipeReveals)
    {
        recipeRevealDescription.SetActive(false);
        localizedDisplayName = item.localizedName;
        localizedDisplayName.StringChanged += UpdateName;
        localizedDisplayDescription = item.localizedDescription;
        localizedDisplayDescription.StringChanged += UpdateDescription;
        //informationDisplayItemName.text = item.localizedName.GetLocalizedString(item.Name);
        informationDisplayItemDescription.text = $"\n<style=\"H1\">{item.localizedName.GetLocalizedString(item.Name)}</style>\n\n{item.localizedDescription.GetLocalizedString(item.Name + " Description")}\n\n";
        if (item.Type == QuantumTek.QuantumInventory.ItemType.Consumable)
        {
            informationDisplayItemDescription.text += GetStatModifierText(item as ConsumableItemData); 
        }
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

    string GetStatModifierText(ConsumableItemData item)
    {
        string text = "";
        foreach (var mod in item.statChangers)
        {
            string amount = mod.ModifierType == Klaxon.StatSystem.ModifierType.Percent ? $"{mod.Amount * 100}%" : $"{mod.Amount}";
            text += $"{mod.StatToModify.Name} {mod.Amount} \n";
        }
        foreach (var mod in item.statModifiers)
        {
            string amount = mod.ModifierType == Klaxon.StatSystem.ModifierType.Percent ? $"{mod.ModifierAmount * 100}%" : $"{mod.ModifierAmount}";
            text += $"{mod.StatToModify.Name} {amount} \n";
        }
        return text;
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
