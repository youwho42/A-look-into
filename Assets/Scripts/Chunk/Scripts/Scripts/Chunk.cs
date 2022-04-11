using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Chunk : MonoBehaviour
{
    public static int size = 10;


    public Tilemap[] tilemaps;

    public TerrainType[] terrainType;



    float sample;


    private void Awake()
    {

        tilemaps = FindObjectsOfType<Tilemap>();


    }

    public void SetTiles()
    {
        StartCoroutine(SetTilesCo());

    }


    IEnumerator SetTilesCo()
    {

        Vector3Int chunkPosition = new Vector3Int((int)transform.position.x, (int)transform.position.y, 0);
        Vector3Int tilePos = Vector3Int.zero;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                foreach (var terrain in terrainType)
                {
                    for (int i = 0; i < tilemaps.Length; i++)
                    {

                        tilePos = new Vector3Int(x + chunkPosition.x, y + chunkPosition.y, 0);
                        if (GenerateTileDictionary.tilePlacements.ContainsKey(tilePos))
                        {
                            GenerateTileDictionary.tilePlacements.TryGetValue(tilePos, out sample);
                            if (tilemaps[i].name == terrain.tilemapName)
                            {
                                if (sample <= terrain.perlinAllowanceMax && sample >= terrain.perlinAllowanceMin && Random.Range(0.0f, 1.0f) <= terrain.chanceToSpawn)
                                {
                                    tilemaps[i].SetTile(tilePos, terrain.tileBase);
                                }
                            }
                        }
                    }
                }

                yield return null;
            }
        }

                
    }

    public void ClearTiles()
    {
        StartCoroutine(ClearTilesCo());

    }

    IEnumerator ClearTilesCo()
    {
        Vector3Int chunkPosition = new Vector3Int((int)transform.position.x, (int)transform.position.y, 0);
        Vector3Int tilePos = Vector3Int.zero;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                tilePos = new Vector3Int(x + chunkPosition.x, y + chunkPosition.y, 0);
                foreach (var terrain in terrainType)
                {
                    for (int i = 0; i < tilemaps.Length; i++)
                    {
                        if (tilemaps[i] != null)
                        {
                            if (tilemaps[i].name == terrain.tilemapName)
                            {
                                tilemaps[i].SetTile(tilePos, null);
                            }
                        }

                    }
                }
            }
        }
        yield return null;
    }

}

[System.Serializable]
public class TerrainType
{
    public string tilemapName;
    public float perlinAllowanceMin;
    public float perlinAllowanceMax;
    public TileBase tileBase;
    [Range(0.0f, 1.0f)]
    public float chanceToSpawn = 1;

}
