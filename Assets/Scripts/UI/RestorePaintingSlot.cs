using QuantumTek.QuantumInventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RestorePaintingSlot : MonoBehaviour
{
    PaintingIngredient ingredient;
    public Image icon;
    public Button slotButton;
    public TextMeshProUGUI itemAmount;
    PlayerInformation player;
    public Image checkMark;
    public void AddItem(PaintingIngredient _ingredient)
    {
        ingredient = _ingredient;
        player = PlayerInformation.instance;
        
        SetButtonActive();

        icon.sprite = ingredient.item.Icon;
    }

    public void GiveIngredient()
    {
        if (ingredient.complete)
            return;
        if (ingredient.isPhysicalItem)
            GiveItem();
        else
            GiveKnowledge();
    }
    void GiveItem()
    {
        player.playerInventory.RemoveItem(ingredient.item, ingredient.amount);
        ingredient.complete = true;
        SetButtonActive();
    }

    void GiveKnowledge()
    {
        ingredient.complete = true;
        SetButtonActive();
    }

    void SetButtonActive()
    {
        itemAmount.gameObject.SetActive(false);
        checkMark.gameObject.SetActive(ingredient.complete);

        if (ingredient.isPhysicalItem)
        {
            itemAmount.gameObject.SetActive(true);
            itemAmount.text = $"{player.playerInventory.GetStock(ingredient.item.Name)}/{ingredient.amount}";
            slotButton.interactable = HasItems();
        }
        else
        {
            slotButton.interactable = HasKnowledge();
        }
    }

    bool HasItems()
    {
        if (ingredient.complete)
            return true;
        return player.playerInventory.HasItem(ingredient.item, ingredient.amount);
         
    }

    bool HasKnowledge()
    {
        if(ingredient.item.Type == ItemType.Animal)
        {
            if (player.animalCompendiumInformation.animalNames.Contains(ingredient.item.Name))
            {
                int index = player.animalCompendiumInformation.animalNames.IndexOf(ingredient.item.Name);
                return player.animalCompendiumInformation.viewedComplete[index];
            }
        }
        if (ingredient.item.Type == ItemType.Encounter)
        {
            if (player.playerEncountersCompendiumDatabase.Items.Contains(ingredient.item))
            {
                return true;
            }
        }

        return false;
        
    }
    public void ShowInformation()
    {
        if (ingredient == null)
            return;
        ItemInformationDisplayUI.instance.ShowItemName(ingredient.item, this.GetComponent<RectTransform>());
    }
    public void HideInformation()
    {
        ItemInformationDisplayUI.instance.HideItemName();
    }


}
