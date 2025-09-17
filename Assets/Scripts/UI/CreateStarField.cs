using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class CreateStarField : MonoBehaviour
{

    public static CreateStarField instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public RectTransform starHolder;
    public RectTransform starObject;

    [Header("Noise")]
    public bool useOpenSimplex;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    float[,] starMap;
    float[,] starIntensities;

    Vector2 lastStarHolderSize;

    public AnimationCurve starAlphaCurve;
    public AnimationCurve starSizeCurve;
    public AnimationCurve starColorCurve;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        CreateStarMap();
    }

    public void CreateStarMap()
    {
        if (lastStarHolderSize.x == starHolder.rect.width && lastStarHolderSize.y == starHolder.rect.height)
            return;
        starMap = NoiseGenerator.GenerateNoiseMap(useOpenSimplex, (int)starHolder.rect.width, (int)starHolder.rect.height, seed, noiseScale, octaves, persistance, lacunarity, offset);
        starIntensities = NoiseGenerator.GenerateNoiseMap(useOpenSimplex, (int)starHolder.rect.width, (int)starHolder.rect.height, 2342, noiseScale, octaves, persistance, lacunarity, offset);
        CreateStars();
        lastStarHolderSize = new Vector2(starHolder.rect.width, starHolder.rect.height);
    }

    void CreateStars()
    {
        ClearStars();

        for (int x = 0; x < (int)starHolder.rect.width; x++)
        {
            for (int y = 0; y < (int)starHolder.rect.height; y++)
            {
                if (starMap[x, y] > .9f)
                {
                    if (!CanSpawn(starMap, x, y))
                        continue;
                    RectTransform star = Instantiate(starObject, starHolder);
                    var starImage = star.GetComponent<Image>();
                    float alphaEval = starAlphaCurve.Evaluate(starIntensities[x, y]);
                    float sizeEval = starSizeCurve.Evaluate(starIntensities[x, y]);
                    float wh = NumberFunctions.RemapNumber(sizeEval, 0.0f, 1.0f, 3.0f, 10.0f);
                    float alpha = alphaEval + 0.002f;
                    Random.InitState(x + y);
                    float h = Random.value;
                    float s = starColorCurve.Evaluate(Random.value);
                    Color bob = Color.HSVToRGB(h, s, 1);


                    Color c = new Color(bob.r, bob.g, bob.b, alpha);
                    starImage.color = c;
                    var size = new Vector2(wh, wh);
                    star.sizeDelta = size;
                    star.anchoredPosition = new Vector2(x - starHolder.rect.width * 0.5f, y - starHolder.rect.height * 0.5f);

                }
            }
        }
    }

    bool CanSpawn(float[,] noiseMap, int x, int y)
    {
        for (int i = -3; i < 3; i++)
        {
            for (int j = -3; j < 3; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                if (x + i < 0 || x + i > starHolder.rect.width - 1 || y + j < 0 || y + j > starHolder.rect.height - 1)
                    continue;

                if (noiseMap[x + i, y + j] > .9f)
                    return false;
            }
        }
        return true;
    }

    void ClearStars()
    {
        
        GameObject[] allChildren = GetAllChildren();

        
        foreach (GameObject child in allChildren)
        {
            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }
    }

    private GameObject[] GetAllChildren()
    {
        
        GameObject[] allChildren = new GameObject[starHolder.childCount];

        
        int childCount = 0;
        foreach (Transform child in starHolder)
        {
            allChildren[childCount] = child.gameObject;
            childCount += 1;
        }

        return allChildren;
    }
    
}
