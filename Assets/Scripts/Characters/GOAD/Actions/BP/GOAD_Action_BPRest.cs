using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_BPRest : GOAD_Action
    {
        float timeIdle;
        bool sleeping;
        public Vector2Int minMaxSittingTime;
        public CycleTicks sitCycle;
        int maxTime;

        public override void StartAction(GOAD_Scheduler_BP agent)
        {
            base.StartAction(agent);
            maxTime = Random.Range(minMaxSittingTime.x, minMaxSittingTime.y);
            sitCycle = RealTimeDayNightCycle.instance.GetCycleTime(maxTime);
            agent.arms.SetActive(false);
            agent.walker.currentDirection = Vector2.zero;
            agent.animator.SetBool(agent.walking_hash, false);
            agent.walker.ResetLastPosition();
        }

        public override void PerformAction(GOAD_Scheduler_BP agent)
        {
            base.PerformAction(agent);
            if (RealTimeDayNightCycle.instance.currentTimeRaw >= sitCycle.tick && RealTimeDayNightCycle.instance.currentDayRaw == sitCycle.day)
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }

            if (sleeping)
            {
                agent.animator.SetBool(agent.sleeping_hash, true);
                if (agent.CheckNearPlayer(1f))
                    sleeping = false;
                return;
            }

            agent.animator.SetBool(agent.sleeping_hash, false);
            timeIdle += Time.deltaTime;
            if (timeIdle >= 10 && !agent.CheckNearPlayer(1.5f))
            {
                sleeping = true;
                timeIdle = 0;
            }
        }

        public override void EndAction(GOAD_Scheduler_BP agent)
        {
            base.EndAction(agent);
            timeIdle = 0;
            sleeping = false;
        }
    }
}
