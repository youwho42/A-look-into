using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_ANIMAL_ChickenPeck : SAP_Action
    {

        float peckTimer;

        static int peck_hash = Animator.StringToHash("Peck");

        public override void StartPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            peckTimer = Random.Range(1, 3);
        }
        public override void PerformAction(SAP_Scheduler_ANIMAL agent)
        {

            peckTimer -= Time.deltaTime;
            agent.animator.SetBool(agent.walking_hash, false);
            if (peckTimer >= 0)
                return;
            if (!AnimatorIsPlaying(agent, "NewChickenPeck"))
            {
                peckTimer = Random.Range(1, 3);
                agent.animator.SetTrigger(peck_hash);
                agent.animator.SetBool(peck_hash, true);
            }

        }
        public override void EndPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            agent.SetBeliefState("Peck", false);
        }

        bool AnimatorIsPlaying(SAP_Scheduler_ANIMAL agent, string stateName)
        {
            if (agent.animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            {
                if (agent.animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
                    return true;
            }
            return false;
        }

    }

}