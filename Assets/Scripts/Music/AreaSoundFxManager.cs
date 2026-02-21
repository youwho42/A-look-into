using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Tilemaps;
using UnityEngine.Audio;


[Serializable]
public class SFXArea
{
    public string mapAreaName;
    public string sfxAreaName;
    public Texture2D sfxAreaTexture;
    [HideInInspector]
    public Color[,] sfxArea;
    

    
    public void SetColorArray()
    {
        sfxArea = ConvertImage();
    }

    Color[,] ConvertImage()
    {

        Color[,] convertedImage = new Color[sfxAreaTexture.width, sfxAreaTexture.height];
        for (int x = 0; x < sfxAreaTexture.width; x++)
        {
            for (int y = 0; y < sfxAreaTexture.height; y++)
            {
                convertedImage[x, y] = sfxAreaTexture.GetPixel(x, y);
            }
        }
        return convertedImage;
    }


}

public class AreaSoundFxManager : MonoBehaviour
{

    public static AreaSoundFxManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public List<SFXArea> sfxAreas = new List<SFXArea>();

    Tilemap baseMap;

    private void Start()
    {
        baseMap = GridManager.instance.groundMap;
        foreach (var map in sfxAreas)
        {
            map.SetColorArray();
        }
    }

    public string GetTileSFXName(Vector3Int position)
    {

        if (baseMap == null && GridManager.instance.groundMap != null)
            baseMap = GridManager.instance.groundMap;
        else
            return "";
        int mapPositionX = (int)NumberFunctions.RemapNumber(position.x, baseMap.cellBounds.min.x, baseMap.cellBounds.max.x, 0, 128);
        int mapPositionY = (int)NumberFunctions.RemapNumber(position.y, baseMap.cellBounds.min.y, baseMap.cellBounds.max.y, 0, 128);
        

        foreach (var map in sfxAreas)
        {
            bool inArea = map.sfxArea[mapPositionX, mapPositionY].a > 0.4f;
            if (inArea)
            {
                return map.sfxAreaName;
            }
        }
        return "Footstep";
    }

    public string GetTileMapName(Vector3Int position)
    {


        int mapPositionX = (int)NumberFunctions.RemapNumber(position.x, baseMap.cellBounds.min.x, baseMap.cellBounds.max.x, 0, 128);
        int mapPositionY = (int)NumberFunctions.RemapNumber(position.y, baseMap.cellBounds.min.y, baseMap.cellBounds.max.y, 0, 128);


        foreach (var map in sfxAreas)
        {
            bool inArea = map.sfxArea[mapPositionX, mapPositionY].a > 0.4f;
            if (inArea)
            {
                return map.mapAreaName;
            }
        }
        return "GroundMap";
    }


}
