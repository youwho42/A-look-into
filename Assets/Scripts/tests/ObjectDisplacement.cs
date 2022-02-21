using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDisplacement : MonoBehaviour
{


    CurrentGridLocation currentGridLocation;

    SurroundingTiles surroundingTiles;
    
    
    Vector3 currentPosition;
    Vector3Int lastTilePosition;

    float velocity = 0.5f;
    [Range(0, 2)]
    public float itemDrag;

    public Vector2 mainDirection;
    public readonly float spriteDisplacementY = 0.27808595f;
    bool bouncingOnWall;

    Vector3Int nextTilePosition;

    private void Start()
    {
        
        currentGridLocation = GetComponent<CurrentGridLocation>();
        surroundingTiles = GetComponent<SurroundingTiles>();
        currentGridLocation.UpdateLocationAndPosition();
        lastTilePosition = currentGridLocation.lastTilePosition;
        surroundingTiles.GetSurroundingTiles();
    }

    private void Update()
    {
        

        if (velocity > 0)
            Move(mainDirection * velocity);
    }
    private void Move(Vector2 dir)
    {
        if (CheckForNewTile())
        {
            
            Vector3Int diff = nextTilePosition - currentGridLocation.lastTilePosition;
            foreach (var tile in surroundingTiles.allCurrentDirections)
            {

                if (tile.Key == diff && !tile.Value.isValid)
                {
                    
                    if (tile.Value.difference == 0)
                        break;
                    // This is where we bounce off of walls, or maybe fall off the cliff, depending on tile.Value.difference  0 = cliff fall, 1 = cliff up

                    mainDirection = new Vector2(mainDirection.y, mainDirection.x);
                    if(tile.Key.x!=0)
                        mainDirection *= -1;
                    
                    velocity *= .75f;
                    
                    break;

                }
            }

        }
       

        currentGridLocation.UpdateLocation();
        surroundingTiles.GetSurroundingTiles();

        currentPosition = transform.position;
        currentPosition = Vector2.MoveTowards(transform.position, (Vector2)transform.position + dir, Time.deltaTime * velocity);
        
        
        currentPosition.z = currentGridLocation.currentLevel;
        transform.position = currentPosition;
        velocity -= itemDrag * Time.deltaTime;
    }
    
    void AddMovement(Vector2 newDirection)
    {
        mainDirection = newDirection.normalized;
        velocity = 0.5f;
    }

   


    bool CheckForNewTile()
    {
        float distance = mainDirection.y < 0 ? 0.2f : 0.05f;
        Vector3 checkPosition = (transform.position + (Vector3)mainDirection * distance) - Vector3.forward;
        nextTilePosition = currentGridLocation.groundGrid.WorldToCell(checkPosition);
        if (nextTilePosition != currentGridLocation.lastTilePosition)
            return true;

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(currentGridLocation.groundGrid.GetCellCenterWorld(nextTilePosition), 0.2f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider is CompositeCollider2D)
            return;
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direction = transform.position - collision.transform.position;
            AddMovement(direction);
        }
        else if (collision.gameObject.CompareTag("Robot") && collision.collider is PolygonCollider2D)
        {
            Vector2 direction = transform.position - collision.transform.position;
            AddMovement(direction);
        }
        else if (!collision.collider.isTrigger)
        {
            mainDirection *= -1;
        }

    }
    

}
