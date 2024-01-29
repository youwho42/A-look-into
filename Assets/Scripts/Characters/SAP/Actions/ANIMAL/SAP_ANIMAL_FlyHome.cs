using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Klaxon.SAP
{
	public class SAP_ANIMAL_FlyHome : SAP_Action
	{
        SAP_WorldBeliefStates worldStates;

        
        

        float timer;
        


        public bool hasFixedHome;
        [ConditionalHide("hasFixedHome", true)]
        public DrawZasYDisplacement home;


        public override void StartPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            worldStates = SAP_WorldBeliefStates.instance;

            if (!hasFixedHome)
                home = agent.CheckForDisplacementSpot();
            

            agent.flier.enabled = true;

            if (agent.walker != null)
            {
                if (agent.walker.enabled)
                {
                    agent.flier.facingRight = agent.walker.facingRight;
                    agent.walker.enabled = false;
                }
            }
            if(home == null)
            {
                agent.currentGoalComplete = true;
                return;
            }
            agent.flier.SetDestination(home);
            agent.flier.isLanding = true;

            timer = agent.SetRandomRange(agent.minMaxFlap);

            agent.animator.SetBool(agent.walking_hash, false);
            agent.animator.SetBool(agent.landed_hash, false);

            if (agent.sounds != null)
            {
                if (agent.sounds.mute)
                    agent.sounds.mute = false; 
            }

            SetBoidsState(agent, false);
        }
        public override void PerformAction(SAP_Scheduler_ANIMAL agent)
        {
            
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                agent.glide = !agent.glide;
                timer = agent.glide ? agent.SetRandomRange(agent.minMaxGlide) : agent.SetRandomRange(agent.minMaxFlap);

                agent.animator.SetBool(agent.gliding_hash, agent.glide);
            }

            if (agent.flier.isStuck || agent.isDeviating || !agent.flier.canReachNextTile)
            {
                agent.DeviateFly();
                return;
            }
            if (home == null)
            {
                agent.currentGoalComplete = true;
                return;
            }

            agent.flier.SetDestination(home);
            if (Vector3.Distance(agent.flier.itemObject.localPosition, home.displacedPosition) <= 0.02f && Vector2.Distance(agent.transform.position, home.transform.position) <= 0.02f)
            {
                home.isInUse = true;
                
                
                if (agent.sounds != null)
                {
                    if (!agent.sounds.mute)
                        agent.sounds.mute = true;
                }
                agent.flier.currentDirection = Vector2.zero;
                
                
                agent.currentGoalComplete = true;

            }
            agent.flier.SetLastPosition();

        }
        public override void EndPerformAction(SAP_Scheduler_ANIMAL agent)
        {

            agent.flier.enabled = true;
            agent.flier.isLanding = false;
            agent.glide = false;
            agent.SetBeliefState("Land", false);
            agent.SetBeliefState("AtHome", true);
            
            if(home != null)
                home.isInUse = false;
            
            
            agent.closestSpots.Clear();

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