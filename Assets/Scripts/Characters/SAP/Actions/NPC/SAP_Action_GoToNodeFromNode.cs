using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_Action_GoToNodeFromNode : SAP_Action
    {
        bool destinationReached;
        public override void StartPerformAction(SAP_Scheduler_NPC agent)
        {
            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);
            if (target != null)
            {

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

            }
        }

        public override void PerformAction(SAP_Scheduler_NPC agent)
        {

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
                    agent.animator.SetFloat(agent.velocityX_hash, 0);
                    agent.walker.currentDirection = Vector2.zero;
                }
            }

            agent.walker.SetLastPosition();

        }
        public override void EndPerformAction(SAP_Scheduler_NPC agent)
        {
            agent.offScreenPosMoved = true;
            agent.lastValidNode = currentNode;
            currentNode = null;
            path.Clear();
            
        }



        public override void ReachFinalDestination(SAP_Scheduler_NPC agent)
        {
            agent.offScreenPosMoved = true;
            agent.isDeviating = false;
            

            agent.animator.SetFloat(agent.velocityX_hash, 0);
            
            agent.walker.currentDirection = Vector2.zero;
            if (setConditionOnComplete)
                agent.SetBeliefState(conditionToSet.Condition, conditionToSet.State);
                
            agent.currentGoalComplete = true;
        }
    }
    
}