using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityItemMovement : MonoBehaviour
{


    CurrentGridLocation currentGridLocation;

    SurroundingTiles surroundingTiles;
    ItemGravity itemGravity;

    Vector3 currentPosition;
    

    
    public LayerMask obstacleLayer;


    Vector3Int nextTilePosition;

    public float moveSpeed;

    private void Start()
    {
        currentGridLocation = GetComponent<CurrentGridLocation>();
        surroundingTiles = GetComponent<SurroundingTiles>();
        itemGravity = GetComponent<ItemGravity>();
        currentGridLocation.UpdateLocationAndPosition();
        surroundingTiles.GetSurroundingTiles();
    }


    public void Move(Vector2 dir, float speed)
    {
        moveSpeed = dir.x + dir.y;
        if (!CanReachNextPosition(dir))
        {
            moveSpeed = 0;
            return;

        }

        currentGridLocation.UpdateLocation();
        surroundingTiles.GetSurroundingTiles();

        currentPosition = transform.position;
        currentPosition = Vector2.MoveTowards(transform.position, (Vector2)transform.position + dir, Time.deltaTime * speed);
        currentPosition.z = currentGridLocation.currentLevel;
        transform.position = currentPosition;
        
    }

    bool CanReachNextPosition(Vector2 movement)
    {
        float distance = movement.y <= 0 ? 0.3f : 0.05f;
        Vector3 checkPosition = (transform.position + (Vector3)movement * distance) - Vector3.forward;
        nextTilePosition = currentGridLocation.groundGrid.WorldToCell(checkPosition);

        Vector3Int diff = nextTilePosition - currentGridLocation.lastTilePosition;
        foreach (var tile in surroundingTiles.allCurrentDirections)
        {
            if (tile.Key == diff && !tile.Value.isValid)
            {
                
                // This is where we fall of a cliff
                if (tile.Value.difference == 0)
                    return true;


                // This is where we hit a wall of height one

                //maybe it's a slope.
                if (tile.Value.difference == 1)
                {
                    // if its's a slope, am I approaching it from the correct angle?
                    if (tile.Value.tileName.Contains("X") && diff.x != 0 || tile.Value.tileName.Contains("Y") && diff.y != 0)
                    {
                        return true;
                    }
                    else //it's just a wall of height +1, if it's a slope i'm approaching it wrong
                    {
                        // but maybe I'm jumping?
                        if(!itemGravity.isGrounded)
                            return true;
                    }
                        
                }


                // it's really just a wall of a height of 2 
                
                return false;
                


                    
            }
        }

        var hit = Physics2D.OverlapPoint(transform.position + (Vector3)movement * 0.05f, obstacleLayer);
        if (hit != null)
            return false;

        return true;
       
    }

}
