using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(NavigationNode))]
public class NavigationNodeEditor : Editor
{

    private NavigationNode navNode;
    bool useNewSelection;
    bool buttonPressed;
    private void OnEnable()
    {
        navNode = (NavigationNode)target;
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    public override void OnInspectorGUI()
    {
        

        DrawDefaultInspector();
        

        if (GUILayout.Button("Add child node"))
        {
            Selection.activeGameObject = navNode.AddNewChild();
        }
        string addSelectedText = buttonPressed ? "Add selection as child - SELECTED" : "Add selection as child";
        if (GUILayout.Button(addSelectedText))
        {
            if (buttonPressed)
            {
                buttonPressed = false;
                useNewSelection = false;
            }
            else
            {
                buttonPressed = true;
                useNewSelection = true;
            }
        }
    }
    private void DuringSceneGUI(SceneView sceneView)
    {
        if (useNewSelection && Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            GameObject selectedObject = HandleUtility.PickGameObject(Event.current.mousePosition, false);
            if (selectedObject != null)
            {
                NavigationNode otherNode = selectedObject.GetComponent<NavigationNode>();
                if (otherNode != null && otherNode != navNode)
                {
                    navNode.children.Add(otherNode);
                    otherNode.children.Add(navNode);
                    useNewSelection = false;
                    Event.current.Use();
                }
            }
        }
    }


}
