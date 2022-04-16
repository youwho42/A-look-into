using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateMapChunks))]
public class GenerateMapChunksEditor : Editor
{
    private readonly List<MapChunk> SelectedChunks = new List<MapChunk>();
    public override void OnInspectorGUI()
    {
        GenerateMapChunks mapChunks = (GenerateMapChunks)target;

        DrawDefaultInspector();
        
        serializedObject.Update();
        

        if (GUILayout.Button("Generate Chunks"))
        {
            mapChunks.GenerateChunks();
        }
        if (GUILayout.Button("Save Chunk Map Object"))
        {
            mapChunks.SaveChunks();
        }
        if (GUILayout.Button("Load Chunk Map Object"))
        {
            mapChunks.LoadChunksFromSave();
        }
        

        EditorGUILayout.Space();

        if (GUILayout.Button("Show All Chunks"))
        {
            mapChunks.DrawAllChunks();
        }
        if (GUILayout.Button("Hide All Chunks"))
        {
            mapChunks.HideAllChunks();
        }

        EditorGUILayout.Space();


        if (GUILayout.Button("Show Selected Chunks"))
        {
            mapChunks.DrawSelectedChunks(SelectedChunks);
        }
        if (GUILayout.Button("Hide Selected Chunks"))
        {
            mapChunks.HideSelectedChunks(SelectedChunks);
        }
        if (GUILayout.Button("Update Selected Chunks"))
        {
            mapChunks.UpdateSelectedChunks(SelectedChunks);
        }
        
        serializedObject.ApplyModifiedProperties();

    }

    protected virtual void OnSceneGUI()
    {
        GenerateMapChunks mapChunks = (GenerateMapChunks)target;

        foreach (var chunk in mapChunks.allMapChunks)
        {
            Handles.color = !SelectedChunks.Contains(chunk) ? Color.green: Color.red;
            if (Handles.Button(chunk.centerTilePosition, Quaternion.identity, 0.5f, 0.5f, Handles.SphereHandleCap))
            {
                if (Event.current.shift)
                {
                    if (SelectedChunks.Contains(chunk))
                    {
                        SelectedChunks.Remove(chunk);
                    }
                    else
                    {
                        
                        SelectedChunks.Add(chunk);
                    }

                }
                else
                {
                    SelectedChunks.Clear();
                    SelectedChunks.Add(chunk);
                }
            }

            
        }

        
    }

}
