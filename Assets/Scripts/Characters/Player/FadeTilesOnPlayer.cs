using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FadeTilesOnPlayer : MonoBehaviour
{
    
    public Tilemap HousesFront;
    public Tilemap HousesInterior;

    HashSet<Vector3Int> allHouseTiles = new HashSet<Vector3Int>();

    bool inHouse;
    

    void FadeTiles()
    {

        if (!inHouse)
            return;
        
        Vector3Int playerTilePosition = PlayerInformation.instance.currentTilePosition.position;
        playerTilePosition.x -= 1;
        playerTilePosition.y -= 1;
        StopAllCoroutines();
        foreach (var tile in allHouseTiles)
        {
            var d = ((Vector2Int)playerTilePosition - (Vector2Int)tile).magnitude;
                
            d = NumberFunctions.RemapNumber(d, 0.0f, 8.0f, 0.0f, 1.0f);
                
            var fd = d > 0.85f ? 1 : d < 0.2f ? 0.05f : d + 0.05f;
            StartCoroutine(FadeTile(tile, fd));
        }
        
    }

    void ClearFadedTiles()
    {
        StopAllCoroutines();

        foreach (Vector3Int pos in allHouseTiles)
        {
            StartCoroutine(FadeTile(pos, 1f)); // Fade back to full visibility
        }
        allHouseTiles.Clear();
    }

    

   
    IEnumerator FadeTile(Vector3Int tilePosition, float targetAlpha)
    {
        float elapsedTime = 0;
        float waitTime = 0.5f;
        HousesFront.SetTileFlags(tilePosition, TileFlags.None);

        Color tempColor = HousesFront.GetColor(tilePosition);
        float startAlpha = tempColor.a;

        while (elapsedTime < waitTime)
        {
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / waitTime);

            HousesFront.SetColor(tilePosition, new Color(tempColor.r, tempColor.g, tempColor.b, newAlpha));

            elapsedTime += Time.deltaTime;
            yield return null;
        }


        // Ensure that the tile is set to the exact target alpha when the coroutine finishes
        HousesFront.SetColor(tilePosition, new Color(tempColor.r, tempColor.g, tempColor.b, targetAlpha));
        yield return null;
    }


    void GetAllTiles(Vector3Int initialPosition)
    {
        bool canCheckAround = false;
        if (HousesFront.HasTile(initialPosition))
        {
            if (!allHouseTiles.Contains(initialPosition))
            {
                allHouseTiles.Add(initialPosition);
                canCheckAround = true;
            }
                
        }
        if (canCheckAround)
        {
            
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (x == 0 && y == 0)
                        continue;
                    var t = new Vector3Int(initialPosition.x + x, initialPosition.y + y, initialPosition.z);
                    GetAllTiles(t);
                    t = new Vector3Int(t.x, t.y, t.z-2);
                    GetAllTiles(t);
                }
            }
            
        }
        

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("House"))
        {
            var t = PlayerInformation.instance.currentTilePosition.position;
            if (!HousesInterior.HasTile(t))
                return;
                

            var pt = new Vector3Int(t.x, t.y, t.z + 4);
                
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    var zt = new Vector3Int(pt.x+x, pt.y+y, pt.z);
                            
                    if (HousesFront.HasTile(zt))
                        GetAllTiles(zt);

                }
            }
                
            inHouse = true;
            GameEventManager.onPlayerPositionUpdateEvent.AddListener(FadeTiles);
            FadeTiles(); // Call FadeTiles immediately when entering the house to avoid initial jittering
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("House"))
        {
            inHouse = false;
            GameEventManager.onPlayerPositionUpdateEvent.RemoveListener(FadeTiles);
            ClearFadedTiles();
        }
    }
}
