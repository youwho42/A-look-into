using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
	public class SAP_ANIMAL_WalkHome : SAP_Action
	{
        SAP_WorldBeliefStates worldStates;

        

        float timer;
        bool isHome;

        public bool hasFixedHome;
        [ConditionalHide("hasFixedHome", true)]
        public DrawZasYDisplacement home;

        public override void StartPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            worldStates = SAP_WorldBeliefStates.instance;

            if (!hasFixedHome)
                home = agent.CheckForDisplacementSpot();

            agent.walker.enabled = true;

            if (agent.flier != null)
            {
                if (agent.flier.enabled)
                {
                    agent.walker.facingRight = agent.flier.facingRight;
                    agent.flier.enabled = false;
                }
            }

            agent.walker.SetWorldDestination(home.transform.position);
            

            
            agent.animator.SetBool(agent.walking_hash, true);

            if (agent.sounds != null)
            {
                if (agent.sounds.mute)
                    agent.sounds.mute = false;
            }

            
        }

        public override void PerformAction(SAP_Scheduler_ANIMAL agent)
        {
            if (worldStates.worldStates.TryGetValue("AnimalDay", out bool isDay))
            {
                bool d = !agent.isNocturnal && isDay || agent.isNocturnal && !isDay;
                agent.currentGoalComplete = d;
            }
            if (isHome)
            {
                if (agent.sounds != null)
                {
                    if (!agent.sounds.mute)
                        agent.sounds.mute = true;
                }
                agent.walker.currentDir = Vector2.zero;
                agent.animator.SetBool(agent.sleeping_hash, true);
                agent.animator.SetBool(agent.walking_hash, false);
                return;
            }

            if (agent.walker.isStuck || agent.isDeviating)
            {

                if (!agent.walker.jumpAhead)
                {
                    agent.DeviateWalk();
                    return;

                }
            }


            agent.walker.SetDirection();

            agent.walker.SetLastPosition();

            if (Vector2.Distance(transform.position, agent.walker.currentDestination) <= 0.02f)
            {
                home.isInUse = true;
                
                isHome = true;
                
            }
        }

        public override void EndPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            
            agent.animator.SetBool(agent.sleeping_hash, false);
            home.isInUse = false;
            
            isHome = false;
            agent.closestSpots.Clear();

        }






    }

}