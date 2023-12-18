using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
	public class SAP_ANIMAL_JumpWander : SAP_Action
	{

        public Vector2 idleTimeRange;
        float timer;

        public float wanderDistance;
        bool atDestination = true;

        public override void StartPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            
            agent.animator.SetBool(agent.landed_hash, true);
            agent.animator.SetBool(agent.walking_hash, false);

            timer = Random.Range(idleTimeRange.x, idleTimeRange.y);
            if (agent.jumper != null)
                agent.jumper.enabled = true;
            if (agent.walker != null)
                agent.walker.enabled = false;

            if (agent.flier != null)
            {
                if (agent.flier.enabled)
                {
                    agent.walker.facingRight = agent.flier.facingRight;
                    agent.flier.enabled = false;
                }
            }

            agent.jumper.SetRandomDestination(wanderDistance);


        }


        public override void PerformAction(SAP_Scheduler_ANIMAL agent)
        {

            
            if (atDestination)
            {

                agent.animator.SetBool(agent.isRunning_hash, false);
                agent.jumper.currentDirection = Vector2.zero;

                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    timer = Random.Range(idleTimeRange.x, idleTimeRange.y);
                    agent.jumper.SetRandomDestination(wanderDistance);
                    atDestination = false;
                    return;
                }

                return;
            }

            agent.animator.SetBool(agent.isRunning_hash, true);


            if (agent.jumper.isStuck || agent.isDeviating)
            {

                if (!agent.jumper.jumpAhead)
                {
                    agent.jumper.SetRandomDestination(wanderDistance);

                }
                agent.jumper.FindDeviateDestination(5);
            }

            //agent.jumper.SetWorldDestination(agent.jumper.currentDestination);
            agent.jumper.SetDirection();

            agent.jumper.SetLastPosition();

            if (Vector2.Distance(transform.position, agent.jumper.currentDestination) <= 0.02f)
            {
                agent.currentGoalComplete = true;
            }


        }


        public override void EndPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            atDestination = true;
            
            
            agent.jumper.currentDirection = Vector2.zero;

            if (agent.currentDisplacementSpot != null)
            {
                agent.currentDisplacementSpot.isInUse = false;
                agent.currentDisplacementSpot = null;
            }

        }
    }

}