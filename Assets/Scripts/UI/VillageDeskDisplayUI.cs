using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class VillageDeskDisplayUI : MonoBehaviour
{
    public static VillageDeskDisplayUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
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
    int initialDisplay = 0;
    public RectTransform contentRect;
    public RectTransform viewportRect;
    public Button purchaseButton;
    public TextMeshProUGUI buttonText;

    public void ShowUI(FixVillageDesk desk)
    {
        PlayerInformation.instance.uiScreenVisible = true;
        PlayerInformation.instance.TogglePlayerInput(false);
        villageDesk = desk;
        SetVillageButtons();
    }

    public void HideUI()
    {
        PlayerInformation.instance.uiScreenVisible = false;
        PlayerInformation.instance.TogglePlayerInput(true);
        UIScreenManager.instance.HideAllScreens();
        if (LevelManager.instance.HUDBinary == 1)
            UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
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
            if (!area.isActive)
                availableAreas.Add(area);
        }

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

    public void SetCurrentVillageArea(FixVillageArea area)
    {
        ClearCurrentVillageArea();
        villageAreaTitle.text = area.areaName;
        villageAreaDescription.text = area.areaDescription;
        villageAreaCost.text = $"<sprite anim=\"3,5,12\"> {area.sparksRequired}";
        
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

    void SetPurchaseButton(FixVillageArea area)
    {
        buttonText.text = area.isFixing ? "In Progress" : "Build Area";

        purchaseButton.interactable = !area.isFixing;
    }

    public void PurchaseArea()
    {
        currentArea.isFixing = true;
        SetPurchaseButton(currentArea);
    }
}