using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using System;
using Klaxon.Interactable;
using UnityEngine.Localization.Settings;

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

    public enum CompendiumItemType
    {
        Animal,
        Resource,
        Recipe,
        Note, 
        Guide
    }
    [Serializable]
    public struct ResourceButtonType
    {
        public ItemType type;
        public Button typeButton;
    }

    CompendiumItemType itemType;
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

    public GameObject resourceButtonTypesHolder;
    public List<ResourceButtonType> resourceButtonType = new List<ResourceButtonType>();

    public List<InteractableCraftingStation> allCraftingStations = new List<InteractableCraftingStation>();
    private void Start()
    {
        playerInformation = PlayerInformation.instance;
        playerAnimalCompendium = playerInformation.playerAnimalCompendiumDatabase;
        playerResourceCompendium = playerInformation.playerResourceCompendiumDatabase;
        playerRecipeCompendium = playerInformation.playerRecipeDatabase;
        playerNoteCompendium = playerInformation.playerNotesCompendiumDatabase;
        playerGuideCompendium = playerInformation.playerGuidesCompendiumDatabase;

        //compendiumUI.SetActive(false);
        
        maxItemTypes = System.Enum.GetValues(typeof(CompendiumItemType)).Length;

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
        itemType = (CompendiumItemType)currentItemTypeIndex;

        switch (itemType)
        {
            case CompendiumItemType.Animal:
                SetCompendiumTypeAnimal();
                break;
            case CompendiumItemType.Resource:
                SetCompendiumTypeResource();
                break;
            case CompendiumItemType.Recipe:
                SetCompendiumTypeRecipe();
                break;
            case CompendiumItemType.Note:
                SetCompendiumTypeNote();
                break;
            case CompendiumItemType.Guide:
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

        itemType = CompendiumItemType.Animal;
        UpdateCompendiumList();
    }
    public void SetCompendiumTypeResource()
    {
        SetButtonSelectedColor(animalButton, false);
        SetButtonSelectedColor(resourceButton, true);
        SetButtonSelectedColor(recipeButton, false);
        SetButtonSelectedColor(noteButton, false);
        SetButtonSelectedColor(guideButton, false);

        itemType = CompendiumItemType.Resource;
        UpdateCompendiumList();
    }
    public void SetCompendiumTypeRecipe()
    {
        SetButtonSelectedColor(animalButton, false);
        SetButtonSelectedColor(resourceButton, false);
        SetButtonSelectedColor(recipeButton, true);
        SetButtonSelectedColor(noteButton, false);
        SetButtonSelectedColor(guideButton, false);

        itemType = CompendiumItemType.Recipe;
        UpdateCompendiumList();
    }
    public void SetCompendiumTypeNote()
    {
        SetButtonSelectedColor(animalButton, false);
        SetButtonSelectedColor(resourceButton, false);
        SetButtonSelectedColor(recipeButton, false);
        SetButtonSelectedColor(noteButton, true);
        SetButtonSelectedColor(guideButton, false);

        itemType = CompendiumItemType.Note;
        UpdateCompendiumList();
    }

    public void SetCompendiumTypeGuide()
    {
        SetButtonSelectedColor(animalButton, false);
        SetButtonSelectedColor(resourceButton, false);
        SetButtonSelectedColor(recipeButton, false);
        SetButtonSelectedColor(noteButton, false);
        SetButtonSelectedColor(guideButton, true);

        itemType = CompendiumItemType.Guide;
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

            case CompendiumItemType.Animal:
                SetResourceButtons(false);
                for (int i = 0; i < playerAnimalCompendium.Items.Count; i++)
                {
                    CompendiumSlot newSlot = Instantiate(compendiumSlotObject, compendiumListHolder.transform);
                    newSlot.AddItem(playerAnimalCompendium.Items[i]);
                    //newSlot.AddRecipeReveal(playerAnimalCompendium.Items[i].ResearchRecipes);
                    compendiumSlots.Add(newSlot);
                }

                recipeRevealDescription.SetActive(true);
                animalRevealDescription.SetActive(true);
                resourceRevealDescription.SetActive(false);
                recipeDirectionsDescription.SetActive(false);
                break;


            case CompendiumItemType.Resource:

                SetResourceButtons(true);

                SetResourceType(ItemType.All);

                recipeRevealDescription.SetActive(true);
                animalRevealDescription.SetActive(false);
                resourceRevealDescription.SetActive(true);
                recipeDirectionsDescription.SetActive(false);
                break;


            case CompendiumItemType.Recipe:
                SetResourceButtons(false);
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


            case CompendiumItemType.Note:
                SetResourceButtons(false);
                for (int i = 0; i < playerNoteCompendium.Items.Count; i++)
                {
                    CompendiumSlot newSlot = Instantiate(compendiumSlotObject, compendiumListHolder.transform);
                    newSlot.AddItem(playerNoteCompendium.Items[i]);
                    compendiumSlots.Add(newSlot);
                }

                recipeRevealDescription.SetActive(false);
                break;

            case CompendiumItemType.Guide:
                SetResourceButtons(false);
                for (int i = 0; i < playerGuideCompendium.Items.Count; i++)
                {
                    CompendiumSlot newSlot = Instantiate(compendiumSlotObject, compendiumListHolder.transform);
                    newSlot.AddItem(playerGuideCompendium.Items[i]);
                    compendiumSlots.Add(newSlot);
                }

                recipeRevealDescription.SetActive(false);
                break;
        }

        SetItemSelected();
    }

    private void SetItemSelected()
    {
        EventSystem.current.SetSelectedGameObject(null);
        if (compendiumSlots.Count > 0)
            EventSystem.current.SetSelectedGameObject(compendiumSlots[0].GetComponentInChildren<Button>().gameObject);
    }

    public void SetResourceAll()
    {
        SetResourceType(ItemType.All);
    }
    public void SetResourceResources()
    {
        SetResourceType(ItemType.Resource);
    }
    public void SetResourceEquipment()
    {
        SetResourceType(ItemType.Equipment);
    }
    public void SetResourceConsumables()
    {
        SetResourceType(ItemType.Consumable);
    }
    public void SetResourceUtilities()
    {
        SetResourceType(ItemType.Utility);
    }
    public void SetResourceDecorations()
    {
        SetResourceType(ItemType.Decoration);
    }
    public void SetResourceFarming()
    {
        SetResourceType(ItemType.FarmStuffs);
    }
    public void SetResourceSmells()
    {
        SetResourceType(ItemType.Smells);
    }

    private void SetResourceButtons(bool active)
    {
        
        resourceButtonTypesHolder.gameObject.SetActive(active);
        if (active)
        {
            var prc = new List<QI_ItemData>(playerResourceCompendium.Items);
            List<ItemType> itemTypes = new List<ItemType>();
            foreach (var item in prc)
            {
                if (itemTypes.Contains(item.Type))
                    continue;
                else
                    itemTypes.Add(item.Type);
            }
            bool all = false;
            foreach (var butt in resourceButtonType)
            {
                butt.typeButton.gameObject.SetActive(false);
                
                foreach (var type in itemTypes)
                {
                    if (butt.type == type)
                    {
                        butt.typeButton.gameObject.SetActive(true);
                        all = true;
                    }
                }
            }
            if (all)
            {
                foreach (var type in resourceButtonType)
                {
                    if (type.type == ItemType.All)
                    {
                        type.typeButton.gameObject.SetActive(true);
                        
                    }
                }
            }
            

        }
            
    }


    private void SetResourceType(ItemType type)
    {
       var prc = new List<QI_ItemData>(playerResourceCompendium.Items);
       if(type == ItemType.All)
        {
            prc.Reverse();
            DisplayResources(prc);
        }
        else
        {
            prc = prc.OrderBy(x => x.Type).ThenBy(x => x.name).ToList();
            prc = RefineList(prc, type);
            DisplayResources(prc);
        }
        SetItemSelected();
    }

    private void DisplayResources(List<QI_ItemData> prc)
    {
        ClearItemInformation();
        ClearCompendiumSlot();
        ClearCompendiumRecipeSlot();
        for (int i = 0; i < prc.Count; i++)
        {
            CompendiumSlot newSlot = Instantiate(compendiumSlotObject, compendiumListHolder.transform);
            newSlot.AddItem(prc[i]);
            newSlot.AddRecipeReveal(prc[i].ResearchRecipes);
            compendiumSlots.Add(newSlot);
        }
    }

    List<QI_ItemData> RefineList(List<QI_ItemData> typeList, ItemType type)
    {
        List<QI_ItemData> refinedList = new List<QI_ItemData>();
        foreach (var item in typeList)
        {
            if (item.Type == type)
                refinedList.Add(item);
        }
        return refinedList;
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

    
    void UpdateDescription(string value)
    {
        informationDisplayItemDescription.text = value;
    }

    public void DisplayItemInformation(QI_ItemData item, QI_CraftingRecipe recipe, List<QI_ItemData.RecipeRevealObject> recipeReveals, bool longDescription = false)
    {
        recipeRevealDescription.SetActive(false);
        informationDisplayItemDescription.text = " ";
        localizedDisplayDescription = longDescription ? item.localizedLongDescription : item.localizedDescription;
        localizedDisplayDescription.StringChanged += UpdateDescription;
        informationDisplayItemDescription.text = $"\n<style=\"H1\">{item.localizedName.GetLocalizedString()}</style>\n\n{localizedDisplayDescription.GetLocalizedString()}\n\n";
        
        if (item.Type == ItemType.Consumable)
            informationDisplayItemDescription.text += $"{GetStatModifierText(item as ConsumableItemData)}\n"; 
        if(item.placementGumption != null)
            informationDisplayItemDescription.text += $"\n{item.placementGumption.EffectDescription.GetLocalizedString()}\n\n";
        if (item.Type == ItemType.Animal)
        {
            int index = PlayerInformation.instance.animalCompendiumInformation.animalNames.IndexOf(item.Name);
            int amount = PlayerInformation.instance.animalCompendiumInformation.animalTimesViewed[index];
            
            informationDisplayItemDescription.text += $"<sprite name=\"Spyglass\"> {amount}X\n";
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
                //agencyText.text = recipe.AgencyCost.ToString();
            }
            recipeDisplay.SetActive(true);
            informationDisplayItemDescription.text += GetCraftingStation(recipe);
        }
        else
        {
            informationDisplayItemDescription.text += GetCraftingStation(item);
        }
        if (recipeReveals.Count > 0)
        {
            ClearCompendiumRecipeRevealSlot();
            for (int i = 0; i < recipeReveals.Count; i++)
            {
                CompendiumRecipeSlot newRecipeSlot = Instantiate(compendiumRecipeSlotObject, compendiumRecipeRevealListHolder.transform);
                if(recipeReveals[i].recipe != null)
                    newRecipeSlot.AddItem(recipeReveals[i].recipe.Product.Item, recipeReveals[i].RecipeRevealAmount);
                compendiumRecipeSlots.Add(newRecipeSlot);
                int times = PlayerInformation.instance.animalCompendiumInformation.GetTimesViewed(item.Name);
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
            text += $"{mod.EffectDescription.GetLocalizedString()} \n";
        }
        foreach (var mod in item.statModifiers)
        {
            text += $"{mod.EffectDescription.GetLocalizedString()} \n";
        }
        return text;
    }

    string GetCraftingStation(QI_CraftingRecipe recipe)
    {
        
        foreach (var station in allCraftingStations)
        {
            foreach (var r in station.recipeDatabase.CraftingRecipes)
            {
                if (r == recipe)
                    return $"{LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", "Crafted using")} {station.GetComponent<QI_Item>().Data.localizedName.GetLocalizedString()} \n";
            }
        }
        return "";
    }

    string GetCraftingStation(QI_ItemData item)
    {

        foreach (var station in allCraftingStations)
        {
            foreach (var recipe in station.recipeDatabase.CraftingRecipes)
            {
                if (recipe.Product.Item == item)
                    return $"{LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", "Crafted using")} {station.GetComponent<QI_Item>().Data.localizedName.GetLocalizedString()} \n";
            }
        }
        return "";
    }



    public void ClearItemInformation()
    {
        
        informationDisplayItemDescription.text = " ";
        agencyText.text = " ";
        timesViewedText.text = " ";
        ClearCompendiumRecipeSlot();
        ClearCompendiumRecipeRevealSlot();
    }
}
