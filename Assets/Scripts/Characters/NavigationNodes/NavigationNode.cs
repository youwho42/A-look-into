using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum NavigationNodeType
{
    Outside,
    Inside
}
public enum NavigationPathType
{
    BluValley,
    Town
}


public class NavigationNode : MonoBehaviour
{
    
    public List<NavigationNode> children = new List<NavigationNode>();
    
    public NavigationNodeType nodeType;
    public NavigationPathType pathType;

    public bool active = true;

    public Color gizmoColor = Color.white;
    public GameObject AddNewChild()
    {
        GameObject go = new GameObject("NavNode");
        var node = go.AddComponent<NavigationNode>();
        var coll = go.AddComponent<CircleCollider2D>();
        coll.radius = 0.01f;
        coll.isTrigger = true;
        int Layer = LayerMask.NameToLayer("NavNode");
        go.layer = Layer;
        go.transform.parent = gameObject.transform.parent;
        go.transform.position = transform.position;
        children.Add(node);
        node.children.Add(this);
        node.pathType = pathType;
        node.gizmoColor = gizmoColor;
        return go;
    }

    public List<NavigationNode> FindPath(NavigationNode targetNode)
    {
        List<NavigationNode> path = new List<NavigationNode>();

        if (this == targetNode)
        {
            // The target node is the same as the starting node,
            // return an empty path.
            
            path.Add(targetNode);
            return path;
        }

        Queue<NavigationNode> queue = new Queue<NavigationNode>();
        Dictionary<NavigationNode, NavigationNode> visitedFrom = new Dictionary<NavigationNode, NavigationNode>();
        HashSet<NavigationNode> visitedNodes = new HashSet<NavigationNode>();

        queue.Enqueue(this);
        visitedFrom[this] = null;
        visitedNodes.Add(this);

        while (queue.Count > 0)
        {
            NavigationNode currentNode = queue.Dequeue();
            List<NavigationNode> nearestNodes = currentNode.children;
            nearestNodes = nearestNodes.OrderBy(
               x => NumberFunctions.GetDistanceV3(targetNode.transform.position, x.transform.position)
               ).ToList();
            for (int i = 0; i < nearestNodes.Count; i++)
            {
                NavigationNode childNode = nearestNodes[i];

                if (!visitedNodes.Contains(childNode))
                {
                    queue.Enqueue(childNode);
                    visitedFrom[childNode] = currentNode;
                    visitedNodes.Add(childNode);

                    if (childNode == targetNode)
                    {
                        // We found the target node, so construct and return the path
                        
                        while (childNode != null)
                        {
                            path.Add(childNode);
                            childNode = visitedFrom[childNode];
                        }
                        path.Reverse();
                        return path;
                    }
                }
            }
        }

        // If we reach here, there is no path to the target node
        
        path.Add(this);
        return path;
        
    }


    public List<NavigationNode> FindPath(Vector3 targetPosition)
    {
        List<NavigationNode> path = new List<NavigationNode>();

        // Find the closest reachable node to the target position
        NavigationNode targetNode = FindClosestReachableNode(targetPosition);

        if (targetNode == null)
        {
            // No reachable node found, return an empty path
            return path;
        }

        if (this == targetNode)
        {
            // The target node is the same as the starting node,
            // return a path with just the starting node.
            path.Add(targetNode);
            return path;
        }

        Queue<NavigationNode> queue = new Queue<NavigationNode>();
        Dictionary<NavigationNode, NavigationNode> visitedFrom = new Dictionary<NavigationNode, NavigationNode>();
        HashSet<NavigationNode> visitedNodes = new HashSet<NavigationNode>();

        queue.Enqueue(this);
        visitedFrom[this] = null;
        visitedNodes.Add(this);

        while (queue.Count > 0)
        {
            NavigationNode currentNode = queue.Dequeue();
            List<NavigationNode> nearestNodes = currentNode.children;
            nearestNodes = nearestNodes.OrderBy(
               x => NumberFunctions.GetDistanceV3(targetPosition, x.transform.position)
               ).ToList();

            for (int i = 0; i < nearestNodes.Count; i++)
            {
                NavigationNode childNode = nearestNodes[i];

                if (!visitedNodes.Contains(childNode))
                {
                    queue.Enqueue(childNode);
                    visitedFrom[childNode] = currentNode;
                    visitedNodes.Add(childNode);

                    if (childNode == targetNode)
                    {
                        // We found the target node, so construct and return the path
                        while (childNode != null)
                        {
                            path.Add(childNode);
                            childNode = visitedFrom[childNode];
                        }
                        path.Reverse();
                        return path;
                    }
                }
            }
        }

        // If we reach here, there is no path to the target node
        return path;
    }

    private NavigationNode FindClosestReachableNode(Vector3 targetPosition)
    {
        Queue<NavigationNode> queue = new Queue<NavigationNode>();
        HashSet<NavigationNode> visitedNodes = new HashSet<NavigationNode>();
        queue.Enqueue(this);
        visitedNodes.Add(this);

        NavigationNode closestNode = null;
        float closestDistance = float.MaxValue;

        while (queue.Count > 0)
        {
            NavigationNode currentNode = queue.Dequeue();
            float distanceToTarget = NumberFunctions.GetDistanceV3(targetPosition, currentNode.transform.position);

            if (distanceToTarget < closestDistance)
            {
                closestDistance = distanceToTarget;
                closestNode = currentNode;
            }

            foreach (var childNode in currentNode.children)
            {
                if (!visitedNodes.Contains(childNode))
                {
                    queue.Enqueue(childNode);
                    visitedNodes.Add(childNode);
                }
            }
        }

        return closestNode;
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, 0.02f);
        if(children.Count > 0)
        {
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, .2f);
            for (int i = 0; i < children.Count; i++)
            {
                Gizmos.DrawLine(transform.position, children[i].transform.position);
            }
            
        }
    }
}
