using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.Interactable;

namespace Klaxon.SAP
{
    public class SAP_Action_GoToNodeFromAStar : SAP_Action
    {
        

        public override void StartPerformAction(SAP_Scheduler_NPC agent)
        {
            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);
            if (target != null)
            {

                
                currentPathIndex = 0;
                agent.aStarPath.Clear();
                if (agent.StartPositionValid())
                    agent.SetAStarDestination(target.transform.position);
                //else
                //{
                //    Debug.Log("start position not valid from start");
                //    agent.aStarPath.Add(agent.lastValidTileLocation);
                //}
                
            }
        }

        public override void PerformAction(SAP_Scheduler_NPC agent)
        {
            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);


            if (agent.offScreen || agent.sleep.isSleeping)
            {
                agent.HandleOffScreenAStar(this);
                return;
            }

            if (agent.walker.isStuck || agent.isDeviating)
            {

                agent.Deviate(this);
                return;
                
            }



            if (agent.aStarPath.Count > 0)
            {
                agent.walker.currentDestination = agent.aStarPath[currentPathIndex];
            }

            agent.walker.SetDirection();

            if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.01f)
            {
                agent.lastValidTileLocation = agent.aStarPath[currentPathIndex];
                if (currentPathIndex < agent.aStarPath.Count - 1)
                {
                    currentPathIndex++;
                    
                    agent.walker.currentDestination = agent.aStarPath[currentPathIndex];
                }
                else if (currentPathIndex >= agent.aStarPath.Count - 1)
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
