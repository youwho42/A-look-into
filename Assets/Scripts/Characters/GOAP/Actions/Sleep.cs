using Klaxon.GOAP;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using UnityEngine;

public class Sleep : GOAP_Action
{
    public InteractableNPCDialogue interactableDialogue;
    public NPC_UndertakingAvailable undertakingAvailable;

    public override bool PrePerform(GOAP_Agent agent)
    {
        agent.animator.SetBool(agent.isSleeping_hash, true);
        agent.animator.SetBool(agent.isGrounded_hash, walker.isGrounded);
        agent.animator.SetFloat(agent.velocityY_hash, walker.isGrounded ? 0 : walker.displacedPosition.y);
        agent.animator.SetFloat(agent.velocityX_hash, 0);
        walker.currentDir = Vector2.zero;
        Vector3 displacement = new Vector3(walker.transform.position.x, walker.transform.position.y, walker.transform.position.z + 0.33f);
        walker.transform.position = displacement;
        interactableDialogue.canInteract = false;
        undertakingAvailable.isInactive = true;
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
        interactableDialogue.canInteract = true;
        undertakingAvailable.isInactive = false;
        undertakingAvailable.SetUndertakingIcon();
        return true;
    }


    


}