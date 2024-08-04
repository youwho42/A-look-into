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
            
            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);

            // Set the destination (currentAction.target) and direction here using currentAction.walker
            
            currentPathIndex = 0;
            agent.aStarPath.Clear();
            if (agent.StartPositionValid())
                agent.GetRandomTilePosition(wanderDistance);
            else
                agent.aStarPath.Add(agent.lastValidTileLocation);
            

            
        }

        public override void PerformAction(SAP_Scheduler_NPC agent)
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


            

            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);

            
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

        
    }
}
