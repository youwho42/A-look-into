using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FadeTilesOnPlayer : MonoBehaviour
{
    public Grid groundGrid;
    public Tilemap tilemap;

    HashSet<Vector3Int> fadedTilePositions = new HashSet<Vector3Int>();
    Vector3Int lastPlayerTilePosition;
    bool inHouse;

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
        foreach (Vector3Int pos in fadedTilePositions)
        {
            StopCoroutine("FadeTile");
            StartCoroutine(FadeTile(pos, 1f)); // Fade back to full visibility
        }
        fadedTilePositions.Clear();
    }

    void GetTileLocation()
    {
        for (int x = -2; x < 3; x++)
        {
            for (int y = -2; y < 3; y++)
            {
                for (int z = 0; z < tilemap.size.z + 1; z++)
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
                        d = Mathf.Clamp(d, 0, 3);
                        StartCoroutine(FadeTile(currentPosition, (d * 0.2f) + 0.05f));
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


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Tilemaps;

//public class FadeTilesOnPlayer : MonoBehaviour
//{
//    public Grid groundGrid;
//    public Tilemap tilemap;


//    List<Vector3Int> lastTilePositions = new List<Vector3Int>();

//    bool inHouse;

//    private void Start()
//    {
//        GameEventManager.onPlayerPositionUpdateEvent.AddListener(FadeTiles);
//    }

//    private void OnDisable()
//    {
//        GameEventManager.onPlayerPositionUpdateEvent.RemoveListener(FadeTiles);
//    }

//    void FadeTiles()
//    {
//        if (inHouse)
//        {
//            ClearFadedTiles();
//            GetTileLocation();
//        }

//    }



//    void ClearFadedTiles()
//    {
//        foreach (Vector3Int pos in lastTilePositions)
//        {
//            StopCoroutine("FadeTile");
//            StartCoroutine(FadeTile(pos));
//        }
//        lastTilePositions.Clear();
//    }



//    void GetTileLocation()
//    {
//        Vector3Int mainPosition = PlayerInformation.instance.currentTilePosition.position;
//        for (int x = -2; x < 3; x++)
//        {
//            for (int y = -2; y < 3; y++)
//            {
//                for (int z = 0; z < tilemap.size.z + 1; z++)
//                {
//                    Vector3Int currentPosition = mainPosition;
//                    currentPosition.x += x;
//                    currentPosition.y += y;
//                    currentPosition.z = z;
//                    TileBase tile = tilemap.GetTile(currentPosition);
//                    if (tile != null)
//                    {
//                        StopCoroutine("FadeTile");
//                        var d = Vector2.Distance((Vector2Int)mainPosition, (Vector2Int)currentPosition);
//                        var sx = Mathf.Sign(x);
//                        var sy = Mathf.Sign(y);
//                        if (sx <= 0 && sy <= 0)
//                            d += (sx + sy);
//                        d = Mathf.Clamp(d, 0, 3);
//                        StartCoroutine(FadeTile(currentPosition, (d * 0.2f) + 0.05f));
//                        lastTilePositions.Add(currentPosition);
//                    }
//                }
//            }
//        }
//    }

//    IEnumerator FadeTile(Vector3Int tilePosition, float amountToFade = 1)
//    {
//        float elapsedTime = 0;
//        float waitTime = .5f;
//        tilemap.SetTileFlags(tilePosition, TileFlags.None);
//        Color tempColor = tilemap.GetColor(tilePosition);
//        while (elapsedTime < waitTime)
//        {
//            tilemap.SetColor(tilePosition, Color.Lerp(tempColor, new Color(tempColor.r, tempColor.g, tempColor.b, amountToFade), elapsedTime / waitTime));
//            elapsedTime += Time.deltaTime;
//            yield return null;
//        }

//    }


//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.CompareTag("House"))
//        {
//            inHouse = true;
//        }
//    }

//    private void OnTriggerExit2D(Collider2D collision)
//    {
//        if (collision.CompareTag("House"))
//        {
//            inHouse = false;
//            ClearFadedTiles();
//        }
//    }

//}
