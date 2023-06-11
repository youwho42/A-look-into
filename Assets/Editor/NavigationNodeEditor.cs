using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(NavigationNode))]
public class NavigationNodeEditor : Editor
{

    private NavigationNode navNode;
    bool addNewSelection;
    bool removeNewSelection;
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
        string addSelectedText = buttonPressed && !removeNewSelection ? "Add selection as child - SELECTED" : "Add selection as child";
        if (GUILayout.Button(addSelectedText))
        {

            if (!removeNewSelection)
            {
                if (buttonPressed)
                {
                    buttonPressed = false;
                    addNewSelection = false;
                }
                else
                {
                    buttonPressed = true;
                    addNewSelection = true;
                } 
            }
        }
        string removeSelectedText = buttonPressed && !addNewSelection ? "Remove selection as child - SELECTED" : "Remove selection as child";
        if (GUILayout.Button(removeSelectedText))
        {
            if (!addNewSelection)
            {
                if (buttonPressed)
                {
                    buttonPressed = false;
                    removeNewSelection = false;
                }
                else
                {
                    buttonPressed = true;
                    removeNewSelection = true;
                } 
            }
        }
    }
    private void DuringSceneGUI(SceneView sceneView)
    {
        if (addNewSelection && Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            GameObject selectedObject = HandleUtility.PickGameObject(Event.current.mousePosition, false);
            if (selectedObject != null)
            {
                NavigationNode otherNode = selectedObject.GetComponent<NavigationNode>();
                if (otherNode != null && otherNode != navNode)
                {
                    navNode.children.Add(otherNode);
                    otherNode.children.Add(navNode);
                    addNewSelection = false;
                    Event.current.Use();
                }
            }
        }
        if (removeNewSelection && Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            GameObject selectedObject = HandleUtility.PickGameObject(Event.current.mousePosition, false);
            if (selectedObject != null)
            {
                NavigationNode otherNode = selectedObject.GetComponent<NavigationNode>();
                if (otherNode != null && otherNode != navNode)
                {
                    navNode.children.Remove(otherNode);
                    otherNode.children.Remove(navNode);
                    removeNewSelection = false;
                    Event.current.Use();
                }
            }
        }
    }


}
