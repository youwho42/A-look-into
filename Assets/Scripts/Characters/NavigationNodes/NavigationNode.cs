using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NavigationNodeType
{
    Outside,
    Inside
}
public class NavigationNode : MonoBehaviour
{
    
    public List<NavigationNode> children = new List<NavigationNode>();
    
    public NavigationNodeType nodeType;
    public void AddNewChild()
    {
        GameObject go = new GameObject("NavNode");
        var node = go.AddComponent<NavigationNode>();
        go.transform.parent = gameObject.transform.parent;
        go.transform.position = transform.position;
        children.Add(node);
        node.children.Add(this);
    }

    public List<NavigationNode> FindPath(NavigationNode targetNode)
    {
        Queue<NavigationNode> queue = new Queue<NavigationNode>();
        Dictionary<NavigationNode, NavigationNode> visitedFrom = new Dictionary<NavigationNode, NavigationNode>();

        queue.Enqueue(this);
        visitedFrom[this] = null;

        while (queue.Count > 0)
        {
            NavigationNode currentNode = queue.Dequeue();

            if (currentNode == targetNode)
            {
                // We found the target node, so construct and return the path
                List<NavigationNode> path = new List<NavigationNode>();
                while (currentNode != null)
                {
                    path.Add(currentNode);
                    currentNode = visitedFrom[currentNode];
                }
                path.Reverse();
                return path;
            }

            foreach (NavigationNode childNode in currentNode.children)
            {
                if (!visitedFrom.ContainsKey(childNode))
                {
                    queue.Enqueue(childNode);
                    visitedFrom[childNode] = currentNode;
                }
            }
        }

        // If we reach here, there is no path to the target node
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.1f);
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
