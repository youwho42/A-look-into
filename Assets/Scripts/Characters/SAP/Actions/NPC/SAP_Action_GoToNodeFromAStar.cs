using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.Interactable;

namespace Klaxon.SAP
{
    public class SAP_Action_GoToNodeFromAStar : SAP_Action
    {
        bool lastPositionReset;
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

                agent.currentFinalDestination = target.transform.position;


            }
        }

        public override void PerformAction(SAP_Scheduler_NPC agent)
        {
            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);
            if (agent.gettingPath)
                return;
            if (!lastPositionReset)
            {
                agent.aStarPath[agent.aStarPath.Count - 1] = target.transform.position;
                lastPositionReset = true;
            }
                

            if (agent.aStarPath.Count > 0)
                agent.walker.currentDestination = agent.aStarPath[currentPathIndex];

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

            agent.walker.SetDirection();

            if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.03f)
            {
                
                if (currentPathIndex < agent.aStarPath.Count - 1)
                {
                    currentPathIndex++;
                    
                    agent.walker.currentDestination = agent.aStarPath[currentPathIndex];
                }
                else if (currentPathIndex >= agent.aStarPath.Count - 1)
                {
                    ReachFinalDestination(agent);
                    
                }
                if(currentPathIndex <= agent.aStarPath.Count - 1)
                    agent.lastValidTileLocation = agent.aStarPath[currentPathIndex];

            }
        

            agent.walker.SetLastPosition();


        }
        public override void EndPerformAction(SAP_Scheduler_NPC agent)
        {
            lastPositionReset = false;
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
