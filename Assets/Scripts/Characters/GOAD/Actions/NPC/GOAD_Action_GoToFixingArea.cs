using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Action_GoToFixingArea : GOAD_Action
	{
        NavigationNode targetNode;
        public FixVillageDesk villageDesk;
        //FixVillageArea currentArea;

        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            base.StartAction(agent);

            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);
            targetNode = GetTarget();
            if (targetNode != null)
            {
                agent.currentPathIndex = 0;
                agent.aStarPath.Clear();
                if (agent.StartPositionValid())
                    agent.SetAStarDestination(targetNode.transform.position, this);

                agent.currentFinalDestination = targetNode.transform.position;
            }
            else
            {
                success = false;
                agent.SetActionComplete(true);
            }
        }


        public override void PerformAction(GOAD_Scheduler_NPC agent)
        {
            base.PerformAction(agent);

            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);
            if (agent.gettingPath)
                return;



            if (agent.offScreen || agent.screenManager.isSleeping)
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

            if (agent.walker.CheckDistanceToDestination() <= GlobalSettings.DistanceCheck)
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
        NavigationNode GetTarget()
        {
            NavigationNode node = null;
            foreach (var area in villageDesk.fixableAreas)
            {
                if (area.isFixing)
                {
                    node = area.reachNode;
                    //currentArea = area;
                    break;
                }

            }
            return node;
        }
    } 
}
