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
    bool showingPrimaryItem = true;
    bool showingInfo;
    public Image itemDotA;
    public Image itemDotB;
    public Sprite itemDotWhite;
    public Sprite itemDotGray;
    private void Start()
    {
        showingPrimaryItem = true;
        craftingStation = CraftingStationDisplayUI.instance;
        button.onClick.AddListener(SetCurrentRecipe);
        button.onClick.AddListener(SetTutorial);
    }
    void OnDisable()
    {
        showingInfo = false;
        StopAllCoroutines();
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
        HideDots();
        item = newItem;
        showingPrimaryItem = true;
        var n = item.Product.Item.localizedName.GetLocalizedString();
        recipeName.text = item.Product.Amount > 1 ? $"{n} x{item.Product.Amount}" : n;
        itemIcon.sprite = item.Product.Item.Icon;
        if (item.SecondaryProduct != null)
        {
            StartCoroutine("SwitchProductsCo");
        }
    }
    IEnumerator SwitchProductsCo()
    {
        while (true)
        {
            SetItemProduct(true);

            yield return new WaitForSeconds(2.0f);

            SetItemProduct(false);

            yield return new WaitForSeconds(2.0f);
        }



    }

    private void SetItemProduct(bool isPrimary)
    {
        showingPrimaryItem = isPrimary;
        itemIcon.sprite = showingPrimaryItem ? item.Product.Item.Icon : item.SecondaryProduct.Icon;
        ShowDots();
        if (showingInfo)
            ShowInformation();
    }

    void ShowDots()
    {
        itemDotA.sprite = showingPrimaryItem ? itemDotWhite : itemDotGray;
        itemDotB.sprite = showingPrimaryItem ? itemDotGray : itemDotWhite;
        itemDotA.gameObject.SetActive(true);
        itemDotB.gameObject.SetActive(true);
    }

    void HideDots()
    {
        itemDotA.gameObject.SetActive(false);
        itemDotB.gameObject.SetActive(false);
    }
    public void ShowInformation()
    {
        if (item == null)
            return;
        showingInfo = true;
        ItemInformationDisplayUI.instance.ShowItemName(showingPrimaryItem ? item.Product.Item : item.SecondaryProduct, this.GetComponent<RectTransform>());
    }
    public void HideInformation()
    {
        showingInfo = false;
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
