using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[Serializable]
public class NavNodeQuadTree
{
    public Bounds bounds;
    public int capacity;
    public List<NavigationNode> allSpots = new List<NavigationNode>();
    public bool divided;
    public NavNodeQuadTree northWest;
    public NavNodeQuadTree northEast;
    public NavNodeQuadTree southWest;
    public NavNodeQuadTree southEast;

    public NavNodeQuadTree(Bounds bounds, int capacity)
    {
        this.bounds = bounds;
        this.capacity = capacity;
    }

    public void Insert(NavigationNode spot)
    {

        if (!bounds.Contains(spot.transform.position))
            return;

        if (allSpots.Count < capacity && !divided)
            allSpots.Add(spot);
        else
        {
            if (!divided)
                Subdivide();


            northEast.Insert(spot);
            northWest.Insert(spot);
            southWest.Insert(spot);
            southEast.Insert(spot);
        }

    }



    public void Subdivide()
    {
        Bounds nw = new Bounds(new Vector3(bounds.center.x - bounds.extents.x / 2, bounds.center.y + bounds.extents.y / 2), bounds.extents);
        northWest = new NavNodeQuadTree(nw, capacity);
        Bounds ne = new Bounds(new Vector3(bounds.center.x + bounds.extents.x / 2, bounds.center.y + bounds.extents.y / 2), bounds.extents);
        northEast = new NavNodeQuadTree(ne, capacity);
        Bounds sw = new Bounds(new Vector3(bounds.center.x - bounds.extents.x / 2, bounds.center.y - bounds.extents.y / 2), bounds.extents);
        southWest = new NavNodeQuadTree(sw, capacity);
        Bounds se = new Bounds(new Vector3(bounds.center.x + bounds.extents.x / 2, bounds.center.y - bounds.extents.y / 2), bounds.extents);
        southEast = new NavNodeQuadTree(se, capacity);

        divided = true;
    }


    public List<NavigationNode> QueryTree(Bounds boundry)
    {

        List<NavigationNode> spots = new List<NavigationNode>();

        if (!bounds.Intersects(boundry))
            return spots;

        foreach (var spot in allSpots)
        {
            if (boundry.Contains(spot.transform.position))
                spots.Add(spot);

        }

        if (divided)
        {
            spots.AddRange(northWest.QueryTree(boundry));
            spots.AddRange(northEast.QueryTree(boundry));
            spots.AddRange(southWest.QueryTree(boundry));
            spots.AddRange(southEast.QueryTree(boundry));
        }

        return spots;
    }



}




public class NavigationNodesManager : MonoBehaviour
{
    public static NavigationNodesManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    //[Serializable]
    //public struct NavigationPath
    //{
    //    public NavigationPathType pathType;
    //    public List<NavigationNode> allNavigationNodes;
    //    public List<NavigationNode> outsideNavigationNodes;
    //    public List<NavigationNode> insideNavigationNodes;

        
    //}
   
    //public List<NavigationPath> paths = new List<NavigationPath>();


    public List<NavigationNode> allAreas = new List<NavigationNode>();
    public Bounds baseBounds = new Bounds(new Vector3(13, -10, 0), new Vector3(256, 256, 40));
    NavNodeQuadTree quadTree;




    private void Start()
    {
        //var allNavigationNodes = GetComponentsInChildren<NavigationNode>().ToList();
        //foreach (var path in paths)
        //{
        //    foreach (var node in allNavigationNodes)
        //    {
        //        if(node.pathType == path.pathType)
        //        {
        //            path.allNavigationNodes.Add(node);
        //            if (node.nodeType == NavigationNodeType.Outside)
        //                path.outsideNavigationNodes.Add(node);
        //            else
        //                path.insideNavigationNodes.Add(node);
        //        }
                
        //    }
        //}

        allAreas.Clear();
        allAreas = FindObjectsOfType<NavigationNode>().ToList();


        quadTree = new NavNodeQuadTree(baseBounds, 10);

        foreach (var spot in allAreas)
        {
            quadTree.Insert(spot);
        }

    }

    public List<NavigationNode> QueryQuadTree(Bounds boundry)
    {
        
        return quadTree.QueryTree(boundry);
    }



    //public NavigationNode GetRandomNode(NavigationNodeType nodeType, NavigationPathType pathType)
    //{
    //    List<NavigationNode> nodes =  new List<NavigationNode>();
    //    foreach  (var path in paths)
    //    {
    //        if (path.pathType != pathType)
    //            continue;
    //        nodes = nodeType == NavigationNodeType.Outside ? path.outsideNavigationNodes : path.insideNavigationNodes;
            
    //    }

    //    int r = -1;
    //    while(r == -1)
    //    {
    //        int n = UnityEngine.Random.Range(0, nodes.Count);
    //        if (nodes[n].active)
    //            r = n;
    //    }
        
    //    return nodes[r];
    //}
    //public NavigationNode GetRandomNode(NavigationNodeType nodeType, NavigationPathType pathType, Vector3 position, float maxDistance)
    //{
    //    List<NavigationNode> nodes = new List<NavigationNode>();
    //    List<NavigationNode> nodesInArea = new List<NavigationNode>();
    //    foreach (var path in paths)
    //    {
    //        if (path.pathType != pathType)
    //            continue;
    //        nodes = nodeType == NavigationNodeType.Outside ? path.outsideNavigationNodes : path.insideNavigationNodes;
    //        foreach (var node in nodes)
    //        {
    //            var dist = Vector3.Distance(position, node.transform.position);
    //            if (dist <= maxDistance)
    //                nodesInArea.Add(node);
    //        }
    //    }

    //    int r = -1;
    //    while (r == -1)
    //    {
    //        int n = UnityEngine.Random.Range(0, nodesInArea.Count);
    //        if (nodesInArea[n].active)
    //            r = n;
    //    }
    //    return nodesInArea[r];
    //}
    //public NavigationNode GetClosestNavigationNode(Vector3 position, NavigationNodeType nodeType, NavigationPathType pathType)
    //{
    //    NavigationNode bestNode = null;
    //    float closestDistanceSqr = Mathf.Infinity;
    //    foreach (var path in paths)
    //    {
    //        if (path.pathType != pathType)
    //            continue;

    //        var searchNodes = nodeType == NavigationNodeType.Outside ? path.outsideNavigationNodes : path.insideNavigationNodes;
    //        foreach (NavigationNode node in searchNodes)
    //        {
    //            if(!node.active)
    //                continue;
    //            Vector3 directionToNode = node.transform.position - position;
    //            float dSqrToNode = directionToNode.sqrMagnitude;
    //            if (dSqrToNode < closestDistanceSqr)
    //            {
    //                closestDistanceSqr = dSqrToNode;
    //                bestNode = node;
    //            }
    //        }
    //    }

        
    //    return bestNode;
    //}
}
