using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricPathfindingXYZ : MonoBehaviour
{

    PathRequestManager requestManager;

    public IsometricGridXYZ isometricGrid;


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

        IsometricNodeXYZ startNode = isometricGrid.GetIsometricNode(startPos);
        IsometricNodeXYZ targetNode = isometricGrid.GetIsometricNode(endPos);

       
        if (startNode.walkable && targetNode.walkable)
        {
            Heap<IsometricNodeXYZ> openSet = new Heap<IsometricNodeXYZ>(isometricGrid.MaxSize);
            HashSet<IsometricNodeXYZ> closedSet = new HashSet<IsometricNodeXYZ>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                IsometricNodeXYZ currentNode = openSet.RemoveFirst();

                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (IsometricNodeXYZ neighbour in isometricGrid.GetNeighbours(currentNode))
                {
                    if (neighbour == null)
                        continue;
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;

                    if(currentNode.gridZ != neighbour.gridZ) 
                    { 
                        int diff = neighbour.gridZ - currentNode.gridZ;

                        // if the next tile is not on the same z level and not a slope
                        if (diff == -1 && !neighbour.slope || diff == 1 && !currentNode.slope)
                        {
                            continue;
                        }
                        // If I am approaching a slope, am i approaching the slope in a valid direction?
                        if (neighbour.slope)
                        {
                            if (neighbour.tileName.Contains("X") && currentNode.gridX - neighbour.gridX == 0 || neighbour.tileName.Contains("Y") && currentNode.gridY - neighbour.gridY == 0)
                                continue;
                        }
                        // I am on a slope
                        if (currentNode.slope)
                        {
                            //am i walking 'off' the slope on the upper part in the right direction?
                            if (currentNode.tileName.Contains("X") && currentNode.gridX - neighbour.gridX == 0 || currentNode.tileName.Contains("Y") && currentNode.gridY - neighbour.gridY == 0)
                            {
                                continue;
                            }
                            
                        }
                    }
                    // if neighbor z is one up and  currentNode is a slope
                    // all good

                    // if neighbor z is one down and IT is a slope
                    // all good

                    // Here we have all the neighboring nodes that are walkable. 
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

    List<Vector3> RetracePath(IsometricNodeXYZ startNode, IsometricNodeXYZ endNode)
    {
        List<IsometricNodeXYZ> path = new List<IsometricNodeXYZ>();
        IsometricNodeXYZ currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        List<Vector3> waypoints = ConvertToWorldPositions(path);
        waypoints.Reverse();
        return waypoints;
    }


    List<Vector3> ConvertToWorldPositions(List<IsometricNodeXYZ> path)
    {
        List<Vector3> waypoints = new List<Vector3>();

        for (int i = 0; i < path.Count; i++)
        {
            waypoints.Add(isometricGrid.GetTileWorldPosition(path[i].worldPosition));
        }
        return waypoints;
    }




    int GetDistanceBetweenNodes(IsometricNodeXYZ nodeA, IsometricNodeXYZ nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        /*if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
            
        return 14 * distX + 10 * (distY - distX);*/
        return distX + distY;
    }
}
