using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GravityItemMovementController : GravityItem
{

    [Space]
    [Header("Player Movement")]
    public float walkSpeed;
    public float runSpeed;
    public float jumpHeight;
    PlayerInput playerInput;
    public bool facingRight;
    public bool isInInteractAction;
    [HideInInspector]
    public float moveSpeed;
    

    
    

    
    

    

    Vector3Int nextTilePosition;
    [HideInInspector]
    public bool onCliffEdge;



    private new IEnumerator Start()
    {
        base.Start();
        playerInput = GetComponent<PlayerInput>();
        
        yield return new WaitForSeconds(0.25f);
        
        surroundingTiles.GetSurroundingTiles();
        
        isGrounded = true;
        
    }



    private new void Update()
    {
        base.Update();
        


        if (playerInput.movement.x != 0 && !isInInteractAction)
        {
            if (playerInput.movement.x > 0.01f && !facingRight)
                Flip();
            else if (playerInput.movement.x < 0.01f && facingRight)
                Flip();
        }

        if (isGrounded && playerInput.isJumping)
            Bounce(jumpHeight);

        moveSpeed = playerInput.movement.x + playerInput.movement.y;


        
        
    }


    public new void FixedUpdate()
    {
        base.FixedUpdate();

        if (isInInteractAction)
            return;

        if (CanReachNextTile(playerInput.movement))
        {
            Move(playerInput.movement, (playerInput.isRunning ? runSpeed : walkSpeed));
        }

        

    }


    bool CanReachNextTile(Vector2 movement)
    {
        if (surroundingTiles.grid == null)
            return false;

        Vector3 checkPosition = (transform.position + (Vector3)movement * checkTileDistance) - Vector3.forward;
        Vector3 doubleCheckPosition = transform.position - Vector3.forward;
        if (CheckForObstacles(checkPosition))
            return false;

        nextTilePosition = surroundingTiles.grid.WorldToCell(checkPosition);
        
        Vector3Int nextTileKey = nextTilePosition - surroundingTiles.currentTilePosition;
        onCliffEdge = false;

        surroundingTiles.GetSurroundingTiles();

        int level = 0;

        foreach (var tile in surroundingTiles.allCurrentDirections)
        {
            // CURRENT TILE ----------------------------------------------------------------------------------------------------
            // right now, where we are, what it be? is it be a slope?
            if (tile.Key == Vector3Int.zero)
            {
                
                slopeDirection = Vector2.zero;
                onSlope = tile.Value.tileName.Contains("Slope");
                if (onSlope)
                {
                    if (tile.Value.tileName.Contains("X"))
                        slopeDirection = tile.Value.tileName.Contains("0") ? new Vector2(-0.9f, -0.5f) : new Vector2(0.9f, 0.5f);
                    else
                        slopeDirection = tile.Value.tileName.Contains("0") ? new Vector2(0.9f, -0.5f) : new Vector2(-0.9f, 0.5f);
                    continue;
                }
                
            }
            if (tile.Key == nextTileKey)
                level = tile.Value.levelZ;
            else
                continue;
            Vector3Int doubleCheckTilePosition = surroundingTiles.grid.WorldToCell(doubleCheckPosition);
            
            

            // JUMPING! ----------------------------------------------------------------------------------------------------
            // I don't care what height the tile is at as long as the sprite is jumping and has a y above the tile height
            if (tile.Key == nextTileKey)
            {

                if (isGrounded && playerInput.canWalkOffCliff && level <= 0 || !isGrounded && Mathf.Abs(itemObject.localPosition.z) >= level)
                {
                    surroundingTiles.currentTilePosition += new Vector3Int(nextTileKey.x, nextTileKey.y, level);

                    if (tile.Value.tileName.Contains("Slope"))
                        onSlope = true;

                    return true;
                }
            }

            // GROUNDED! ----------------------------------------------------------------------------------------------------
            // the next tile is valid



                if (tile.Key == nextTileKey && tile.Value.isValid)
            {
                
                // if the next tile is a slope, am i approaching it in the right direction?
                if (tile.Value.tileName.Contains("Slope"))
                {
                    if (tile.Value.tileName.Contains("X") && nextTileKey.x == 0 || tile.Value.tileName.Contains("Y") && nextTileKey.y == 0)
                        return false;

                    onSlope = true;
                    
                    // is the slope is lower?
                    if (tile.Value.levelZ < 0)
                        getOnSlope = true;
                    

                }

                // I am on a slope
                if (onSlope)
                {
                    //am i walking 'off' the slope on the upper part in the right direction?
                    if (surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("X") && nextTileKey.x == 0 || surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("Y") && nextTileKey.y == 0)
                    {
                        onCliffEdge = true;
                        return false;
                    }
                    if (tile.Value.levelZ > 0)
                        getOffSlope = true;
                }

            }

            // the next tile is NOT valid
            if (tile.Key == nextTileKey && !tile.Value.isValid)
            {
                if(doubleCheckTilePosition == nextTilePosition)
                {
                    Nudge(movement);
                }

                // If I am on a slope, am i approaching or leaving the slope in a valid direction?
                if (onSlope)
                {
                    if (surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("X") && nextTileKey.x != 0 || surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("Y") && nextTileKey.y != 0)
                        continue;
                }

                // This is where we are on top of a cliff
                if (tile.Value.levelZ <= 0)
                {
                    onCliffEdge = true;
                }

                // This is where we hit a wall of height 1 or above
                return false;
            }
        }
        
        surroundingTiles.currentTilePosition += new Vector3Int(nextTileKey.x, nextTileKey.y, level);
        


        return true;
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
