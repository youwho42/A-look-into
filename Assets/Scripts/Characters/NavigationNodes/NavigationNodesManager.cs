using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    [Serializable]
    public struct NavigationPath
    {
        public NavigationPathType pathType;
        public List<NavigationNode> allNavigationNodes;
        public List<NavigationNode> outsideNavigationNodes;
        public List<NavigationNode> insideNavigationNodes;

        
    }
   
    public List<NavigationPath> paths = new List<NavigationPath>();

    private void Start()
    {
        var allNavigationNodes = GetComponentsInChildren<NavigationNode>().ToList();
        foreach (var path in paths)
        {
            foreach (var node in allNavigationNodes)
            {
                if(node.pathType == path.pathType)
                {
                    path.allNavigationNodes.Add(node);
                    if (node.nodeType == NavigationNodeType.Outside)
                        path.outsideNavigationNodes.Add(node);
                    else
                        path.insideNavigationNodes.Add(node);
                }
                
            }
        }
        
    }

    public NavigationNode GetRandomNode(NavigationNodeType nodeType, NavigationPathType pathType)
    {
        List<NavigationNode> nodes =  new List<NavigationNode>();
        foreach  (var path in paths)
        {
            if (path.pathType != pathType)
                continue;
            nodes = nodeType == NavigationNodeType.Outside ? path.outsideNavigationNodes : path.insideNavigationNodes;
            
        }
        int r = UnityEngine.Random.Range(0, nodes.Count);
        return nodes[r];
    }
    public NavigationNode GetRandomNode(NavigationNodeType nodeType, NavigationPathType pathType, Vector3 position, float maxDistance)
    {
        List<NavigationNode> nodes = new List<NavigationNode>();
        List<NavigationNode> nodesInArea = new List<NavigationNode>();
        foreach (var path in paths)
        {
            if (path.pathType != pathType)
                continue;
            nodes = nodeType == NavigationNodeType.Outside ? path.outsideNavigationNodes : path.insideNavigationNodes;
            foreach (var node in nodes)
            {
                var dist = Vector2.Distance(position, node.transform.position);
                if (dist <= maxDistance)
                    nodesInArea.Add(node);
            }
        }
            
        int r = UnityEngine.Random.Range(1, nodesInArea.Count);
        return nodesInArea[r];
    }
    public NavigationNode GetClosestNavigationNode(Vector3 position, NavigationNodeType nodeType, NavigationPathType pathType)
    {
        NavigationNode bestNode = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach (var path in paths)
        {
            if (path.pathType != pathType)
                continue;
            foreach (NavigationNode node in path.allNavigationNodes)
            {
                if (node.nodeType != nodeType)
                    continue;
                Vector3 directionToNode = node.transform.position - position;
                float dSqrToNode = directionToNode.sqrMagnitude;
                if (dSqrToNode < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToNode;
                    bestNode = node;
                }
            }
        }
        

        return bestNode;
    }
}
