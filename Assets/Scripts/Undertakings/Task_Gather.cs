using UnityEngine;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using UnityEngine.Localization.Settings;

namespace Klaxon.UndertakingSystem
{
    [CreateAssetMenu(menuName = "Undertakings/Task/Gather Task")]
    public class Task_Gather : UndertakingTaskObject
    {

        public QI_ItemData itemData;
        public int amount;
        
        public override void ActivateTask(UndertakingObject undertaking)
        {
            base.ActivateTask(undertaking);
            
            GameEventManager.onInventoryUpdateEvent.AddListener(CheckGatherAmount);
        }

        public override void DeactivateTask()
        {
            GameEventManager.onInventoryUpdateEvent.RemoveListener(CheckGatherAmount);
        }

        
        void CheckGatherAmount()
        {
            if (player.playerInventory.HasItem(itemData, amount))
                CompleteTask();
        }
    }
}