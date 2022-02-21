using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatePlayer : MonoBehaviour
{

    public Animator animator;
    GravityItemMovement itemMovement;
    PlayerInput playerInput;

    private void Start()
    {
        itemMovement = GetComponent<GravityItemMovement>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void LateUpdate()
    {
        
        animator.SetBool("IsRunning", playerInput.isRunning);
        animator.SetFloat("VelocityX", Mathf.Abs(itemMovement.moveSpeed));
        
    }

    public void TriggerPickUp()
    {
       
        animator.SetTrigger("PickUp");
    }
}
