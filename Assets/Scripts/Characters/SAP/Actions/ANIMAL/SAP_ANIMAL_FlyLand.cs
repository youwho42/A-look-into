using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
	public class SAP_ANIMAL_FlyLand : SAP_Action
	{

        float timer;
        Bounds bounds;
        public float minDistance = 1f;
        public List<DrawZasYDisplacement> closestSpots = new List<DrawZasYDisplacement>();
      
        public override void StartPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            if (agent.walker != null)
                agent.walker.enabled = false;

            bounds = new Bounds(transform.position, new Vector3(4, 4, 4));
            closestSpots = agent.interactAreas.quadTree.QueryTree(bounds);
            agent.currentLandingSpot = CheckForLandingArea(agent);

            if (agent.currentLandingSpot == null)
            {
                agent.currentGoalComplete = true;
                return;
            }


            agent.flier.SetDestination(agent.currentLandingSpot);
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
            
            

            if (agent.flier.isStuck || agent.isDeviating || !agent.flier.canReachNextTile)
            {
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

            agent.flier.SetDestination(agent.currentLandingSpot);
            if (Vector3.Distance(agent.flier.itemObject.localPosition, agent.flier.currentDestinationZ) <= 0.02f && Vector2.Distance(agent.transform.position, agent.flier.currentDestination) <= 0.02f)
            {
                agent.currentGoalComplete = true;
                agent.SetBeliefState("Landed", true);
            }
            agent.flier.SetLastPosition();

        }
        public override void EndPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            agent.flier.isLanding = false;
            agent.glide = false;
            agent.SetBeliefState("Land", false);
            closestSpots.Clear();
            
        }

        void SetBoidsState(SAP_Scheduler_ANIMAL agent, bool isInBoids)
        {
            if (agent.flier.boid != null)
            {
                agent.flier.useBoids = isInBoids;
                agent.flier.boid.inBoidPool = isInBoids;
            }

        }

        DrawZasYDisplacement CheckForLandingArea(SAP_Scheduler_ANIMAL agent)
        {
            if (closestSpots.Count <= 0)
                return null;

            DrawZasYDisplacement bestTarget = null;
            float closestDistance = Mathf.Infinity;
            Vector2 currentPosition = transform.position;
            foreach (var item in closestSpots)
            {
                if (item == null || item.isInUse || item.transform.position.z != transform.position.z)
                    continue;
                var dist = Vector2.Distance(currentPosition, item.transform.position);

                if (dist < closestDistance)
                {
                    if (dist < minDistance)
                        continue;
                    closestDistance = dist;
                    bestTarget = item;
                }
            }

            if (bestTarget == null)
                return null;

            agent.currentLandingSpot = bestTarget;
            agent.currentLandingSpot.isInUse = true;

            return bestTarget;

        }

    }

}