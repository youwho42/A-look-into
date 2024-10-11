using Klaxon.GravitySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DetectVisibility : MonoBehaviour
{

    AllTilesInfoManager allTilesInfo;
    CurrentTilePosition currentPosition;

    SpriteRenderer sprite;
    public Transform objectCorrectionZ;

    public bool isHidden;

    List<GameObject> tileHiders = new List<GameObject>();
    GravityItemFly gravityItemFly;
    float displacementPos;
    private void Start()
    {
        allTilesInfo = AllTilesInfoManager.instance;
        currentPosition = GetComponent<CurrentTilePosition>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        gravityItemFly = GetComponent<GravityItemFly>();
    }


    private void Update()
    {

        if (sprite.isVisible)
        {
            if (gravityItemFly)
            {
                if (gravityItemFly.enabled)
                {
                    objectCorrectionZ.localPosition = Vector3.zero;
                    return;
                }
                    
            }
            CheckTiles(); 
        }
        
            
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
    public bool CheckObjectPosition()
    {

        
        Vector3 pos = transform.position - Vector3.forward;

        Vector3Int tilepos = currentPosition.grid.WorldToCell(pos);
        Vector3 tileworldpos = currentPosition.grid.CellToWorld(tilepos);
        Vector3 relativePos = pos - tileworldpos;

        if (relativePos.y < 0.33f)
        {
            displacementPos = relativePos.y;
            return true;
        }
            

        return false;
    }

    void ChangeObjectZ(bool isHidden)
    {
        float remap = NumberFunctions.RemapNumber(displacementPos, 0.0f, 0.33f, -1f, -0.33f);
        Vector3 pos = new Vector3(0, 0, isHidden ? remap : 0);
        objectCorrectionZ.localPosition = pos;
        
        
    }

}
