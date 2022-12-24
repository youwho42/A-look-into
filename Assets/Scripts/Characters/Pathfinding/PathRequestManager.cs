using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


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


    Queue<PathResult> results = new Queue<PathResult>();

    IsometricPathfindingXYZ pathfinding;
    static List<IsometricNodeXYZ> walkableNodes = new List<IsometricNodeXYZ>();

    private void Start()
    {
        Invoke("GetWalkableNodes", 2f);
    }


    private void Update()
    {
        if(results.Count > 0) 
        {
            int itemsInQueue = results.Count;
            lock (results)
            {
                for (int i = 0; i < itemsInQueue; i++)
                {
                    PathResult result = results.Dequeue();
                    var path = pathfinding.ConvertToWorldPositions(result.path);
                    result.callback(path, result.success);
                }
            }
        }
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

    public static void RequestPath(PathRequest request)
    {
        ThreadStart threadStart = delegate
        {
            instance.pathfinding.FindPath(request, instance.FinishedProcessingPath);
        };
        Thread thread = new Thread(threadStart);
        thread.Start();
    }

    
    public void FinishedProcessingPath(PathResult result)
    {
        lock (results)
        {
            results.Enqueue(result);
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

public struct PathResult
{
    public List<IsometricNodeXYZ> path;
    public bool success;
    public Action<List<Vector3>, bool> callback;

    public PathResult(List<IsometricNodeXYZ> path, bool success, Action<List<Vector3>, bool> callback)
    {
        this.path = path;
        this.success = success;
        this.callback = callback;
    }
}



public struct PathRequest
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
