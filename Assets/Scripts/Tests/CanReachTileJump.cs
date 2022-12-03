using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanReachTileJump : MonoBehaviour
{

    GravityItemNew gravityItem;



    public float roamingDistance;
    [HideInInspector]
    public Vector2 currentDestination;
    [HideInInspector]
    public Vector2 currentDirection;
    bool onSlope;

    public float walkSpeed;
    Vector3Int nextTilePosition;
    public SpriteRenderer characterRenderer;
    
    public float jumpHeight;
    public float jumpVariation;
    bool jumpAhead;
    float finalJumpVariation;

    

    const float spriteDisplacementY = 0.2790625f;

    private void Start()
    {
        gravityItem = GetComponent<GravityItemNew>();

    }


    public void Move()
    {


        if (CanReachNextTile(currentDirection))
        {
            gravityItem.isWeightless = false;
            if (gravityItem.isGrounded)
            {
                finalJumpVariation = jumpHeight + Random.Range(-jumpVariation, jumpVariation);
                gravityItem.Bounce(finalJumpVariation);
            }
                
            if (!gravityItem.isGrounded)
                gravityItem.Move(currentDirection, walkSpeed);


        }
        else
        {

            if (!jumpAhead)
                SetRandomDestination();
            if (jumpAhead && gravityItem.isGrounded)
            {
                gravityItem.Bounce(jumpHeight);
            }


        }




    }

    void TileFound(List<TileDirectionInfo> tileBlock, bool success)
    {
        if (success)
            gravityItem.tileBlockInfo = tileBlock;
    }



    public bool CanReachNextTile(Vector2 direction)
    {
        

        Vector3 checkPosition = (transform.position + (Vector3)direction * gravityItem.checkTileDistance) - Vector3.forward;
        Vector3 doubleCheckPosition = transform.position - Vector3.forward;
        if (gravityItem.CheckForObstacles(checkPosition, doubleCheckPosition, direction))
            return false;

        nextTilePosition = gravityItem.currentTilePosition.grid.WorldToCell(checkPosition);

        Vector3Int nextTileKey = nextTilePosition - gravityItem.currentTilePosition.position;

        if (nextTileKey == Vector3Int.zero)
            return true;


       

        int level = 0;
        jumpAhead = false;
        TileInfoRequestManager.RequestTileInfo(gravityItem.currentTilePosition.position, TileFound);

        if (gravityItem.tileBlockInfo == null)
            return true;

        foreach (var tile in gravityItem.tileBlockInfo)
        {
            // CURRENT TILE ----------------------------------------------------------------------------------------------------
            // right now, where we are, what it be? is it be a slope?
            if (tile.direction == Vector3Int.zero)
            {

                gravityItem.slopeDirection = Vector2.zero;
                onSlope = tile.tileName.Contains("Slope");
                if (onSlope)
                {
                    if (tile.tileName.Contains("X"))
                        gravityItem.slopeDirection = tile.tileName.Contains("0") ? new Vector2(-0.9f, -0.5f) : new Vector2(0.9f, 0.5f);
                    else
                        gravityItem.slopeDirection = tile.tileName.Contains("0") ? new Vector2(0.9f, -0.5f) : new Vector2(-0.9f, 0.5f);
                    continue;
                }

            }
            if (tile.direction == nextTileKey)
                level = tile.levelZ;
            else
                continue;
            Vector3Int doubleCheckTilePosition = gravityItem.currentTilePosition.grid.WorldToCell(doubleCheckPosition);



            // JUMPING! ----------------------------------------------------------------------------------------------------
            // I don't care what height the tile is at as long as the sprite is jumping and has a y above the tile height
            if (tile.direction == nextTileKey)
            {
                
                if (!gravityItem.isGrounded && Mathf.Abs(gravityItem.itemObject.localPosition.z) >= level)
                {
                    if (level < -1)
                        return false;
                    gravityItem.currentTilePosition.position += new Vector3Int(nextTileKey.x, nextTileKey.y, level);

                    if (tile.tileName.Contains("Slope"))
                        onSlope = true;

                    return true;
                }
            }

            // GROUNDED! ----------------------------------------------------------------------------------------------------
            // the next tile is valid



            if (tile.direction == nextTileKey && tile.isValid)
            {

                // if the next tile is a slope, am i approaching it in the right direction?
                if (tile.tileName.Contains("Slope"))
                {
                    if (tile.tileName.Contains("X") && nextTileKey.x == 0 || tile.tileName.Contains("Y") && nextTileKey.y == 0)
                        return false;

                    onSlope = true;

                    // is the slope is lower?
                    if (tile.levelZ < 0)
                        gravityItem.getOnSlope = true;


                }

                // I am on a slope
                if (onSlope)
                {
                    //am i walking 'off' the slope on the upper part in the right direction?
                    if (tile.direction == Vector3Int.zero && tile.tileName.Contains("X") && nextTileKey.x == 0 || tile.direction == Vector3Int.zero && tile.tileName.Contains("Y") && nextTileKey.y == 0)
                    {
                        //onCliffEdge = true;
                        return false;
                    }
                    if (tile.levelZ > 0)
                        gravityItem.getOffSlope = true;
                }

            }

            // the next tile is NOT valid
            if (tile.direction == nextTileKey && !tile.isValid)
            {
                if (doubleCheckTilePosition == nextTilePosition)
                {
                    gravityItem.Nudge(direction);
                }

                // If I am on a slope, am i approaching or leaving the slope in a valid direction?
                if (onSlope)
                {
                    if (tile.direction == Vector3Int.zero && tile.tileName.Contains("X") && nextTileKey.x != 0 || tile.direction == Vector3Int.zero && tile.tileName.Contains("Y") && nextTileKey.y != 0)
                        continue;
                }

                

                if (tile.levelZ == 1 || tile.levelZ == -1 || tile.levelZ == -2)
                    jumpAhead = true;


                // This is where we are on top of a cliff
                if (tile.levelZ <= 0)
                {
                    return false;
                }

                // This is where we hit a wall of height 1 or above
                return false;
            }
        }

        gravityItem.currentTilePosition.position += new Vector3Int(nextTileKey.x, nextTileKey.y, level);



        return true;
    }


    public void SetFacingDirection(Vector2 direction)
    {
        // Set facing direction
        var dir = Mathf.Sign(direction.x);
        characterRenderer.flipX = dir > 0;
    }



    public void SetRandomDestination()
    {

        if (gravityItem == null || gravityItem.currentTilePosition.groundMap == null)
            return;

        Vector2 rand = (Random.insideUnitCircle * roamingDistance);
        var d = gravityItem.currentTilePosition.groundMap.WorldToCell(new Vector2(transform.position.x + rand.x, transform.position.y + rand.y));
        for (int z = gravityItem.currentTilePosition.groundMap.cellBounds.zMax; z > gravityItem.currentTilePosition.groundMap.cellBounds.zMin - 1; z--)
        {
            d.z = z;
            if (gravityItem.currentTilePosition.groundMap.GetTile(d) != null)
            {

                currentDestination = gravityItem.currentTilePosition.groundMap.GetCellCenterWorld(d);
                break;
            }

        }


        SetDirection();
    }

    public void SetDestination(DrawZasYDisplacement destination)
    {
        currentDestination = destination.transform.position;
        SetDirection();
    }

    void SetDirection()
    {
        currentDirection = currentDestination - (Vector2)gravityItem.gameObject.transform.position;
        SetFacingDirection(currentDirection);
    }

    
    

}
