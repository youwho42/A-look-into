using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_RobotGather : GOAD_Action
    {
        bool isGathering;
        float timer = 0;
        [Range(0.0f, 1.0f)]
        public float gatherChance;
        bool gathered;
        public override void StartAction(GOAD_Scheduler_Robot agent)
        {
            base.StartAction(agent);
            if(Random.Range(0.0f, 1.0f) > gatherChance)
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }
            agent.robotLights.SetCurrentFunction(RobotLightManager.RobotStates.Gathering);
        }

        public override void PerformAction(GOAD_Scheduler_Robot agent)
        {
            base.PerformAction(agent);

            if (agent.sleep.isSleeping)
            {
                agent.HandleSleepGather(this);
                return;
            }

            float dir = agent.animator.GetFloat(agent.Direction_hash);
            if (dir > 0)
            {
                agent.animator.SetFloat(agent.Direction_hash, dir - Time.deltaTime);
                return;
            }

            if (!isGathering)
            {
                isGathering = true;
                agent.animator.SetTrigger(agent.Gather_hash);
                return;
            }
            timer += Time.deltaTime;
            if (timer >= 3.5f && !gathered)
            {
                gathered = true;
                agent.agentInventory.AddItem(agent.interactable.robotPriorities[agent.interactable.currentPriority].PriorityDatabase.GetRandomWeightedItem(), 1, false);
                agent.robotLights.SetInventoryLights();
            }
            if (timer >= 6)
            {
                success = true;
                agent.SetActionComplete(true);
            }
            
            

        }

        public override void EndAction(GOAD_Scheduler_Robot agent)
        {
            base.EndAction(agent);
            isGathering = false;
            timer = 0;
            gathered = false;
        }

        
    }
}
