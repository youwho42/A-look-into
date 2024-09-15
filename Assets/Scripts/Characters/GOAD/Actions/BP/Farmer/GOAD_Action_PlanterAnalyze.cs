using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Action_PlanterAnalyze : GOAD_Action
	{
        CycleTicks endCycleTicks;
        public override void StartAction(GOAD_Scheduler_BP agent)
        {
            base.StartAction(agent);

            endCycleTicks = RealTimeDayNightCycle.instance.GetCycleTime(20);
            agent.arms.SetActive(false);
            agent.plantingArea.CheckForPlantable();
            agent.walker.currentDirection = Vector2.zero;
            agent.animator.SetBool(agent.walking_hash, false);
            agent.walker.ResetLastPosition();
        }

        public override void PerformAction(GOAD_Scheduler_BP agent)
        {
            base.PerformAction(agent);

            if (Time.frameCount % 60 == 0)
                agent.plantingArea.CheckForPlantable();

            if (agent.plantingArea.canPlant)
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }

            if (RealTimeDayNightCycle.instance.currentTimeRaw >= endCycleTicks.tick && endCycleTicks.day == RealTimeDayNightCycle.instance.currentDayRaw)
            {
                agent.plantingArea.ballPersonPlanterActive = false;
                success = false;
                agent.SetActionComplete(true);
                return;

            }

        }

    } 
}
