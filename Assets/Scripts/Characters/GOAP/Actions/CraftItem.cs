using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAP
{
    public class CraftItem : GOAP_Action
    {
        public QI_ItemDatabase itemsToCraft;
        public int amountToCraft;
        public int individualItemsAmount;
        public NPC_CraftingStation craftingStation;
        public override bool PrePerform(GOAP_Agent agent)
        {
            agent.animator.SetBool(agent.isGrounded_hash, walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, walker.isGrounded ? 0 : walker.displacedPosition.y);

            agent.animator.SetFloat(agent.velocityX_hash, 0);
            walker.currentDir = Vector2.zero;
            craftingStation.SetCraftingOn();
            return true;
        }
        public override void Perform(GOAP_Agent agent)
        {

            agent.destinationReached = true;
        }
        public override void PrePostPerform(GOAP_Agent agent)
        {
            
        }
        public override bool PostPerform(GOAP_Agent agent)
        {
            CraftItems(agent);
            craftingStation.SetCraftingOff();
            return true;
        }

        void CraftItems(GOAP_Agent agent)
        {

            List<QI_ItemData> items = new List<QI_ItemData>();
            while (items.Count < individualItemsAmount)
            {
                var newitem = GetRandomItem();
                if(!items.Contains(newitem))
                    items.Add(newitem);
            }

            for (int i = 0; i < items.Count; i++)
            {
                agent.agentInventory.AddItem(items[i], amountToCraft, false);
            }
            
        }

        QI_ItemData GetRandomItem()
        {
            return itemsToCraft.GetRandomWeightedItem();
            
        }
    } 
}
