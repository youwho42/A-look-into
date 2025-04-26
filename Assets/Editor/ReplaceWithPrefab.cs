using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Klaxon.SaveSystem;

public class ReplaceWithPrefab : EditorWindow
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private List<GameObject> prefabList = new List<GameObject>();

    [MenuItem("Tools/Replace With Prefab")]
    static void CreateReplaceWithPrefab()
    {
        EditorWindow.GetWindow<ReplaceWithPrefab>();
    }

    private void OnGUI()
    {
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        if (GUILayout.Button("Add to List") && prefab != null)
        {
            if (!prefabList.Contains(prefab))
            {
                prefabList.Add(prefab);
            }
            else
            {
                Debug.LogWarning("Prefab is already in the list!");
            }
        }

        EditorGUILayout.LabelField("Prefab List:", EditorStyles.boldLabel);

        for (int i = 0; i < prefabList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            prefabList[i] = (GameObject)EditorGUILayout.ObjectField(prefabList[i], typeof(GameObject), false);

            if (GUILayout.Button("Remove"))
            {
                prefabList.RemoveAt(i);
            }

            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Clear List"))
        {
            prefabList.Clear();
        }

        if (GUILayout.Button("Replace"))
        {
            var selection = Selection.gameObjects;

            for (var i = selection.Length - 1; i >= 0; --i)
            {
                var selected = selection[i];
                var currentPrefab = prefab;
                if (prefabList.Count > 0)
                {
                    int r = Random.Range(0, prefabList.Count);
                    currentPrefab = prefabList[r];
                }
                var prefabType = PrefabUtility.GetPrefabAssetType(currentPrefab);
                GameObject newObject;

                if (prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant)
                {
                    newObject = (GameObject)PrefabUtility.InstantiatePrefab(currentPrefab);
                }
                else
                {
                    newObject = Instantiate(currentPrefab);
                    newObject.name = currentPrefab.name;
                }

                if (newObject == null)
                {
                    Debug.LogError("Error instantiating prefab");
                    break;
                }

                Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
                newObject.transform.parent = selected.transform.parent;
                newObject.transform.localPosition = selected.transform.localPosition;
                newObject.transform.localRotation = selected.transform.localRotation;
                newObject.transform.localScale = selected.transform.localScale;
                if(newObject.TryGetComponent(out SaveableWorldEntity newID))
                {
                    if (selected.TryGetComponent(out SaveableWorldEntity selectedID))
                    {
                        if (selectedID.ID !="")
                            newID.SetID(selectedID.ID);
                        else
                            newID.GenerateId();
                    }
                    else
                        newID.GenerateId();
                }
                newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
                Undo.DestroyObjectImmediate(selected);
            }
        }

        GUI.enabled = false;
        EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
    }
}