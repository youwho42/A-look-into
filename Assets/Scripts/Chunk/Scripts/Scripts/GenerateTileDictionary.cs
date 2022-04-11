using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTileDictionary : MonoBehaviour
{
    public int size = 1000;
    public float scale = 58.675f;
    public Vector2 offset = new Vector2(10000, 10000);
    public int octaves = 4;
    public float persistance = 0.5f;
    public float lacunarity = 1.87f;

    public static Dictionary<Vector3Int, float> tilePlacements = new Dictionary<Vector3Int, float>();

    private void Awake()
    {
        for (int x = -size; x < size; x++)
        {
            for (int y = -size; y < size; y++)
            {
                Noise(x, y);
            }
        }
    }


    void Noise(int x, int y)
    {
        Vector3Int placement = new Vector3Int(x, y, 0);
        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        for (int i = 0; i < octaves; i++)
        {
            float sampleX = (x - size/2) / scale * frequency + (int)offset.x;
            float sampleY = (y - size / 2) / scale * frequency + (int)offset.y;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
            noiseHeight += perlinValue * amplitude;

            amplitude *= persistance;
            frequency *= lacunarity;
        }

        tilePlacements.Add(placement, noiseHeight);

    }

}
