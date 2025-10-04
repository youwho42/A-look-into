using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_HarvestPlant : GOAD_Action
    {
        bool atPlant;
        bool hasLicked;
        float timer;

        public override void StartAction(GOAD_Scheduler_BP agent)
        {
            base.StartAction(agent);

            agent.arms.SetActive(false);
            agent.animator.SetBool(agent.walking_hash, true);
            agent.currentPlantDestination = GetPlantPosition(agent);
        }

        public override void PerformAction(GOAD_Scheduler_BP agent)
        {
            base.PerformAction(agent);

            if (atPlant)
            {

                if (!hasLicked)
                {
                    agent.animator.SetBool(agent.walking_hash, false);
                    agent.walker.currentDirection = Vector2.zero;
                    agent.animator.SetTrigger(agent.lick_hash);
                    hasLicked = true;
                }
                if (agent.sleep.isSleeping)
                {
                    agent.plantingArea.plantFreeLocations.Add(agent.currentPlantDestination);
                    agent.plantingArea.plantUsedLocations.Remove(agent.currentPlantDestination);
                    agent.plantingArea.harvestablePlants.Remove(agent.currentHarvestable);
                    Destroy(agent.currentHarvestable.gameObject);
                    success = true;
                    agent.SetActionComplete(true);
                    return;
                }

                if (timer < 0.6f && (!agent.offScreen || !agent.sleep.isSleeping))
                    timer += Time.deltaTime;
                else
                {
                    agent.plantingArea.plantFreeLocations.Add(agent.currentPlantDestination);
                    agent.plantingArea.plantUsedLocations.Remove(agent.currentPlantDestination);
                    agent.plantingArea.harvestablePlants.Remove(agent.currentHarvestable);
                    Destroy(agent.currentHarvestable.gameObject);
                    success = true;
                    agent.SetActionComplete(true);
                }
                return;
            }

            if (agent.sleep.isSleeping)
            {
                agent.HandleOffScreen(this, agent.currentPlantDestination);
                return;
            }


            if (agent.walker.isStuck || agent.isDeviating)
            {
                if (!agent.walker.jumpAhead)
                {
                    agent.Deviate();
                    return;
                }
            }

            agent.walker.hasDeviatePosition = false;



            agent.walker.SetWorldDestination(agent.currentPlantDestination);
            agent.walker.SetDirection();
            if (agent.walker.CheckDistanceToDestination() <= 0.0036f)
                atPlant = true;

            agent.walker.SetLastPosition();
        }

        public override void EndAction(GOAD_Scheduler_BP agent)
        {
            base.EndAction(agent);
            atPlant = false;
            hasLicked = false;
            timer = 0;
        }

        Vector3 GetPlantPosition(GOAD_Scheduler_BP agent)
        {

            var pos = agent.plantingArea.harvestablePlants[0].transform.position;
            agent.currentHarvestable = agent.plantingArea.harvestablePlants[0];
            agent.plantingArea.harvestablePlants.RemoveAt(0);
            return pos;
        }

        public override void ReachFinalDestination(GOAD_Scheduler_BP agent)
        {
            base.ReachFinalDestination(agent);
            atPlant = true;
        }
    } 
}
