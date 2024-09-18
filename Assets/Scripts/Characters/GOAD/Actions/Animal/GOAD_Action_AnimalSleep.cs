using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_AnimalSleep : GOAD_Action
    {
        public GOAD_ScriptableCondition wakeCondition;
        
    public override void StartAction(GOAD_Scheduler_Animal agent)
        {
            base.StartAction(agent);

            

            if(agent.walker != null)
            {
                agent.walker.enabled = true;
                agent.walker.currentDirection = Vector2.zero;
            }
                

            if (agent.flier != null)
            {
                if (agent.walker != null)
                {
                    if (agent.walker.itemObject.localPosition.y != 0)
                        agent.walker.isWeightless = true; 
                }
                if (agent.flier.enabled)
                {
                    if (agent.walker != null)
                        agent.walker.facingRight = agent.flier.facingRight;
                    agent.flier.enabled = false;
                }
            }
            if (agent.jumper != null)
            {
                if (agent.jumper.enabled && agent.walker != null)
                    agent.walker.facingRight = agent.jumper.facingRight;
                agent.jumper.enabled = false;
            }

            agent.SetMusic(false);
            agent.SetHidden(true);

            agent.animator.SetBool(agent.landed_hash, true);
            agent.animator.SetBool(agent.walking_hash, false);
            agent.animator.SetBool(agent.sleeping_hash, true);
            
        }

        public override void PerformAction(GOAD_Scheduler_Animal agent)
        {
            base.PerformAction(agent);
            if (Time.frameCount % 30 != 0)
                return;
            
            if (agent.IsConditionMet(wakeCondition))
            {
                success = true;
                agent.SetActionComplete(true);
            }

        }

        public override void EndAction(GOAD_Scheduler_Animal agent)
        {
            base.EndAction(agent);

            agent.SetHidden(false);
            agent.SetMusic(true);
        }

        

    }
}
