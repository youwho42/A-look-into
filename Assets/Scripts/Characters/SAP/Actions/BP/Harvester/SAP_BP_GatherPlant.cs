using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Klaxon.SAP
{
    public class SAP_BP_GatherPlant : SAP_Action
    {
        bool atPlant;
        bool hasLicked;
        float timer;
        

        public override void StartPerformAction(SAP_Scheduler_BP agent)
        {
            agent.arms.SetActive(false);
            agent.animator.SetBool(agent.walking_hash, true);
            agent.currentPlantDestination = GetPlantPosition(agent);
        }
        public override void PerformAction(SAP_Scheduler_BP agent)
        {
            if (atPlant)
            {

                if (!hasLicked)
                {
                    agent.animator.SetBool(agent.walking_hash, false);
                    agent.walker.currentDir = Vector2.zero;
                    agent.animator.SetTrigger(agent.lick_hash);
                    hasLicked = true;
                }


                if (timer < 0.6f)
                    timer += Time.deltaTime;
                else
                {
                    agent.plantingArea.plantFreeLocations.Add(agent.currentPlantDestination);
                    agent.plantingArea.plantUsedLocations.Remove(agent.currentPlantDestination);
                    agent.plantingArea.harvestablePlants.Remove(agent.currentHarvestable);
                    Destroy(agent.currentHarvestable.gameObject);
                    agent.SetBeliefState("HasPlant", true);
                    agent.SetBeliefState("PlantAvailable", false);
                    agent.currentGoalComplete = true;
                }
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
            if (agent.walker.CheckDistanceToDestination() <= 0.06f)
                atPlant = true;

            agent.walker.SetLastPosition();
        }
        public override void EndPerformAction(SAP_Scheduler_BP agent)
        {
            atPlant = false;
            hasLicked = false;
            timer = 0;
        }
        Vector3 GetPlantPosition(SAP_Scheduler_BP agent)
        {

            var pos = agent.plantingArea.harvestablePlants[0].transform.position;
            agent.currentHarvestable = agent.plantingArea.harvestablePlants[0];
            agent.plantingArea.harvestablePlants.RemoveAt(0);
            return pos;
        }
    }
}
