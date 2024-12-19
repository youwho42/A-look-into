using System.Collections;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_RobotRest : GOAD_Action
    {
        bool inPosition;
        bool hasSetAnim;
        bool wake;
        public override void StartAction(GOAD_Scheduler_Robot agent)
        {
            base.StartAction(agent);
            StartCoroutine(PositionRobotCo(agent));
            inPosition = false;
            hasSetAnim = false;
            wake = false;
            agent.isResting = true;
        }

        public override void PerformAction(GOAD_Scheduler_Robot agent)
        {
            base.PerformAction(agent);
            if (wake)
                return;
            if (inPosition && !hasSetAnim)
            {
                
                hasSetAnim = true;
                agent.animator.SetBool(agent.Active_hash, false);
            }
            if(agent.HasBelief(agent.robotActiveCondition.Condition, true) && !wake)
            {
                wake = true;
                StartCoroutine(WakeCo(agent));
            }
        }

        public override void EndAction(GOAD_Scheduler_Robot agent)
        {
            base.EndAction(agent);
            agent.isResting = false;
        }

        IEnumerator WakeCo(GOAD_Scheduler_Robot agent)
        {
            agent.animator.SetBool(agent.Active_hash, true);
            yield return new WaitForSeconds(2);
            
            success = true;
            agent.SetActionComplete(true);
        }

        IEnumerator PositionRobotCo(GOAD_Scheduler_Robot agent)
        {
            
            
            var pos = transform.position;
            float timer = 0;
            float maxTime = 0.8f;
            while (timer < maxTime)
            {
                timer += Time.deltaTime;
                var p = Vector3.Lerp(pos, agent.homeBase, timer / maxTime);
                transform.position = p;
                yield return null;
            }
            yield return null;
            float dir = agent.animator.GetFloat(agent.Direction_hash);
            while (dir > 0)
            {
                dir -= Time.deltaTime;
                agent.animator.SetFloat(agent.Direction_hash, dir);
                yield return null;
            }
            agent.robotLights.SetCurrentFunction(RobotLightManager.RobotStates.Deactivated);
            inPosition = true;
            yield return null;
        }
        
    }
}
