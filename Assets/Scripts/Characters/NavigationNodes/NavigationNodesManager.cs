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

    private void Start()
    {
        allNavigationNodes = GetComponentsInChildren<NavigationNode>().ToList();
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
