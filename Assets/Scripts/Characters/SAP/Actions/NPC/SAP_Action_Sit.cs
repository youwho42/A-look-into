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

        bool isGettingUp;
        //int timer;

        public override void StartPerformAction(SAP_Scheduler_NPC agent)
        {
            //isGettingUp = false;
            maxTime = Random.Range(minMaxSittingTime.x, minMaxSittingTime.y);
            sitCycle = RealTimeDayNightCycle.instance.GetCycleTime(maxTime);
            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);
            if (target == null)
            {

                chair = SAP_WorldBeliefStates.instance.FindNearestSeat(transform.position);
                target = chair.sitNode;
                currentNode = chair.findNode;
                chair.canInteract = false;
                path.Clear();
                currentPathIndex = 0;
                
                path = currentNode.FindPath(target);

                agent.walker.currentDestination = path[currentPathIndex].transform.position;
            }
        }

        public override void PerformAction(SAP_Scheduler_NPC agent)
        {


           
            if (sitting)
            {
                
                if (!isGettingUp && RealTimeDayNightCycle.instance.currentTimeRaw >= sitCycle.tick && RealTimeDayNightCycle.instance.currentDayRaw == sitCycle.day)
                {
                    agent.animator.SetBool(agent.isSitting_hash, false);
                    StartCoroutine(PlaceNPC(agent, chair.sitNode.transform.position, true));
                    isGettingUp = true;
                    destinationReached = false;
                    currentPathIndex = 0;
                    path.Reverse();
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
                
                agent.lastValidNode = currentNode;
                StartCoroutine(PlaceNPC(agent, chair.transform.position, false));
                if (agent.walker.facingRight && !chair.facingRight || !agent.walker.facingRight && chair.facingRight)
                    agent.walker.Flip();
                
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
                    agent.Deviate();
                    return;
                }
            }



            if (path.Count > 0)
            {
                agent.walker.currentDestination = path[currentPathIndex].transform.position;
            }
            agent.walker.SetDirection();

            if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.03f)
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

                    if (!isGettingUp)
                        ReachFinalDestination(agent);
                    else
                        agent.currentGoalComplete = true;
                }
            }

            agent.walker.SetLastPosition();



        }
        public override void EndPerformAction(SAP_Scheduler_NPC agent)
        {
            agent.offScreenPosMoved = true;
            agent.lastValidNode = currentNode;
            agent.SetBeliefState("Tired", false);
            sitCycle = null;
            sitting = false;
            destinationReached = false;
            isGettingUp = false;
            chair = null;
            path.Clear();
            currentNode = null;
            target = null;
            currentPathIndex = 0;
        }


        public override void ReachFinalDestination(SAP_Scheduler_NPC agent)
        {
            agent.offScreenPosMoved = true;
            agent.isDeviating = false;
            destinationReached = true;
            
            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.walker.currentDirection = Vector2.zero;
        }



        IEnumerator PlaceNPC(SAP_Scheduler_NPC agent, Vector3 position, bool standingUp)
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
            if (standingUp)
            {
                
                chair.canInteract = true;
                sitting = false;
            }
                
            yield return null;
        }
    }
}
