using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_BP_PlanterAnalyze : SAP_Action
    {
        float timer;
        public override void StartPerformAction(SAP_Scheduler_BP agent)
        {
            agent.arms.SetActive(false);
            agent.plantingArea.CheckForPlantable();
            agent.walker.currentDir = Vector2.zero;
            agent.animator.SetBool(agent.walking_hash, false);
            agent.walker.ResetLastPosition();
        }
        public override void PerformAction(SAP_Scheduler_BP agent)
        {

            if (Time.frameCount % 60 == 0)
                agent.plantingArea.CheckForPlantable();

            timer += Time.deltaTime;

            if (agent.plantingArea.canPlant)
            {
                agent.SetBeliefState("SeedAvailable", true);
                agent.currentGoalComplete = true;
                return;
            }

            if(timer >= 10f)
            {
                agent.SetBeliefState("QuestOver", true);
                
                agent.plantingArea.ballPersonPlanterActive = false;
                agent.currentGoalComplete = true;
                return;
                
            }
            


        }
        public override void EndPerformAction(SAP_Scheduler_BP agent)
        {
            timer = 0;
            
        }
    }
}

