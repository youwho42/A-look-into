using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_ChickenPeck : GOAD_Action
    {
        public Vector2 minMaxPeckTime = new Vector2(3, 8);
        float maxPeckTime;
        
        float peckTimer;

        static int peck_hash = Animator.StringToHash("Peck");
        
        public override void StartAction(GOAD_Scheduler_Animal agent)
        {
            base.StartAction(agent);
            peckTimer = agent.SetRandomRange(new Vector2(0.5f, 2.0f));
            maxPeckTime = agent.SetRandomRange(minMaxPeckTime);
            agent.animator.SetBool(agent.walking_hash, false);
        }

        public override void PerformAction(GOAD_Scheduler_Animal agent)
        {
            base.PerformAction(agent);


            maxPeckTime -= Time.deltaTime;
            
            if (maxPeckTime <= 0 && !AnimatorIsPlaying(agent, "NewChickenPeck"))
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }
                


            peckTimer -= Time.deltaTime;
            
            if (peckTimer >= 0)
                return;
            if (!AnimatorIsPlaying(agent, "NewChickenPeck"))
            {
                peckTimer = agent.SetRandomRange(new Vector2(0.5f, 2.0f));
                agent.animator.SetTrigger(peck_hash);
                agent.animator.SetBool(peck_hash, true);
            }


        }

        public override void EndAction(GOAD_Scheduler_Animal agent)
        {
            base.EndAction(agent);
        }

        bool AnimatorIsPlaying(GOAD_Scheduler_Animal agent, string stateName)
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
