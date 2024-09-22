using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_Eat : GOAD_Action
    {

        public Vector2 minMaxEatTime;
        float eatTimer;
        bool swayed;


        public override void StartAction(GOAD_Scheduler_Animal agent)
        {
            base.StartAction(agent);

            agent.walker.currentDirection = Vector2.zero;
            agent.animator.SetBool(agent.landed_hash, true);
            agent.animator.SetBool(agent.walking_hash, false);
            
            eatTimer = Random.Range(minMaxEatTime.x, minMaxEatTime.y);
        }

        public override void PerformAction(GOAD_Scheduler_Animal agent)
        {
            base.PerformAction(agent);

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
                agent.AddEatAmount();
                Destroy(agent.currentEdible);
                agent.currentEdible = null;
                agent.isEating = false;
                success = true;
                agent.SetActionComplete(true);

            }


        }

        public override void EndAction(GOAD_Scheduler_Animal agent)
        {
            base.EndAction(agent);
        }

        void SwayItem(GOAD_Scheduler_Animal agent)
        {
            swayed = true;
            if (agent.currentEdible.TryGetComponent(out GrassSway sway))
                sway.SwayMedium();
        }
    }
}
