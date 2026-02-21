
using UnityEngine;
using NoiseTest;

public class WindManager : MonoBehaviour
{
    public static WindManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    
    public float maxMagnitude;
    public float maxModifiable;
    public Texture2D windModifierTexture;
    [HideInInspector]
    public Color[,] windModifier;

    int currentSeed;
    float currentZ;

    OpenSimplexNoise openSimplexNoise;
    GridManager gridManager;
    private void Start()
    {
        gridManager = GridManager.instance;
        currentSeed = Random.Range(0, 10000);
        openSimplexNoise = new OpenSimplexNoise(currentSeed);
        windModifier = ConvertImage();

        GameEventManager.onTimeTickEvent.AddListener(SetWind);
        SetWind(0);
        
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(SetWind);
    }

    Color[,] ConvertImage()
    {

        Color[,] convertedImage = new Color[windModifierTexture.width, windModifierTexture.height];
        for (int x = 0; x < windModifierTexture.width; x++)
        {
            for (int y = 0; y < windModifierTexture.height; y++)
            {
                convertedImage[x, y] = windModifierTexture.GetPixel(x, y);
            }
        }
        return convertedImage;
    }



    void SetWind(int tick)
    {
        currentZ += 0.01f;
    }

    public float GetWindMagnitude(Vector3 position)
    {
        float a = GetWindMagnitudeRaw(position);
        a = Mathf.Abs(maxMagnitude * a) + GetWindMapModifier(gridManager.GetTilePosition(position));
        return a;
    }

    public float GetWindMagnitudeRaw(Vector3 position)
    {
        float offset = 2500;
        float scale = 10.0f;

        float a = (float)openSimplexNoise.Evaluate(position.x / scale + offset, position.y / scale + offset, currentZ + (position.z / scale + offset));
        a = NumberFunctions.RemapNumber(a, -1.0f, 1.0f, 0.0f, 1.0f);
        return a;
    }

    public Vector2 GetWindDirectionFromPosition(Vector3 position)
    {
        float offset = 5000;
        float scale = 10.0f;

        float a = (float)openSimplexNoise.Evaluate(position.x / scale + offset, position.y / scale + offset, (currentZ + position.z) / (scale + offset)) * (2 * Mathf.PI);

        var dir = new Vector2(Mathf.Sin(a), Mathf.Cos(a));
        return dir;
    }

    float GetWindMapModifier(Vector3Int position)
    {
        
        int mapPositionX = (int)NumberFunctions.RemapNumber(position.x, gridManager.groundMap.cellBounds.xMin, gridManager.groundMap.cellBounds.xMax, 0, 128);
        int mapPositionY = (int)NumberFunctions.RemapNumber(position.y, gridManager.groundMap.cellBounds.yMin, gridManager.groundMap.cellBounds.yMax, 0, 128);
        if(mapPositionX < 0 || mapPositionX>127||mapPositionY<0||mapPositionY>127)
            return 0;
        float w = windModifier[mapPositionX, mapPositionY].a;
        
        return w * maxModifiable;
    }

}
