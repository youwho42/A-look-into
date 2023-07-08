using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableChair : Interactable
{
    
    bool isSitting;
    PlayerInformation player;
    public bool facingRight;
    public NavigationNode navigationNode;
    
    public override void Start()
    {
        base.Start();
        
    }
    

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        player = PlayerInformation.instance;
        if(!isSitting)
        {
            StartCoroutine(PlacePlayer(transform.position));
            if (player.playerController.facingRight && !facingRight || !player.playerController.facingRight && facingRight)
                player.playerController.Flip();
            player.playerAnimator.SetBool("IsSitting", true);
            player.isSitting = true;
            isSitting = true;
            canInteract = false;
        }
        
    }

    IEnumerator PlacePlayer(Vector3 position)
    {
        float timer = 0;
        float maxTime = 0.45f;
        while (timer < maxTime)
        {
            Vector3 pos = Vector3.Lerp(player.player.position, position, timer / maxTime);
            player.player.position = pos;
            timer += Time.deltaTime;
            yield return null;
        }

        

    }

    private void Update()
    {
        if (!isSitting)
            return;
        
        if(player.playerInput.movement != Vector2.zero)
        {
            //Vector3 exitPos = player.player.position + ((Vector3)player.playerInput.movement * .09f);
            StartCoroutine(PlacePlayer(navigationNode.transform.position));
            player.playerAnimator.SetBool("IsSitting", false);
            playerInformation.isSitting = false;
            isSitting = false;
            canInteract = true;
        }
        
    }

}
 