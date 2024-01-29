using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_Action_CraftItem : SAP_Action
    {

        
        

        bool canCraft;

        int currentTimer;
        public int craftingTime;

        public QI_ItemDatabase itemsToCraft;
        public int craftingTypesAmount;
        public int individualItemsAmount;
        public NPC_CraftingStation craftingStation;

        

        public override void StartPerformAction(SAP_Scheduler_NPC agent)
        {
            if (target == null)
                return;


            // Set the destination (currentAction.target) and direction here using currentAction.walker
            if (currentNode == null && path.Count <= 0)
            {
                path.Clear();
                currentPathIndex = 0;
                if (agent.lastValidNode != null)
                    currentNode = agent.lastValidNode;
                else
                    currentNode = NavigationNodesManager.instance.GetClosestNavigationNode(transform.position, agent.currentNavigationNodeType, agent.pathType);
                path = currentNode.FindPath(target);
                agent.walker.currentDestination = path[currentPathIndex].transform.position;
            }

            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);
        }

        public override void PerformAction(SAP_Scheduler_NPC agent)
        {

            if (canCraft)
            {
                
                if (RealTimeDayNightCycle.instance.currentTimeRaw == currentTimer)
                {
                    CraftItems(agent);
                    craftingStation.SetCraftingOff();
                    agent.SetBeliefState("ItemCrafted", true);
                    agent.SetBeliefState("CanCraft", false);
                    agent.SetBeliefState("CanSell", true);
                    agent.currentGoalComplete = true;
                }
                return;
            }
            

            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);


            if (agent.offScreen || agent.sleep.isSleeping)
            {
                agent.HandleOffScreen(this);
                return;
            }

            if (agent.walker.isStuck || agent.isDeviating)
            {
                if (!agent.walker.jumpAhead)
                {
                    agent.Deviate();
                    return;
                }
            }



            if (path.Count > 0)
            {
                agent.walker.currentDestination = path[currentPathIndex].transform.position;
            }

            //if (!walker.onSlope)
            agent.walker.SetDirection();
            if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.01f)
            {

                if (currentPathIndex < path.Count - 1)
                {
                    currentPathIndex++;
                    currentNode = path[currentPathIndex];
                    agent.walker.currentDestination = path[currentPathIndex].transform.position;
                }
                else if (currentPathIndex >= path.Count - 1)
                {
                    agent.lastValidNode = currentNode;

                    ReachFinalDestination(agent);
                }


            }

            agent.walker.SetLastPosition();



        }
        public override void EndPerformAction(SAP_Scheduler_NPC agent)
        {
            agent.animator.SetBool(agent.isCrafting_hash, false);
            agent.offScreenPosMoved = true;
            currentTimer = -1;
            canCraft = false;
            currentNode = null;
            path.Clear();
        }

        void CraftItems(SAP_Scheduler_NPC agent)
        {

            List<QI_ItemData> items = new List<QI_ItemData>();

            while (items.Count < craftingTypesAmount)
            {
                var newitem = GetRandomItem();
                if (!items.Contains(newitem))
                    items.Add(newitem);
            }

            for (int i = 0; i < items.Count; i++)
            {
                agent.agentInventory.AddItem(items[i], individualItemsAmount, false);
            }

        }

        QI_ItemData GetRandomItem()
        {
            return itemsToCraft.GetRandomWeightedItem();

        }



        

        public override void ReachFinalDestination(SAP_Scheduler_NPC agent)
        {
            agent.animator.SetBool(agent.isCrafting_hash, true);
            agent.offScreenPosMoved = true;
            craftingStation.SetCraftingOn();
            currentTimer = (RealTimeDayNightCycle.instance.currentTimeRaw + craftingTime) % 1440;
            agent.isDeviating = false;
            canCraft = true;
            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.walker.currentDirection = Vector2.zero;
        }
    }
}

