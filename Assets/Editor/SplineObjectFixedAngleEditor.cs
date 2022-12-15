using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SplineObjectFixedAngles))]
public class SplineObjectFixedAngleEditor : Editor
{
    

    SplineObjectFixedAngles splineObject;

    SelectionInfo selectionInfo;
    bool needsRepaint;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Set GameObject Positions"))
        {
            
            splineObject.SetAllPositions();

        }
        if (GUILayout.Button("Clear GameObjects"))
        {
            
            splineObject.ClearObjects();

        }
        EditorGUILayout.Space();
        if (GUILayout.Button("Clear All Points"))
        {
            Undo.RecordObject(splineObject, "Clear All Points");
            splineObject.points.Clear();
            
        }

        if (GUI.changed)
        {
            needsRepaint = true;
            SceneView.RepaintAll();
        }
    }

    private void OnSceneGUI()
    {
        Event guiEvent = Event.current;

        if(guiEvent.type == EventType.Repaint)
        {
            DrawPoints();
        }
        else if (guiEvent.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        else
        {
            HandleInput(guiEvent);
            if (needsRepaint)
            {
                HandleUtility.Repaint();
                needsRepaint = false;
            }
        }
    }


    void HandleInput(Event guiEvent)
    {
        Vector3 mousePosition = Event.current.mousePosition;
        mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
        mousePosition = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePosition);
        mousePosition.z = splineObject.zPosition;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.Shift)
        {
            HandleShiftLeftMouseDown(mousePosition);
        }
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleLeftMouseDown(mousePosition);
        }
        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleLeftMouseUp(mousePosition);
        }
        if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleLeftMouseDrag(mousePosition);
        }

        if (!selectionInfo.pointIsSelected)
            UpdateMouseOverInfo(mousePosition);
    }

    void HandleShiftLeftMouseDown(Vector3 mousePosition)
    {
        if (selectionInfo.mouseIsOverPoint)
            DeletePointUnderMouse();
    }

    void HandleLeftMouseDown(Vector3 mousePosition)
    {
        if (!selectionInfo.mouseIsOverPoint)
        {
            int newPointIndex = selectionInfo.mouseIsOverLine ? selectionInfo.lineIndex + 1 : splineObject.points.Count;
            Undo.RecordObject(splineObject, "Add point");
            splineObject.points.Insert(newPointIndex, mousePosition);
            selectionInfo.pointIndex = newPointIndex;
        }

        selectionInfo.pointIsSelected = true;
        selectionInfo.positionAtStartOfDrag = mousePosition;
        needsRepaint = true;
    }

    void HandleLeftMouseUp(Vector3 mousePosition)
    {
        if(selectionInfo.pointIsSelected)
        {
            splineObject.points[selectionInfo.pointIndex] = selectionInfo.positionAtStartOfDrag;
            Undo.RecordObject(splineObject, "Move point");
            splineObject.points[selectionInfo.pointIndex] = mousePosition;

            selectionInfo.pointIsSelected = false;
            selectionInfo.pointIndex = -1;
            needsRepaint = true;
        }
    }

    void HandleLeftMouseDrag(Vector3 mousePosition)
    {
        if (selectionInfo.pointIsSelected)
        {
            splineObject.points[selectionInfo.pointIndex] = mousePosition;
            needsRepaint = true;
        }
    }

    void DeletePointUnderMouse()
    {
        Undo.RecordObject(splineObject, "Delete point");
        splineObject.points.RemoveAt(selectionInfo.pointIndex);
        selectionInfo.pointIsSelected = false;
        selectionInfo.mouseIsOverPoint = false;
        needsRepaint = true;
    }

    void UpdateMouseOverInfo(Vector3 mousePosition)
    {
        // Check if mouse is over a handle
        int mouseOverPointIndex = -1;
        for (int i = 0; i < splineObject.points.Count; i++)
        {
            if(Vector3.Distance(mousePosition, splineObject.points[i]) < splineObject.handleRadius)
            {
                mouseOverPointIndex = i;
                break;
            }
        }
        if(mouseOverPointIndex != selectionInfo.pointIndex)
        {
            selectionInfo.pointIndex = mouseOverPointIndex;
            selectionInfo.mouseIsOverPoint = mouseOverPointIndex != -1;

            needsRepaint = true;
        }

        // The mouse is on a point
        if (selectionInfo.mouseIsOverPoint)
        {
            selectionInfo.mouseIsOverLine = false;
            selectionInfo.lineIndex = -1;
        }
        else // and here the mouse is on a line
        {
            int mouseOverLineIndex = -1;
            float closestLineDistance = splineObject.handleRadius;
            for (int i = 0; i < splineObject.points.Count; i++)
            {
                Vector3 nextPointInShape = splineObject.points[(i + 1) % splineObject.points.Count];
                float distFromMouseToLine = HandleUtility.DistancePointToLineSegment(mousePosition, splineObject.points[i], nextPointInShape);
                if (distFromMouseToLine < closestLineDistance)
                {
                    closestLineDistance = distFromMouseToLine;
                    mouseOverLineIndex = i;
                }
            }
            if(selectionInfo.lineIndex != mouseOverLineIndex)
            {
                selectionInfo.lineIndex = mouseOverLineIndex;
                selectionInfo.mouseIsOverLine = mouseOverLineIndex != -1;
                needsRepaint = true;
            }
        }
    }

    void DrawPoints()
    {
        for (int i = 0; i < splineObject.points.Count; i++)
        {
            Vector3 nextPoint;
            if (splineObject.closeObject) {
                nextPoint = splineObject.points[(i + 1) % splineObject.points.Count];
                Handles.color = i == selectionInfo.lineIndex ? Color.red : Color.black;
                Handles.DrawDottedLine(splineObject.points[i], nextPoint, 4);
            }
            else
            {
                if (i < splineObject.points.Count -1)
                {
                    nextPoint = splineObject.points[(i + 1)];
                    Handles.color = i == selectionInfo.lineIndex ? Color.red : Color.black;
                    Handles.DrawDottedLine(splineObject.points[i], nextPoint, 4);
                }
            }
            if (i == selectionInfo.pointIndex)
                Handles.color = selectionInfo.pointIsSelected ? Color.black : Color.red;
            else
                Handles.color = Color.white;
            Handles.DrawSolidDisc(splineObject.points[i], Vector3.forward, splineObject.handleRadius);
        }
        needsRepaint = false;
    }


    private void OnEnable()
    {
        splineObject = target as SplineObjectFixedAngles;
        selectionInfo = new SelectionInfo();
        Tools.hidden = true;
    }

    private void OnDisable()
    {
        Tools.hidden = false;
    }

    public class SelectionInfo 
    {
        public int pointIndex = -1;
        public bool mouseIsOverPoint;
        public bool pointIsSelected;
        public Vector3 positionAtStartOfDrag;

        public int lineIndex = -1;
        public bool mouseIsOverLine;
    }

}
