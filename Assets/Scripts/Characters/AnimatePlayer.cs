using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatePlayer : MonoBehaviour
{

    Animator animator;
    Rigidbody2D body;
   

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Vector2 direction = body.velocity.normalized;
        animator.SetFloat("VelocityX", direction.x);
        animator.SetFloat("VelocityY", direction.y);
    }

    public void TriggerPickUp()
    {
       
        animator.SetTrigger("PickUp");
    }
}
