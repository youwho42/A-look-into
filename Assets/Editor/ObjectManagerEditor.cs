using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectManagerCircle))]
public class ObjectManagerEditor : Editor
{
    private ObjectManagerCircle objectManager;

    //The center of the circle
    private Vector3 center;

    private void OnEnable()
    {
        objectManager = target as ObjectManagerCircle;

        //Hide the handles of the GO so we dont accidentally move it instead of moving the circle
        Tools.hidden = true;
    }

    private void OnDisable()
    {
        //Unhide the handles of the GO
        Tools.hidden = false;
    }

    
    private void OnSceneGUI()
    {
        //Move the circle when moving the mouse
        //A ray from the mouse position
        var hit = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        //var hit = Camera.current.ScreenPointToRay(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin);
        var position = hit.origin;
        
        position.z = 0;

        int offsetZ = objectManager.GetTileZ(position);

        

            //Where did we hit the ground?
        center = position;

        //Need to tell Unity that we have moved the circle or the circle may be displayed at the old position
        SceneView.RepaintAll();

        Handles.color = Color.blue;

        Handles.DrawWireDisc(center + new Vector3(0, offsetZ * 0.2790625f, offsetZ), Vector3.forward, objectManager.radius);

        //Display the circle
        Handles.color = Color.white;

        Handles.DrawWireDisc(center, Vector3.forward, objectManager.radius);
        

        //Add or remove objects with left mouse click

        //First make sure we cant select another gameobject in the scene when we click
        HandleUtility.AddDefaultControl(0);

        //Have we clicked with the left mouse button?
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            //Should we add or remove objects?
            if (objectManager.action == ObjectManagerCircle.Actions.AddObjects)
            {
                AddNewPrefabs(center + new Vector3(0, offsetZ * 0.2790625f, offsetZ));

                MarkSceneAsDirty();
            }
            else if (objectManager.action == ObjectManagerCircle.Actions.RemoveObjects)
            {
                objectManager.RemoveObjects(center);

                MarkSceneAsDirty();
            }
        }
    }

    //Add buttons this scripts inspector
    public override void OnInspectorGUI()
    {
        //Add the default stuff
        DrawDefaultInspector();

        //Remove all objects when pressing a button
        if (GUILayout.Button("Remove all objects"))
        {
            //Pop-up so you don't accidentally remove all objects
            if (EditorUtility.DisplayDialog("Safety check!", "Do you want to remove all objects?", "Yes", "No"))
            {
                objectManager.RemoveAllObjects();

                MarkSceneAsDirty();
            }
        }
    }

    //Force unity to save changes or Unity may not save when we have instantiated/removed prefabs despite pressing save button
    private void MarkSceneAsDirty()
    {
        UnityEngine.SceneManagement.Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(activeScene);
    }

    //Instantiate prefabs at random positions within the circle
    private void AddNewPrefabs(Vector3 center)
    {
        //How many prefabs do we want to add
        int howManyObjects = objectManager.howManyObjects;

        //Which prefab to we want to add
        
        

        for (int i = 0; i < howManyObjects; i++)
        {
            int rand = Random.Range(0, objectManager.prefabGO.Length);
            GameObject prefabGO = objectManager.prefabGO[rand];
            GameObject newGO = PrefabUtility.InstantiatePrefab(prefabGO) as GameObject;

            //Send it to the main script to add it at a random position within the circle
            objectManager.AddPrefab(newGO, center);
        }
    }
}