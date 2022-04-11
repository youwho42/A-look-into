using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityItemMovementFree : GravityItem
{


    [Range(0, 2)]
    public float itemDrag;
    [Range(0, 1)]
    public float itemBounceFriction;

    float velocity;
    Vector2 mainDirection;
    Vector3Int nextTilePosition;

    private new IEnumerator Start()
    {
        base.Start();
        

        yield return new WaitForSeconds(0.25f);

        
        surroundingTiles.GetSurroundingTiles();
        

        isGrounded = true;

    }

    private new void Update()
    {
        base.Update();
        
    }
    public new void FixedUpdate()
    {
        base.FixedUpdate();
        if (!CanReachNextTile(mainDirection))
        {
            CheckBounce();
            
        }

        Move(mainDirection, velocity);

        if (velocity > 0)
            velocity -= itemDrag * Time.fixedDeltaTime;

    }

    void CheckBounce()
    {
        Vector3Int diff = nextTilePosition - surroundingTiles.currentTilePosition;
        if (surroundingTiles.allCurrentDirections.TryGetValue(diff, out SurroundingTilesInfo.DirectionInfo tile))
        {
            if (tile.levelZ >= 0)
            {
                mainDirection = new Vector2(mainDirection.y, mainDirection.x);
                if (diff.x > 0)
                    mainDirection *= -1;

                velocity *= itemBounceFriction;

            }
        }
    }

    bool CanReachNextTile(Vector2 movement)
    {
        if (surroundingTiles.grid == null)
            return false;

        Vector3 checkPosition = (transform.position + (Vector3)movement * checkTileDistance) - Vector3.forward;

        if (CheckForObstacles(checkPosition))
        {
            mainDirection *= -1;
            AddMovement(mainDirection,  velocity * itemBounceFriction);
            return false;
        }
            

        nextTilePosition = surroundingTiles.grid.WorldToCell(checkPosition);
        Vector3Int nextTileKey = nextTilePosition - surroundingTiles.currentTilePosition;
        

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


            // IN AIR! ----------------------------------------------------------------------------------------------------
            // I don't care what height the tile is at as long as the sprite is in the air and has a y above the tile height
            if (tile.Key == nextTileKey && !isGrounded && Mathf.Abs(itemObject.localPosition.z) >= level)
            {

                surroundingTiles.currentTilePosition += new Vector3Int(nextTileKey.x, nextTileKey.y, level);

                if (tile.Value.tileName.Contains("Slope"))
                    onSlope = true;

                return true;
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
                    /*if (surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("X") && nextTileKey.x == 0 || surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("Y") && nextTileKey.y == 0)
                    {
                        onCliffEdge = true;
                        return false;
                    }*/
                    if (tile.Value.levelZ > 0)
                        getOffSlope = true;
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

                // This is where we are at the top of a cliff
                if (tile.Value.levelZ <= 0)
                {
                    surroundingTiles.currentTilePosition += new Vector3Int(nextTileKey.x, nextTileKey.y, level);
                    return true;
                }
                    

                // This is where we hit a wall of height 1 or above
                return false;
            }
        }

        surroundingTiles.currentTilePosition += new Vector3Int(nextTileKey.x, nextTileKey.y, level);

        return true;
    }

    


    void AddMovement(Vector2 newDirection, float _velocity)
    {
        mainDirection = newDirection.normalized;
        velocity = _velocity;
    }

   private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider is CompositeCollider2D)
            return;
        
        if (collision.gameObject.TryGetComponent(out GravityItem gravityItem))
        {
            Vector2 direction = transform.position - collision.transform.position;
            
            AddMovement(direction, gravityItem.currentVelocity != 0 ? gravityItem.currentVelocity : velocity * itemBounceFriction);
        }
        
    }


    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            var direction = transform.position - collision.transform.position;
            AddMovement(direction, velocity * itemBounceFriction);
        }
    }
*/

}
