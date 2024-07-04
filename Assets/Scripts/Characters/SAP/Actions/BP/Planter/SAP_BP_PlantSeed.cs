using Klaxon.SaveSystem;
using Klaxon.Interactable;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_BP_PlantSeed : SAP_Action
    {
        bool isPlanting;
        bool hasLicked;
        float timer;
        public override void StartPerformAction(SAP_Scheduler_BP agent)
        {
            agent.arms.SetActive(false);
            if (agent.plantingArea.plantFreeLocations.Count > 0)
            {
                agent.currentPlantDestination = agent.plantingArea.plantFreeLocations[0];
                agent.plantingArea.plantFreeLocations.RemoveAt(0);
                
            }
            agent.animator.SetBool(agent.walking_hash, true);
        }
        public override void PerformAction(SAP_Scheduler_BP agent)
        {

            if (isPlanting)
            {

                if (!hasLicked)
                {
                    agent.animator.SetBool(agent.walking_hash, false);
                    agent.walker.currentDirection = Vector2.zero;
                    agent.animator.SetTrigger(agent.lick_hash);
                    hasLicked = true;
                }

                if(agent.sleep.isSleeping)
                {
                    PlantSeed(agent);
                    agent.SetBeliefState("HasSeed", false);
                    agent.currentGoalComplete = true;
                    return;
                }
                if (timer < 0.6f)
                    timer += Time.deltaTime;
                else
                {
                    PlantSeed(agent);
                    agent.SetBeliefState("HasSeed", false);
                    agent.currentGoalComplete = true;
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
            if (agent.walker.CheckDistanceToDestination() <= 0.06f)
                isPlanting = true;

            agent.walker.SetLastPosition();

        }
        public override void EndPerformAction(SAP_Scheduler_BP agent)
        {
            isPlanting = false;
            hasLicked = false;
            timer = 0;
        }

        void PlantSeed(SAP_Scheduler_BP agent)
        {
            agent.plantingArea.plantUsedLocations.Add(agent.currentPlantDestination);
            var go = Instantiate(agent.plantingArea.seedItem.plantedObject.ItemPrefabVariants[0], agent.currentPlantDestination, Quaternion.identity);
            go.GetComponent<InteractableFarmPlant>().plantingArea = agent.plantingArea;
            go.GetComponent<SaveableItemEntity>().GenerateId();
            PlantLife plant = go.GetComponent<PlantLife>();
            plant.SetSprites();
            plant.SetNextCycleTime();
        }

        public override void ReachFinalDestination(SAP_Scheduler_BP agent)
        {
            isPlanting = true;
        }
    }

}

