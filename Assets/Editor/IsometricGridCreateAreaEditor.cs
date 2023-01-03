using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IsometricGridCreateArea))]
public class IsometricGridCreateAreaEditor : Editor
{
    IsometricGridCreateArea isometricGrid;
    
    private void OnEnable()
    {
        isometricGrid = target as IsometricGridCreateArea;
        
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
        Vector3 mousePosition = Event.current.mousePosition;
        mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
        mousePosition = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePosition);
        mousePosition.z = 0;

        

        SceneView.RepaintAll();

        //First make sure we cant select another gameobject in the scene when we click
        HandleUtility.AddDefaultControl(0);

        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            int offsetZ = isometricGrid.gridObject.GetTileZ(mousePosition).z;
            Vector3 pos = isometricGrid.gridObject.FindPositionOnGrid(mousePosition);

            //Debug.Log("Placing Object" + mousePosition);
            AddNewPrefab(pos / 2 + new Vector3(0, 0, offsetZ+1));

            MarkSceneAsDirty();
            
        }


    }

    private void MarkSceneAsDirty()
    {
        UnityEngine.SceneManagement.Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(activeScene);
    }

    private void AddNewPrefab(Vector3 center)
    {
        //Send it to the main script to add it at a random position within the circle
        isometricGrid.PlaceObject(center);
         
    }
    
}
