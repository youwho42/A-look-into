using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
	public class SAP_ANIMAL_Eat : SAP_Action
	{
        public Vector2 minMaxEatTime;
        float eatTimer;
        bool swayed;
        public override void StartPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            agent.walker.currentDirection = Vector2.zero;
            agent.animator.SetBool(agent.landed_hash, true);
            agent.animator.SetBool(agent.walking_hash, false);
            if (agent.flier != null)
                agent.flier.enabled = false;
            eatTimer = Random.Range(minMaxEatTime.x, minMaxEatTime.y);
        }
        public override void PerformAction(SAP_Scheduler_ANIMAL agent)
        {
            eatTimer -= Time.deltaTime;
            if (eatTimer > 0)
            {

                if (!swayed)
                    SwayItem(agent);

                agent.animator.SetBool(agent.isEating_hash, true);
            }
            else
            {
                
                agent.animator.SetBool(agent.isEating_hash, false);
                
                Destroy(agent.currentEdible);
                agent.currentEdible = null;
                agent.isEating = false;
                agent.SetBeliefState("Eating", false);
                agent.currentGoalComplete = true;
                
            }
            

        }
        void SwayItem(SAP_Scheduler_ANIMAL agent)
        {
            swayed = true;
            if (agent.currentEdible.TryGetComponent(out GrassSway sway))
                sway.SwayItem();
        }

        public override void EndPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            swayed = false;
            
        }

    }

}