using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Action_WanderAStar : GOAD_Action
	{
        public float wanderDistance = 1f;


        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            base.StartAction(agent);

            agent.offScreenPosMoved = true;

            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);

            agent.currentPathIndex = 0;
            agent.aStarPath.Clear();
            if (agent.StartPositionValid())
                agent.GetRandomTilePosition(wanderDistance, this);
            else
                agent.aStarPath.Add(agent.lastValidTileLocation);
        }


        // when completing the task remember to set success to true or false
        public override void PerformAction(GOAD_Scheduler_NPC agent)
        {
            base.PerformAction(agent);

            if (agent.gettingPath)
                return;
            if (agent.aStarPath.Count <= 0)
            {
                success = false;
                agent.SetActionComplete(true);
                return;
            }
            if (agent.currentPathIndex >= agent.aStarPath.Count)
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }



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

                agent.AStarDeviate(this);
                return;

            }

            if (agent.aStarPath.Count > 0)
                agent.walker.currentDestination = agent.aStarPath[agent.currentPathIndex];

            agent.walker.SetDirection();

            if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.02f)
            {
                agent.lastValidTileLocation = agent.aStarPath[agent.currentPathIndex];
                if (agent.currentPathIndex < agent.aStarPath.Count - 1)
                {
                    agent.currentPathIndex++;
                    agent.walker.currentDestination = agent.aStarPath[agent.currentPathIndex];
                }
                else if (agent.currentPathIndex >= agent.aStarPath.Count - 1)
                {
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
            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.walker.currentDirection = Vector2.zero;
            agent.aStarPath.Clear();
            agent.currentPathIndex = 0;
        }

        public override void AStarDestinationIsCurrentPosition(GOAD_Scheduler_NPC agent)
        {
            success = true;
            agent.SetActionComplete(true);
        }
    } 
}