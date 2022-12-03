using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FadeTilesOnPlayer : MonoBehaviour
{
    public Grid groundGrid;
    public Tilemap tilemap;

    public Transform playerPosition;
    Vector3Int lastPlayerPos;
    List<Vector3Int> lastTilePositions = new List<Vector3Int>();

    public LayerMask fadableAreaMask;
    Collider2D lastHit;
    private void Update()
    {
        
        Collider2D hit = Physics2D.OverlapPoint(playerPosition.position, fadableAreaMask);
        if(lastHit != hit)
        {
            lastHit = hit;
            ClearFadedTiles();
        }
        if (lastPlayerPos != CurrentGridLocation() && hit != null)
        {
            if (hit.CompareTag("House"))
            {
                ClearFadedTiles();
                GetTileLocation();
                lastPlayerPos = CurrentGridLocation();
            }
            
        }
    }

    

    void ClearFadedTiles()
    {
        foreach (Vector3Int pos in lastTilePositions)
        {
            StopCoroutine("FadeTile");
            StartCoroutine(FadeTile(pos));
        }
    }

    Vector3Int CurrentGridLocation()
    {
        Vector3Int checkPosition = groundGrid.WorldToCell(playerPosition.position);
        return checkPosition;
    }

    void GetTileLocation()
    {
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                for (int z = 0; z < tilemap.size.z + 1; z++)
                {
                    Vector3Int currentPosition = CurrentGridLocation();
                    currentPosition.x += x;
                    currentPosition.y += y;
                    currentPosition.z = z;
                    TileBase tile = tilemap.GetTile(currentPosition);
                    if (tile != null)
                    {
                        StopCoroutine("FadeTile");
                        StartCoroutine(FadeTile(currentPosition, (Mathf.Abs(x) * 0.3f) + 0.15f));
                        lastTilePositions.Add(currentPosition);
                    }
                }
            }
        }
    }

    IEnumerator FadeTile(Vector3Int tilePosition, float amountToFade = 1)
    {
        float elapsedTime = 0;
        float waitTime = .5f;
        tilemap.SetTileFlags(tilePosition, TileFlags.None);
        Color tempColor = tilemap.GetColor(tilePosition);
        while (elapsedTime < waitTime)
        {
            tilemap.SetColor(tilePosition, Color.Lerp(tempColor, new Color(tempColor.r, tempColor.g, tempColor.b, amountToFade), elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
    }

    
}
