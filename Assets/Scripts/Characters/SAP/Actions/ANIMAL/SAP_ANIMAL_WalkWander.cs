using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
	public class SAP_ANIMAL_WalkWander : SAP_Action
	{
        public Vector2 idleTimeRange;
        float timer;
        public Vector2 headTimeRange;
        float headTimer;
        public float wanderDistance;
        bool atDestination;

        public override void StartPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            if (agent.walker.itemObject.localPosition.y != 0)
                agent.walker.isWeightless = true;
            agent.animator.SetBool(agent.landed_hash, true);

            agent.walker.enabled = true;

            if (agent.flier != null)
            {
                if (agent.flier.enabled)
                {
                    agent.walker.facingRight = agent.flier.facingRight;
                    agent.flier.enabled = false;
                }
            }

            

            timer = agent.SetRandomRange(idleTimeRange);
            headTimer = agent.SetRandomRange(headTimeRange);
            agent.walker.SetRandomDestination(wanderDistance);
        }
        public override void PerformAction(SAP_Scheduler_ANIMAL agent)
        {

            if (agent.walker.itemObject.localPosition.y == 0)
            {

                if (agent.walker.isStuck || agent.isDeviating)
                {
                    if (!agent.walker.jumpAhead)
                    {
                        agent.DeviateWalk();
                        return;
                    }
                }

                if (atDestination)
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        timer = agent.SetRandomRange(idleTimeRange);
                        agent.walker.SetRandomDestination(wanderDistance);
                        atDestination = false;
                    }

                    headTimer -= Time.deltaTime;
                    if (headTimer <= 0)
                        TurnHead(agent);

                    return;
                }
                else
                {
                    agent.animator.SetBool(agent.walking_hash, true);
                }

                agent.walker.SetDirection();
                if (Vector2.Distance(transform.position, agent.walker.currentDestination) <= 0.02f)
                    atDestination = true;

                agent.walker.SetLastPosition();

            }
            else
            {
                headTimer -= Time.deltaTime;
                if (headTimer <= 0)
                    TurnHead(agent);
            }
        }
        public override void EndPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            atDestination = true;
            timer = 0;
            headTimer = 0;
            agent.walker.currentDir = Vector2.zero;

            if (agent.currentLandingSpot != null)
                agent.currentLandingSpot.isInUse = false;
            agent.currentLandingSpot = null;


        }

        void TurnHead(SAP_Scheduler_ANIMAL agent)
        {

            headTimer = agent.SetRandomRange(headTimeRange);
            agent.animator.SetTrigger(agent.idle_hash);
            
        }
    } 
}
