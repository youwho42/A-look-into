using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GenerateAllTilesInfo))]
public class GenerateAllTilesInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GenerateAllTilesInfo allTilesInfo = (GenerateAllTilesInfo)target;

        DrawDefaultInspector();

        serializedObject.Update();


        if (GUILayout.Button("Save All Tiles Info"))
        {
            allTilesInfo.GetAllTiles();
            SaveTilesInfo(allTilesInfo);
        }
        
    }

    public void SaveTilesInfo(GenerateAllTilesInfo allTiles)
    {
        if (allTiles.allTilesValues.Count <= 0)
            return;
        TileInfoObject newTileInfo = ScriptableObject.CreateInstance<TileInfoObject>();

        newTileInfo.SaveTileInfo(allTiles.allTilesValues);
        if (!AssetDatabase.IsValidFolder("Assets/Terrain/"))
            AssetDatabase.CreateFolder("Assets/", "Terrain");

        string path = "Assets/Terrain/" + allTiles.allTilesName + ".asset";

        AssetDatabase.CreateAsset(newTileInfo, path);
        
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newTileInfo;
        allTiles.ClearAllTiles();
    }
}
