using Klaxon.Interactable;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Action_RunShop : GOAD_Action
	{
        public InteractableChair workSeat;
        NavigationNode target;
        bool closeShop;
        bool sitting;
        bool gettingUp;
        bool shopOpen;
        
        public List<InteractableMerchantTable> merchantTables = new List<InteractableMerchantTable>();

        public bool useDatabaseForTables;
        [ConditionalHide("useDatabaseForTables", true)]
        public QI_ItemDatabase merchantDatabase;
        [Min(1)]
        public int maxItems = 1;
        GOAD_Scheduler_NPC currentAgent;
        
        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            currentAgent = agent;
            sitting = true;
            closeShop = false;
            gettingUp = false;
            base.StartAction(agent);
            // Setting up the seat for when we leave
            if (target == null)
            {
                target = workSeat.findNode;
                agent.currentNode = workSeat.sitNode;
                workSeat.canInteract = false;
                agent.nodePath.Clear();
                agent.currentPathIndex = 0;
                agent.nodePath = agent.currentNode.FindPath(target);
                //agent.walker.currentDestination = agent.nodePath[agent.currentPathIndex].transform.position;
            }
        }

        public override void PerformAction(GOAD_Scheduler_NPC agent)
        {
            base.PerformAction(agent);
            
            closeShop = agent.HasBelief("CanWork", false);

            if (closeShop)
            {
                if(shopOpen)
                {
                    CloseShop(agent);
                }
                GetUpAndLeave(agent);
                return;
            }

            if(!shopOpen)
            {
                CheckTables();
                shopOpen= true;
                InvokeRepeating("CheckTables", 30, 30);
            }
            

        }

        void CheckTables()
        {
            //if(!useDatabaseForTables)
            //    CheckTableInventory(currentAgent);
            //else
                CheckTableDatabase(currentAgent);
        }

        private void GetUpAndLeave(GOAD_Scheduler_NPC agent)
        {
            if (sitting)
            {
                if (!gettingUp)
                {
                    agent.animator.SetBool(agent.isSitting_hash, false);
                    StartCoroutine(PlaceNPC(agent, workSeat.sitNode.transform.position, true));
                    
                }
                return;
            }

            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);

            if (agent.nodePath.Count > 0)
            {
                agent.walker.currentDestination = agent.nodePath[agent.currentPathIndex].transform.position;
            }
            agent.walker.SetDirection();

            if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.01f)
            {
                if (agent.currentPathIndex < agent.nodePath.Count - 1)
                {
                    agent.currentPathIndex++;
                    agent.currentNode = agent.nodePath[agent.currentPathIndex];
                    agent.walker.currentDestination = agent.nodePath[agent.currentPathIndex].transform.position;
                }
                else if (agent.currentPathIndex >= agent.nodePath.Count - 1)
                {
                    agent.lastValidNode = agent.currentNode;
                    success = true;
                    agent.SetActionComplete(true);

                }
            }

            agent.walker.SetLastPosition();

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
            
        }

        IEnumerator PlaceNPC(GOAD_Scheduler_NPC agent, Vector3 position, bool standingUp)
        {
            float timer = 0;
            float maxTime = 0.45f;
            gettingUp = true;
            while (timer < maxTime)
            {
                Vector3 pos = Vector3.Lerp(agent.transform.position, position, timer / maxTime);
                agent.transform.position = pos;
                timer += Time.deltaTime;
                yield return null;
            }
            sitting = false;
            gettingUp = false;
            yield return null;
        }

        
        void CloseShop(GOAD_Scheduler_NPC agent)
        {
            for (int i = 0; i < merchantTables.Count; i++)
            {
                merchantTables[i].ClearTable();
            }
            
            
        }

        //void CheckTableInventory(GOAD_Scheduler_NPC agent)
        //{
            

        //    for (int i = 0; i < merchantTables.Count; i++)
        //    {
        //        if (merchantTables[i].amount > 0)
        //            continue;

        //        if (agent.agentInventory.Stacks.Count <= 0)
        //            break;

        //        if (agent.agentInventory.Stacks[0].Item != null)
        //        {
                    
        //            var item = agent.agentInventory.Stacks[0].Item;
        //            var amount = agent.agentInventory.Stacks[0].Amount;
        //            merchantTables[i].SetUpTable(item, amount, agent);
        //            agent.agentInventory.RemoveItem(item, amount);
        //        }


        //    }
            
        //}


        void CheckTableDatabase(GOAD_Scheduler_NPC agent)
        {
            List<QI_ItemData> currentItems = new List<QI_ItemData>();
            foreach (var table in merchantTables)
            {
                currentItems.Add(table.item);
            }
            for (int i = 0; i < merchantTables.Count; i++)
            {
                if (merchantTables[i].amount > 0)
                    continue;
                QI_ItemData newitem = null;
                do
                {
                    newitem = merchantDatabase.GetRandomWeightedItem();
                } while (currentItems.Contains(newitem));

                int r = Random.Range(1, maxItems);
                merchantTables[i].SetUpTable(newitem, r, agent);
                
            }
        }



    }
}
