using Klaxon.StatSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.UndertakingSystem
{
    [CreateAssetMenu(menuName = "Undertakings/Task")]
    public class UndertakingTaskObject : ScriptableObject
    {
        public string Name;
        public bool IsComplete;


        public int SparksReward;
        public StatChanger GumptionReward;
        public StatChanger AgencyReward;

        public QI_ItemData ItemReward;
        public int ItemQuantity;
        public QI_CraftingRecipe RecipeReward;

        public string mapName;
        public void CompleteTask()
        {
            IsComplete = true;
            RewardTask();
        }

        public void ResetTask()
        {
            IsComplete = false;
        }

        public void RewardTask()
        {

            var player = PlayerInformation.instance;
            if (GumptionReward != null)
                player.statHandler.ChangeStat(GumptionReward);
            if (AgencyReward != null)
                player.statHandler.ChangeStat(AgencyReward);
            if (SparksReward > 0)
                player.purse.AddToPurse(SparksReward);
            if (ItemReward != null)
                player.playerInventory.AddItem(ItemReward, ItemQuantity, false);
            if (RecipeReward != null)
                player.playerRecipeDatabase.CraftingRecipes.Add(RecipeReward);
            if (mapName != "")
                PlayerMapsManager.instance.ActivateMapArea(mapName);
        }
    }
}

