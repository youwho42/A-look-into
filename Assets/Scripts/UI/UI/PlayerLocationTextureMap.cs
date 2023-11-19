using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLocationTextureMap : MonoBehaviour
{
    public RawImage playerLocationImageUI;

    public int maxPositionsViewed;
    Queue<int> positions = new Queue<int>();

    private IEnumerator Start()
    {
        GameEventManager.onPlayerPositionUpdateEvent.AddListener(DrawPlayerMap);
        yield return new WaitForSeconds(2);
        DrawPlayerMap();
    }
    private void OnDestroy()
    {
        GameEventManager.onPlayerPositionUpdateEvent.RemoveListener(DrawPlayerMap);

    }
    


    

    public void DrawPlayerMap()
    {
        //if (LevelManager.instance.isInCutscene)
        //    return;
        int width = 128;
        int height = 128;

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        Color[] colorMap = new Color[width * height];
        Color neutral = new Color(0, 0, 0, 0);
        var map = PlayerInformation.instance.playerController.currentTilePosition.groundMap;
        var playerPos = PlayerInformation.instance.playerController.currentTilePosition.position;
        playerPos.x = (int)MapNumber.Remap(playerPos.x, map.cellBounds.xMin, map.cellBounds.xMax, 0, 128);
        playerPos.y = (int)MapNumber.Remap(playerPos.y, map.cellBounds.yMin, map.cellBounds.yMax, 0, 128);
        for (int y = 0; y < height-1; y++)
        {
            for (int x = 0; x < width-1; x++)
            {
                if(x==playerPos.x && y==playerPos.y)
                {
                    positions.Enqueue(y * width + x);
                    if (positions.Count >= maxPositionsViewed)
                    {
                        positions.Dequeue();
                    }
                    
                }
                else
                {
                    colorMap[y * width + x] = neutral;
                }
                
            }
        }
        var pos = positions.ToArray();
        for (int i = 0; i < pos.Length; i++)
        {
            float alpha = MapNumber.Remap(i, 0, pos.Length - 1, 0, 1);
            colorMap[pos[i]] = new Color(1, 1, 0, alpha);
        }
        texture.SetPixels(colorMap);
        texture.Apply();
        texture.filterMode = FilterMode.Point;
        
        playerLocationImageUI.texture = texture;
    }
}
