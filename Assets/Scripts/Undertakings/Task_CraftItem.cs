using UnityEngine;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;

namespace Klaxon.UndertakingSystem
{
    [CreateAssetMenu(menuName = "Undertakings/Task/Craft Item Task")]
    public class Task_CraftItem : UndertakingTaskObject
    {
        public QI_ItemData itemToCraft;

        public override void ActivateTask(UndertakingObject undertakingObject)
        {
            base.ActivateTask(undertakingObject);
            GameEventManager.onItemCrafted.AddListener(CheckItemCrafted);
        }

        public override void DeactivateTask()
        {
            GameEventManager.onItemCrafted.RemoveListener(CheckItemCrafted);
        }

        public void CheckItemCrafted(QI_ItemData item)
        {
            if (item == itemToCraft)
                CompleteTask();
        }
    }
}
