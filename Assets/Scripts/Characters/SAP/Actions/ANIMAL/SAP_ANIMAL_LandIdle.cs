using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
	public class SAP_ANIMAL_LandIdle : SAP_Action
	{

        public Vector2 headTimeRange;
        float headTimer;
        
        public override void StartPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            agent.animator.SetBool(agent.landed_hash, true);

            agent.flier.enabled = false;
            headTimer = agent.SetRandomRange(headTimeRange);

            if (agent.sounds != null)
            {
                if (agent.sounds.continuous)
                    agent.sounds.mute = true; 
            }
        }
        public override void PerformAction(SAP_Scheduler_ANIMAL agent)
        {
            if (headTimeRange.y <= 0)
                return;
            headTimer -= Time.deltaTime;
            if (headTimer <= 0)
                TurnHead(agent);
        }
        public override void EndPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            headTimer = 0;

            if (agent.currentDisplacementSpot != null)
                agent.currentDisplacementSpot.isInUse = false;
            agent.currentDisplacementSpot = null;

            if (agent.sounds != null)
            {
                if (agent.sounds.continuous)
                    agent.sounds.mute = false;
            }

            agent.flier.enabled = true;
        }

        void TurnHead(SAP_Scheduler_ANIMAL agent)
        {
            headTimer = agent.SetRandomRange(headTimeRange);
            agent.animator.SetTrigger(agent.idle_hash);
        }

    } 
}
