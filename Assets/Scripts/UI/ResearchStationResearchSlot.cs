using QuantumTek.QuantumInventory;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
using System.Collections;

public class ResearchStationResearchSlot : MonoBehaviour
{
    QI_ItemData item;
    public Image icon;
    
    public TextMeshProUGUI recipesText;
    public Button researchButton;
    ResearchStationDisplayUI researchDisplayUI;
    public Image scanner;
    public Image productIcon;
    public Color baseColor;
    public Color successColor;
    
    private void Start()
    {
        ClearSlot();
    }
    private void OnEnable()
    {
        ClearSlot();
        
    }
    void OnDisable()
    {
        ClearSlot();
    }
    public void AddItem(QI_ItemData newItem)
    {
        productIcon.sprite = null;
        researchButton.interactable = true;
        item = newItem;
        icon.sprite = item.Icon;
        icon.enabled = true;
        researchDisplayUI.currentResearchStation.HideResearchItem();
        researchDisplayUI.currentResearchStation.ShowResearchItem(item);
        AddRecipes();
        
    }

    void AddRecipes()
    {
        string textToAdd = "";
        for (int i = 0; i < item.ResearchRecipes.Count; i++)
        {
            if (!PlayerInformation.instance.playerRecipeDatabase.CraftingRecipes.Contains(item.ResearchRecipes[i].recipe))
            {
                int inventoryAmount = PlayerInformation.instance.playerInventory.GetStock(item.Name);
                string amt = item.ResearchRecipes[i].recipe.Product.Amount > 1 ? $"x{item.ResearchRecipes[i].recipe.Product.Amount}" : "";
                var n = item.ResearchRecipes[i].recipe.Product.Item.localizedName.GetLocalizedString();
                var c = inventoryAmount >= item.ResearchRecipes[i].RecipeRevealAmount ? "" : "<color=#FF0000>";
                textToAdd += $"{c}{inventoryAmount}/{item.ResearchRecipes[i].RecipeRevealAmount} - {n} {amt}\n";
            }
                
        }
        recipesText.text = textToAdd;
    }


    public void ClearSlot()
    {
        if(researchDisplayUI == null)
            researchDisplayUI = ResearchStationDisplayUI.instance;
        if(researchDisplayUI.currentResearchStation != null)
            researchDisplayUI.currentResearchStation.HideResearchItem();

        researchButton.interactable = false;
        foreach (var slot in researchDisplayUI.researchStationInventorySlots)
            slot.button.interactable = true;
        
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        recipesText.text = LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", "Research Recipe Description");
        productIcon.sprite = null;
        scanner.rectTransform.sizeDelta = new Vector2(scanner.rectTransform.sizeDelta.x, 0);
    }

    public void LearnRecipes()
    {
        StartCoroutine("LearnRecipesCo");
        
    }
    IEnumerator LearnRecipesCo()
    {
        foreach (var slot in researchDisplayUI.researchStationInventorySlots)
            slot.button.interactable = false;
        researchButton.interactable = false;

        var currentItem = item;
        // loop through all  recipeReveals 
        for (int i = 0; i < item.ResearchRecipes.Count; i++)
        {
            scanner.color = baseColor;
            if (PlayerCrafting.instance.craftingRecipeDatabase.CraftingRecipes.Contains(item.ResearchRecipes[i].recipe) || !CheckForInventoryQuantity(i))
                continue;

            productIcon.sprite = item.ResearchRecipes[i].recipe.Product.Item.Icon;
            Vector2 sd = scanner.rectTransform.sizeDelta;
            // Reset timers
            float time = 0;
            float scanTime = 4.0f;
            float height = 0;
            AudioManager.instance.PlaySound("ScanItem");
            // scan item
            while (time <= scanTime)
            {
                time += Time.deltaTime;
                height = Mathf.Lerp(0, 69, time / scanTime);
                sd.y = height;
                scanner.rectTransform.sizeDelta = sd;
                yield return null;
            }
            

            // give recipe
            PlayerInformation.instance.statHandler.ChangeStat(item.ResearchRecipes[i].agencyStatChanger);
            PlayerCrafting.instance.AddCraftingRecipe(item.ResearchRecipes[i].recipe);
            scanner.color = successColor;
            AudioManager.instance.PlaySound("ScanSuccess");
            

            // reset timers
            time = 0;
            float endTime = 0.3f;
            // end Scan
            while (time <= endTime)
            {
                time += Time.deltaTime;
                height = Mathf.Lerp(69, 0, time / endTime);
                sd.y = height;
                scanner.rectTransform.sizeDelta = sd;
                yield return null;
            }
            sd.y = 0;
            scanner.rectTransform.sizeDelta = sd;
            //reset recipeTexts
            AddRecipes();
            productIcon.sprite = null;
            yield return new WaitForSeconds(0.35f);
        }
        
        ResearchStationDisplayUI.instance.UpdateResearchDisplay();
        ClearSlot();
        
       
        yield return null;

    }

    public bool CheckForInventoryQuantity(int index)
    {
        int t = PlayerInformation.instance.playerInventory.GetStock(item.Name);
        //if (t < item.ResearchRecipes[index].RecipeRevealAmount)
        //{
        //    Notifications.instance.SetNewNotification($"{item.ResearchRecipes[index].RecipeRevealAmount - t} {item.localizedName.GetLocalizedString()}", null, 0, NotificationsType.Warning);
        //    AudioManager.instance.PlaySound("ScanFail");
        //    return false;
        //}

        return t > item.ResearchRecipes[index].RecipeRevealAmount;
        //return true;
    }

}
