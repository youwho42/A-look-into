using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToIsometricNode : MonoBehaviour
{

    CurrentGridLocation currentGridLocation;
    List<Vector3> path = new List<Vector3>();
    int currentPathIndex;
    Vector3 targetPos;
    public Vector3 currentDestination;
    Vector3 currentPosition;
    public float speed;
    public bool hasPath;
    public bool pathComplete;
    private IEnumerator Start()
    {
        currentGridLocation = GetComponent<CurrentGridLocation>();

        
        yield return new WaitForSeconds(0.3f);
        pathComplete = true;
        currentPathIndex = 0;
        var currentLocation = currentGridLocation.GetCurrentGridLocation();
        currentLocation.z = 0;
        
    }

    public void PathFound(List<Vector3> newPath)
    {
        path.Clear();
        currentPathIndex = 0;
        path = newPath;
        hasPath = true;
        currentDestination = path[0];
        pathComplete = false;
        
    }

    
    
    public void Move()
    {

        var dist = Vector2.Distance(transform.position, currentDestination);
        if (dist <= 0.001f && currentPathIndex == path.Count-1)
        {
            hasPath = false;
            pathComplete = true;
        }

        if (!pathComplete)
        {
            currentGridLocation.UpdateLocation();
            currentPosition = transform.position;
            currentPosition = Vector2.MoveTowards(transform.position, new Vector3(currentDestination.x, currentDestination.y, currentGridLocation.currentLevel), Time.deltaTime * speed);
            currentPosition.z = currentGridLocation.currentLevel;
            transform.position = currentPosition;

            

            if (dist <= 0.001f && currentPathIndex < path.Count)
            {
                currentPathIndex++;
                currentDestination = path[currentPathIndex];
            }
            

        }
    }

    public Vector3 GetTileWorldPosition(Vector3Int tile)
    {

        var tileworldpos = currentGridLocation.groundMap.GetCellCenterWorld(tile);
        return tileworldpos;
    }

    
}
