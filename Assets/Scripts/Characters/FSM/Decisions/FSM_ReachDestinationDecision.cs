using Klaxon.GravitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Klaxon.FSM
{
    [CreateAssetMenu(menuName = "FSM/Decision/Reach Destination")]
    public class FSM_ReachDestinationDecision : FSM_Decision
    {
        public override bool Decide(FSM_Brain brain)
        {
            GravityItemWalker walker = brain.FSM_GetComponent<GravityItemWalker>();
            return walker.CheckDistanceToDestination() <= 0.02f;
            
        }

        public override void ResetDecision(FSM_Brain brain)
        {
            
        }
    }
}