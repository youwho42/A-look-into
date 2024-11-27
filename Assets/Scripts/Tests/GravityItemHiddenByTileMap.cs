using Klaxon.GravitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GravityItemHiddenByTileMap : MonoBehaviour
{

    public Transform objectCorrectionZ;
    float displacementPos;
    GridManager gridManager;
    Transform _transform;
    GravityItemNew gravityItem;
    //HashSet<Vector3Int> fadedTilePositions = new HashSet<Vector3Int>();
    bool canHide;
    private void Start()
    {
        gravityItem = GetComponent<GravityItemNew>();
        gridManager = GridManager.instance;
        _transform = transform;
    }

    

    void Update()
    {
        if(GetTileInfo())
            CheckHidden();
    }
    

    private void CheckHidden()
    {
        displacementPos = 0;
        Vector3 pos = _transform.position;
        
        pos.z += 1;
        if (gridManager.GetTileExisting(pos))
            displacementPos = CheckObjectPosition(Vector3.zero);

        if (displacementPos <= 0)
            DoubleCheck();

        ChangeObjectZ(displacementPos);
    }

    bool GetTileInfo()
    {
        foreach (var item in gravityItem.tileBlockInfo)
        {
            if (item.levelZ > 0)
                return true;
        }
        ChangeObjectZ(0);
        return false;
    }
    //private void CheckHidden()
    //{
    //    displacementPos = 0;
    //    Vector3 pos = _transform.position;
    //    //ClearFadedTiles();
    //    for (int i = 20; i > _transform.position.z; i--)
    //    {
    //        pos.z = i;
    //        if (gridManager.GetTileExisting(pos))
    //        {
    //            if (i == _transform.position.z || i == _transform.position.z + 1)
    //                displacementPos = CheckObjectPosition(Vector3.zero);
                
    //            //Debug.Log("Need to show the player behind a tile somehow");
    //            //HideTiles();
    //            break;
    //        }
    //    }
    //    if(displacementPos <= 0)
    //        DoubleCheck();
    //    ChangeObjectZ(displacementPos);

    //}

    void DoubleCheck()
    {
        var yOff = NumberFunctions.RemapNumber(displacementPos, 0.0f, 0.33f, 0.0f, 1f);
        var offset = new Vector3(0, GlobalSettings.SpriteDisplacementY * yOff, 0);
        displacementPos = CheckObjectPosition(offset);
        
    }

    


    float CheckObjectPosition(Vector3 offset)
    {
        
        foreach (var tile in gravityItem.tileBlockInfo)
        {
            if (tile.direction == Vector3Int.left || tile.direction == -(Vector3Int)Vector2Int.one || tile.direction == Vector3Int.down)
            {
                if (tile.levelZ > 0 || tile.levelZ == 0 && tile.tileName.Contains("Slope"))
                {
                    Vector3 pos = _transform.position - Vector3.forward;
                    pos += offset;
                    Vector3Int tilepos = gridManager.Grid.WorldToCell(pos);
                    Vector3 tileworldpos = gridManager.Grid.CellToWorld(tilepos);
                    Vector3 relativePos = pos - tileworldpos;

                    if(relativePos.x < 0 && tile.direction == Vector3Int.left ||
                        relativePos.x > 0 && tile.direction == Vector3Int.down ||
                        tile.direction == -(Vector3Int)Vector2Int.one)
                    {
                        if (relativePos.y < 0.33f)
                            return relativePos.y;
                    }
                    
                }

            }
        }

        return 0;

        
    }

    void ChangeObjectZ(float disp)
    {
        float remap = 0;
        if (disp > 0)
            remap = NumberFunctions.RemapNumber(displacementPos, 0.0f, 0.33f, -1f, -0.33f);
        Vector3 pos = new Vector3(0, 0, remap);
        objectCorrectionZ.localPosition = pos;

    }

    //void HideTiles()
    //{
    //    Vector3 pos = _transform.position;
    //    for (int i = 20; i > _transform.position.z; i--)
    //    {
    //        pos.z = i;
    //        Vector3Int tilepos = gridManager.Grid.WorldToCell(pos);
    //        FadeTile(tilepos, 0);
    //        FadeTile(tilepos + (Vector3Int)Vector2Int.one, 0.1f);
    //        FadeTile(tilepos + Vector3Int.right, 0.1f);
    //        FadeTile(tilepos + Vector3Int.up, 0.1f);


    //    }
    //}
    //void ClearFadedTiles()
    //{

    //    foreach (Vector3Int pos in fadedTilePositions)
    //    {
    //        FadeTile(pos, 1f);
    //    }
    //    fadedTilePositions.Clear();
    //}

    //void FadeTile(Vector3Int tilePosition, float targetAlpha)
    //{

    //    gridManager.groundMap.SetTileFlags(tilePosition, TileFlags.None);

    //    Color tempColor = gridManager.groundMap.GetColor(tilePosition);

    //    gridManager.groundMap.SetColor(tilePosition, new Color(tempColor.r, tempColor.g, tempColor.b, targetAlpha));

    //    fadedTilePositions.Add(tilePosition);

    //}
}
