using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterGravityItem : MonoBehaviour
{
    

    GravityItemMovement itemMovement;

    public float velocity;
    public Vector2 mainDirection;
    

    private void Start()
    {
        itemMovement = GetComponent<GravityItemMovement>();

    }

    private void Update()
    {
        if (velocity > 0)
        {

            if (!itemMovement.CanReachNextPosition(mainDirection))
            {

                Vector3Int diff = itemMovement.nextTilePosition - itemMovement.currentGridLocation.lastTilePosition;
                foreach (var tile in itemMovement.surroundingTiles.allCurrentDirections)
                {

                    if (tile.Key == diff && !tile.Value.isValid)
                    {

                        if (tile.Value.difference == 0)
                        {
                            velocity += 0.1f;
                            break;
                        }

                        // This is where we bounce off of walls, or maybe fall off the cliff, depending on tile.Value.difference  0 = cliff fall, 1 = cliff up

                        mainDirection = new Vector2(mainDirection.y, mainDirection.x);
                        if (tile.Key.x != 0)
                            mainDirection *= -1;

                       

                        break;

                    }
                }

            }
            itemMovement.Move(mainDirection, velocity);
            
        }


    }


   
}
