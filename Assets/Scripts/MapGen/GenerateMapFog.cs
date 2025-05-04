using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateMapFog : MonoBehaviour
{
    public RawImage mapImage;
    public RawImage mapRevealImage;
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

    List<PlayerMapsManager.MapArea> playerMaps = new List<PlayerMapsManager.MapArea>();
    List<Color[]> playerMapsColors = new List<Color[]>();
    bool animatingReveal;
    
    Texture2D blankTexture;

    bool hasLoaded;
    private void Start()
    {
        SetBlankTexture();
    }

    private void OnEnable()
    {
        if (!hasLoaded)
        {
            hasLoaded = true;
            return;
        }
        mapRevealImage.texture = blankTexture;
        playerMapsColors = PlayerMapsManager.instance.colors;
        playerMaps = PlayerMapsManager.instance.mapAreas;
        GenerateMapFromNoise();
        InvokeRepeating("Boop", 0, 0.25f);
    }

    private void OnDisable()
    {
        
        animatingReveal = false;
        mapImage.texture = null;
        mapRevealImage.texture = null;
        CancelInvoke("Boop");
    }

    
    private void Boop()
    {
        offset.x -= 0.025f;
        //if (UIScreenManager.instance.CurrentUIScreen() == UIScreenType.MapScreen)
        GenerateMapFromNoise();

        CheckForNewMaps();

    }

    [ContextMenu("Generate Map Fog Image")]
    public void GenerateMapFromNoise()
    {
        
        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(true, mapSize, mapSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
        var t = CreateTexture(noiseMap);
        mapImage.texture = t;
        if(mapImage.texture != null)
            mapImage.texture.filterMode = FilterMode.Point;
    }

    Texture2D CreateTexture(float[,] tileMapArray)
    {
        if (PlayerMapsManager.instance == null)
            return null;
        playerMapsColors = PlayerMapsManager.instance.colors;
        playerMaps = PlayerMapsManager.instance.mapAreas;

        

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
                    
                    if (playerMaps[i].active && playerMaps[i].hasAnimated && playerMapsColors[i][y * width + x].a > 0)
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
    
    void CheckForNewMaps()
    {
        if (animatingReveal)
            return;
        PlayerMapsManager.MapArea map = null;
        Color[] mapColor = null;
        for (int i = 0; i < playerMaps.Count; i++)
        {
            if (playerMaps[i].active && !playerMaps[i].hasAnimated)
            {
                mapColor = playerMapsColors[i];
                map = playerMaps[i];
                break;
            }
        }
        if (mapColor != null)
            StartCoroutine(FadeActiveMap(map, mapColor));
    }

    IEnumerator FadeActiveMap(PlayerMapsManager.MapArea map, Color[] mapColor)
    {
        animatingReveal = true;
        Texture2D texture = new Texture2D(mapSize, mapSize);
        Color[] colorMap = new Color[mapSize * mapSize];
        // Loop through the maps and get all the pixels 
        // set each of those pixels to something that changes over a loop
        int animIndex = 0;
        int animMax = 12;
        float timeDelay = 0.2f;
        while (animIndex < animMax)
        {
            float t = (float)animIndex / (float)animMax;
            float outTimer = NumberFunctions.RemapNumber(t, timeDelay, 1.0f, 1.0f, 0.0f);
            float inTimer = NumberFunctions.RemapNumber(t, 0.0f, timeDelay, 0.0f, 1.0f);
            if(t >= timeDelay)
                map.hasAnimated = true;
            for (int y = 0; y < mapSize; y++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    Color pColor = new Color(0, 0, 0, 0);
                    
                    if (mapColor[y * mapSize + x].a > 0)
                        pColor = new Color(1, 1, 1, t <= timeDelay ? mapColor[y * mapSize + x].a * inTimer : mapColor[y * mapSize + x].a * outTimer);
                    colorMap[y * mapSize + x] = pColor;
                }
            }
            texture.SetPixels(colorMap);
            texture.Apply();
            mapRevealImage.texture = texture;
            mapRevealImage.texture.filterMode = FilterMode.Point;
            animIndex++;
            yield return new WaitForSeconds(0.25f);
        }

        // set the image texture so it can be seen

        yield return null;
        // reset the texture at the end so that it doesnt interfere later on.
        mapRevealImage.texture = blankTexture;
        animatingReveal = false;
    }

    float CellShade(float c)
    {
        return Mathf.RoundToInt(c * 7) / 7.0f;
    }


    void SetBlankTexture()
    {
        Texture2D texture = new Texture2D(mapSize, mapSize);
        Color[] colorMap = new Color[mapSize * mapSize];

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                colorMap[y * mapSize + x] = new Color(0, 0, 0, 0);
            }
        }
        texture.SetPixels(colorMap);
        texture.Apply();
        blankTexture = texture;
        mapRevealImage.texture = blankTexture;
    }
}
