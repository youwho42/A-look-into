using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_BP_TravellerHome : SAP_Action
    {

        float timeIdle;
        bool sleeping;

        public override void StartPerformAction(SAP_Scheduler_BP agent)
        {
            agent.arms.SetActive(false);
            agent.walker.currentDir = Vector2.zero;
            agent.animator.SetBool(agent.walking_hash, false);
            agent.walker.ResetLastPosition();
        }
        public override void PerformAction(SAP_Scheduler_BP agent)
        {
            if (sleeping)
            {
                agent.animator.SetBool(agent.sleeping_hash, true);
                if (agent.CheckPlayerDistance() <= 1f)
                    sleeping = false;
                return;
            }

            agent.animator.SetBool(agent.sleeping_hash, false);
            timeIdle += Time.deltaTime;
            if(timeIdle >= 10 && agent.CheckPlayerDistance() > 1.5f)
            { 
                sleeping = true;
                timeIdle = 0;
            }
        }
        public override void EndPerformAction(SAP_Scheduler_BP agent)
        {
            timeIdle = 0;
            sleeping = false;
        }

        
    }

}
