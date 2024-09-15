using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_FlyWander : GOAD_Action
    {

        public bool useBoids;
        float timer;
        public Vector2 minMaxWanderTime;
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
            timer = agent.SetRandomRange(agent.minMaxFlap);
            wanderTimer = agent.SetRandomRange(minMaxWanderTime);
            agent.SetBoidsState(useBoids);
            
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



            timer -= Time.deltaTime;
            
            if (timer <= 0)
            {
                agent.glide = !agent.glide;
                timer = agent.glide ? agent.SetRandomRange(agent.minMaxGlide) : agent.SetRandomRange(agent.minMaxFlap);

                agent.animator.SetBool(agent.gliding_hash, agent.glide);
            }

            agent.SetBoidsState(useBoids);

            if (agent.flier.isStuck || agent.isDeviating || !agent.flier.canReachNextTile)
            {
                if (useBoids)
                {
                    agent.SetBoidsState(false);
                    agent.DeviateFly();
                    return;
                }
                success = true;
                agent.SetActionComplete(true);


                return;

            }

            
            if(!useBoids)
            { 
            
                if (Vector2.Distance(agent.flier.itemObject.localPosition, agent.flier.currentDestinationZ) <= 0.02f || agent.flier.CheckDistanceToDestination() <= 0.02f)
                {
                    success = true;
                    agent.SetActionComplete(true);
                }
            }
            agent.flier.SetLastPosition();


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
