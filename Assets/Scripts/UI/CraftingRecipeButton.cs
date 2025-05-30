using System;
using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class CraftingRecipeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    QI_CraftingRecipe item;
    public TextMeshProUGUI recipeName;
    public Image itemIcon;
    CraftingStationDisplayUI craftingStation;
    public Button button;

    private void Start()
    {
        craftingStation = CraftingStationDisplayUI.instance;
        button.onClick.AddListener(SetCurrentRecipe);
        button.onClick.AddListener(SetTutorial);
    }

    private void SetCurrentRecipe()
    {
        craftingStation.SetCurrentRecipe(item);
        AudioManager.instance.PlaySound("Crafting_SetRecipe");
    }

    private void SetTutorial()
    {
        craftingStation.tutorial.SetNextTutorialIndex(1);
    }

    public void AddItem(QI_CraftingRecipe newItem)
    {
        item = newItem;
        var n = item.Product.Item.localizedName.GetLocalizedString();
        recipeName.text = item.Product.Amount > 1 ? $"{n} x{item.Product.Amount}" : n;
        itemIcon.sprite = item.Product.Item.Icon;
    }

    public void ShowInformation()
    {
        if (item == null)
            return;
        ItemInformationDisplayUI.instance.ShowItemName(item.Product.Item, this.GetComponent<RectTransform>());
    }
    public void HideInformation()
    {
        ItemInformationDisplayUI.instance.HideItemName();
    }

    void ClearSlot()
    {
        item = null;
        recipeName.text = "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowInformation();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideInformation();
    }
}
