using Klaxon.QuestSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace Klaxon.UndertakingSystem
{
    public enum UndertakingState
    {
        Inactive,
        Active,
        Complete
    }

    [CreateAssetMenu(menuName = "Undertakings/Undertaking")]
    public class UndertakingObject : ScriptableObject
    {
        public string Name;
        [TextArea]
        public string Description;
        [TextArea]
        public string CompletedDescription;
        public List<UndertakingTaskObject> Tasks;
        public UndertakingState CurrentState;

        public int SparksReward;
        public int GumptionReward;
        public int AgencyReward;
        public QI_ItemData ItemReward;
        public int ItemQuantity;
        public QI_CraftingRecipe RecipeReward;

        public void ActivateUndertaking()
        {
            if (CurrentState == UndertakingState.Inactive)
            {
                CurrentState = UndertakingState.Active;
                PlayerInformation.instance.playerUndertakings.AddUndertaking(this);
                //NotificationManager.instance.SetNewNotification($"{Name} undertaking started", NotificationManager.NotificationType.Undertaking);
                NotificationCenter.instance.SetUndertakingText(this);
                GameEventManager.onUndertakingsUpdateEvent.Invoke();
            }
        }


        public void TryCompleteTask(UndertakingTaskObject taskObject)
        {
            if (CurrentState != UndertakingState.Active)
                return;
            foreach (var task in Tasks)
            {
                if (task == taskObject)
                {
                    task.CompleteTask();
                    TryCompleteQuest();
                    GameEventManager.onUndertakingsUpdateEvent.Invoke();
                }
            }
        }


        public void TryCompleteQuest()
        {
            bool complete = true;
            foreach (var task in Tasks)
            {
                if (task.IsComplete)
                    continue;
                complete = false;
            }
            if (complete)
            {
                CompleteUndertaking();
            }
                
            
        }
        public void CompleteUndertaking()
        {
            

            var player = PlayerInformation.instance;
            if (GumptionReward > 0)
                player.playerStats.AddToStat("Gumption", GumptionReward);
            if (AgencyReward > 0)
                player.playerStats.AddToStat("Agency", GumptionReward);
            if (SparksReward > 0)
                player.purse.AddToPurse(SparksReward);
            if (ItemReward != null)
                player.playerInventory.AddItem(ItemReward, ItemQuantity, false);
            if(RecipeReward != null)
                player.playerRecipeDatabase.CraftingRecipes.Add(RecipeReward);

            CurrentState = UndertakingState.Complete;
            //NotificationManager.instance.SetNewNotification($"{Name} undertaking completed", NotificationManager.NotificationType.Undertaking);
            NotificationCenter.instance.SetUndertakingText(this);
            GameEventManager.onUndertakingsUpdateEvent.Invoke();
        }

        

        public void ResetUndertaking()
        {
            foreach (var task in Tasks)
            {
                task.ResetTask();
            }
            CurrentState = UndertakingState.Inactive;
            GameEventManager.onUndertakingsUpdateEvent.Invoke();
        }
    }
}

