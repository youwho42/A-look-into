using QuantumTek.QuantumInventory;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;

public class ResearchStationResearchSlot : MonoBehaviour
{
    QI_ItemData item;
    public Image icon;
    public TextMeshProUGUI recipesText;
    public Button researchButton;
    ResearchStationDisplayUI researchDisplayUI;
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
                var n = item.ResearchRecipes[i].recipe.Product.Item.localizedName.GetLocalizedString();
                var c = inventoryAmount >= item.ResearchRecipes[i].RecipeRevealAmount ? "" : "<color=#FF0000>";
                textToAdd += $"{c}{inventoryAmount}/{item.ResearchRecipes[i].RecipeRevealAmount} - {n}\n";
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
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        recipesText.text = LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", "Research Recipe Description");
    }

    public void LearnRecipes()
    {
        // loop through all  recipeReveals 
        for (int i = 0; i < item.ResearchRecipes.Count; i++)
        {
            if (PlayerCrafting.instance.craftingRecipeDatabase.CraftingRecipes.Contains(item.ResearchRecipes[i].recipe))
                continue;
            if (CheckForInventoryQuantity(i))
            {
                PlayerInformation.instance.statHandler.ChangeStat(item.ResearchRecipes[i].agencyStatChanger);
                PlayerCrafting.instance.AddCraftingRecipe(item.ResearchRecipes[i].recipe);
                Notifications.instance.SetNewNotification($"{item.ResearchRecipes[i].recipe.Product.Item.localizedName.GetLocalizedString()}", null, 0, NotificationsType.Compendium);
            }
        }
        ResearchStationDisplayUI.instance.UpdateResearchDisplay();
        ClearSlot();
        
    }

    public bool CheckForInventoryQuantity(int index)
    {
        
            int t = PlayerInformation.instance.playerInventory.GetStock(item.Name);
            if (t < item.ResearchRecipes[index].RecipeRevealAmount)
            {
                //string plural = item.ResearchRecipes[index].RecipeRevealAmount - t == 1 ? "" : "'s";
                Notifications.instance.SetNewNotification($"{item.ResearchRecipes[index].RecipeRevealAmount - t} {item.localizedName.GetLocalizedString()}", null, 0, NotificationsType.Warning);

                //NotificationManager.instance.SetNewNotification($"{item.ResearchRecipes[index].RecipeRevealAmount - t} {item.Name}{plural} missing", NotificationManager.NotificationType.Warning);
                return false;
            }

        
        return true;
    }

}
