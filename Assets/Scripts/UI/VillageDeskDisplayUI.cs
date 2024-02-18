using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Klaxon.SAP;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.Localization.SmartFormat.Extensions;

public class VillageDeskDisplayUI : MonoBehaviour
{
    public static VillageDeskDisplayUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        var source = LocalizationSettings.StringDatabase.SmartFormatter.GetSourceExtension<PersistentVariablesSource>();
        villageAreaTime = source["global"]["VillageAreaTime"] as IntVariable;
    }

    public GameObject villageAreasButtonHolder;
    public VillageDeskAreaButton villageAreaButton;
    public FixVillageArea currentArea;
    FixVillageDesk villageDesk;
    public TextMeshProUGUI villageAreaTitle;
    public TextMeshProUGUI villageAreaDescription;

    List<VillageDeskAreaButton> villageAreasButtons = new List<VillageDeskAreaButton>();
    public List<CraftingSlot> ingredientSlots = new List<CraftingSlot>();
    public TextMeshProUGUI villageAreaCost;
    public TextMeshProUGUI approxDays;
    int initialDisplay = 0;
    public RectTransform contentRect;
    public RectTransform viewportRect;
    public Button purchaseButton;
    public TextMeshProUGUI buttonText;
    IntVariable villageAreaTime;

    private void Start()
    {
        gameObject.SetActive(false);
    }
    public void ShowUI(FixVillageDesk desk)
    {
        PlayerInformation.instance.uiScreenVisible = true;
        PlayerInformation.instance.TogglePlayerInput(false);
        UIScreenManager.instance.DisplayPlayerHUD(true);
        villageDesk = desk;
        SetVillageButtons();
    }

    public void HideUI()
    {
        PlayerInformation.instance.uiScreenVisible = false;
        PlayerInformation.instance.TogglePlayerInput(true);
        UIScreenManager.instance.HideAllScreens();
        UIScreenManager.instance.DisplayPlayerHUD(LevelManager.instance.HUDBinary == 1);
    }

    void SetVillageButtons()
    {
        ClearInventorySlots();
        ClearCurrentVillageArea();
        //var activeUndertakings = PlayerInformation.instance.playerUndertakings.activeUndertakings;
        initialDisplay = 0;
        bool chosen = false;
        // create undertaking buttons of NOT complete undertakings
        List<FixVillageArea> availableAreas = new List<FixVillageArea>();
        foreach (var area in villageDesk.fixableAreas)
        {
            if (!area.isActive && area.undertakingObject.CurrentState == Klaxon.UndertakingSystem.UndertakingState.Active)
                availableAreas.Add(area);
        }

        if(availableAreas.Count > 0)
        {
            for (int i = 0; i < availableAreas.Count; i++)
            {

                if (!availableAreas[i].isFixing && !availableAreas[i].isActive && !chosen)
                {
                    chosen = true;
                    initialDisplay = i;
                }
                CreateVillageAreaButton(availableAreas[i]);
            }

            SetCurrentVillageArea(availableAreas[initialDisplay]);

            EventSystem.current.SetSelectedGameObject(null);
            if (villageAreasButtons.Count > 0)
                EventSystem.current.SetSelectedGameObject(villageAreasButtons[initialDisplay].GetComponentInChildren<Button>().gameObject);
            ScrollToSelectedObject(villageAreasButtons[initialDisplay].GetComponent<RectTransform>());
        }
        else
        {
            buttonText.text = LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Nothing To Build");
            purchaseButton.interactable = false;
            villageAreaDescription.text = LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Mayor needed at desk");
            villageAreaCost.text = $"<sprite anim=\"3,5,12\">";
            approxDays.text = "";
        }
        
    }

    public void SetCurrentVillageArea(FixVillageArea area)
    {
        ClearCurrentVillageArea();
        villageAreaTitle.text = area.localizedName.GetLocalizedString();
        villageAreaDescription.text = area.localizedDescription.GetLocalizedString();
        villageAreaCost.text = $"<sprite anim=\"3,5,12\"> {area.sparksRequired}";
        //approxDays.text = $"{GetTimeToFix(area)}";
        var xx = GetTimeToFix(area);
        //approxDays.text= LocalizationSettings.StringDatabase.GetLocalizedString("Variable-Texts", "Village Area Time");
        for (int i = 0; i < area.ingredients.Count; i++)
        {
            ingredientSlots[i].AddItem(area.ingredients[i].item, area.ingredients[i].amount, PlayerInformation.instance.playerInventory.GetStock(area.ingredients[i].item.Name));
        }

        // set button text and interactable
        currentArea = area;
        SetPurchaseButton(area);

    }

    void CreateVillageAreaButton(FixVillageArea area)
    {
        VillageDeskAreaButton newArea = Instantiate(villageAreaButton, villageAreasButtonHolder.transform);
        villageAreasButtons.Add(newArea);
        newArea.AddVillageArea(area);
       
    }

    public void ClearCurrentVillageArea()
    {
        currentArea = null;
        villageAreaTitle.text = "";
        villageAreaDescription.text = "";
        foreach (var slot in ingredientSlots)
        {
            slot.ClearSlot();
        }
    }

    public void ClearInventorySlots()
    {
        villageAreasButtons.Clear();
        foreach (Transform child in villageAreasButtonHolder.transform)
        {
            Destroy(child.gameObject);
        }

    }

    int GetTimeToFix(FixVillageArea area)
    {
        villageAreaTime.Value = area.ticksToFix / 60;
        return villageAreaTime.Value;
        //return Mathf.CeilToInt((float)area.ticksToFix / 510.0f);
    }


    public void ScrollToSelectedObject(RectTransform targetObject)
    {
        

        // Calculate the target object's position within the content's local coordinates
        Vector2 targetLocalPos = contentRect.InverseTransformPoint(targetObject.position);

        // Calculate the target position to center it within the ScrollView viewport
        Vector2 offset = (Vector2)viewportRect.rect.size - (Vector2)targetObject.rect.size;
        Vector2 targetPos = new Vector2(
            Mathf.Clamp(targetLocalPos.x, offset.x, contentRect.rect.width - offset.x),
             contentRect.anchoredPosition.y
        );

        // Set the content's anchored position to the calculated target position
        contentRect.anchoredPosition = -targetPos;
    }

    bool CanFixArea()
    {
        if (PlayerInformation.instance.purse.GetPurseAmount() < currentArea.sparksRequired)
            return false;
        foreach (var ingredient in currentArea.ingredients)
        {
            if (PlayerInformation.instance.playerInventory.GetStock(ingredient.item.Name) < ingredient.amount)
                return false;
        }
        return true;
    }

    void RemovePlayerRequirements()
    {
        PlayerInformation.instance.purse.RemoveFromPurse(currentArea.sparksRequired);
        foreach (var ingredient in currentArea.ingredients)
        {
            PlayerInformation.instance.playerInventory.RemoveItem(ingredient.item, ingredient.amount);
        }
    }

    void SetPurchaseButton(FixVillageArea area)
    {
        bool noOthersFixing = true;
        foreach (var a in villageDesk.fixableAreas)
        {
            if(a.isFixing) noOthersFixing = false; 
            break;
        }
        buttonText.text = area.isFixing ? LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "In Progress") : LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Build Area");
        purchaseButton.interactable = villageDesk.isActive && CanFixArea() && !area.isFixing && noOthersFixing;

    }

    public void PurchaseArea()
    {
        currentArea.isFixing = true;
        SetPurchaseButton(currentArea);
        RemovePlayerRequirements();
        SAP_WorldBeliefStates.instance.SetWorldState("FixingAreaAvailable", true);
        SetCurrentVillageArea(currentArea);
    }
}
