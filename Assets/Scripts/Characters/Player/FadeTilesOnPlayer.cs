using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FadeTilesOnPlayer : MonoBehaviour
{
    
    public Tilemap tilemap;

    HashSet<Vector3Int> fadedTilePositions = new HashSet<Vector3Int>();
    Vector3Int lastPlayerTilePosition;
    bool inHouse;
    bool debugging;
    private void Start()
    {
        GameEventManager.onPlayerPositionUpdateEvent.AddListener(FadeTiles);
    }

    private void OnDisable()
    {
        GameEventManager.onPlayerPositionUpdateEvent.RemoveListener(FadeTiles);
    }

    

    void FadeTiles()
    {
        Vector3Int playerTilePosition = PlayerInformation.instance.currentTilePosition.position;
        if (inHouse && playerTilePosition != lastPlayerTilePosition)
        {
            ClearFadedTiles();
            lastPlayerTilePosition = playerTilePosition;
            GetTileLocation();
        }
    }

    void ClearFadedTiles()
    {
        StopAllCoroutines();
        foreach (Vector3Int pos in fadedTilePositions)
        {
            StartCoroutine(FadeTile(pos, 1f)); // Fade back to full visibility
        }
        fadedTilePositions.Clear();
    }

    void GetTileLocation()
    {
        for (int x = -3; x < 4; x++)
        {
            for (int y = -3; y < 4; y++)
            {
                for (int z = lastPlayerTilePosition.z; z < lastPlayerTilePosition.z + 6; z++)
                {
                    Vector3Int currentPosition = lastPlayerTilePosition;
                    currentPosition.x += x;
                    currentPosition.y += y;
                    currentPosition.z = z;
                    TileBase tile = tilemap.GetTile(currentPosition);
                    if (tile != null && !fadedTilePositions.Contains(currentPosition))
                    {
                        var d = Vector2.Distance((Vector2Int)lastPlayerTilePosition, (Vector2Int)currentPosition);
                        var sx = Mathf.Sign(x);
                        var sy = Mathf.Sign(y);
                        if (sx <= 0 && sy <= 0)
                            d += (sx + sy);
                        d = Mathf.Clamp(d, 0, 4);
                        StartCoroutine(FadeTile(currentPosition, (d * 0.1f) + 0.05f));
                        fadedTilePositions.Add(currentPosition);
                    }
                }
            }
        }
    }

   
    IEnumerator FadeTile(Vector3Int tilePosition, float targetAlpha)
    {
        float elapsedTime = 0;
        float waitTime = 0.5f;
        tilemap.SetTileFlags(tilePosition, TileFlags.None);

        Color tempColor = tilemap.GetColor(tilePosition);
        float startAlpha = tempColor.a;

        while (elapsedTime < waitTime)
        {
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / waitTime);

            tilemap.SetColor(tilePosition, new Color(tempColor.r, tempColor.g, tempColor.b, newAlpha));

            elapsedTime += Time.deltaTime;
            yield return null;
        }


        // Ensure that the tile is set to the exact target alpha when the coroutine finishes
        tilemap.SetColor(tilePosition, new Color(tempColor.r, tempColor.g, tempColor.b, targetAlpha));
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("House"))
        {
            inHouse = true;
            FadeTiles(); // Call FadeTiles immediately when entering the house to avoid initial jittering
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("House"))
        {
            inHouse = false;
            ClearFadedTiles();
        }
    }
}
