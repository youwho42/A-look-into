using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    public GameObject compendiumUI;
    PlayerInformation playerInformation;

    public GameObject compendiumListHolder;
    public CompendiumSlot compendiumSlotObject;
    public List<CompendiumSlot> compendiumSlots = new List<CompendiumSlot>();

    public GameObject compendiumRecipeListHolder;
    public CompendiumRecipeSlot compendiumRecipeSlotObject;
    public List<CompendiumRecipeSlot> compendiumRecipeSlots = new List<CompendiumRecipeSlot>();
    public GameObject recipeDisplay;
    public TextMeshProUGUI agencyText;

    public Button animalButton, resourceButton, recipeButton, noteButton;
    public Color selectedColor;
    public Color idleColor;

    public TextMeshProUGUI informationDisplayItemName;
    public TextMeshProUGUI informationDisplayItemDescription;

    QI_ItemDatabase playerAnimalCompendium;
    QI_ItemDatabase playerResourceCompendium;
    QI_ItemDatabase playerNoteCompendium;
    QI_CraftingRecipeDatabase playerRecipeCompendium;

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

        SetCompendiumTypeAnimal();
    }
    private void OnDestroy()
    {
        GameEventManager.onAnimalCompediumUpdateEvent.RemoveListener(UpdateCompendiumList);
        GameEventManager.onResourceCompediumUpdateEvent.RemoveListener(UpdateCompendiumList);
        GameEventManager.onRecipeCompediumUpdateEvent.RemoveListener(UpdateCompendiumList);
        GameEventManager.onNoteCompediumUpdateEvent.RemoveListener(UpdateCompendiumList);
    }
    public void ShowUI()
    {
        ClearItemInformation();
        playerInformation.TogglePlayerInput(false);
    }

    public void HideUI()
    {
        
        
        playerInformation.TogglePlayerInput(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (UIScreenManager.instance.CurrentUIScreen() == UIScreenType.PlayerUI)
            {
                ClearItemInformation();
                UIScreenManager.instance.DisplayScreen(UIScreenType.CompendiumScreen);
                PlayerInformation.instance.uiScreenVisible = true;
                ItemInformationDisplayUI.instance.HideInformationDisplay();
            }
            else if (UIScreenManager.instance.CurrentUIScreen() == UIScreenType.CompendiumScreen)
            {
                UIScreenManager.instance.HideScreens(UIScreenType.CompendiumScreen);
                UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);

                PlayerInformation.instance.uiScreenVisible = false;
                ItemInformationDisplayUI.instance.HideInformationDisplay();
            }

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
        agencyText.text = "";
        switch (itemType)
        {
            
            case ItemType.Animal:
                for (int i = 0; i < playerAnimalCompendium.Items.Count; i++)
                {

                    CompendiumSlot newSlot = Instantiate(compendiumSlotObject, compendiumListHolder.transform);
                    newSlot.AddItem(playerAnimalCompendium.Items[i]);
                    compendiumSlots.Add(newSlot);

                }
                break;
            case ItemType.Resource:
                for (int i = 0; i < playerResourceCompendium.Items.Count; i++)
                {

                    CompendiumSlot newSlot = Instantiate(compendiumSlotObject, compendiumListHolder.transform);
                    newSlot.AddItem(playerResourceCompendium.Items[i]);
                    compendiumSlots.Add(newSlot);

                }
                break;
            case ItemType.Recipe:
                for (int i = 0; i < playerRecipeCompendium.CraftingRecipes.Count; i++)
                {

                    CompendiumSlot newSlot = Instantiate(compendiumSlotObject, compendiumListHolder.transform);
                    newSlot.AddItem(playerRecipeCompendium.CraftingRecipes[i].Product.Item, playerRecipeCompendium.CraftingRecipes[i]);
                    compendiumSlots.Add(newSlot);
                    recipeDisplay.SetActive(true);
                    
                }
                break;
            case ItemType.Note:
                for (int i = 0; i < playerNoteCompendium.Items.Count; i++)
                {

                    CompendiumSlot newSlot = Instantiate(compendiumSlotObject, compendiumListHolder.transform);
                    newSlot.AddItem(playerNoteCompendium.Items[i]);
                    compendiumSlots.Add(newSlot);

                }
                break;
        }
    }


    public void ClearCompendiumSlot()
    {
        while (compendiumListHolder.transform.childCount > 0)
        {
            DestroyImmediate(compendiumListHolder.transform.GetChild(0).gameObject);
        }
    }
    public void ClearCompendiumRecipeSlot()
    {
        while (compendiumRecipeListHolder.transform.childCount > 0)
        {
            DestroyImmediate(compendiumRecipeListHolder.transform.GetChild(0).gameObject);
        }
    }

    public void DisplayItemInformation(QI_ItemData item, QI_CraftingRecipe recipe)
    {
        informationDisplayItemName.text = item.Name;
        informationDisplayItemDescription.text = item.Description;
        if(recipe!= null)
        {
            ClearCompendiumRecipeSlot();
            for (int i = 0; i < recipe.Ingredients.Count; i++)
            {
                CompendiumRecipeSlot newRecipeSlot = Instantiate(compendiumRecipeSlotObject, compendiumRecipeListHolder.transform);
                newRecipeSlot.AddItem(recipe.Ingredients[i].Item, recipe.Ingredients[i].Amount);
                compendiumRecipeSlots.Add(newRecipeSlot);
                agencyText.text = recipe.AgencyCost.ToString();
            }
        }
    }

    public void ClearItemInformation()
    {
        
        informationDisplayItemName.text = "";
        informationDisplayItemDescription.text = "";
        agencyText.text = "";
        ClearCompendiumRecipeSlot();
    }
}
