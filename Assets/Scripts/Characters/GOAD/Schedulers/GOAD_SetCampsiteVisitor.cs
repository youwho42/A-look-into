using Klaxon.GOAD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAD_SetCampsiteVisitor : MonoBehaviour
{
    public GOAD_Action_AStarGoToNode goHomeAction;
    public GOAD_Action_GoToNodeFromClosestNode reachBedAction;
    public GOAD_Action_GoToNodeFromClosestNode leaveHomeAction;

    public void SetNodes(NavigationNode homeNode, NavigationNode bedNode)
    {
        goHomeAction.targetNode = homeNode;
        leaveHomeAction.endNode = homeNode;
        reachBedAction.endNode = bedNode;
    }

}
