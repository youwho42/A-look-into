using Klaxon.Interactable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_AttendGathering : GOAD_Action
    {
        bool atPosition;
        float timer;
        public Transform gatheringCenter;
        Vector3 standingPosition;
        Vector2 standingTimeMinMax = new Vector2(5, 20);
        bool addedLastPosition;
        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            base.StartAction(agent);
            atPosition = false;
            addedLastPosition = false;
            timer = 0;
            standingPosition = GetPositionAtGathering();

            agent.currentPathIndex = 0;
            agent.aStarPath.Clear();
            if (agent.StartPositionValid())
                agent.SetAStarDestination(standingPosition, this);

            agent.currentFinalDestination = standingPosition;
            
            timer = Random.Range(standingTimeMinMax.x, standingTimeMinMax.y);
            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);
        }

        

        public override void PerformAction(GOAD_Scheduler_NPC agent)
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
            if(!addedLastPosition)
            {
                agent.aStarPath.Add(standingPosition);
                addedLastPosition = true;
            }
            
            

            if (atPosition)
            {
                // some type of "hanging out with others" logic... gl
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    success = true;
                    agent.SetActionComplete(true);
                }
                return;
            }


            if (!atPosition)
            {
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
                        agent.animator.SetFloat(agent.velocityX_hash, 0);
                        agent.walker.currentDirection = Vector2.zero;
                        atPosition = true;
                    }
                    //agent.walker.SetDirection();
                    //if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.02f)
                    //{
                    //    agent.animator.SetFloat(agent.velocityX_hash, 0);
                    //    agent.walker.currentDirection = Vector2.zero;
                    //    atPosition = true;
                    //}
                }
            }
        }

        public override void EndAction(GOAD_Scheduler_NPC agent)
        {
            base.EndAction(agent);
            addedLastPosition = false;
        }


        Vector3 GetPositionAtGathering()
        {
            Vector3 position = Vector3.zero;
            bool found = false;
            do
            {
                var r = Random.insideUnitCircle * 1.2f;
                var pos = gatheringCenter.position + (Vector3)r;
                var hit = Physics2D.OverlapCircle(pos, 0.08f, LayerMask.GetMask("Obstacle"), transform.position.z, transform.position.z);
                if (hit == null)
                {
                    position = pos;
                    found = true;
                }
            } while (!found);
            return position;

        }


    }

}