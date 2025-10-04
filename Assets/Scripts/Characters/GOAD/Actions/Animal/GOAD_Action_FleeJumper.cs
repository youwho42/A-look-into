using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_FleeJumper : GOAD_Action
    {
        public float fleeDistance;
       
        public override void StartAction(GOAD_Scheduler_Animal agent)
        {
            base.StartAction(agent);

            agent.animator.SetBool(agent.landed_hash, true);
            agent.animator.SetBool(agent.walking_hash, false);
            agent.animator.SetBool(agent.isRunning_hash, true);


            if (agent.jumper != null)
                agent.jumper.enabled = true;
            if (agent.walker != null)
            {
                if (agent.walker.enabled)
                    agent.jumper.facingRight = agent.walker.facingRight;
                
                agent.walker.enabled = false;
            }


            if (agent.flier != null)
            {
                if (agent.flier.enabled)
                {
                    if (agent.walker != null)
                        agent.jumper.facingRight = agent.flier.facingRight;
                    agent.flier.enabled = false;
                }
            }

            agent.jumper.SetWorldDestination(FleeDestination(agent));
        }

        public override void PerformAction(GOAD_Scheduler_Animal agent)
        {
            base.PerformAction(agent);

            
            agent.animator.SetBool(agent.isRunning_hash, true);


            if (agent.jumper.isStuck)
            {

                agent.jumper.SetRandomDestination(fleeDistance);
                agent.jumper.isStuck = false;

            }

            agent.jumper.SetWorldDestination(agent.jumper.currentDestination);
            
            agent.jumper.SetDirection();

            agent.jumper.SetLastPosition();

            if (NumberFunctions.GetDistanceV2(transform.position, agent.jumper.currentDestination) <= GlobalSettings.DistanceCheck)
            {
                success = true;
                agent.SetActionComplete(true);
            }

            
        }

        public override void EndAction(GOAD_Scheduler_Animal agent)
        {
            base.EndAction(agent);
            agent.animator.SetBool(agent.isRunning_hash, false);
        }

        public Vector2 FleeDestination(GOAD_Scheduler_Animal agent)
        {
            Vector2 direction = (Vector2)transform.position - (Vector2)PlayerInformation.instance.player.position;
            direction = direction.normalized;
            //Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));
            //direction = randomRotation * direction;

            Vector2 destination = (Vector2)transform.position + (direction * fleeDistance);

            return destination;
        }

    }
}
