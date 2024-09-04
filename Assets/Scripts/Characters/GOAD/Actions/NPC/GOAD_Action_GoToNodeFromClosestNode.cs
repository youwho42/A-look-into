using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Action_GoToNodeFromClosestNode : GOAD_Action
	{
        public NavigationNode endNode;

        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            base.StartAction(agent);

            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);
            if (endNode != null)
            {
                 
                agent.nodePath.Clear();
                agent.currentPathIndex = 0;
                agent.nodePath = endNode.FindPath(transform.position);
                agent.nodePath.Reverse();
                agent.currentNode = agent.nodePath[agent.currentPathIndex];
                transform.position = agent.currentNode.transform.position;
                agent.walker.currentDestination = agent.nodePath[agent.currentPathIndex].transform.position;
                agent.lastValidNode = agent.currentNode;
                
            }

        }
        public override void PerformAction(GOAD_Scheduler_NPC agent)
        {
            base.PerformAction(agent);

            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);

            if (agent.offScreen || agent.sleep.isSleeping)
            {
                agent.HandleOffScreenNodes(this);
                return;
            }

            if (agent.walker.isStuck || agent.isDeviating)
            {

                agent.NodeDeviate(this);
                return;

            }

            if (agent.nodePath.Count > 0)
            {
                agent.walker.currentDestination = agent.nodePath[agent.currentPathIndex].transform.position;
            }

            agent.walker.SetDirection();

            if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.02f)
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
            agent.currentNode = null;
            agent.nodePath.Clear();
            agent.currentPathIndex = 0;
        }

        public override void OffscreenNodeHandleComplete(GOAD_Scheduler_NPC agent)
        {
            agent.lastValidNode = agent.currentNode;
            success = true;
            agent.SetActionComplete(true);
        }
    } 
}
