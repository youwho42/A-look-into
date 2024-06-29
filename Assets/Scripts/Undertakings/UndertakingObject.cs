using Klaxon.SAP;
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
            if (GumptionReward != null)
                player.statHandler.ChangeStat(GumptionReward);
            if (AgencyReward != null)
                player.statHandler.ChangeStat(AgencyReward);
            if (SparksReward > 0)
                player.purse.AddToPurse(SparksReward);
            if (ItemReward != null)
                player.playerInventory.AddItem(ItemReward, ItemQuantity, false);
            if(RecipeReward != null)
                player.playerRecipeDatabase.CraftingRecipes.Add(RecipeReward);
            
            CurrentState = UndertakingState.Complete;
            SAP_WorldBeliefStates.instance.SetWorldState(Name, true);
            Notifications.instance.SetNewNotification($"{localizedName.GetLocalizedString()}", null, 0, NotificationsType.UndertakingComplete);
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

