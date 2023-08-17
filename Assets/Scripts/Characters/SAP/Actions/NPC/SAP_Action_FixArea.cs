using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_Action_FixArea : SAP_Action
    {
        public FixVillageDesk villageDesk;
        FixVillageArea currentArea;
        bool canFix;

       
        NavigationNode GetTarget()
        {
            NavigationNode node = null;
            foreach (var area in villageDesk.fixableAreas)
            {
                if (area.isFixing)
                {
                    node = area.fixingNode;
                    currentArea = area;
                    break;
                }
                    
            }
            return node;
        }

        void AddTick(int tick)
        {
            if (canFix)
                currentArea.fixTimer++;
        }

        public override void StartPerformAction(SAP_Scheduler_NPC agent)
        {
            GameEventManager.onTimeTickEvent.AddListener(AddTick);
            if (target == null)
                target = GetTarget();
            
                


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

            if (canFix)
            {
                if(SAP_WorldBeliefStates.instance.HasWorldState("Work", false))
                {
                    agent.currentGoalComplete = true;
                    return;
                }

                if (currentArea.fixTimer >= currentArea.ticksToFix)
                {
                    SAP_WorldBeliefStates.instance.SetWorldState(currentArea.SAP_CompletedCondition.Condition, currentArea.SAP_CompletedCondition.State);
                    SAP_WorldBeliefStates.instance.SetWorldState("FixingAreaAvailable", false);
                    
                    currentArea.activeArea.SetActive(true);
                    currentArea.inactiveArea.SetActive(false);
                    currentArea.isActive = true;
                    currentArea.isFixing = false;
                    currentArea.undertakingObject.TryCompleteTask(currentArea.taskObject);
                    SAP_WorldBeliefStates.instance.SetWorldState(currentArea.SAP_CompletedCondition.Condition, currentArea.SAP_CompletedCondition.State);
                    foreach (var node in currentArea.navigationNodes)
                    {
                        node.active = true;
                    }
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
            if (agent.walker.CheckDistanceToDestination() <= 0.02f)
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
            GameEventManager.onTimeTickEvent.RemoveListener(AddTick);
            currentArea.fixingSounds.StopSoundsNoTimer();
            currentArea.fixingEffect.Stop();
            target = null;
            canFix = false;
            currentArea = null;

            agent.offScreenPosMoved = true;
            agent.lastValidNode = currentNode;
            currentNode = null;
            path.Clear();
        }

        public override void ReachFinalDestination(SAP_Scheduler_NPC agent)
        {
            agent.offScreenPosMoved = true;
            agent.isDeviating = false;
            canFix = true;
            currentArea.fixingSounds.StartSoundsNoTimer();
            currentArea.fixingEffect.Play();
            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.walker.currentDir = Vector2.zero;
        }
    }

}