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
    PlayerSilhouetteManager silhouetteManager;
    bool canHide;
    private void Start()
    {
        silhouetteManager = PlayerSilhouetteManager.instance;
        gravityItem = GetComponent<GravityItemNew>();
        gridManager = GridManager.instance;
        _transform = transform;
    }

    

    void Update()
    {
        if (GetTileInfo())
        {
            CheckHidden();
        }
            
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

    
}
