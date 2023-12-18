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
        bool atDestination = true;

        public override void StartPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            
            agent.animator.SetBool(agent.landed_hash, true);

            agent.walker.enabled = true;

            if (agent.flier != null)
            {
                if (agent.walker.itemObject.localPosition.y != 0)
                    agent.walker.isWeightless = true;
                if (agent.flier.enabled)
                {
                    agent.walker.facingRight = agent.flier.facingRight;
                    agent.flier.enabled = false;
                }
            }
            if (agent.jumper != null)
            {
                if(agent.jumper.enabled)
                    agent.walker.facingRight = agent.jumper.facingRight;
                agent.jumper.enabled = false;
            }
            

            timer = agent.SetRandomRange(idleTimeRange);
            headTimer = agent.SetRandomRange(headTimeRange);

            agent.walker.SetRandomDestination(wanderDistance);

        }
        public override void PerformAction(SAP_Scheduler_ANIMAL agent)
        {

            if (agent.walker.itemObject.localPosition.y > 0)
            {
                headTimer -= Time.deltaTime;
                if (headTimer <= 0)
                    TurnHead(agent);
            }
            else
            {
                if (atDestination)
                {

                    agent.animator.SetBool(agent.walking_hash, false);
                    agent.walker.currentDirection = Vector2.zero;

                    headTimer -= Time.deltaTime;
                    if (headTimer <= 0)
                        TurnHead(agent);

                   

                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        agent.walker.SetRandomDestination(wanderDistance);
                        atDestination = false;
                        return;
                    }

                    return;
                }
                
                agent.animator.SetBool(agent.walking_hash, true);
                

                if (agent.walker.isStuck || agent.isDeviating)
                {
                    
                    if (!agent.walker.jumpAhead)
                    {
                        agent.walker.SetRandomDestination(wanderDistance);
                        
                    }
                    agent.walker.FindDeviateDestination(5);
                }

                agent.walker.SetWorldDestination(agent.walker.currentDestination);
                agent.walker.SetDirection();

                agent.walker.SetLastPosition();

                if (Vector2.Distance(transform.position, agent.walker.currentDestination) <= 0.02f)
                {
                    agent.currentGoalComplete = true;
                }

                

            }
            
            
        }
        public override void EndPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            atDestination = true;
            timer = 0;
            headTimer = 0;
            agent.walker.currentDirection = Vector2.zero;

            if (agent.currentDisplacementSpot != null) 
            { 
                agent.currentDisplacementSpot.isInUse = false;
                agent.currentDisplacementSpot = null;
            }

        }

        
        void TurnHead(SAP_Scheduler_ANIMAL agent)
        {

            headTimer = agent.SetRandomRange(headTimeRange);
            agent.animator.SetTrigger(agent.idle_hash);
            
        }
    } 
}
