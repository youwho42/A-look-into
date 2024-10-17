using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_GhostWander : GOAD_Action
    {

        public int wanderDistance = 1;
        public override void StartAction(GOAD_Scheduler_Ghost agent)
        {
            base.StartAction(agent);
            agent.offScreenPosMoved = true;



            agent.currentPathIndex = 0;
            agent.aStarPath.Clear();
            if (agent.StartPositionValid())
                agent.GetRandomTilePosition(wanderDistance, this);
            else
                agent.aStarPath.Add(agent.lastValidTileLocation);

            agent.animator.SetFloat(agent.velocityX_hash, 1);
            
            

        }

        

        public override void PerformAction(GOAD_Scheduler_Ghost agent)
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
            if (agent.currentPathIndex >= agent.aStarPath.Count - 1)
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }

            agent.animator.SetFloat(agent.velocityX_hash, 1);

            if (agent.offScreen || agent.sleep.isSleeping)
            {
                agent.HandleOffScreenAStar(this);
                return;
            }

            if (agent.ghoster.isStuck || agent.isDeviating)
            {

                agent.AStarDeviate(this);
                return;

            }

            if (agent.aStarPath.Count > 0)
                agent.ghoster.currentDestination = agent.aStarPath[agent.currentPathIndex];

            agent.ghoster.SetDirection();

            if (agent.ghoster.CheckDistanceToDestination() <= agent.ghoster.checkTileDistance + 0.02f)
            {
                agent.lastValidTileLocation = agent.aStarPath[agent.currentPathIndex];
                if (agent.currentPathIndex < agent.aStarPath.Count - 1)
                {
                    agent.currentPathIndex++;
                    agent.ghoster.currentDestination = agent.aStarPath[agent.currentPathIndex];
                }
                else if (agent.currentPathIndex >= agent.aStarPath.Count - 1)
                {
                    success = true;
                    agent.SetActionComplete(true);
                }


            }


            agent.ghoster.SetLastPosition();
        }

        public override void EndAction(GOAD_Scheduler_Ghost agent)
        {
            base.EndAction(agent);

            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.ghoster.currentDirection = Vector2.zero;
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
