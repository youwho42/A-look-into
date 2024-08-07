using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_BP_Vanish : SAP_Action
    {
        bool disolved;
        float timer;
        public override void StartPerformAction(SAP_Scheduler_BP agent)
        {
            if(agent.interactor != null)
                agent.interactor.canInteract = false;

            agent.walker.currentDirection = Vector2.zero;
        }
        public override void PerformAction(SAP_Scheduler_BP agent)
        {
            
            if (!disolved)
            {
                disolved = true;
                agent.Disolve(false);
            }


            if (timer < 2f)
                timer += Time.deltaTime;
            else
                agent.currentGoalComplete = true;
                
        }
        public override void EndPerformAction(SAP_Scheduler_BP agent)
        {
            
            timer = 0;
            disolved = false;
            Destroy(gameObject);
        }
    }

}
