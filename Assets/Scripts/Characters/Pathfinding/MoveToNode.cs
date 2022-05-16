using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToNode : MonoBehaviour
{
    GravityItem gravityItem;
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

        gravityItem = GetComponent<GravityItem>();
        yield return new WaitForSeconds(0.3f);
        pathComplete = true;
        currentPathIndex = 0;
        

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
        if (dist <= 0.001f && currentPathIndex == path.Count - 1)
        {
            hasPath = false;
            pathComplete = true;
        }

        if (!pathComplete)
        {
            
            currentPosition = transform.position;
            currentPosition = Vector2.MoveTowards(transform.position, new Vector2(currentDestination.x, currentDestination.y), Time.deltaTime * speed);
            currentPosition.z = gravityItem.surroundingTiles.currentTilePosition.z + 1;
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

        var tileworldpos = gravityItem.surroundingTiles.groundMap.GetCellCenterWorld(tile);
        return tileworldpos;
    }
}
