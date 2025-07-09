using BezierSolution;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_WalkWander : GOAD_Action
    {

        public float wanderDistance;
        
        float headTimer;
        public Vector2 idleTimerRange;
        float idleTimer;
        public Vector2 wanderTimerRange;
        float wanderTimer;
        bool atDestination;

        public override void StartAction(GOAD_Scheduler_Animal agent)
        {
            base.StartAction(agent);
            //atDestination = true;
            agent.animator.SetBool(agent.landed_hash, true);

            if (agent.walker != null)
                agent.walker.enabled = true;

            if (agent.walker.itemObject.transform.localPosition.y < 0)
                agent.walker.itemObject.transform.localPosition = Vector3.zero;
            agent.walker.SetLastPosition();

            if (agent.flier != null)
            {
                agent.walker.isWeightless = agent.walker.itemObject.localPosition.y > 0;
                if (agent.flier.enabled)
                {
                    agent.walker.facingRight = agent.flier.facingRight;
                    agent.flier.enabled = false;
                }
            }
            if (agent.jumper != null)
            {
                if (agent.jumper.enabled)
                    agent.walker.facingRight = agent.jumper.facingRight;
                agent.jumper.enabled = false;
            }

            if (agent.currentDisplacementSpot != null)
            {
                agent.currentDisplacementSpot.isInUse = false;
                agent.currentDisplacementSpot = null;
            }

            headTimer = agent.SetRandomRange(agent.headTimeRange);
            idleTimer = agent.SetRandomRange(idleTimerRange);
            wanderTimer = agent.SetRandomRange(wanderTimerRange);

            
            agent.walker.SetRandomDestination(wanderDistance);
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

            if (agent.walker.itemObject.localPosition.y > 0)
            {
                agent.animator.SetBool(agent.landed_hash, true);
                headTimer -= Time.deltaTime;
                if (headTimer <= 0)
                    headTimer = agent.TurnHead();
            }
            else
            {
                if (atDestination)
                {

                    agent.animator.SetBool(agent.walking_hash, false);
                    agent.walker.currentDirection = Vector2.zero;

                    headTimer -= Time.deltaTime;
                    if (headTimer <= 0)
                        headTimer = agent.TurnHead();



                    idleTimer -= Time.deltaTime;
                    if (idleTimer <= 0)
                    {
                        agent.walker.SetRandomDestination(wanderDistance);
                        atDestination = false;
                        return;
                    }

                    return;
                }

                agent.animator.SetBool(agent.walking_hash, true);

                if (agent.sleep.isSleeping)
                {
                    agent.HandleOffScreen(this);
                    return;
                }


                if (agent.walker.isStuck || agent.isDeviating)
                {

                    if (!agent.walker.jumpAhead)
                    {
                        agent.DeviateWalk();
                        return;
                    }
                    //agent.walker.FindDeviateDestination(5);
                }

                agent.walker.SetWorldDestination(agent.walker.currentDestination);
                agent.walker.SetDirection();

                agent.walker.SetLastPosition();

                if (Vector2.Distance(transform.position, agent.walker.currentDestination) <= 0.02f)
                {
                    atDestination = true;
                }



            }

        }

        public override void EndAction(GOAD_Scheduler_Animal agent)
        {
            base.EndAction(agent);
            agent.walker.currentDirection = Vector2.zero;
            
            
        }

        
    }
}
