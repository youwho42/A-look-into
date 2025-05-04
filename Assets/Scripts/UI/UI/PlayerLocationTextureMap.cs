using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLocationTextureMap : MonoBehaviour
{
    public RawImage playerLocationImageUI;
    Texture2D texture;
    public int maxPositionsViewed;
    Queue<int> positions = new Queue<int>();
    bool addedToManager;
    private void OnEnable()
    {
        //yield return new WaitForSeconds(2);
        if (!addedToManager)
        {
            GameEventManager.onPlayerPositionUpdateEvent.AddListener(DrawPlayerMap);
            addedToManager = true;
            return;
        }
            
        DrawPlayerMap();
        
    }
   
    private void OnDestroy()
    {
        GameEventManager.onPlayerPositionUpdateEvent.RemoveListener(DrawPlayerMap);
    }
    


    

    public void DrawPlayerMap()
    {
        
        int width = 128;
        int height = 128;

        if (texture == null)
        {
            texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            playerLocationImageUI.texture = texture;
        }

        Color[] colorMap = new Color[width * height];
        Color neutral = new Color(0, 0, 0, 0);
        Array.Fill(colorMap, neutral);

        
        var map = PlayerInformation.instance.playerController.currentTilePosition.groundMap;
        var playerPos = PlayerInformation.instance.playerController.currentTilePosition.position;
        playerPos.x = (int)NumberFunctions.RemapNumber(playerPos.x, map.cellBounds.xMin, map.cellBounds.xMax, 0, 128);
        playerPos.y = (int)NumberFunctions.RemapNumber(playerPos.y, map.cellBounds.yMin, map.cellBounds.yMax, 0, 128);

        int playerIndex = playerPos.y * width + playerPos.x;
        positions.Enqueue(playerIndex);
        if (positions.Count > maxPositionsViewed)
        {
            positions.Dequeue();
        }

        var pos = positions.ToArray();
        for (int i = 0; i < pos.Length; i++)
        {
            float alpha = NumberFunctions.RemapNumber(i, 0, pos.Length - 1, 0, 1);
            if(alpha >= 1)
                colorMap[pos[i]] = new Color(1f, 0, 0.1f, alpha);
            else
                colorMap[pos[i]] = new Color(1, 0.6f, 0, alpha);
        }
        texture.SetPixels(colorMap);
        texture.Apply();
        texture.filterMode = FilterMode.Point;
        
        playerLocationImageUI.texture = texture;
    }

}
