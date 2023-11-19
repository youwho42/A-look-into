using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapLegendManager : MonoBehaviour
{
    public TilemapToPNG tilemapToPNG;
    public Transform legendHolder;
    public MapLegend mapLegendOject;
    public Transform schemeHolder;
    public MapColorScemeObject colorScemeObject;
    public PlayerMarkerTextureMap markerTextureMap;


    private void Start()
    {
        SetAllLegends();
        SetAllSchemes();
    }
    void SetAllLegends()
    {
        foreach (var item in tilemapToPNG.mapLayers)
        {
            var go = Instantiate(mapLegendOject, legendHolder);
            go.SetLegend(item.color, item.mapName);
        }
    }

    void SetAllSchemes()
    {
        foreach (var scheme in markerTextureMap.colorSchemes)
        {
            var go = Instantiate(colorScemeObject, schemeHolder);
            go.SetSchemeObject(scheme.schemeName, scheme.colors.ToList(), markerTextureMap);
        }
    }
}
