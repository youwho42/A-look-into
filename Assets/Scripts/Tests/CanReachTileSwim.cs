using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanReachTileSwim : MonoBehaviour
{
    GravityItem gravityItem;
   
    public Transform centerOfActiveArea;

    public Boid boid;
    public float swimSpeed;
    public float swimRoamingDistance;
    public Vector2 minMaxSwimZ;
    [HideInInspector]
    public Vector2 currentDestination;
    [HideInInspector]
    public Vector3 mainDestinationZ;
    public Vector3 currentDestinationZ;

    
    public Vector2 currentDirection;
    [HideInInspector]
    public Vector3 currentDirectionZ;
    bool onSlope;

    

    Vector3Int nextTilePosition;
    public SpriteRenderer characterRenderer;

    const float spriteDisplacementY = 0.2790625f;

    
    float boidTimer = 0;
    private void Start()
    {
        boid = GetComponent<Boid>();
        gravityItem = GetComponent<GravityItem>();
        SetRandomDirection();
        boid.currentDirection = currentDirection;
        var s = swimSpeed / 5;
        swimSpeed += Random.Range(-s, s);
    }


    public void Swim()
    {


        
        if (CanReachNextTile(currentDirection))
        {
            if (!boid.inBoidPool)
            {
                boidTimer += Time.deltaTime;
                if (boidTimer > 4f)
                {
                    boid.inBoidPool = true;
                    boidTimer = 0;
                }
                    
            }
            
            gravityItem.isWeightless = true;
            SetDirectionZ();
            SetFacingDirection(currentDirection);
            gravityItem.MoveZ(currentDirectionZ, swimSpeed);
            gravityItem.Move(currentDirection, swimSpeed);
            if(boid.inBoidPool)
                currentDirection = boid.SteerBoid(currentDirection, swimRoamingDistance);

        }
        else
        {
            
            boid.inBoidPool = false;
            SetRandomDirection();
            

            
        }


        if (boid.currentDirection == Vector2.zero)
            SetRandomDirection();

        if (Vector2.Distance(gravityItem.itemObject.localPosition, currentDestinationZ) <= 0.01f)
        {
            SetRandomDestinationZ();
        }

        

    }

    
    

    public bool CanReachNextTile(Vector2 direction)
    {
        if (gravityItem.surroundingTiles.grid == null)
            return false;

        Vector3 checkPosition = (transform.position + (Vector3)direction * gravityItem.checkTileDistance) - Vector3.forward;
        Vector3 doubleCheckPosition = transform.position - Vector3.forward;
        if (gravityItem.CheckForObstacles(checkPosition, doubleCheckPosition, direction))
            return false;

        nextTilePosition = gravityItem.surroundingTiles.grid.WorldToCell(checkPosition);

        Vector3Int nextTileKey = nextTilePosition - gravityItem.surroundingTiles.currentTilePosition;

        if (nextTileKey == Vector3Int.zero)
            return true;



        gravityItem.surroundingTiles.GetSurroundingTiles();

        int level = 0;

        foreach (var tile in gravityItem.surroundingTiles.allCurrentDirections)
        {
            // CURRENT TILE ----------------------------------------------------------------------------------------------------
            // right now, where we are, what it be? is it be a slope?
            if (tile.Key == Vector3Int.zero)
            {

                gravityItem.slopeDirection = Vector2.zero;
                onSlope = tile.Value.tileName.Contains("Slope");
                if (onSlope)
                {
                    if (tile.Value.tileName.Contains("X"))
                        gravityItem.slopeDirection = tile.Value.tileName.Contains("0") ? new Vector2(-0.9f, -0.5f) : new Vector2(0.9f, 0.5f);
                    else
                        gravityItem.slopeDirection = tile.Value.tileName.Contains("0") ? new Vector2(0.9f, -0.5f) : new Vector2(-0.9f, 0.5f);
                    continue;
                }

            }
            if (tile.Key == nextTileKey)
                level = tile.Value.levelZ;
            else
                continue;
            Vector3Int doubleCheckTilePosition = gravityItem.surroundingTiles.grid.WorldToCell(doubleCheckPosition);



            // JUMPING! ----------------------------------------------------------------------------------------------------
            // I don't care what height the tile is at as long as the sprite is jumping and has a y above the tile height
            if (tile.Key == nextTileKey)
            {

                if (Mathf.Abs(gravityItem.itemObject.localPosition.z) >= level)
                {
                    gravityItem.surroundingTiles.currentTilePosition += new Vector3Int(nextTileKey.x, nextTileKey.y, level);

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
                    {
                        
                        return false;
                    }
                        

                    onSlope = true;

                    // is the slope is lower?
                    if (tile.Value.levelZ < 0)
                        gravityItem.getOnSlope = true;


                }

                // I am on a slope
                if (onSlope)
                {
                    //am i walking 'off' the slope on the upper part in the right direction?
                    if (gravityItem.surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("X") && nextTileKey.x == 0 || gravityItem.surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("Y") && nextTileKey.y == 0)
                    {
                        //onCliffEdge = true;
                        return false;
                    }
                    if (tile.Value.levelZ > 0)
                        gravityItem.getOffSlope = true;
                }

            }

            // the next tile is NOT valid
            if (tile.Key == nextTileKey && !tile.Value.isValid)
            {
                if (doubleCheckTilePosition == nextTilePosition)
                {
                    gravityItem.Nudge(direction);
                }

                // If I am on a slope, am i approaching or leaving the slope in a valid direction?
                if (onSlope)
                {
                    if (gravityItem.surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("X") && nextTileKey.x != 0 || gravityItem.surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("Y") && nextTileKey.y != 0)
                        continue;
                }

                // This is where we are at he bottom of a cliff
                if (tile.Value.levelZ > 0)
                {
                    return false;
                }
                // This is where we hit a wall of height 1 or above
                return false;
            }
        }

        gravityItem.surroundingTiles.currentTilePosition += new Vector3Int(nextTileKey.x, nextTileKey.y, level);



        return true;
    }


    void SetRandomDirection()
    {
        currentDirection = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        currentDirection = currentDirection.normalized;
        SetFacingDirection(currentDirection);
    }

    public void SetFacingDirection(Vector2 direction)
    {
        // Set facing direction
        //Vector2 dir = currentDestination - (Vector2)transform.position;
        var dir = Mathf.Sign(direction.x);
        characterRenderer.flipX = dir > 0;
    }

    public void SetRandomDestinationZ()
    {
        float randZ = Random.Range(minMaxSwimZ.x, minMaxSwimZ.y);
        mainDestinationZ = new Vector3(0, spriteDisplacementY * randZ, randZ);
        currentDestinationZ = mainDestinationZ;
        SetDirectionZ();
    }


    public void SetRandomDestination()
    {
        
        if (gravityItem == null || gravityItem.surroundingTiles == null || gravityItem.surroundingTiles.groundMap == null)
            return;
        
        Vector2 rand = (Random.insideUnitCircle * swimRoamingDistance);
        var d = gravityItem.surroundingTiles.groundMap.WorldToCell(new Vector2(centerOfActiveArea.position.x + rand.x, centerOfActiveArea.position.y + rand.y));
        for (int z = gravityItem.surroundingTiles.groundMap.cellBounds.zMax; z > gravityItem.surroundingTiles.groundMap.cellBounds.zMin - 1; z--)
        {
            d.z = z;
            if (gravityItem.surroundingTiles.groundMap.GetTile(d) != null)
            {
                currentDestination = gravityItem.surroundingTiles.groundMap.GetCellCenterWorld(d);
                break;
            }

        }


        SetDirection();
    }



    public void SetDestination(DrawZasYDisplacement destination)
    {
        currentDestination = destination.transform.position;
        mainDestinationZ = destination.displacedPosition;

        SetDirection();
        SetDirectionZ();
    }

    public void SetDirectionZ()
    {
        if (gravityItem == null || gravityItem.surroundingTiles == null || gravityItem.surroundingTiles.groundMap == null)
            return;
        currentDirectionZ = mainDestinationZ - gravityItem.itemObject.localPosition;

    }
    public void SetDirection()
    {
        currentDirection = currentDestination - (Vector2)transform.position;
        SetFacingDirection(currentDirection);
    }


}
