using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.FSM
{
    [CreateAssetMenu(menuName = "FSM/Decision/Timer")]
    public class FSM_TimerDecision : FSM_Decision
    {
        public Vector2 minMaxTime;
        float timeMax;
        float timer = 0;
        public override bool Decide(FSM_Brain brain)
        {
            timer += Time.deltaTime;
            return timer >= timeMax;
        }

        public override void ResetDecision(FSM_Brain brain)
        {
            timeMax = Random.Range(minMaxTime.x, minMaxTime.y);
            timer = 0;
        }
    }
}
