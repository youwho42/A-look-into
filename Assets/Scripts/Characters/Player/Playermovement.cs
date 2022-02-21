using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermovement : MonoBehaviour
{

    public float walkSpeed;
    public float runSpeed;
    PlayerInput playerInput;
    GravityItemMovement gravityItemMovement;
    public bool facingRight;
    public bool isInInteractAction;
    ItemGravity itemGravity;

    // Start is called before the first frame update
    void Start()
    {
        
        playerInput = GetComponent<PlayerInput>();
        gravityItemMovement = GetComponent<GravityItemMovement>();
        itemGravity = GetComponent<ItemGravity>();

    }

    private void Update()
    {

        if (playerInput.movement.x != 0)
        {
            if (playerInput.movement.x > 0.01f && !facingRight)
                Flip();
            else if (playerInput.movement.x < 0.01f && facingRight)
                Flip();
        }

        if (itemGravity.isGrounded && playerInput.isJumping)
            itemGravity.Bounce(8);
        
    }


    void FixedUpdate()
    {
        if (!isInInteractAction)
            gravityItemMovement.Move(playerInput.movement, (playerInput.isRunning ? runSpeed : walkSpeed));
        else
            gravityItemMovement.Move(Vector2.zero, 0);
    }

    void Flip()
    {
        // Switch the way the player is labelled as facing
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

}
