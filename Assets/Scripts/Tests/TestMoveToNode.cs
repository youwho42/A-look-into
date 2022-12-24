using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;


public class TestMoveToNode : MonoBehaviour
{
    MoveToNode moveToNode;
    public GameObject target;
    [HideInInspector]
    public Grid grid;
    [HideInInspector]
    public Tilemap groundMap;
    private void Start()
    {
        moveToNode = GetComponent<MoveToNode>();
        
    }

    private void Update()
    {

        if (moveToNode.hasPath)
        {
            moveToNode.Move();
            
            
        }
        
        
    }

    [ContextMenu("Set Desination")]
    void SetDestination()
    {
        //Vector3Int dest = PathRequestManager.GetRandomWalkableNode();

        PathRequestManager.RequestPath(new PathRequest(GetTileZ(transform.position), GetTileZ(target.transform.position), OnPathFound));
    }
    public void OnPathFound(List<Vector3> newPath, bool success)
    {
        if (success)
        {
            moveToNode.PathFound(newPath);
        }
        else
        {
            Debug.Log("Path not found");
        }
        
    }

    public Vector3Int GetTileZ(Vector3 position)
    {
        SetGrid();

        Vector3Int cellIndex = groundMap.WorldToCell(position - Vector3.forward);
        for (int i = groundMap.cellBounds.zMax; i > groundMap.cellBounds.zMin - 1; i--)
        {
            cellIndex.z = i;
            var tile = groundMap.GetTile(cellIndex);
            if (tile != null)
            {

                return cellIndex;
            }
        }

        return cellIndex;

    }
    void SetGrid()
    {
        if (grid != null)
            return;

        grid = FindObjectOfType<Grid>();
        Tilemap[] maps = grid.GetComponentsInChildren<Tilemap>();
        foreach (var map in maps)
        {
            if (map.gameObject.name == "GroundTiles")
            {
                groundMap = map;
            }
        }
    }

}
