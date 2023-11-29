using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.Interactable;


namespace Klaxon.SAP
{
    public class SAP_Action_Sit : SAP_Action
    {


        
        InteractableChair chair;

        bool destinationReached;
        bool sitting;
        public Vector2Int minMaxSittingTime;
        public CycleTicks sitCycle;
        int maxTime;
        //int timer;

        public override void StartPerformAction(SAP_Scheduler_NPC agent)
        {
            maxTime = Random.Range(minMaxSittingTime.x, minMaxSittingTime.y);
            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);
            if (target == null)
            {

                chair = SAP_WorldBeliefStates.instance.FindNearestSeat(transform.position);
                target = chair.navigationNode;
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
            

            if (!chair.canInteract && !sitting)
            {
                agent.currentGoalComplete = true;
                return;
            }

            if (sitting)
            {
                
                if (RealTimeDayNightCycle.instance.currentTimeRaw >= sitCycle.tick && RealTimeDayNightCycle.instance.currentDayRaw == sitCycle.day /*timer == RealTimeDayNightCycle.instance.currentTimeRaw*/)
                {
                    StartCoroutine(PlaceNPC(agent, chair.navigationNode.transform.position));
                    agent.currentGoalComplete = true;
                    agent.SetBeliefState("Tired", false);
                    chair.canInteract = true;
                }
                return;
            }
                
            if (destinationReached && !sitting)
            {
                agent.offScreenPosMoved = false;
                sitting = true;
                agent.animator.SetBool(agent.isSitting_hash, true);
                agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
                agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
                agent.animator.SetFloat(agent.velocityX_hash, 0);
                agent.walker.currentDirection = Vector2.zero;
                Vector3 displacement = new Vector3(agent.walker.transform.position.x, agent.walker.transform.position.y, agent.walker.transform.position.z + 0.33f);
                agent.walker.transform.position = displacement;
                sitCycle = RealTimeDayNightCycle.instance.GetCycleTime(maxTime);
                agent.lastValidNode = currentNode;
                StartCoroutine(PlaceNPC(agent, chair.transform.position));
                if (agent.walker.facingRight && !chair.facingRight || !agent.walker.facingRight && chair.facingRight)
                    agent.walker.Flip();
                chair.canInteract = false;
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
            if (agent.walker.CheckDistanceToDestination() <= 0.02f)
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

            sitCycle = null;
            sitting = false;
            destinationReached = false;
            
            chair = null;
            path.Clear();
            currentNode = null;
            target = null;
        }


        public override void ReachFinalDestination(SAP_Scheduler_NPC agent)
        {
            agent.offScreenPosMoved = true;
            agent.isDeviating = false;
            destinationReached = true;

            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.walker.currentDirection = Vector2.zero;
        }



        IEnumerator PlaceNPC(SAP_Scheduler_NPC agent, Vector3 position)
        {
            float timer = 0;
            float maxTime = 0.45f;
            while (timer < maxTime)
            {
                Vector3 pos = Vector3.Lerp(agent.transform.position, position, timer / maxTime);
                agent.transform.position = pos;
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
}
