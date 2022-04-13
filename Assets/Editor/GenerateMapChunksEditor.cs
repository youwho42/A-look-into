using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateMapChunks))]
public class GenerateMapChunksEditor : Editor
{

    public override void OnInspectorGUI()
    {
        GenerateMapChunks mapChunks = (GenerateMapChunks)target;

        DrawDefaultInspector();
        
        serializedObject.Update();
        

        if (GUILayout.Button("Generate Chunks"))
        {
            mapChunks.GenerateChunks();
        }
        if (GUILayout.Button("Show All Chunks"))
        {
            mapChunks.DrawAllChunks();
        }
        if (GUILayout.Button("Hide All Chunks"))
        {
            mapChunks.HideAllChunks();
        }

        serializedObject.ApplyModifiedProperties();

    }

    protected virtual void OnSceneGUI()
    {
        GenerateMapChunks mapChunks = (GenerateMapChunks)target;

        foreach (var chunk in mapChunks.allMapChunks)
        {
            Handles.color = Color.grey;
            if (Handles.Button(chunk.centerTilePosition, Quaternion.identity, 0.5f, 0.5f, Handles.SphereHandleCap))
            {
                if (!chunk.isVisible)
                    mapChunks.DrawChunk(chunk);
                else
                    mapChunks.HideChunk(chunk);
            }
                

            GUI.color = Color.black;
            Handles.Label(chunk.centerTilePosition, "pos: " + chunk.centerTilePosition);
        }

        
    }

}
