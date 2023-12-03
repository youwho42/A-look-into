using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMapsManager : MonoBehaviour
{
    public static PlayerMapsManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    [Serializable]
    public class MapArea
    {
        public string mapName;
        public Sprite mapImage;
        public bool active;
    }

    public List<MapArea> mapAreas = new List<MapArea>();
    public List<Color[]> colors = new List<Color[]>();

    private void Start()
    {
        foreach (var map in mapAreas)
        {
            Color[] c = map.mapImage.texture.GetPixels();
            colors.Add(c);
        }
    }

    public void ActivateMapArea(string mapName)
    {
        foreach (var map in mapAreas)
        {
            if (map.mapName == mapName)
                map.active = true;
        }
    }
}
