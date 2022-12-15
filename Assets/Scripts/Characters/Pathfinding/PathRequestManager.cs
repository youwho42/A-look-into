using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{

    public static PathRequestManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        pathfinding = GetComponent<IsometricPathfindingXYZ>();
    }


    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    IsometricPathfindingXYZ pathfinding;
    bool isProcessingPath;
    static List<IsometricNodeXYZ> walkableNodes = new List<IsometricNodeXYZ>();

    private void Start()
    {
        Invoke("GetWalkableNodes", 2f);
    }

    public void GetWalkableNodes()
    {
        List<IsometricNodeXYZ> tempNodes = pathfinding.isometricGrid.isometricNodes;
        for (int i = 0; i < tempNodes.Count; i++)
        {
            if (tempNodes[i].walkable)
                walkableNodes.Add(tempNodes[i]);
        }
    }

    public static void RequestPath(Vector3Int pathStart, Vector3Int pathEnd, Action<List<Vector3>, bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if(!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(List<Vector3> path, bool success)
    {
        currentPathRequest.pathCallback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3Int pathStart;
        public Vector3Int pathEnd;
        public Action<List<Vector3>, bool> pathCallback;

        public PathRequest(Vector3Int start, Vector3Int end, Action<List<Vector3>, bool> callback)
        {
            pathStart = start;
            pathEnd = end;
            pathCallback = callback;
        }
    }

    public static Vector3Int GetRandomWalkableNode()
    {
        if (walkableNodes.Count == 0)
            instance.GetWalkableNodes();
        int r = UnityEngine.Random.Range(0, walkableNodes.Count-1);
        return walkableNodes[r].worldPosition;
    }
    
    public static Vector3Int GetRandomNeighbourTile(Vector3Int currentPosition)
    {
        int x = UnityEngine.Random.Range(-1, 2);
        int y = UnityEngine.Random.Range(-1, 2);
        Vector3Int newTile = new Vector3Int(currentPosition.x + x, currentPosition.y + y, 0);
        
        foreach (var node in walkableNodes)
        {
            if (node.worldPosition == newTile && !node.walkable)
            {
                GetRandomNeighbourTile(currentPosition);

            }
               

        }
        return newTile;

    }

    public static Vector3Int GetRandomDistancedTile(Vector3Int currentPosition, int distanceRadius)
    {
        
        int x = UnityEngine.Random.Range(-distanceRadius, distanceRadius);
        int y = UnityEngine.Random.Range(-distanceRadius, distanceRadius);
        Vector3Int newTile = new Vector3Int(currentPosition.x + x, currentPosition.y + y, 0);

        foreach (var node in walkableNodes)
        {
            if (node.worldPosition == newTile && !node.walkable)
            {
                GetRandomNeighbourTile(currentPosition);

            }


        }
        return newTile;

    }
}
