using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_SitOnGround : GOAD_Action
    {

        public Vector2Int minMaxSittingTime;
        public CycleTicks sitCycle;
        int maxTime;
        RealTimeDayNightCycle dayNightCycle;
        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            base.StartAction(agent);
            dayNightCycle = RealTimeDayNightCycle.instance;
            maxTime = Random.Range(minMaxSittingTime.x, minMaxSittingTime.y);
            sitCycle = RealTimeDayNightCycle.instance.GetCycleTime(maxTime);
            agent.walker.currentDirection = Vector2.zero;
            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);
            agent.animator.SetBool(agent.isIdleSitting_hash, true);
        }

        public override void PerformAction(GOAD_Scheduler_NPC agent)
        {
            base.PerformAction(agent);
            if (dayNightCycle.currentTimeRaw >= sitCycle.tick && dayNightCycle.currentDayRaw == sitCycle.day)
            {
                success = true;
                agent.SetActionComplete(true);
            }
        }
        public override void EndAction(GOAD_Scheduler_NPC agent)
        {
            base.EndAction(agent);
            agent.animator.SetBool(agent.isIdleSitting_hash, false);
        }
        
    }
}
