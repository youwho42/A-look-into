using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermovement : MonoBehaviour
{

    public float walkSpeed;
    public float runSpeed;
    public float jumpHeight;
    PlayerInput playerInput;
    GravityItemMovement gravityItemMovement;
    public bool facingRight;
    public bool isInInteractAction;
    public float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
        playerInput = GetComponent<PlayerInput>();
        gravityItemMovement = GetComponent<GravityItemMovement>();
        
    }

    private void Update()
    {

        if (playerInput.movement.x != 0 && !isInInteractAction)
        {
            if (playerInput.movement.x > 0.01f && !facingRight)
                Flip();
            else if (playerInput.movement.x < 0.01f && facingRight)
                Flip();
        }

        if (gravityItemMovement.isGrounded && playerInput.isJumping)
            gravityItemMovement.Bounce(jumpHeight);

        moveSpeed = playerInput.movement.x + playerInput.movement.y;
        
    }


    void FixedUpdate()
    {
        if (isInInteractAction)
            return;

        /*if (gravityItemMovement.CanReachNextTile(dir))
        {*/
            gravityItemMovement.Move(playerInput.movement, (playerInput.isRunning ? runSpeed : walkSpeed));
        /*}*/
            
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
