using Klaxon.StatSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace Klaxon.UndertakingSystem
{
    [CreateAssetMenu(menuName = "Undertakings/Task/Basic Task")]
    public class UndertakingTaskObject : ScriptableObject
    {
        public string Name;

        public LocalizedString localizedDescription;

        public bool IsComplete;


        public int SparksReward;
        public StatChanger GumptionReward;
        public StatChanger AgencyReward;

        public QI_ItemData ItemReward;
        public int ItemQuantity;
        public QI_CraftingRecipe RecipeReward;

        public string mapName;
        UndertakingObject undertaking;
        [HideInInspector]
        public PlayerInformation player;

        public virtual void ActivateTask(UndertakingObject undertakingObject)
        {
            player = PlayerInformation.instance;
            undertaking = undertakingObject;
        }

        public virtual void DeactivateTask()
        {

        }


        public void CompleteTask()
        {
            if (undertaking == null)
                return;
            IsComplete = true;
            RewardTask();
            DeactivateTask();
            undertaking.TryCompleteQuest();
            GameEventManager.onUndertakingsUpdateEvent.Invoke();

            int r = Random.Range(1, 3);
            AudioManager.instance.PlaySound($"CompleteTask{r}");
        }

        public void SetTaskFromSave(UndertakingObject undertaking, bool state)
        {
            IsComplete = state;
            if (!state)
                ActivateTask(undertaking);
        }

        public void ResetTask()
        {
            IsComplete = false;
            DeactivateTask();
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

