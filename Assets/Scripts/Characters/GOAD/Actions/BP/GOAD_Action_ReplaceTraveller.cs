using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_ReplaceTraveller : GOAD_Action
    {
        bool disolved;
        float timer;
        public override void StartAction(GOAD_Scheduler_BP agent)
        {
            base.StartAction(agent);
            if (agent.interactor != null)
                agent.interactor.canInteract = false;
            agent.arms.SetActive(false);
            agent.walker.currentDirection = Vector2.zero;
        }
        
        public override void PerformAction(GOAD_Scheduler_BP agent)
        {
            base.PerformAction(agent);
            if (!disolved)
            {
                disolved = true;
                agent.Disolve(false);
            }


            if (timer < 2f)
                timer += Time.deltaTime;
            else
            {
                success = true;
                agent.SetActionComplete(true);
            }
                
        }

        public override void SucceedAction(GOAD_Scheduler_BP agent)
        {
            base.SucceedAction(agent);
        }

        public override void FailAction(GOAD_Scheduler_BP agent)
        {
            base.FailAction(agent);
        }

        public override void EndAction(GOAD_Scheduler_BP agent)
        {
            base.EndAction(agent);
            int index = GetComponent<RandomAccessories>().accessoryIndex;
            Color c = GetComponent<RandomColor>().randomColor;
            BallPeopleManager.instance.SpawnTravellerHome(agent.travellerDestination, transform.position, index, c);
            timer = 0;
            disolved = false;
            Destroy(gameObject);
        }

    }
}
