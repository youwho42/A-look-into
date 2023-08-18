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
    public GameObject AddNewChild()
    {
        GameObject go = new GameObject("NavNode");
        var node = go.AddComponent<NavigationNode>();
        go.transform.parent = gameObject.transform.parent;
        go.transform.position = transform.position;
        children.Add(node);
        node.children.Add(this);
        node.pathType = pathType;
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
               x => Vector2.Distance(targetNode.transform.position, x.transform.position)
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


    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.02f);
        if(children.Count > 0)
        {
            Gizmos.color = new Color(1, 1, 1, .2f);
            for (int i = 0; i < children.Count; i++)
            {
                Gizmos.DrawLine(transform.position, children[i].transform.position);
            }
            
        }
    }
}
