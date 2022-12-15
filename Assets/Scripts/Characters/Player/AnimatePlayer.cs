using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.GravitySystem;
public class AnimatePlayer : MonoBehaviour
{

    public Animator animator;
    GravityItemMovementControllerNew playerMovement;
    //GravityItemMovement gravityItemMovement;
    PlayerInput playerInput;
    bool lostBalance;
    private void Start()
    {
        //gravityItemMovement = GetComponent<GravityItemMovement>();
        playerMovement = GetComponent<GravityItemMovementControllerNew>();
        playerInput = GetComponent<PlayerInput>();
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
}
