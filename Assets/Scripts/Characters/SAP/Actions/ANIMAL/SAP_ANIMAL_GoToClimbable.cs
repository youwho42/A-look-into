using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
	public class SAP_ANIMAL_GoToClimbable : SAP_Action
	{


        
        


        public override void StartPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            

            
            agent.currentDisplacementSpot = agent.CheckForDisplacementSpot();

            agent.walker.enabled = true;

            if (agent.flier != null)
            {
                if (agent.flier.enabled)
                {
                    agent.walker.facingRight = agent.flier.facingRight;
                    agent.flier.enabled = false;
                }
            }

            agent.walker.SetWorldDestination(agent.currentDisplacementSpot.transform.position);



            agent.animator.SetBool(agent.walking_hash, true);

            if (agent.sounds != null)
            {
                if (agent.sounds.mute)
                    agent.sounds.mute = false;
            }


        }

        public override void PerformAction(SAP_Scheduler_ANIMAL agent)
        {

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
                agent.currentDisplacementSpot.isInUse = true;

                agent.currentGoalComplete = true;

            }
        }

        public override void EndPerformAction(SAP_Scheduler_ANIMAL agent)
        {

            agent.SetBeliefState("AtClimbable", true);
            agent.closestSpots.Clear();

        }


        


    }

}