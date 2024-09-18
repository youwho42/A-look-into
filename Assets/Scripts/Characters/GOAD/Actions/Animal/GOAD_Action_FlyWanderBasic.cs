using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_FlyWanderBasic : GOAD_Action
    {

        float glideTimer;

        public Vector2 minMaxWanderTime = new Vector2(5, 15);
        float wanderTimer;


        public override void StartAction(GOAD_Scheduler_Animal agent)
        {
            base.StartAction(agent);
            
            if (agent.flier != null)
                agent.flier.enabled = true;

            if (agent.walker != null)
            {
                if (agent.walker.enabled)
                {
                    agent.flier.facingRight = agent.walker.facingRight;
                    agent.walker.enabled = false;
                }
            }
            agent.flier.SetRandomDestination();
            agent.animator.SetBool(agent.walking_hash, false);
            agent.animator.SetBool(agent.landed_hash, false);
            agent.animator.SetBool(agent.gliding_hash, false);
            agent.animator.SetBool(agent.sleeping_hash, false);
            if (agent.sounds != null)
            {
                if (agent.sounds.mute)
                    agent.sounds.mute = false;
            }
            glideTimer = agent.SetRandomRange(agent.minMaxFlap);
            wanderTimer = agent.SetRandomRange(minMaxWanderTime);
            
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

            if (glideTimer <= 0)
            {
                agent.glide = !agent.glide;
                glideTimer = agent.glide ? agent.SetRandomRange(agent.minMaxGlide) : agent.SetRandomRange(agent.minMaxFlap);
                agent.animator.SetBool(agent.gliding_hash, agent.glide);
            }




            if (agent.flier.isStuck || agent.isDeviating || !agent.flier.canReachNextTile)
            {
                agent.DeviateFly();
                return;
            }



            if (Vector2.Distance(agent.flier.itemObject.localPosition, agent.flier.currentDestinationZ) <= 0.02f || agent.flier.CheckDistanceToDestination() <= 0.02f)
                agent.flier.SetRandomDestination();
                
            
        }

        public override void EndAction(GOAD_Scheduler_Animal agent)
        {
            base.EndAction(agent);
            agent.flier.enabled = true;
            agent.glide = false;
            agent.flier.isStuck = false;
            agent.isDeviating = false;
        }

        
    }
}
