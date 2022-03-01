using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatePlayer : MonoBehaviour
{

    public Animator animator;
    Playermovement playerMovement;
    GravityItemMovement gravityItemMovement;
    PlayerInput playerInput;

    private void Start()
    {
        gravityItemMovement = GetComponent<GravityItemMovement>();
        playerMovement = GetComponent<Playermovement>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void LateUpdate()
    {
        
        animator.SetBool("IsRunning", playerInput.isRunning);
        animator.SetFloat("VelocityX", Mathf.Abs(playerMovement.moveSpeed));
        animator.SetBool("IsGrounded", gravityItemMovement.isGrounded);
        animator.SetFloat("VelocityY", gravityItemMovement.displacedPosition.y);
    }

    public void TriggerPickUp()
    {
       
        animator.SetTrigger("PickUp");
    }
}
