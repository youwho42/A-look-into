using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapToPNG : MonoBehaviour
{
    public Tilemap groundMap;
    public string textureName;

    
    public void SaveBaseMap()
    {
        float[,] tiles = new float[groundMap.cellBounds.size.x, groundMap.cellBounds.size.y];
        int indX = 0;
        int indY = 0;
        float indZ = 0;
        
        for (int x = groundMap.cellBounds.xMin; x < groundMap.cellBounds.xMax; x++)
        {
            indY = 0;
            for (int y = groundMap.cellBounds.yMin; y < groundMap.cellBounds.yMax; y++)
            {
                for (int z = groundMap.cellBounds.zMax; z > groundMap.cellBounds.zMin; z--)
                {
                    if(groundMap.GetTile(new Vector3Int(x,y,z)) != null)
                    {
                        indZ = MapNumber.Remap(z, groundMap.cellBounds.zMin, groundMap.cellBounds.zMax, 0, 1);
                        tiles[indX, indY] = indZ;
                        break;
                    }
                }
                indY++;
            }
            indX++;
        }
        
        var tex = CreateTexture(tiles);
        SaveTexture(tex, "Base");
    }
    

    public Texture2D CreateTexture(float[,] tileMapArray)
    {
        int width = tileMapArray.GetLength(0);
        int height = tileMapArray.GetLength(1);

        Texture2D texture = new Texture2D(width, height);
        
        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = new Color(0.21f, tileMapArray[x, y], 0.14f, 1);
            }
        }
        texture.SetPixels(colorMap);
        texture.Apply();

        
        return texture;
    }

    private void SaveTexture(Texture2D texture, string mapType)
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


}
