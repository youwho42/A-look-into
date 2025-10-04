using Klaxon.Interactable;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_GoToNearestSeat : GOAD_Action
	{

        public InteractableChair chair;
        bool lastPositionReset;
        NavigationNode target;

        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            base.StartAction(agent);

            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);

            
            chair = GOAD_WorldBeliefStates.instance.FindNearestSeat(transform.position);
            
            

            target = chair.findNode;
            agent.currentRestSeat = chair;

            if (target != null)
            {
                agent.currentPathIndex = 0;
                agent.aStarPath.Clear();
                if (agent.StartPositionValid())
                    agent.SetAStarDestination(target.transform.position, this);
                
                agent.currentFinalDestination = target.transform.position;
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

            if (agent.aStarPath.Count <= 0)
            {
                success = false;
                agent.SetActionComplete(true);
                return;
            }
            if (!lastPositionReset)
            {
                agent.aStarPath.Add(target.transform.position);
                lastPositionReset = true;
            }

            if (agent.aStarPath.Count > 0)
            {
                agent.walker.currentDestination = agent.aStarPath[agent.currentPathIndex];
            }

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

            agent.offScreenPosMoved = true;
            agent.isDeviating = false;

            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.walker.currentDirection = Vector2.zero;

            lastPositionReset = false;
            agent.offScreenPosMoved = true;
            agent.lastValidNode =  agent.currentNode;
            agent.currentPathIndex = 0;
            agent.currentNode = null;
            agent.aStarPath.Clear();
            target = null;
        }

        public override void AStarDestinationIsCurrentPosition(GOAD_Scheduler_NPC agent)
        {
            success = true;
            agent.SetActionComplete(true);
        }
    } 
}
