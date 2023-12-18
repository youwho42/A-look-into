using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
	public class SAP_ANIMAL_JumpFlee : SAP_Action
	{
        

        public float fleeDistance;
        bool atDestination;
        float fleeTimer;
        public override void StartPerformAction(SAP_Scheduler_ANIMAL agent)
        {

            agent.animator.SetBool(agent.landed_hash, true);
            agent.animator.SetBool(agent.walking_hash, false);
            agent.animator.SetBool(agent.isRunning_hash, true);

            
            if (agent.jumper != null)
                agent.jumper.enabled = true;
            if (agent.walker != null)
            {
                if(agent.walker.enabled)
                {
                    agent.jumper.facingRight = agent.walker.facingRight;
                }
                agent.walker.enabled = false;
            }
                

            if (agent.flier != null)
            {
                if (agent.flier.enabled)
                {
                    agent.walker.facingRight = agent.flier.facingRight;
                    agent.flier.enabled = false;
                }
            }

            agent.jumper.SetWorldDestination(FleeDestination(agent));


        }


        public override void PerformAction(SAP_Scheduler_ANIMAL agent)
        {


            if (atDestination)
            {

                agent.jumper.SetWorldDestination(FleeDestination(agent));
                atDestination = false;
                 
            }

            agent.animator.SetBool(agent.isRunning_hash, true);


            if (agent.jumper.isStuck)
            {

                agent.jumper.SetRandomDestination(fleeDistance);
                agent.jumper.isStuck = false;
                
            }

            agent.jumper.SetWorldDestination(agent.jumper.currentDestination);
            agent.jumper.SetDirection();

            agent.jumper.SetLastPosition();

            if (Vector2.Distance(transform.position, agent.jumper.currentDestination) <= 0.02f)
            {
                atDestination = true;
            }

            fleeTimer += Time.deltaTime;
            if (Vector3.Distance(transform.position, agent.fleeTransfrom.position) >= 3.5f && fleeTimer > 5)
            {
                agent.SetBeliefState("Flee", false);
                agent.currentGoalComplete = true;
            }
        }


        public override void EndPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            atDestination = false;
            fleeTimer = 0;
            agent.animator.SetBool(agent.isRunning_hash, false);
            agent.jumper.currentDirection = Vector2.zero;

            if (agent.currentDisplacementSpot != null)
            {
                agent.currentDisplacementSpot.isInUse = false;
                agent.currentDisplacementSpot = null;
            }

        }

        public Vector2 FleeDestination(SAP_Scheduler_ANIMAL agent)
        {
            Vector2 direction = (Vector2)transform.position - (Vector2)agent.fleeTransfrom.position;
            direction = direction.normalized;
            Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));
            direction = randomRotation * direction;

            Vector2 destination = (Vector2)transform.position + (direction * fleeDistance);
           
            return destination;
        }
    } 
}
