using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.FSM
{
    [CreateAssetMenu(menuName = "FSM/State")]
    public class FSM_State : FSM_BaseState
    {
        public List<FSM_Action> actions = new List<FSM_Action>();
        public List<FSM_Transition> transitions = new List<FSM_Transition>();

        public override void InitializeState(FSM_Brain brain)
        {
            foreach (var action in actions)
            {
                action.ResetAction(brain);
            }
            foreach (var transition in transitions)
            {
                transition.decision.ResetDecision(brain);
            }
        }

        public override void ExecuteState(FSM_Brain brain)
        {
            foreach (var action in actions)
            {
                action.ExecuteAction(brain);
            }
            foreach (var transition in transitions)
            {
                transition.ExecuteTransition(brain);
            }
        }

        public override void LateExecuteState(FSM_Brain brain)
        {
            foreach (var action in actions)
            {
                action.LateExecuteAction(brain);
            }
        }
    }
}

