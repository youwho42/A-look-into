using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
	public class SAP_ANIMAL_FlyWander : SAP_Action
	{

        public bool useBoids;
        float timer;
        public Vector2 hoverTime;
        float hoverTimer;
        bool hover;
        public override void StartPerformAction(SAP_Scheduler_ANIMAL agent)
        {

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
            hoverTimer = agent.SetRandomRange(hoverTime);
            timer = agent.SetRandomRange(agent.minMaxFlap);

            agent.animator.SetBool(agent.walking_hash, false);
            agent.animator.SetBool(agent.landed_hash, false);
            agent.animator.SetBool(agent.gliding_hash, false);
            agent.animator.SetBool(agent.sleeping_hash, false);
            if (agent.sounds != null)
            {
                if (agent.sounds.mute)
                    agent.sounds.mute = false; 
            }

            SetBoidsState(agent, useBoids);
        }
        public override void PerformAction(SAP_Scheduler_ANIMAL agent)
        {

            if (hover)
            {
                agent.flier.enabled = false;
                agent.glide = false;
                agent.animator.SetBool(agent.gliding_hash, false);
                hoverTimer -= Time.deltaTime;
                if (hoverTimer <= 0)
                    agent.currentGoalComplete = true;

                return;
            }
            
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                agent.glide = !agent.glide;
                timer = agent.glide ? agent.SetRandomRange(agent.minMaxGlide) : agent.SetRandomRange(agent.minMaxFlap);
                
                agent.animator.SetBool(agent.gliding_hash, agent.glide);
            }

            if (agent.flier.isStuck || agent.isDeviating || !agent.flier.canReachNextTile)
            {
                if (useBoids)
                {
                    SetBoidsState(agent, false);
                    agent.DeviateFly();
                }
                else
                    agent.currentGoalComplete = true;
                
                
                return;
                
            }
            SetBoidsState(agent, useBoids);

            if (Vector2.Distance(agent.flier.itemObject.localPosition, agent.flier.currentDestinationZ) <= 0.02f || agent.flier.CheckDistanceToDestination() <= 0.02f)
            {
                if (hoverTimer > 0)
                    hover = true;
                else
                    agent.currentGoalComplete = true;
            }
            agent.flier.SetLastPosition();
        }
        public override void EndPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            agent.flier.enabled = true;
            hover = false;
            agent.glide = false;
            hoverTimer = 0;
        }

        void SetBoidsState(SAP_Scheduler_ANIMAL agent, bool isInBoids)
        {
            if (agent.flier.boid != null)
            {
                agent.flier.useBoids = isInBoids;
                agent.flier.boid.inBoidPool = isInBoids;
            }

        }

        

    }

}