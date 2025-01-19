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
        public bool hasAnimated = true;
    }

    public List<MapArea> mapAreas = new List<MapArea>();
    public List<Color[]> colors = new List<Color[]>();

    private void Start()
    {
        SetMapAreas();
    }

    public void SetMapAreas()
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
            if (map.mapName == mapName && !map.active)
            {
                map.active = true;
                map.hasAnimated = false;
                Notifications.instance.SetNewNotification("Map Added", null, 0, NotificationsType.Map);
                
            }
                
        }
    }

    public void ActivateMapAreaFromSave(string mapName, bool animated)
    {
        foreach (var map in mapAreas)
        {
            if (map.mapName == mapName && !map.active)
            {
                map.active = true;
                map.hasAnimated = animated;
            }

        }
    }
}
