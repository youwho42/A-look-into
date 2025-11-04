
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_AStarGoToNode : GOAD_Action
    {

        public NavigationNode targetNode;
        bool wandering;
        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            base.StartAction(agent);
            wandering = false;
            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);
            if (targetNode != null)
            {
                agent.currentPathIndex = 0;
                agent.aStarPath.Clear();
                if (agent.StartPositionValid())
                    agent.SetAStarDestination(targetNode.transform.position, this);

                agent.currentFinalDestination = targetNode.transform.position;
                
            }
        }


        public override void PerformAction(GOAD_Scheduler_NPC agent)
        {
            base.PerformAction(agent);
            if (agent.gettingPath)
                return;

            if (agent.aStarPath.Count == 0)
            {
                if (agent.StartPositionValid())
                    agent.GetRandomTilePosition(2, this);
                else
                    agent.aStarPath.Add(agent.lastValidTileLocation);

                wandering = true;

                return;
                
                //Debug.Log("A* path to node empty", gameObject);
                //agent.aStarPath.Add(targetNode.transform.position);
            }

            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);

            



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
                    if (!wandering)
                    {
                        success = true;
                        agent.SetActionComplete(true);
                    }
                    else
                    {
                        agent.currentPathIndex = 0;
                        agent.aStarPath.Clear();
                        if (agent.StartPositionValid())
                            agent.SetAStarDestination(targetNode.transform.position, this);

                        agent.currentFinalDestination = targetNode.transform.position;

                    }
                    
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
            agent.aStarPath.Add(targetNode.transform.position);
        }
    }

}