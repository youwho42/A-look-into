using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Action_Craft : GOAD_Action
	{

        //public QI_ItemDatabase itemsToCraft;
        //public int craftingTypesAmount;
        //public int individualItemsAmount;
        public NPC_CraftingStation craftingStation;


        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            base.StartAction(agent);
            agent.animator.SetBool(agent.isCrafting_hash, true);
            agent.offScreenPosMoved = true;
            craftingStation.SetCraftingOn();
            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.walker.currentDirection = Vector2.zero;
        }

        public override void PerformAction(GOAD_Scheduler_NPC agent)
        {
            base.PerformAction(agent);
            if(agent.HasBelief("CanCraft", false))
            {
                //CraftItems(agent);
                craftingStation.SetCraftingOff();
                success = true;
                agent.SetActionComplete(true);
                return;
            }
        }

        public override void SucceedAction(GOAD_Scheduler_NPC agent)
        {
            base.SucceedAction(agent);
        }

        public override void FailAction(GOAD_Scheduler_NPC agent)
        {
            base.FailAction(agent);
        }

        public override void EndAction(GOAD_Scheduler_NPC agent)
        {
            base.EndAction(agent);
            agent.animator.SetBool(agent.isCrafting_hash, false);
            
        }

        //void CraftItems(GOAD_Scheduler_NPC agent)
        //{

        //    List<QI_ItemData> items = new List<QI_ItemData>();

        //    while (items.Count < craftingTypesAmount)
        //    {
        //        var newitem = itemsToCraft.GetRandomWeightedItem();
        //        if (!items.Contains(newitem))
        //            items.Add(newitem);
        //    }

        //    for (int i = 0; i < items.Count; i++)
        //    {
        //        agent.agentInventory.AddItem(items[i], individualItemsAmount, false);
        //    }

        //}

    }
}
