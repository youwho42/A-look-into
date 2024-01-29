using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.Interactable;


namespace Klaxon.SAP
{
    public class SAP_Action_Sleep : SAP_Action
    {
        SAP_WorldBeliefStates worldStates;

        

        public InteractableDialogue interactableDialogue;
        public NPC_UndertakingAvailable undertakingAvailable;

        bool destinationReached;
        bool sleeping;

        public override void StartPerformAction(SAP_Scheduler_NPC agent)
        {
            worldStates = SAP_WorldBeliefStates.instance;
            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);
            if (target != null)
            {

                // Set the destination (currentAction.target) and direction here using currentAction.walker
                if (currentNode == null && path.Count <= 0)
                {
                    path.Clear();
                    currentPathIndex = 0;
                    if (agent.lastValidNode != null)
                        currentNode = agent.lastValidNode;
                    else
                        currentNode = NavigationNodesManager.instance.GetClosestNavigationNode(transform.position, agent.currentNavigationNodeType, agent.pathType);
                    path = currentNode.FindPath(target);
                    agent.walker.currentDestination = path[currentPathIndex].transform.position;
                }
            }
        }

        public override void PerformAction(SAP_Scheduler_NPC agent)
        {
            if(worldStates.worldStates.TryGetValue("Day", out bool isDay))
            {
                agent.currentGoalComplete = isDay;
            }

            if (sleeping)
                return;
            if (destinationReached && !sleeping)
            {
                sleeping = true;
                interactableDialogue.canInteract = false;
                undertakingAvailable.isInactive = true;
                agent.animator.SetBool(agent.isSleeping_hash, true);
                agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
                agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
                agent.animator.SetFloat(agent.velocityX_hash, 0);
                agent.walker.currentDirection = Vector2.zero;
                Vector3 displacement = new Vector3(agent.walker.transform.position.x, agent.walker.transform.position.y, agent.walker.transform.position.z + 0.33f);
                agent.walker.transform.position = displacement;
                return;
            }

            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            // this is where we need to make the npc GO TO the destination.
            // use currentAction.walker here


            agent.animator.SetFloat(agent.velocityX_hash, 1);

            if (agent.offScreen || agent.sleep.isSleeping)
            {
                agent.HandleOffScreen(this);
                return;
            }

            if (agent.walker.isStuck || agent.isDeviating)
            {
                if (!agent.walker.jumpAhead)
                {
                    agent.Deviate();
                    return;
                }
            }

            if (path.Count > 0)
            {
                agent.walker.currentDestination = path[currentPathIndex].transform.position;
            }
            
            if (!agent.walker.onSlope)
                agent.walker.SetDirection();
            if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.01f)
            {
                if (currentPathIndex < path.Count - 1)
                {
                    currentPathIndex++;
                    currentNode = path[currentPathIndex];
                    agent.walker.currentDestination = path[currentPathIndex].transform.position;
                }
                else if (currentPathIndex >= path.Count - 1)
                {
                    agent.lastValidNode = currentNode;

                    ReachFinalDestination(agent);
                    agent.animator.SetFloat(agent.velocityX_hash, 0);
                    agent.walker.currentDirection = Vector2.zero;
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
            sleeping = false;
            destinationReached = false;
            interactableDialogue.canInteract = true;
            undertakingAvailable.isInactive = false;
            undertakingAvailable.SetUndertakingIcon();
        }

        
        
        public override void ReachFinalDestination(SAP_Scheduler_NPC agent)
        {
            agent.offScreenPosMoved = true;
            agent.isDeviating = false;
            destinationReached = true;
            
            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.walker.currentDirection = Vector2.zero;
        }
    }
}
