using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.GravitySystem;
public class AnimatePlayer : MonoBehaviour
{

    public Animator animator;
    GravityItemMovementControllerNew playerMovement;
    
    PlayerInputController playerInput;
    bool lostBalance;
    float timeIdle;
    private void Start()
    {
        
        playerMovement = GetComponent<GravityItemMovementControllerNew>();
        playerInput = GetComponent<PlayerInputController>();
    }
    private void Update()
    {
        if (playerMovement.onCliffEdge && !lostBalance && playerMovement.isGrounded)
        {
            playerMovement.isInInteractAction = true;
            lostBalance = true;
            animator.SetTrigger("LostBalance");
            playerMovement.onCliffEdge = false;
            float t = animator.GetCurrentAnimatorStateInfo(0).length;
            Invoke("ResetBalance", t);
        }

        if (GetIdleAnimState())
        {
            timeIdle += Time.deltaTime;
        }
        else
        {
            timeIdle = 0;
        }
        
        animator.SetBool("IdleSit", timeIdle > 20);
          
    }

    private void LateUpdate()
    {
        
        animator.SetBool("IsRunning", playerInput.isRunning);
        animator.SetFloat("VelocityX", Mathf.Abs(playerMovement.moveSpeed));
        animator.SetBool("IsGrounded", playerMovement.isGrounded);
        if(!playerMovement.isGrounded)
            animator.SetFloat("VelocityY", playerMovement.displacedPosition.y);
        else
            animator.SetFloat("VelocityY", 0);
    }

    void ResetBalance()
    {
        playerMovement.isInInteractAction = false;
        lostBalance = false;
    }

    public void TriggerPickUp()
    {
       
        animator.SetTrigger("PickUp");
    }
    bool GetIdleAnimState()
    {
        if (PlayerInformation.instance.uiScreenVisible)
            return false;
        var currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (currentClipInfo[0].clip.name == "Idle")
            return true;
        if (currentClipInfo[0].clip.name == "SitOnGround")
            return true;
        return false;
        
    }
}
