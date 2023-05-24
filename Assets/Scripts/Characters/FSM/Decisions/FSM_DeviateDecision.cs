using Klaxon.GravitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.FSM
{
    [CreateAssetMenu(menuName = "FSM/Decision/Deviate")]
    public class FSM_DeviateDecision : FSM_Decision
    {
        public override bool Decide(FSM_Brain brain)
        {
            
            GravityItemWalker walker = brain.FSM_GetComponent<GravityItemWalker>();
            return walker.isStuck;
        }

        public override void ResetDecision(FSM_Brain brain)
        {
            
        }
    }
}
