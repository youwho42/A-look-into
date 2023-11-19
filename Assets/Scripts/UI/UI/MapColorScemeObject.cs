using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapColorScemeObject : MonoBehaviour
{
    public string schemeName;

    public List<Image> swatches = new List<Image>();
    PlayerMarkerTextureMap markerTextureMap;

    public void SetSchemeObject(string name, List<Color> colors, PlayerMarkerTextureMap playerMarkerTextureMap)
    {
        markerTextureMap = playerMarkerTextureMap;
        schemeName = name;
        for (int i = 0; i < swatches.Count; i++)
        {
            swatches[i].color = colors[i];
        }
    }

    public void SetSchemeActive()
    {
        markerTextureMap.SetColorScheme(schemeName);
    }
}
