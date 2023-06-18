using Klaxon.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SitDown : GOAP_Action
{

    public override bool PrePerform(GOAP_Agent agent)
    {
        agent.animator.SetBool(agent.isSitting_hash, true);
        agent.animator.SetBool(agent.isGrounded_hash, walker.isGrounded);
        agent.animator.SetFloat(agent.velocityY_hash, walker.isGrounded ? 0 : walker.displacedPosition.y);
        agent.animator.SetFloat(agent.velocityX_hash, 0);
        walker.currentDir = Vector2.zero;
        
        return true;
    }

    public override void Perform(GOAP_Agent agent)
    {
        agent.destinationReached = true;
    }

    public override void PrePostPerform(GOAP_Agent agent)
    {

    }

    public override bool PostPerform(GOAP_Agent agent)
    {
        return true;
    }


    


}
