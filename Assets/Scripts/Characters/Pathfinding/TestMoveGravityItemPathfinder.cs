using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMoveGravityItemPathfinder : MonoBehaviour
{
    bool gettingPath;

    public Transform destination;

    //private void Start()
    //{
    //    Invoke("SetDestination", 5f);
    //}
    public List<Vector3> path = new List<Vector3>();

    [ContextMenu("Look for path")]
    void SetDestination(/*Vector3Int tilePosition*/)
    {
        Debug.Log("Looking for path");
        Vector3 destPos = destination.position;
        destPos.z -= 1;
        Vector3Int gridPos = GridManager.instance.groundMap.WorldToCell(destPos);
        //Vector3Int dest = PathRequestManager.GetRandomWalkableNode();
        gettingPath = true;
        PathRequestManager.RequestPath(new PathRequest(PlayerInformation.instance.currentTilePosition.position, gridPos, OnPathFound));
    }

    public void OnPathFound(List<Vector3> newPath, bool success)
    {
        if (success)
        {
            path.Clear();
            path = newPath;
            
            //moveToNode.PathFound(newPath);
        }
        else
        {
            Debug.Log("Path not found");
        }
        gettingPath = false;
    }
    private void OnDrawGizmosSelected()
    {
        foreach (var pos in path)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(pos, 0.1f);  
        }
    }

}
