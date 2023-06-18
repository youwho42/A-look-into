using Klaxon.GOAP;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenShop : GOAP_Action
{

    bool isSitting;
    public InteractableChair chair;
    public List<InteractableMerchantTable> merchantTables = new List<InteractableMerchantTable>();

    public override bool PrePerform(GOAP_Agent agent)
    {
        agent.animator.SetBool(agent.isGrounded_hash, walker.isGrounded);
        agent.animator.SetFloat(agent.velocityY_hash, walker.isGrounded ? 0 : walker.displacedPosition.y);
        agent.animator.SetFloat(agent.velocityX_hash, 0);
        walker.currentDir = Vector2.zero;
        if (!isSitting)
        {

            StartCoroutine(PlaceNPC(agent));
            if (walker.facingRight && !chair.facingRight || !walker.facingRight && chair.facingRight)
                walker.Flip();
            agent.animator.SetBool(agent.isSitting_hash, true);
            isSitting = true;
            chair.canInteract = false;
        }

        List<QI_ItemData> allItems = new List<QI_ItemData>();
        List<int> itemAmount = new List<int>();
        for (int i = 0; i < agent.agentInventory.Stacks.Count; i++)
        {
            if(agent.agentInventory.Stacks[i].Item != null)
            {
                allItems.Add(agent.agentInventory.Stacks[i].Item);
                itemAmount.Add(agent.agentInventory.Stacks[i].Amount);
            }
        }
        
        for (int i = 0; i < merchantTables.Count; i++)
        {

            merchantTables[i].SetUpTable(allItems[i], itemAmount[i]);
            merchantTables[i].canInteract = true;
                
        }
        agent.agentInventory.RemoveAllItems();
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


    IEnumerator PlaceNPC(GOAP_Agent agent)
    {
        float timer = 0;
        float maxTime = 0.45f;
        while (timer < maxTime)
        {
            Vector3 pos = Vector3.Lerp(agent.transform.position, chair.transform.position, timer / maxTime);
            agent.transform.position = pos;
            timer += Time.deltaTime;
            yield return null;
        }



    }

}
