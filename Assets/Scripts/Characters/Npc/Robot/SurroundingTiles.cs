using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SurroundingTiles : MonoBehaviour
{
    public Grid grid;
    public Tilemap groundMap;
    CurrentGridLocation currentLocation;
    
    public Dictionary<Vector3Int, bool> directions = new Dictionary<Vector3Int, bool>();
    
    private void Start()
    {
        currentLocation = GetComponent<CurrentGridLocation>();
        GetSurroundingTiles();


    }
   
    public void GetSurroundingTiles()
    {
        SetGrid();
        directions.Clear();
        CheckDirection(true);
        CheckDirection(false);

    }
    void CheckDirection(bool onX)
    {
        for (int x = -1; x < 2; x++)
        {
            if (x == 0)
                continue;

            bool walkable = false;

            for (int z = 0; z < 2; z++)
            {
                Vector3Int currentPosition = currentLocation.lastTilePosition;
                currentPosition.x += onX ? x : 0;
                currentPosition.y += onX ? 0 : x;
                currentPosition.z += z;
                TileBase tile = groundMap.GetTile(currentPosition);
                
                if (tile != null) //if tile is not null
                {
                    if (z == 0) // we are on z0
                        walkable = true;
                    else // we are on z1
                        walkable = false;
                }
                else// the tile is null
                {
                    if (z != 0)// we are on z1
                        walkable = true;
                    else //we are on z0
                        walkable = false;
                }

                if (directions.ContainsKey(new Vector3Int(onX ? x : 0, onX ? 0 : x, 0)))
                {
                    if (!directions[new Vector3Int(onX ? x : 0, onX ? 0 : x, 0)])
                        continue;
                    else
                        directions[new Vector3Int(onX ? x : 0, onX ? 0 : x, 0)] = walkable;
                }
                    
                else
                    directions.Add(new Vector3Int(onX ? x : 0, onX ? 0 : x, 0), walkable);

            }

        }
    }

    

    public Vector3 GetTileWorldPosition(Vector3Int tile)
    {

        var tileworldpos = groundMap.GetCellCenterWorld(tile);
        return tileworldpos;
    }


    


    private void OnDrawGizmosSelected()
    {
        
        if (directions.Count > 0)
        {
            foreach (var item in directions)
            {
                Gizmos.color = item.Value ? Color.green : Color.red;
                var p = item.Key + currentLocation.lastTilePosition;
                Gizmos.DrawWireSphere(GetTileWorldPosition(p), 0.1f);
            }
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(GetTileWorldPosition(currentLocation.lastTilePosition), 0.1f);
        }
        
        
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
