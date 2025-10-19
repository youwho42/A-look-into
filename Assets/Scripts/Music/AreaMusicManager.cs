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
    private void Start()
    {
        baseMap = GridManager.instance.groundMap;
        GameEventManager.onPlayerPositionUpdateEvent.AddListener(CheckTile);
        foreach (var map in allMusicAreas)
        {
            map.SetColorArray();
        }
       
        
    }

    void CheckTile(Vector3Int position)
    {

        if (audioMixer == null)
            return;


        int mapPositionX = (int)NumberFunctions.RemapNumber(position.x, baseMap.cellBounds.min.x, baseMap.cellBounds.max.x, 0, 127);
        int mapPositionY = (int)NumberFunctions.RemapNumber(position.y, baseMap.cellBounds.min.y, baseMap.cellBounds.max.y, 0, 127);

        foreach (var map in allMusicAreas)
        {
            bool inArea = map.musicArea[mapPositionX, mapPositionY].a > 0.4f;
            

            if (lastState == inArea)
                return;
            
            lastState = inArea;


            if (inArea)
            {
                StopAllCoroutines();
                StartCoroutine(FadeInCo("AreaMusics", 5.0f));
                StartCoroutine(FadeOutCo("LandscapeMusic", 2.0f));

                AudioManager.instance.PlaySound($"{map.musicAreaName}");
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(FadeInCo("LandscapeMusic", 20.0f));
                StartCoroutine(FadeOutCo("AreaMusics", 10.0f));
            }
        }
        
    }

    IEnumerator FadeInCo(string mixerGroup, float timer)
    {
        float currentTime = 0;
        while (currentTime < timer)
        {
            currentTime += Time.deltaTime;
            float t = currentTime/timer;
            audioMixer.SetFloat(mixerGroup, Mathf.Log10(t) * 20);
            yield return null;
        }
        yield return null;

    }

    IEnumerator FadeOutCo(string mixerGroup, float timer)
    {
        float currentTime = 0;
        while (currentTime < timer)
        {
            currentTime += Time.deltaTime;
            float t = 1.0f - (currentTime / timer);
            audioMixer.SetFloat(mixerGroup, Mathf.Log10(t) * 20);
            yield return null;
        }
        yield return null;
    }

    //Color[,] ConvertImage()
    //{

    //    Color[,] convertedImage = new Color[musicAreaTexture.width, musicAreaTexture.height];
    //    for (int x = 0; x < musicAreaTexture.width; x++)
    //    {
    //        for (int y = 0; y < musicAreaTexture.height; y++)
    //        {
    //            convertedImage[x, y] = musicAreaTexture.GetPixel(x, y);
    //        }
    //    }
    //    return convertedImage;
    //}
}
