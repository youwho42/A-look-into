using UnityEngine;

namespace Klaxon.GOAD
{
    public class CokernutIdleOoze : GOAD_Action
    {
        float idleTime;
        public FlumpOozeManager oozeManager;
        public override void StartAction(GOAD_Scheduler_CF agent)
        {
            base.StartAction(agent);
            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.walker.currentDirection = Vector2.zero;
            idleTime = Random.Range(2.2f, 5.5f);
            oozeManager.StartOoze(transform.position, agent.sleep.isSleeping);
        }

        public override void PerformAction(GOAD_Scheduler_CF agent)
        {
            base.PerformAction(agent);

            if (agent.sleep.isSleeping)
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }
            idleTime -= Time.deltaTime;
            if(idleTime < 0)
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }
            
        }

        public override void EndAction(GOAD_Scheduler_CF agent)
        {
            base.EndAction(agent);
            oozeManager.StopOoze();
        }

    }
}
