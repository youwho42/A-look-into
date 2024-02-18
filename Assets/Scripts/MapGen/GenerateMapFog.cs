using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateMapFog : MonoBehaviour
{
    public RawImage mapImage;
    public int mapSize;
    [Range(0, 1)]
    public float minAlpha;


    [Header("Noise")]
    

    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;


    public int seed;
    public Vector2 offset;



    private void OnEnable()
    {
        
        GenerateMapFromNoise();
        InvokeRepeating("Boop", 0, 0.25f);
    }
    private void OnDisable()
    {
        mapImage.texture = null;
        CancelInvoke("Boop");
    }

    
    private void Boop()
    {
        offset.x -= 0.025f;
        //if (UIScreenManager.instance.CurrentUIScreen() == UIScreenType.MapScreen)
        GenerateMapFromNoise();
    }

    [ContextMenu("Generate Map Fog Image")]
    public void GenerateMapFromNoise()
    {
        
        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(true, mapSize, mapSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
        var t = CreateTexture(noiseMap);
        mapImage.texture = t;
        mapImage.texture.filterMode = FilterMode.Point;
    }

    Texture2D CreateTexture(float[,] tileMapArray)
    {
        var playerMapsColors = PlayerMapsManager.instance.colors;
        var playerMaps = PlayerMapsManager.instance.mapAreas;


        int width = tileMapArray.GetLength(0);
        int height = tileMapArray.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float alpha = 0;
                for (int i = 0; i < playerMaps.Count; i++)
                {
                    if (playerMaps[i].active)
                        alpha += playerMapsColors[i][y * width + x].a;
                }
                float c = CellShade(tileMapArray[x, y]);
                
                colorMap[y * width + x] = new Color(c - alpha, c - alpha, c - alpha, Mathf.Clamp(c + minAlpha, 0, 1.17f) - alpha);
            }
        }

        

        texture.SetPixels(colorMap);
        texture.Apply();


        return texture;
    }
    

    float CellShade(float c)
    {
        return Mathf.RoundToInt(c * 7) / 7.0f;
    }
    
}
