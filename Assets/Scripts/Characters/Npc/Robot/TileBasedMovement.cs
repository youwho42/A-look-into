using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBasedMovement : MonoBehaviour
{

    public float speed;
    
    public Vector2 worldDestination;
    SurroundingTiles surroundingTiles;
    CurrentGridLocation currentLocation;
    Vector3 currentPosition;
    private IEnumerator Start()
    {
        
        surroundingTiles = GetComponent<SurroundingTiles>();
        currentLocation = GetComponent<CurrentGridLocation>();
        yield return new WaitForSeconds(0.5f);
        SetDestination(currentLocation.lastTilePosition);
    }

    public void Move()
    {
        currentPosition = transform.position;
        currentPosition = Vector2.MoveTowards(transform.position, worldDestination, Time.deltaTime * speed);
        currentPosition.z = currentLocation.currentLevel;
        transform.position = currentPosition;
    }

    public void SetDestination(Vector3Int tileDestination)
    {


        worldDestination = surroundingTiles.GetTileWorldPosition(tileDestination);
    }

}
