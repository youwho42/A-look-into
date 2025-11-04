using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Linq;

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

    static Thread thread;
    Queue<PathResult> results = new Queue<PathResult>();
    Queue<PathRequest> requests = new Queue<PathRequest>();

    [HideInInspector]
    public IsometricPathfindingXYZ pathfinding;
    

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

        if (thread != null)
        {
            if (requests.Count > 0 && thread.ThreadState == ThreadState.Stopped)
            {
                RequestPath(requests.Dequeue());
            }
        }
    }



    public static void RequestPath(PathRequest request)
    {
        if (thread != null) {
            if (thread.ThreadState == ThreadState.Running)
            {
                lock (instance.requests)
                {
                    instance.requests.Enqueue(request);
                }
                return;
            }
        }
        
       

        thread = new Thread(delegate()
        {
            instance.pathfinding.FindPath(request, instance.FinishedProcessingPath);
        });
        thread.Start();
        
    }


    public void FinishedProcessingPath(PathResult result)
    {
        
        lock (results)
        {
            results.Enqueue(result);
        }
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
    public bool needsSlopes;
    public Action<List<Vector3>, bool> pathCallback;
    

    public PathRequest(Vector3Int start, Vector3Int end, bool _needsSlopes, Action<List<Vector3>, bool> callback)
    {
        pathStart = start;
        pathEnd = end;
        needsSlopes = _needsSlopes;
        pathCallback = callback;
    }
}
