using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMarkerTextureMap : MonoBehaviour
{

    public static PlayerMarkerTextureMap instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }


    [Serializable]
    public struct ColorScheme
    {
        public string schemeName;
        public Color[] colors;
    }

    public class Marker
    {
        public Vector2Int mapPosition;
        public Vector2Int terrainPosition;
        public bool active;
    }
    

    public RawImage playerMarkerImageUI;

   

    public Marker[] markers;
    

    public int currentMarkers = 1;
    int maxMarkers = 5;
    

    public List<ColorScheme> colorSchemes = new List<ColorScheme>();
    public int selectedColorScheme = 0;

    


    private void Start()
    {
        
        markers = new Marker[maxMarkers];
        for (int i = 0; i < markers.Length; i++)
        {
            markers[i] = new Marker();
        }
        DrawPlayerMap();
    }

    private void OnEnable()
    {
        GameEventManager.onMapClickEvent.AddListener(SetMarkerPosition);
    }
    private void OnDisable()
    {
        GameEventManager.onMapClickEvent.RemoveListener(SetMarkerPosition);
    }

    public void SetColorScheme(string schemeName)
    {
        
        for (int i = 0; i < colorSchemes.Count; i++)
        {
            if (colorSchemes[i].schemeName == schemeName)
            {
                selectedColorScheme = i;
                DrawPlayerMap();
                return;
            }
                
        }
    }

    void SetMarkerPosition()
    {
        
        var rt = (RectTransform)playerMarkerImageUI.transform;
        var screenPos = Mouse.current.position.value;
        bool inside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rt,
            screenPos,
            null,
            out Vector2 pointInRect
        );

        Vector2Int mapPos = -Vector2Int.one;
        if (inside)
        {
            Vector2 textureCoord = pointInRect - rt.rect.min;
            textureCoord.x *= playerMarkerImageUI.uvRect.width / rt.rect.width;
            textureCoord.y *= playerMarkerImageUI.uvRect.height / rt.rect.height;
            textureCoord += playerMarkerImageUI.uvRect.min;

            Vector2Int pos = Vector2Int.zero;
            pos.x = (int)MapNumber.Remap(textureCoord.x, 0, 1, 0, 128);
            pos.y = (int)MapNumber.Remap(textureCoord.y, 0, 1, 0, 128);

            if (pos.x > 0 && pos.y > 0 && pos.x < 128 && pos.y < 128)
                mapPos = pos;
        }
        if (mapPos != -Vector2.one)
        {
            var map = PlayerInformation.instance.playerController.currentTilePosition.groundMap;
            Vector2Int terrainPos = Vector2Int.zero;
            terrainPos.x = (int)MapNumber.Remap(mapPos.x, 0, 128, map.cellBounds.xMin, map.cellBounds.xMax);
            terrainPos.y = (int)MapNumber.Remap(mapPos.y, 0, 128, map.cellBounds.yMin, map.cellBounds.yMax);
            AddMarkers(mapPos, terrainPos);
            DrawPlayerMap();
        }
    }

    void AddMarkers(Vector2Int mapPos, Vector2Int terrainPos)
    {
        if (!PlayerInformation.instance.equipmentManager.HasItemEquipped(EquipmentSlot.Compass))
            return;
        else
            SetCurrentMarkers(PlayerInformation.instance.equipmentManager.GetEquipmentTier(EquipmentSlot.Compass));
        
        for (int i = 0; i < currentMarkers; i++)
        {
            if (!markers[i].active)
            {
                markers[i].mapPosition = mapPos;
                markers[i].terrainPosition = terrainPos;
                markers[i].active = true;
                var pos = PlayerInformation.instance.player.position + new Vector3(UnityEngine.Random.Range(-.3f, .3f), UnityEngine.Random.Range(.1f, .3f), 0);
                BallPeopleManager.instance.SpawnIndicator(i, colorSchemes[selectedColorScheme].colors[i], pos);
                return;
            }
            else
            {
                if (markers[i].mapPosition == mapPos)
                {
                    markers[i].active = false;
                    return;
                }
                    
            }
        }


        
    }

    void SetCurrentMarkers(EquipmentTier tier)
    {
        switch (tier)
        {
            case EquipmentTier.I:
                currentMarkers = 1;
                break;
            case EquipmentTier.II:
                currentMarkers = 3;
                break;
            case EquipmentTier.III:
                currentMarkers = 5;
                break;
            
        }
    }

    public void RemoveMarkerAtIndex(int index)
    {
        markers[index].active = false;
        DrawPlayerMap();
    }

    public void RemoveAllMarkers()
    {
        foreach (var mark in markers)
        {
            mark.active = false;
        }
        
        DrawPlayerMap();
    }

    public void DrawPlayerMap()
    {
        int width = 128;
        int height = 128;

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        Color[] colorMap = new Color[width * height];
        Color neutral = new Color(0, 0, 0, 0);
        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                colorMap[y * width + x] = neutral;
            }
        }
        texture.SetPixels(colorMap);
        

        for (int i = 0; i < markers.Length; i++)
        {
            if (markers[i].active)
            {
                Color c = colorSchemes[selectedColorScheme].colors[i];
                c.a = 0.35f;
                DrawCircle(texture, c, markers[i].mapPosition.x, markers[i].mapPosition.y, 5, true);
            }
            
        }
        for (int i = 0; i < markers.Length; i++)
        {
            if (markers[i].active)
                DrawCircle(texture, colorSchemes[selectedColorScheme].colors[i], markers[i].mapPosition.x, markers[i].mapPosition.y, 1, false);
        }


        texture.Apply();
        texture.filterMode = FilterMode.Point;

        playerMarkerImageUI.texture = texture;
    }

    public void DrawCircle(Texture2D tex, Color color, int x, int y, int radius,bool blend)
    {
        float rSquared = radius * radius;

        for (int u = x - radius; u < x + radius + 1; u++)
        {
            for (int v = y - radius; v < y + radius + 1; v++)
            {
                if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                {
                    Color blendedColor = color;
                    if (blend)
                    {
                        Color textureColor = tex.GetPixel(u, v);
                        if(textureColor.a != 0)
                        {
                            float blendFactor = 0.5f;
                            blendedColor = Color.Lerp(color, textureColor, blendFactor);
                        }
                    }
                    tex.SetPixel(u, v, blendedColor);
                }
            }
        }
    }
}
