using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GridManager gridManager = (GridManager)target;

        DrawDefaultInspector();


        if (GUILayout.Button("Compress All Tilemap Bounds"))
        {
            gridManager.CompressAllTilemapBounds();
        }
        

    }
}
