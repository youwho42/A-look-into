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
        

        if (GUILayout.Button("Generate/Save Chunks"))
        {
            mapChunks.GenerateChunks();
            SaveChunks(mapChunks);
        }
        /*if (GUILayout.Button("Save Chunk Map Object"))
        {
            SaveChunks(mapChunks);
        }*/
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
            UpdateMapChunk(mapChunks, SelectedChunks);
        }
        
        serializedObject.ApplyModifiedProperties();

    }

    public void UpdateMapChunk(GenerateMapChunks mapChunks, List<MapChunk> chunks)
    {
        foreach (MapChunk chunk in chunks)
        {
            
            mapChunks.chunkLoadScriptableObject.UpdateChunks(chunk);
                    
        }
    }

    public void SaveChunks(GenerateMapChunks map)
    {
        if (map.allMapChunks.Count <= 0)
            return;
        ChunkTerrainData newMap = ScriptableObject.CreateInstance<ChunkTerrainData>();

        newMap.SaveTerrainData(map.allMapChunks, (Vector2Int)map.fullMap.position);
        if (!AssetDatabase.IsValidFolder("Assets/Terrain/"))
            AssetDatabase.CreateFolder("Assets/", "Terrain");

        string path = "Assets/Terrain/" + map.mapName + ".asset";

        AssetDatabase.CreateAsset(newMap, path);
        map.chunkLoadScriptableObject = newMap;
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newMap;
    }

    protected virtual void OnSceneGUI()
    {
        
        GenerateMapChunks mapChunks = (GenerateMapChunks)target;
        if (mapChunks.allMapChunks == null || mapChunks.allMapChunks.Count <= 0)
        {
            if (mapChunks.chunkLoadScriptableObject != null)
                mapChunks.LoadChunksFromSave(); 
            return;
        }
            
        

        
        foreach (var chunk in mapChunks.chunkLoadScriptableObject.allChunks)
        {
            
            Handles.color = !SelectedChunks.Contains(chunk) ? Color.green : Color.red;
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
