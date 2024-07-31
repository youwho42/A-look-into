using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.Interactable;


namespace Klaxon.SAP
{
    public class SAP_Action_SellItem : SAP_Action
    {
        


        bool canSell;
        public InteractableChair chair;
        public List<InteractableMerchantTable> merchantTables = new List<InteractableMerchantTable>();


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
            

            if (canSell)
            {
                agent.animator.SetBool(agent.isSitting_hash, true);
                CheckTableInventory(agent);
                if(agent.HasBelief("CanSell", false))
                {
                    CloseShop(agent);
                    agent.currentGoalComplete = true;
                }
                    
                return;
            }


            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);


            if (agent.offScreen || agent.sleep.isSleeping)
            {
                agent.HandleOffScreenNodes(this);
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
            currentNode = null;
            path.Clear();
            canSell = false;
            agent.offScreenPosMoved = true;
            
            
            StartCoroutine(PlaceNPC(agent, target.transform.position));
            
            agent.animator.SetBool(agent.isSitting_hash, false);
            chair.canInteract = true;
        }

        public override void ReachFinalDestination(SAP_Scheduler_NPC agent)
        {
            agent.lastValidNode = currentNode;
            agent.offScreenPosMoved = true;
            agent.isDeviating = false;
            canSell = true;
            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.walker.currentDirection = Vector2.zero;
            OpenShop(agent);
            
            if (agent.walker.facingRight && !chair.facingRight || !agent.walker.facingRight && chair.facingRight)
                agent.walker.Flip();
            StartCoroutine(PlaceNPC(agent, chair.transform.position));
            agent.animator.SetBool(agent.isSitting_hash, true);
            chair.canInteract = false;
        }

        void OpenShop(SAP_Scheduler_NPC agent)
        {
            List<QI_ItemData> allItems = new List<QI_ItemData>();
            List<int> itemAmount = new List<int>();
            for (int i = 0; i < agent.agentInventory.Stacks.Count; i++)
            {
                if (agent.agentInventory.Stacks[i].Item != null)
                {
                    allItems.Add(agent.agentInventory.Stacks[i].Item);
                    itemAmount.Add(agent.agentInventory.Stacks[i].Amount);
                }
            }
            
            for (int i = 0; i < allItems.Count; i++)
            {
                
                merchantTables[i].SetUpTable(allItems[i], itemAmount[i], agent);
            }
            
        }

        void CloseShop(SAP_Scheduler_NPC agent)
        {
            for (int i = 0; i < merchantTables.Count; i++)
            {
                merchantTables[i].ClearTable();
            }
            agent.SetBeliefState("ItemCrafted", false);
        }

        void CheckTableInventory(SAP_Scheduler_NPC agent)
        {
            bool canSell = false;
            for (int i = 0; i < merchantTables.Count; i++)
            {
                if(merchantTables[i].amount > 0)
                    canSell = true;
            }
            if (!canSell)
                agent.SetBeliefState("CanSell", false);
        }

        IEnumerator PlaceNPC(SAP_Scheduler_NPC agent, Vector3 position)
        {
            float timer = 0;
            float maxTime = 0.45f;
            while (timer < maxTime)
            {
                Vector3 pos = Vector3.Lerp(agent.transform.position, position, timer / maxTime);
                agent.transform.position = pos;
                timer += Time.deltaTime;
                yield return null;
            }

        }
    }

}
