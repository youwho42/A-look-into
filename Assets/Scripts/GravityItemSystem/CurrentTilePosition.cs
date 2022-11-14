using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CurrentTilePosition : MonoBehaviour
{
    [HideInInspector]
    public Grid grid;
    [HideInInspector]
    public Tilemap groundMap;
    public Vector3Int currentTilePosition;

    private void Start()
    {
        currentTilePosition = CheckCurrentTilePosition();
    }
    private Vector3Int CheckCurrentTilePosition()
    {
        SetGrid();
        return grid.WorldToCell(transform.position - Vector3.forward);
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
