using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GravityItemMovementController : GravityItem
{

    [Space]
    [Header("Movement")]
    public float walkSpeed;
    public float runSpeed;
    public float jumpHeight;
    PlayerInput playerInput;
    public bool facingRight;
    public bool isInInteractAction;
    [HideInInspector]
    public float moveSpeed;
    

    // surroundings

    [Header("Obstacles")]
    public LayerMask obstacleLayer;
    

    
    int lastLevel;

    Vector3 currentPosition;

    Vector3Int nextTilePosition;
    [HideInInspector]
    public bool onCliffEdge;



    private new IEnumerator Start()
    {
        base.Start();
        playerInput = GetComponent<PlayerInput>();
        
        yield return new WaitForSeconds(0.25f);

        currentGridLocation.UpdateLocationAndPosition();
        surroundingTiles.GetSurroundingTiles();

        lastLevel = currentGridLocation.currentLevel;
        isGrounded = true;
        
    }



    private new void Update()
    {
        base.Update();
        SetIsGrounded();

        currentGridLocation.UpdateLocation();

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


        if (currentGridLocation.currentLevel != lastLevel && isGrounded)//either jumping or falling down a cliff
        {
            ChangeLevel();
        }
        
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

        Vector3 checkPosition = (transform.position + (Vector3)movement * checkTileDistance) - Vector3.forward;

        if (CheckForObstacles(checkPosition))
            return false;

        nextTilePosition = currentGridLocation.groundGrid.WorldToCell(checkPosition);
        Vector3Int nextTileKey = nextTilePosition - currentGridLocation.lastTilePosition;
        onCliffEdge = false;


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


            // JUMPING! ----------------------------------------------------------------------------------------------------
            // I don't care what height the tile is at as long as the sprite is jumping and has a y above the tile height
            if (tile.Key == nextTileKey && !isGrounded && Mathf.Abs(displacedPosition.y) >= tile.Value.levelZ)
            {
                onCliffEdge = false;
                return true;
            }


            // GROUNDED! ----------------------------------------------------------------------------------------------------
            // the next tile is valid
            if (tile.Key == nextTileKey && tile.Value.isValid)
            {
                nextTileIsSlope = false;
                // if the next tile is a slope, am i approaching it in the right direction?
                if (tile.Value.tileName.Contains("Slope"))
                {
                    if (tile.Value.tileName.Contains("X") && nextTileKey.x == 0 || tile.Value.tileName.Contains("Y") && nextTileKey.y == 0)
                        return false;
                    nextTileIsSlope = true;
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
                }

            }

            // the next tile is NOT valid
            if (tile.Key == nextTileKey && !tile.Value.isValid)
            {

                // If I am on a slope, am i approaching or leaving the slope in a valid direction?
                if (onSlope)
                {
                    if (surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("X") && nextTileKey.x != 0 || surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("Y") && nextTileKey.y != 0)
                        continue;
                }

                // This is where we are on top of a cliff
                if (tile.Value.levelZ == 0)
                {
                    onCliffEdge = true;
                }



                // This is where we hit a wall of height 1 or above
                return false;
            }
        }
        return true;
    }

    bool CheckForObstacles(Vector3 checkPosition)
    {
        // Check for gameobjects on the obstacle layer
        var hit = Physics2D.OverlapPoint(checkPosition, obstacleLayer);
        if (hit != null)
        {
            if (hit.gameObject.transform.position.z == transform.position.z)
                return true;
        }
        return false;
    }


    void ChangeLevel()
    {
        //displacing = true;
        //bounceFactor = 1;

        if(nextTileIsSlope)
        {

        }
        int dif = currentGridLocation.currentLevel - lastLevel;
        float displacement = dif * spriteDisplacementY;
        currentPosition = transform.position;
        transform.position = new Vector3(currentPosition.x, currentPosition.y + displacement, currentGridLocation.currentLevel);
        
        lastLevel = currentGridLocation.currentLevel;

        //Invoke(nameof(ResetDisplacing), 0.3f);
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
