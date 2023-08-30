using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Klaxon.SAP
{
	public class SAP_ANIMAL_FlyHome : SAP_Action
	{
        SAP_WorldBeliefStates worldStates;

        
        public DrawZasYDisplacement home;

        float timer;
        bool isHome;
        Bounds bounds;
        public bool isNocturnal;

        public bool chooseRandomHome;
        [ConditionalHide("chooseRandomHome", true)]
        public float minHomeDistance = 1f;
        List<DrawZasYDisplacement> closestSpots = new List<DrawZasYDisplacement>();


        public override void StartPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            worldStates = SAP_WorldBeliefStates.instance;

            if (chooseRandomHome)
            {
                bounds = new Bounds(transform.position, new Vector3(4, 4, 4));
                closestSpots = agent.interactAreas.quadTree.QueryTree(bounds);
                home = CheckForLandingArea(agent);
            }

            agent.flier.enabled = true;

            if (agent.walker != null)
            {
                if (agent.walker.enabled)
                {
                    agent.flier.facingRight = agent.walker.facingRight;
                    agent.walker.enabled = false;
                }
            }

            agent.flier.SetDestination(home);
            agent.flier.isLanding = true;

            timer = agent.SetRandomRange(agent.minMaxFlap);

            agent.animator.SetBool(agent.walking_hash, false);
            agent.animator.SetBool(agent.landed_hash, false);

            if (agent.sounds.mute)
                agent.sounds.mute = false;

            SetBoidsState(agent, false);
        }
        public override void PerformAction(SAP_Scheduler_ANIMAL agent)
        {
            if (worldStates.worldStates.TryGetValue("AnimalDay", out bool isDay))
            {
                bool d = !isNocturnal && isDay || isNocturnal && !isDay;
                agent.currentGoalComplete = d;
            }

            if (isHome)
            {
                agent.flier.enabled = false;
                if (!agent.sounds.mute)
                    agent.sounds.mute = true;
                agent.flier.currentDirection = Vector2.zero;
                agent.animator.SetBool(agent.landed_hash, true);
                agent.animator.SetBool(agent.sleeping_hash, true);
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
                agent.DeviateFly();
                return;
            }

            agent.flier.SetDestination(home);
            if (Vector3.Distance(agent.flier.itemObject.localPosition, home.displacedPosition) <= 0.02f && Vector2.Distance(agent.transform.position, home.transform.position) <= 0.02f)
            {
                home.isInUse = true;
                isHome = true;
            }
            agent.flier.SetLastPosition();

        }
        public override void EndPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            agent.flier.enabled = true;
            agent.flier.isLanding = false;
            agent.glide = false;
            agent.SetBeliefState("Land", false);
            agent.animator.SetBool(agent.sleeping_hash, false);
            home.isInUse = false;
            
            isHome = false;
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
                    if (dist < minHomeDistance)
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