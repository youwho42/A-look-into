using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Action_GoToFixNode : GOAD_Action
	{
        public FixVillageDesk villageDesk;
        FixVillageArea currentArea;
        bool destinationReached;
        bool atSpot;


        
        FixVillageArea GetFixableArea()
        {
            FixVillageArea fixArea = null;
            foreach (var area in villageDesk.fixableAreas)
            {
                if (area.isFixing)
                {
                    fixArea = area;
                    break;
                }

            }
            return fixArea;
        }
        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            base.StartAction(agent);


            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);
            currentArea = GetFixableArea();
            if (currentArea != null)
            {
                
                agent.currentNode = currentArea.reachNode;
                var target = currentArea.fixingNode;
                
                agent.nodePath.Clear();
                agent.currentPathIndex = 0;

                agent.nodePath = agent.currentNode.FindPath(target);

                agent.walker.currentDestination = agent.nodePath[agent.currentPathIndex].transform.position;


            }
        }

        public override void PerformAction(GOAD_Scheduler_NPC agent)
        {
            base.PerformAction(agent);


            if (atSpot)
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }


            if (destinationReached && !atSpot)
            {
                agent.offScreenPosMoved = false;
                atSpot = true;
                agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
                agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
                agent.animator.SetFloat(agent.velocityX_hash, 0);
                agent.walker.currentDirection = Vector2.zero;
                
                agent.lastValidNode = agent.currentNode;
                
                return;
            }

            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);
            //// this is where we need to make the npc GO TO the destination.
            //// use currentAction.walker here




            if (agent.offScreen || agent.sleep.isSleeping)
            {
                agent.HandleOffScreenNodes(this);
                return;
            }


            if (agent.walker.isStuck || agent.isDeviating)
            {
                if (!agent.walker.jumpAhead)
                {
                    agent.NodeDeviate(this);
                    return;
                }
            }



            if (agent.nodePath.Count > 0)
            {
                agent.walker.currentDestination = agent.nodePath[agent.currentPathIndex].transform.position;
            }
            agent.walker.SetDirection();

            if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.03f)
            {
                if (agent.currentPathIndex < agent.nodePath.Count - 1)
                {
                    agent.currentPathIndex++;
                    agent.currentNode = agent.nodePath[agent.currentPathIndex];
                    agent.walker.currentDestination = agent.nodePath[agent.currentPathIndex].transform.position;
                }
                else if (agent.currentPathIndex >= agent.nodePath.Count - 1)
                {
                    agent.lastValidNode = agent.currentNode;
                    ReachSpot(agent);

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

            agent.offScreenPosMoved = true;
            atSpot = false;
            destinationReached = false;
            agent.nodePath.Clear();
            agent.currentNode = null;
            agent.currentPathIndex = 0;
            currentArea = null;
        }

        public void ReachSpot(GOAD_Scheduler_NPC agent)
        {
            agent.offScreenPosMoved = true;
            agent.isDeviating = false;
            destinationReached = true;
            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.walker.currentDirection = Vector2.zero;
        }

        public override void OffscreenNodeHandleComplete(GOAD_Scheduler_NPC agent)
        {
            agent.lastValidNode = agent.currentNode;
            ReachSpot(agent);
        }
    } 
}
