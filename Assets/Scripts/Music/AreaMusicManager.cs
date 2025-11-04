using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine.Audio;
using System.Collections;
using System;

[Serializable]
public class MusicArea
{
    public Texture2D musicAreaTexture;
    [HideInInspector]
    public Color[,] musicArea;
    public string musicAreaName;
    [HideInInspector]
    public bool isPlaying;
    [HideInInspector]
    public float t;

    

    public void SetColorArray()
    {
        musicArea = ConvertImage();
    }

    Color[,] ConvertImage()
    {

        Color[,] convertedImage = new Color[musicAreaTexture.width, musicAreaTexture.height];
        for (int x = 0; x < musicAreaTexture.width; x++)
        {
            for (int y = 0; y < musicAreaTexture.height; y++)
            {
                convertedImage[x, y] = musicAreaTexture.GetPixel(x, y);
            }
        }
        return convertedImage;
    }


}
public class AreaMusicManager : MonoBehaviour
{
    public List<MusicArea> allMusicAreas = new List<MusicArea>();
    //public Texture2D musicAreaTexture;
    //Color[,] musicArea;
    Tilemap baseMap;
    public AudioMixer audioMixer;
    bool lastState;
    float currentLandscapeT;

    bool landscapeMusicIsPlaying;

    MusicArea currentArea;

    private void Start()
    {
        baseMap = GridManager.instance.groundMap;
        GameEventManager.onPlayerPositionUpdateEvent.AddListener(CheckTile);
        foreach (var map in allMusicAreas)
        {
            map.SetColorArray();
        }
       
        
    }

    private void OnDestroy()
    {
        GameEventManager.onPlayerPositionUpdateEvent.RemoveListener(CheckTile);
    }

    void CheckTile(Vector3Int position)
    {

        if (audioMixer == null)
            return;


        int mapPositionX = (int)NumberFunctions.RemapNumber(position.x, baseMap.cellBounds.min.x, baseMap.cellBounds.max.x, 0, 128);
        int mapPositionY = (int)NumberFunctions.RemapNumber(position.y, baseMap.cellBounds.min.y, baseMap.cellBounds.max.y, 0, 128);
        bool inMapArea = false;
        
        foreach (var map in allMusicAreas)
        {
            bool inArea = map.musicArea[mapPositionX, mapPositionY].a > 0.4f;
            if (inArea)
            {
                inMapArea = true;
                currentArea = map;
                break;
            }
        }

        if (currentArea == null)
            return;

        if (lastState == inMapArea)
            return;

        lastState = inMapArea;


        if (inMapArea)
        {
            StopAllCoroutines();
            StartCoroutine(FadeInCo("AreaMusics", 5.0f, currentArea));
            StartCoroutine(FadeOutCo("LandscapeMusic", 2.0f, null));


        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FadeInCo("LandscapeMusic", 20.0f, null));
            StartCoroutine(FadeOutCo("AreaMusics", 10.0f, currentArea));
        }
        

    }

    IEnumerator FadeInCo(string mixerGroup, float timer, MusicArea musicArea)
    {
        float currentFadeTime = musicArea != null ? musicArea.t * timer : currentLandscapeT * timer;
        if (musicArea != null)
        {
            if (!musicArea.isPlaying)
            {
                AudioManager.instance.PlaySound($"{musicArea.musicAreaName}");
                musicArea.isPlaying = true;
            }
        }
        

        while (currentFadeTime < timer)
            {
                currentFadeTime += Time.deltaTime;
                float t = currentFadeTime / timer;
                audioMixer.SetFloat(mixerGroup, Mathf.Log10(t) * 20);

            if (musicArea != null)
                musicArea.t = t;
            else
                currentLandscapeT = t;

                yield return null;
            }
        if (musicArea != null)
            musicArea.t = 0;
        else
            currentLandscapeT = 0;
        yield return null;

    }

    IEnumerator FadeOutCo(string mixerGroup, float timer, MusicArea musicArea)
    {
        float currentFadeTime = musicArea != null ? musicArea.t * timer : currentLandscapeT * timer;
        
        while (currentFadeTime < timer)
            {
                currentFadeTime += Time.deltaTime;
                float t = 1.0f - (currentFadeTime / timer);
                audioMixer.SetFloat(mixerGroup, Mathf.Log10(t) * 20);

                if (musicArea != null)
                    musicArea.t = t;
            else
                currentLandscapeT = t;
            yield return null;
            }

        if (musicArea != null)
            musicArea.t = 0;
        else
            currentLandscapeT = 0;

        if (musicArea != null)
        {
            if (musicArea.isPlaying)
            {
                AudioManager.instance.StopSound($"{musicArea.musicAreaName}");
                musicArea.isPlaying = false;
            }
            currentArea = null;
        }
        yield return null;
    }

    
}
