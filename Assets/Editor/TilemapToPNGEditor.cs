using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TilemapToPNG))]
public class TilemapToPNGEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TilemapToPNG tilemapToImage = (TilemapToPNG)target;

        DrawDefaultInspector();


        if (GUILayout.Button("Generate Base Map Image"))
        {
            tilemapToImage.SaveBaseMap();
        }
        //if (GUILayout.Button("Generate Full Map Image"))
        //{
        //    tilemapToImage.GenerateMapFromImage();
        //}

       
    }
}
