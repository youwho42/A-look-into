using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.FSM
{
    [CreateAssetMenu(menuName = "FSM/Transition")]
    public class FSM_Transition : ScriptableObject
    {
        public FSM_Decision decision;
        public FSM_BaseState trueState;
        public FSM_BaseState falseState;

        public void ExecuteTransition(FSM_Brain brain)
        {
            if(decision.Decide(brain) && (trueState is not FSM_RemainInState))
            {
                trueState.InitializeState(brain);
                brain.currentState = trueState;
            }
            else if((falseState is not FSM_RemainInState))
            {
                falseState.InitializeState(brain);
                brain.currentState = falseState;
            }
                
        }
    } 
}
