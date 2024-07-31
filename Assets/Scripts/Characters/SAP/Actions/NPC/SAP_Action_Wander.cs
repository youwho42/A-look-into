using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_Action_Wander : SAP_Action
    {
        
        public float wanderDistance = 1f;
        SAP_Scheduler_NPC thisAgent;
        public override void StartPerformAction(SAP_Scheduler_NPC agent)
        {
            thisAgent = agent;
            agent.offScreenPosMoved = true;
            if (target == null)
                target = NavigationNodesManager.instance.GetRandomNode(NavigationNodeType.Outside, agent.pathType, transform.position, 3f);


            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);

            // Set the destination (currentAction.target) and direction here using currentAction.walker
            if (!agent.useAStar)
            {
                if (currentNode == null)
                {
                    path.Clear();
                    currentPathIndex = 0;
                    if (agent.lastValidNode != null)
                        currentNode = agent.lastValidNode;
                    else
                        currentNode = NavigationNodesManager.instance.GetClosestNavigationNode(transform.position, agent.currentNavigationNodeType, agent.pathType);

                    path = currentNode.FindPath(target);

                    //if (path == null)
                    //{
                    //    agent.currentGoalComplete = true;
                    //    return;
                    //}

                    agent.walker.currentDestination = path[currentPathIndex].transform.position;
                } 
            }
            else
            {
                currentPathIndex = 0;
                agent.aStarPath.Clear();
                if (agent.StartPositionValid())
                    agent.GetRandomTilePosition(wanderDistance);
                else
                {
                    Debug.Log("start position not valid from start");
                    agent.aStarPath.Add(agent.lastValidTileLocation);
                }

            }
        }

        public override void PerformAction(SAP_Scheduler_NPC agent)
        {
            if (agent.useAStar)
            {
                if (agent.gettingPath)
                    return;
                if (agent.aStarPath.Count <= 0)
                {
                    ReachFinalDestination(agent);
                    return;
                }
                if (currentPathIndex >= agent.aStarPath.Count)
                {
                    ReachFinalDestination(agent);
                    return;
                }


            }

            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);

            if (!agent.useAStar)
            {
                if (agent.offScreen || agent.sleep.isSleeping)
                {
                    agent.HandleOffScreenNodes(this);
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

                if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.01f)
                {
                    if (!finalDestination)
                    {
                        if (currentPathIndex < path.Count - 1)
                        {
                            currentPathIndex++;
                            //currentNode = path[currentPathIndex];
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
            }
            else
            {


                if (agent.walker.isStuck || agent.isDeviating)
                {
                    
                    agent.Deviate(this);
                    return;
                    //if (!agent.walker.jumpAhead)
                    //{
                    //    if (finalDestination)
                    //    {
                    //        ReachFinalDestination(agent);
                    //        return;
                    //    }

                    //    agent.Deviate();
                    //    return;
                    //}
                }



                if (agent.aStarPath.Count > 0)
                {
                    agent.walker.currentDestination = agent.aStarPath[currentPathIndex];
                }

                agent.walker.SetDirection();

                if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.02f)
                {
                    agent.lastValidTileLocation = agent.aStarPath[currentPathIndex];
                    if (currentPathIndex < agent.aStarPath.Count - 1)
                    {
                        currentPathIndex++;
                        //currentNode = path[currentPathIndex];
                        agent.walker.currentDestination = agent.aStarPath[currentPathIndex];
                    }
                    else if (currentPathIndex >= agent.aStarPath.Count - 1)
                    {
                        ReachFinalDestination(agent);
                        //agent.lastValidNode = currentNode;

                        //finalDestination = true;
                        //agent.walker.SetRandomDestination(wanderDistance);
                    }
                    
                    
                }
            }

            agent.walker.SetLastPosition();
        }
        public override void EndPerformAction(SAP_Scheduler_NPC agent)
        {
            
            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.walker.currentDirection = Vector2.zero;
            agent.offScreenPosMoved = true;
            agent.lastValidNode = currentNode;
            ReachFinalDestination(agent);
            currentNode = null;
            agent.aStarPath.Clear();
            path.Clear();
            target = null;
        }

        
        public override void ReachFinalDestination(SAP_Scheduler_NPC agent)
        {
            
            agent.offScreenPosMoved = true;
            agent.isDeviating = false;
            finalDestination = false;
            agent.currentGoalComplete = true;
            
        }

        private void OnDrawGizmosSelected()
        {
            if (thisAgent == null)
                return;
            for (int i = 0; i < thisAgent.aStarPath.Count; i++)
            {
                Gizmos.color = Color.white;
                if (i == 0)
                    Gizmos.color = Color.blue;
                if (i == thisAgent.aStarPath.Count - 1)
                    Gizmos.color = Color.red;
                if (i == currentPathIndex)
                    Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(thisAgent.aStarPath[i], 0.1f);

            }

        }
    }
}
