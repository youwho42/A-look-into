using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavigationNode))]
public class NavigationNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NavigationNode navNode = (NavigationNode)target;

        DrawDefaultInspector();


        if (GUILayout.Button("Add child node"))
        {
            navNode.AddNewChild();
        }
        
    }
}
