using Klaxon.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseShop : GOAP_Action
{
    public InteractableChair chair;
    public List<InteractableMerchantTable> merchantTables = new List<InteractableMerchantTable>();

    public override bool PrePerform(GOAP_Agent agent)
    {
        
        chair.canInteract = true;


        for (int i = 0; i < merchantTables.Count; i++)
        {
            merchantTables[i].ClearTable();
            merchantTables[i].canInteract = false;
        }
            

        
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
