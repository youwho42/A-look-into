using Klaxon.GOAD;
using Klaxon.StatSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Localization;

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
        public LocalizedString localizedName;
        public LocalizedString localizedDescription;
        public LocalizedString localizedCompleteDescription;
        
        public List<UndertakingTaskObject> Tasks;
        public UndertakingState CurrentState;

        public GOAD_ScriptableCondition QuestCompleteCondition;
        [Header("Rewards")]
        public int SparksReward;
        public StatChanger GumptionReward;
        public StatChanger AgencyReward;

        public QI_ItemData ItemReward;
        public int ItemQuantity;
        public QI_CraftingRecipe RecipeReward;

        public void ActivateUndertaking()
        {
            if (CurrentState == UndertakingState.Inactive)
            {
                
                CurrentState = UndertakingState.Active;
                PlayerInformation.instance.playerUndertakings.AddUndertaking(this);
                Notifications.instance.SetNewLargeNotification(this, null, null, NotificationsType.UndertakingStart);
                foreach (var task in Tasks)
                {
                    task.ActivateTask();
                }
                TryCompleteQuest();
            }
        }


        public void TryCompleteTask(UndertakingTaskObject taskObject)
        {
            if (CurrentState == UndertakingState.Complete)
                return;
            foreach (var task in Tasks)
            {
                if (task == taskObject)
                {
                    task.CompleteTask();
                    TryCompleteQuest();
                    
                }
            }
        }


        public void TryCompleteQuest()
        {
            if (CurrentState != UndertakingState.Active)
                return;

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
                GameEventManager.onUndertakingsUpdateEvent.Invoke();
            }
                
            
        }
        public void CompleteUndertaking()
        {

            bool addedToLostAndFound = false; ;
            var player = PlayerInformation.instance;
            if (GumptionReward != null)
                player.statHandler.ChangeStat(GumptionReward);
            if (AgencyReward != null)
                player.statHandler.ChangeStat(AgencyReward);
            if (SparksReward > 0)
                player.purse.AddToPurse(SparksReward);
            if (ItemReward != null) 
            {
                if (!player.playerInventory.AddItem(ItemReward, ItemQuantity, false))
                {
                    LostAndFoundManager.instance.AddToLostAndFound(ItemReward, ItemQuantity);
                    addedToLostAndFound = true; 
                }
                    
            }
                
            if(RecipeReward != null)
                player.playerRecipeDatabase.CraftingRecipes.Add(RecipeReward);
            
            CurrentState = UndertakingState.Complete;
            if(QuestCompleteCondition != null)
                GOAD_WorldBeliefStates.instance.SetWorldState(QuestCompleteCondition.Condition, QuestCompleteCondition.State);

            Notifications.instance.SetNewLargeNotification(this, null, null, NotificationsType.UndertakingComplete);
            if(addedToLostAndFound)
                Notifications.instance.SetNewNotification($"{localizedName.GetLocalizedString()} sent to lost and found", null, 0, NotificationsType.Warning);
            
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

