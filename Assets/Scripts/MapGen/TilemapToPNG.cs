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
    
    //public void SaveBaseMap()
    //{
    //    float[,] tiles = new float[groundMap.cellBounds.size.x, groundMap.cellBounds.size.y];
    //    int indX = 0;
    //    int indY = 0;
    //    float indZ = 0;
        
    //    for (int x = groundMap.cellBounds.xMin; x < groundMap.cellBounds.xMax; x++)
    //    {
    //        indY = 0;
    //        for (int y = groundMap.cellBounds.yMin; y < groundMap.cellBounds.yMax; y++)
    //        {
    //            for (int z = groundMap.cellBounds.zMax; z > groundMap.cellBounds.zMin; z--)
    //            {
    //                if(groundMap.GetTile(new Vector3Int(x,y,z)) != null)
    //                {
    //                    indZ = NumberFunctions.RemapNumber(z, groundMap.cellBounds.zMin, groundMap.cellBounds.zMax, 0, 1);
    //                    tiles[indX, indY] = indZ;
    //                    break;
    //                }
    //            }
    //            indY++;
    //        }
    //        indX++;
    //    }
        
    //    var tex = CreateBaseTexture(tiles);
    //    SaveTexture(tex, "Base");
    //}

    public void SaveFinalMap()
    {
        if(mapLayers.Count <= 0) 
            return;
        List<Texture2D> allLayers = new List<Texture2D>();
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
                            //tiles[indX, indY] = 1;
                            break;
                        }
                       
                    }
                    indY++;
                }
                indX++;
            }
            layer = CreateFinalTexture(tiles, i);
            SaveTexture(layer, mapLayers[i].mapName);
            
            //SaveTexture(Resize(layer, 1280, 1280), $"{mapLayers[i].mapName}_384");
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

    //public Texture2D CreateBaseTexture(float[,] tileMapArray)
    //{
    //    int width = tileMapArray.GetLength(0);
    //    int height = tileMapArray.GetLength(1);
        
    //    Texture2D texture = new Texture2D(width, height);

    //    Color[] colorMap = new Color[width * height];
    //    for (int y = 0; y < height; y++)
    //    {
    //        for (int x = 0; x < width; x++)
    //            colorMap[y * width + x] = new Color(0.21f, tileMapArray[x, y], 0.14f, 1);
            
    //    }
    //    texture.SetPixels(colorMap);
    //    texture.Apply();


    //    return texture;
    //}

    public void SaveTexture(Texture2D texture, string mapType)
    {
        byte[] bytes = texture.EncodeToPNG();
        
        var dirPath = Application.dataPath + "/MapImages";
        if (!System.IO.Directory.Exists(dirPath))
        {
            System.IO.Directory.CreateDirectory(dirPath);
        }
        System.IO.File.WriteAllBytes(dirPath + "/Map_" + mapType + "_" + textureName + ".png", bytes);
        Debug.Log(bytes.Length / 1024 + "Kb was saved as: " + dirPath);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        
#endif

    }

    //Texture2D Resize(Texture2D texture2D, int targetX, int targetY)
    //{
    //    RenderTexture rt = new RenderTexture(targetX, targetY, 24);
    //    RenderTexture.active = rt;
    //    Graphics.Blit(texture2D, rt);
    //    Texture2D result = new Texture2D(targetX, targetY);
    //    result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
    //    result.Apply();
    //    return result;
    //}

}
