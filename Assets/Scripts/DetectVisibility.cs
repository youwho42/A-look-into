using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DetectVisibility : MonoBehaviour
{

    SurroundingTilesInfo surroundingTiles;
    //CurrentGridLocation currentGridLocation;
    
    Vector3Int lastTilePosition;

    public Transform objectCorrectionZ;

    public bool isHidden;

    private void Start()
    {
        surroundingTiles = GetComponent<SurroundingTilesInfo>();
        /*currentGridLocation = GetComponent<CurrentGridLocation>();
        lastTilePosition = currentGridLocation.lastTilePosition;*/
    }


    private void Update()
    {
        /*if (lastTilePosition != currentGridLocation.lastTilePosition)
        {
            CheckTiles();
        }*/
        CheckTiles();
    }

    void CheckTiles()
    {
        isHidden = false;
        foreach (var tile in surroundingTiles.allCurrentDirections)
        {
            if (tile.Key == Vector3Int.left || tile.Key == -(Vector3Int)Vector2Int.one || tile.Key == Vector3Int.down)
            {
                if (tile.Value.levelZ > 0 && !tile.Value.isValid)
                {
                    if (CheckObjectPosition())
                        isHidden = true;
                }
                if (tile.Value.levelZ == 0 && tile.Value.isValid && tile.Value.tileName.Contains("Slope"))
                {
                    if (CheckObjectPosition())
                        isHidden = true;
                }
            }
        }
        ChangeObjectZ(isHidden);

    }
    bool CheckObjectPosition()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);

        Vector3Int tilepos = surroundingTiles.grid.WorldToCell(pos);
        Vector3 tileworldpos = surroundingTiles.grid.CellToWorld(tilepos);
        Vector3 relativePos = pos - tileworldpos;
        if (relativePos.y < 0.3f)
            return true;

        return false;
    }

    void ChangeObjectZ(bool isHidden)
    {
        Vector3 pos = new Vector3(0, 0, isHidden ? -.9f : 0);
        objectCorrectionZ.localPosition = pos;

    }


}
