using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Klaxon.UndertakingSystem;
using System.Linq;

[CustomEditor(typeof(UndertakingDatabase))]
public class UndertakingDatabaseHolder : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draw the default inspector fields

        UndertakingDatabase scriptableObject = (UndertakingDatabase)target;

        if (GUILayout.Button("Get All Undertakings"))
        {
            scriptableObject.allUndertakingObjects.Clear();
            // Perform your custom context menu action here
            scriptableObject.allUndertakingObjects = Resources.LoadAll<UndertakingObject>("Undertakings/").ToList();
            scriptableObject.allUndertakingObjects = scriptableObject.allUndertakingObjects.OrderBy(x => x.name).ThenBy(x => x.Name).ToList();

        }
        
    }
}