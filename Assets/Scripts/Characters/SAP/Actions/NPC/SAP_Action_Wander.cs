using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_Action_Wander : SAP_Action
    {
        
        public float wanderDistance = 1f;
        public override void StartPerformAction(SAP_Scheduler_NPC agent)
        {
            agent.offScreenPosMoved = true;
            if (target == null)
                target = NavigationNodesManager.instance.GetRandomNode(NavigationNodeType.Outside, agent.pathType, transform.position, 3f);


            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);

            // Set the destination (currentAction.target) and direction here using currentAction.walker
            if (currentNode == null)
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

        public override void PerformAction(SAP_Scheduler_NPC agent)
        {
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
                    if (finalDestination)
                    {
                        ReachFinalDestination(agent);
                        return;
                    }

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
                if (!finalDestination)
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
                        
                        finalDestination = true;
                        agent.walker.SetRandomDestination(wanderDistance);
                    }
                }
                else
                {
                    ReachFinalDestination(agent);
                }

            }

            agent.walker.SetLastPosition();
        }
        public override void EndPerformAction(SAP_Scheduler_NPC agent)
        {
            agent.offScreenPosMoved = true;
            agent.lastValidNode = currentNode;
            ReachFinalDestination(agent);
            currentNode = null;
            path.Clear();
            target = null;
        }

        
        public override void ReachFinalDestination(SAP_Scheduler_NPC agent)
        {
            agent.offScreenPosMoved = true;
            agent.isDeviating = false;
            finalDestination = false;
            agent.currentGoalComplete = true;
            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.walker.currentDir = Vector2.zero;
        }

    }
}
