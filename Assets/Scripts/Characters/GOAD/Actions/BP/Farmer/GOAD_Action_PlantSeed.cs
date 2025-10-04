using Klaxon.Interactable;
using Klaxon.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_PlantSeed : GOAD_Action
    {
        bool isPlanting;
        bool hasLicked;
        float timer;
        public override void StartAction(GOAD_Scheduler_BP agent)
        {
            base.StartAction(agent);
            agent.arms.SetActive(false);
            if (agent.plantingArea.plantFreeLocations.Count > 0)
            {
                agent.currentPlantDestination = agent.plantingArea.plantFreeLocations[0];
                agent.plantingArea.plantFreeLocations.RemoveAt(0);

            }
            agent.animator.SetBool(agent.walking_hash, true);
        }

        public override void PerformAction(GOAD_Scheduler_BP agent)
        {
            base.PerformAction(agent);
            if (isPlanting)
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
                    PlantSeed(agent);
                    success = true;
                    agent.SetActionComplete(true);
                    return;
                }
                if (timer < 0.6f)
                    timer += Time.deltaTime;
                else
                {
                    PlantSeed(agent);
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
                isPlanting = true;

            agent.walker.SetLastPosition();

        }
       
        public override void EndAction(GOAD_Scheduler_BP agent)
        {
            base.EndAction(agent);
            isPlanting = false;
            hasLicked = false;
            timer = 0;
        }
        void PlantSeed(GOAD_Scheduler_BP agent)
        {
            agent.plantingArea.plantUsedLocations.Add(agent.currentPlantDestination);
            var go = Instantiate(agent.plantingArea.seedItem.plantedObject.ItemPrefabVariants[0], agent.currentPlantDestination, Quaternion.identity);
            go.GetComponent<InteractableFarmPlant>().plantingArea = agent.plantingArea;
            go.GetComponent<SaveableItemEntity>().GenerateId();
            PlantLife plant = go.GetComponent<PlantLife>();
            plant.SetSprites();
            plant.SetNextCycleTime();
        }

        public override void ReachFinalDestination(GOAD_Scheduler_BP agent)
        {
            isPlanting = true;
        }
    }

}