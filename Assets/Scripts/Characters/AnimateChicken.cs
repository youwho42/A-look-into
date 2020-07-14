using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateChicken : MonoBehaviour
{
    Animator animator;
    Rigidbody2D body;

    private void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector2 direction = body.velocity.normalized;
        animator.SetFloat("VelocityX", direction.x);
        animator.SetFloat("VelocityY", direction.y);
    }
}
