using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGenerator = (MapGenerator)target;

        if(DrawDefaultInspector())
        {
            if (mapGenerator.autoUpdate)
            {
                mapGenerator.GenerateMapFromNoise();
            }
        }


        if(GUILayout.Button("Generate Map From Noise"))
        {
            mapGenerator.GenerateMapFromNoise();
        }
        if (GUILayout.Button("Generate Map From Image"))
        {
            mapGenerator.GenerateMapFromImage();
        }
        if (GUILayout.Button("Erase Map"))
        {
            mapGenerator.ClearTilesBlock();
        }
    }
}
