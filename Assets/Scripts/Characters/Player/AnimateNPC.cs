using Klaxon.GravitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateNPC : MonoBehaviour
{
    public Animator animator;
    MoveToNode npcMovement;
    
    private void Start()
    {
        npcMovement = GetComponent<MoveToNode>();
    }
    

    private void LateUpdate()
    {
        animator.SetFloat("VelocityX", Mathf.Abs(npcMovement.moveSpeed));
        animator.SetBool("IsGrounded", true);
    }

    

    public void TriggerPickUp()
    {
        animator.SetTrigger("PickUp");
    }
}
