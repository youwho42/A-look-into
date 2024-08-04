using Klaxon.Interactable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Action_Rest : GOAD_Action
	{

        InteractableChair chair;

        bool destinationReached;
        bool sitting;
        public Vector2Int minMaxSittingTime;
        public CycleTicks sitCycle;
        int maxTime;

        bool isGettingUp;
        NavigationNode target;

        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            base.StartAction(agent);

            maxTime = Random.Range(minMaxSittingTime.x, minMaxSittingTime.y);
            sitCycle = RealTimeDayNightCycle.instance.GetCycleTime(maxTime);
            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);
            if (target == null)
            {

                chair = GOAD_WorldBeliefStates.instance.FindNearestSeat(transform.position);
                if(chair != agent.currentRestSeat)
                {
                    success = false;
                    agent.SetActionComplete(true);
                }
                target = chair.sitNode;
                agent.currentNode = chair.findNode;
                chair.canInteract = false;
                agent.nodePath.Clear();
                agent.currentPathIndex = 0;

                agent.nodePath = agent.currentNode.FindPath(target);

                agent.walker.currentDestination = agent.nodePath[agent.currentPathIndex].transform.position;
            }

        }

        public override void PerformAction(GOAD_Scheduler_NPC agent)
        {
            base.PerformAction(agent);


            if (sitting)
            {

                if (!isGettingUp && RealTimeDayNightCycle.instance.currentTimeRaw >= sitCycle.tick && RealTimeDayNightCycle.instance.currentDayRaw == sitCycle.day)
                {
                    agent.animator.SetBool(agent.isSitting_hash, false);
                    StartCoroutine(PlaceNPC(agent, chair.sitNode.transform.position, true));
                    isGettingUp = true;
                    destinationReached = false;
                    agent.currentPathIndex = 0;
                    agent.nodePath.Reverse();
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

                agent.lastValidNode = agent.currentNode;
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
                    agent.NodeDeviate();
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

                    if (!isGettingUp)
                        ReachSeat(agent);
                    else
                    {
                        success = true;
                        agent.SetActionComplete(true);
                    }
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
            agent.SetBeliefState("AtRestSeat", false);
            agent.SetBeliefState("Tired", true);
        }

        public override void EndAction(GOAD_Scheduler_NPC agent)
        {
            base.EndAction(agent);

            agent.offScreenPosMoved = true;
            //agent.lastValidNode = currentNode;
            agent.SetBeliefState("Tired", false);
            agent.SetBeliefState("AtRestSeat", false);
            sitCycle = null;
            sitting = false;
            destinationReached = false;
            isGettingUp = false;
            chair = null;
            agent.nodePath.Clear();
            agent.currentNode = null;
            target = null;
            agent.currentPathIndex = 0;
        }

        public void ReachSeat(GOAD_Scheduler_NPC agent)
        {
            agent.offScreenPosMoved = true;
            agent.isDeviating = false;
            destinationReached = true;

            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.walker.currentDirection = Vector2.zero;
        }



        IEnumerator PlaceNPC(GOAD_Scheduler_NPC agent, Vector3 position, bool standingUp)
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
