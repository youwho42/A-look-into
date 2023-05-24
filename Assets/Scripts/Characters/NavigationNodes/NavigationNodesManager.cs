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
    public List<NavigationNode> allNavigationNodes = new List<NavigationNode>();
    public List<NavigationNode> outsideNavigationNodes = new List<NavigationNode>();
    public List<NavigationNode> insideNavigationNodes = new List<NavigationNode>();

    private void Start()
    {
        allNavigationNodes = GetComponentsInChildren<NavigationNode>().ToList();
        foreach (var node in allNavigationNodes)
        {
            if (node.nodeType == NavigationNodeType.Outside)
                outsideNavigationNodes.Add(node);
            else
                insideNavigationNodes.Add(node);
        }
    }

    public NavigationNode GetRandomNode(NavigationNodeType nodeType)
    {
        var nodes = nodeType == NavigationNodeType.Outside ? outsideNavigationNodes : insideNavigationNodes;
        int r = Random.Range(0, nodes.Count);
        return nodes[r];
    }
    public NavigationNode GetRandomNode(NavigationNodeType nodeType, Vector3 position, float maxDistance)
    {
        var nodes = nodeType == NavigationNodeType.Outside ? outsideNavigationNodes : insideNavigationNodes;
        List<NavigationNode> nodesInArea = new List<NavigationNode>();
        foreach (var node in nodes)
        {
            var dist = Vector2.Distance(position, node.transform.position);
            if (dist <= maxDistance)
                nodesInArea.Add(node);
        }
        int r = Random.Range(1, nodesInArea.Count);
        return nodesInArea[r];
    }
    public NavigationNode GetClosestNavigationNode(Vector3 position, NavigationNodeType nodeType)
    {
        NavigationNode bestNode = null;
        float closestDistanceSqr = Mathf.Infinity;
        
        foreach (NavigationNode node in allNavigationNodes)
        {
            if (node.nodeType != nodeType) continue;
            Vector3 directionToNode = node.transform.position - position;
            float dSqrToNode = directionToNode.sqrMagnitude;
            if (dSqrToNode < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToNode;
                bestNode = node;
            }
        }

        return bestNode;
    }
}
