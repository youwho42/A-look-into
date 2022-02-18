using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class IsometricPathfinding : MonoBehaviour
{

    PathRequestManager requestManager;

    public IsometricGrid isometricGrid;


    void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
    }

    public void StartFindPath(Vector3Int startPos, Vector3Int endPos)
    {
        StartCoroutine(FindPath(startPos, endPos));
    }
   
    IEnumerator FindPath(Vector3Int startPos, Vector3Int endPos)
    {

        List<Vector3> waypoints = new List<Vector3>();
        bool pathSuccess = false;
        
        IsometricNode startNode = isometricGrid.GetIsometricNode(startPos);
        IsometricNode targetNode = isometricGrid.GetIsometricNode(endPos);


        if(startNode.walkable && targetNode.walkable) 
        {
            Heap<IsometricNode> openSet = new Heap<IsometricNode>(isometricGrid.MaxSize);
            HashSet<IsometricNode> closedSet = new HashSet<IsometricNode>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                IsometricNode currentNode = openSet.RemoveFirst();

                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (IsometricNode neighbour in isometricGrid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;

                    
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistanceBetweenNodes(currentNode, neighbour) + neighbour.movementPenalty;
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistanceBetweenNodes(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        

        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    List<Vector3> RetracePath(IsometricNode startNode, IsometricNode endNode)
    {
        List<IsometricNode> path = new List<IsometricNode>();
        IsometricNode currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        List<Vector3> waypoints = ConvertToWorldPositions(path);
        waypoints.Reverse();
        return waypoints;
    }


    List<Vector3> ConvertToWorldPositions(List<IsometricNode> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        
        for (int i = 1; i < path.Count; i++)
        {
            waypoints.Add(isometricGrid.GetTileWorldPosition(path[i].worldPosition));
        }
        return waypoints;
    }


    /*Vector3[] SimplifyPath(List<IsometricNode> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if(directionNew != directionOld)
            {
                waypoints.Add(GetTileWorldPosition(path[i].worldPosition));
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }
*/



    int GetDistanceBetweenNodes(IsometricNode nodeA, IsometricNode nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        /*if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
            
        return 14 * distX + 10 * (distY - distX);*/
        return distX + distY;
    }
}
