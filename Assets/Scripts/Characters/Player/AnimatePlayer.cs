using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatePlayer : MonoBehaviour
{

    public Animator animator;
    Rigidbody2D body;
    PlayerInput playerInput;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        //Vector2 direction = body.velocity.normalized;
        float mover = body.velocity.x + body.velocity.y;
        animator.SetBool("IsRunning", playerInput.isRunning);
        animator.SetFloat("VelocityX", Mathf.Abs(mover));
        //animator.SetFloat("VelocityX", Mathf.Abs(direction.y));
    }

    public void TriggerPickUp()
    {
       
        animator.SetTrigger("PickUp");
    }
}
