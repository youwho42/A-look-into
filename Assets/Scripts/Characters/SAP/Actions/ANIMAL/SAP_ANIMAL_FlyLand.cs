using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
	public class SAP_ANIMAL_FlyLand : SAP_Action
	{

        float timer;
        
      
        public override void StartPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            if (agent.walker != null)
                agent.walker.enabled = false;

            
            //agent.closestSpots = agent.interactAreas.quadTree.QueryTree(agent.bounds);
            agent.currentDisplacementSpot = agent.CheckForDisplacementSpot();

            if (agent.currentDisplacementSpot == null)
            {
                agent.currentGoalComplete = true;
                return;
            }


            agent.flier.SetDestination(agent.currentDisplacementSpot);
            agent.flier.isLanding = true;
            
            timer = agent.SetRandomRange(agent.minMaxFlap);

            agent.animator.SetBool(agent.walking_hash, false);
            agent.animator.SetBool(agent.landed_hash, false);

            if(agent.sounds != null)
            {
                if (agent.sounds.mute)
                    agent.sounds.mute = false;
            }
            

            SetBoidsState(agent, false);
        }
        public override void PerformAction(SAP_Scheduler_ANIMAL agent)
        {

            if (agent.sleep.isSleeping)
            {
                agent.HandleOffScreen(this);
                return;
            }

            

            if (agent.flier.isStuck || agent.isDeviating || !agent.flier.canReachNextTile )
            {

                if (Vector3.Distance(agent.flier.itemObject.localPosition, agent.flier.mainDestinationZ) <= 0.03f && Vector2.Distance(agent.transform.position, agent.flier.currentDestination) <= 0.03f)
                {
                    agent.currentGoalComplete = true;
                    agent.SetBeliefState("Landed", true);
                    agent.flier.SetLastPosition();
                    return;
                }
                agent.DeviateFly();
                return;
            }
            


            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                agent.glide = !agent.glide;
                timer = agent.glide ? agent.SetRandomRange(agent.minMaxGlide) : agent.SetRandomRange(agent.minMaxFlap);

                agent.animator.SetBool(agent.gliding_hash, agent.glide);
            }
            
            agent.flier.SetDestination(agent.currentDisplacementSpot);


            //if (Vector3.Distance(agent.flier.itemObject.localPosition, agent.flier.currentDestinationZ) <= 0.02f && Vector2.Distance(agent.transform.position, agent.flier.currentDestination) <= 0.02f)
            //{
            //    agent.currentGoalComplete = true;
            //    agent.SetBeliefState("Landed", true);
            //}
            if (agent.currentDisplacementSpot.positionZ == 0)
            {
                if (Vector3.Distance(agent.flier.itemObject.localPosition, agent.flier.mainDestinationZ) <= 0.03f)
                {
                    agent.currentGoalComplete = true;
                    agent.SetBeliefState("Landed", true);
                }
            }
            else
            {
                if (Vector3.Distance(agent.flier.itemObject.localPosition, agent.flier.mainDestinationZ) <= 0.03f && Vector2.Distance(agent.transform.position, agent.flier.currentDestination) <= 0.03f)
                {
                    agent.currentGoalComplete = true;
                    agent.SetBeliefState("Landed", true);
                }
            }

            agent.flier.SetLastPosition();

        }
        public override void EndPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            agent.flier.isLanding = false;
            agent.glide = false;
            agent.SetBeliefState("Land", false);
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