using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_JumperWalk : GOAD_Action
    {

        public float wanderDistance;
        
        public Vector2 idleTimerRange;
        float idleTimer;
        public Vector2 wanderTimerRange;
        float wanderTimer;
        bool atDestination;

        public override void StartAction(GOAD_Scheduler_Animal agent)
        {
            base.StartAction(agent);

            agent.animator.SetBool(agent.landed_hash, true);
            agent.animator.SetBool(agent.walking_hash, false);
            idleTimer = Random.Range(idleTimerRange.x, idleTimerRange.y);
            wanderTimer = Random.Range(wanderTimerRange.x, wanderTimerRange.y);
            if (agent.jumper != null)
                agent.jumper.enabled = true;
            if (agent.walker != null)
                agent.walker.enabled = false;

            if (agent.flier != null)
            {
                if (agent.flier.enabled)
                {
                    if (agent.walker != null)
                        agent.walker.facingRight = agent.flier.facingRight;
                    agent.flier.enabled = false;
                }
            }

            agent.jumper.SetRandomDestination(wanderDistance);
        }

        public override void PerformAction(GOAD_Scheduler_Animal agent)
        {
            base.PerformAction(agent);

            wanderTimer -= Time.deltaTime;
            if (wanderTimer <= 0)
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }



            if (atDestination)
            {
                
                agent.animator.SetBool(agent.isRunning_hash, false);
                agent.jumper.currentDirection = Vector2.zero;

                idleTimer -= Time.deltaTime;
                if (idleTimer <= 0)
                {
                    idleTimer = Random.Range(idleTimerRange.x, idleTimerRange.y);
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
                    return;
                }
                
            }

            //agent.jumper.SetWorldDestination(agent.jumper.currentDestination);
            agent.jumper.SetDirection();

            agent.jumper.SetLastPosition();

            if (NumberFunctions.GetDistanceV2(transform.position, agent.jumper.currentDestination) <= 0.0004f)
                atDestination = true;


           
                
        }

        public override void EndAction(GOAD_Scheduler_Animal agent)
        {
            base.EndAction(agent);

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
