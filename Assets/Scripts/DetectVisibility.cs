using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DetectVisibility : MonoBehaviour
{

    AllTilesInfoManager allTilesInfo;
    CurrentTilePosition currentPosition;

    
    public Transform objectCorrectionZ;

    public bool isHidden;

    private void Start()
    {
        allTilesInfo = AllTilesInfoManager.instance;
        currentPosition = GetComponent<CurrentTilePosition>();
    }


    private void Update()
    {
        
        CheckTiles();
    }

    void CheckTiles()
    {
        isHidden = false;
        List<TileDirectionInfo> tileBlock;
        allTilesInfo.allTilesDictionary.TryGetValue(currentPosition.position, out tileBlock); 
        if (tileBlock == null)
            return;
        foreach (var tile in tileBlock)
        {
            if (tile.direction == Vector3Int.left || tile.direction == -(Vector3Int)Vector2Int.one || tile.direction == Vector3Int.down)
            {
                if (tile.levelZ > 0 && !tile.isValid)
                {
                    if (CheckObjectPosition())
                        isHidden = true;
                    
                    
                }
                if (tile.levelZ == 0 && tile.isValid && tile.tileName.Contains("Slope"))
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

        Vector3Int tilepos = currentPosition.grid.WorldToCell(pos);
        Vector3 tileworldpos = currentPosition.grid.CellToWorld(tilepos);
        Vector3 relativePos = pos - tileworldpos;

        if (relativePos.y < 0.33f)
            return true;

        return false;
    }

    void ChangeObjectZ(bool isHidden)
    {
        
        Vector3 pos = new Vector3(0, 0, isHidden ? -0.9f : 0);
        objectCorrectionZ.localPosition = pos;
        
        
    }


}
