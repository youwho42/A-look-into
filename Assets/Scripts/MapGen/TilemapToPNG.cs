using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapToPNG : MonoBehaviour
{
    [Serializable]
    public struct MapLayer
    {
        public string mapName;
        public Tilemap tilemap;
        public Color color;
        public bool changeColorZ;
        [ConditionalHide("changeColorZ", true)]
        public Color ZColorDifference;
        public bool createMapLegend;
    }
    public Tilemap groundMap;
    public string textureName;

    public List<MapLayer> mapLayers = new List<MapLayer>();

    
    public void SaveFinalMap()
    {
        if(mapLayers.Count <= 0) 
            return;
        
        Texture2D layer = null;
        for (int i = 0; i < mapLayers.Count; i++)
        {
            float[,] tiles = new float[groundMap.cellBounds.size.x, groundMap.cellBounds.size.y];
            int indX = 0;
            int indY = 0;
            float indZ = 1;
            for (int x = groundMap.cellBounds.xMin; x < groundMap.cellBounds.xMax; x++)
            {
                indY = 0;
                for (int y = groundMap.cellBounds.yMin; y < groundMap.cellBounds.yMax; y++)
                {
                    for (int z = groundMap.cellBounds.zMax; z >= groundMap.cellBounds.zMin; z--)
                    {
                        tiles[indX, indY] = -1;
                        if (mapLayers[i].tilemap.GetTile(new Vector3Int(x, y, z)) != null)
                        {
                            if(mapLayers[i].changeColorZ)
                                indZ = NumberFunctions.RemapNumber(z, groundMap.cellBounds.zMin, groundMap.cellBounds.zMax, -0.23f, 1.08f);
                            tiles[indX, indY] = indZ;
                            break;
                        }
                       
                    }
                    indY++;
                }
                indX++;
            }
            layer = CreateFinalTexture(tiles, i);
            SaveTexture(layer, mapLayers[i].mapName);
            
        }
        

        
        
    }


    public Texture2D CreateFinalTexture(float[,] tileMapArray, int layerIndex)
    {
        int width = tileMapArray.GetLength(0);
        int height = tileMapArray.GetLength(1);
        
        Texture2D texture = new Texture2D(width, height);
        
        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color c = mapLayers[layerIndex].color;
                
                if (mapLayers[layerIndex].changeColorZ && tileMapArray[x, y] != -1)
                    c = Color.Lerp(mapLayers[layerIndex].color, mapLayers[layerIndex].ZColorDifference, tileMapArray[x, y]);
                
                    
                colorMap[y * width + x] = new Color(c.r, c.g, c.b, tileMapArray[x, y] == -1 ? 0 : 1);
            }
        }
        texture.SetPixels(colorMap);
        texture.Apply();

        
        return texture;
    }

    

    public void SaveTexture(Texture2D texture, string mapType)
    {
        byte[] bytes = texture.EncodeToPNG();
        
        var dirPath = Application.dataPath + "/MapImages";
        if (!System.IO.Directory.Exists(dirPath))
        {
            System.IO.Directory.CreateDirectory(dirPath);
        }
        string path = $"{dirPath}/Map_{mapType}_{textureName}.png";
        System.IO.File.WriteAllBytes(path, bytes);
        Debug.Log($"{bytes.Length}b was saved as: Map_{mapType}_{textureName}.png");

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        
#endif

    }

}
