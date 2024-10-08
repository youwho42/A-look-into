using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Action_PassAway : GOAD_Action
	{
        

        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            base.StartAction(agent);
           

            agent.walker.currentDirection = Vector2.zero;
            success = true;
            agent.SetActionComplete(true);
        }

        

        public override void EndAction(GOAD_Scheduler_NPC agent)
        {
            base.EndAction(agent);
           
            Destroy(gameObject);
        }
    } 
}
