using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SplineObjectFixedAngles : MonoBehaviour
{

    [HideInInspector]
    public List<Vector3> points = new List<Vector3>();
    [HideInInspector]
    public float handleRadius = 0.025f;

    public List<GameObject> itemsToPlace = new List<GameObject>();
    public Transform objectHolder;
    public float distanceBetweenObjects;
    public int zPosition;
    public bool closeObject;

#if (UNITY_EDITOR)
    public void SetAllPositions()
    {
        ClearObjects();
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 nextPoint;
            if (closeObject)
            {
                nextPoint = points[(i + 1) % points.Count];
                SetSegmentPositions(points[i], nextPoint);
            }
            else
            {
                if (i < points.Count - 1)
                {
                    nextPoint = points[(i + 1)];
                    SetSegmentPositions(points[i], nextPoint);
                }
            }
        }
    }

    void SetSegmentPositions(Vector3 startPos, Vector3 endPos)
    {

        
        float dist = Vector3.Distance(startPos, endPos);
        int quant = (int)(dist / distanceBetweenObjects);

        float indexPos = 1.0f / quant;
        
        for (int i = 0; i <= quant-1; i++)
        {
            int rand = UnityEngine.Random.Range(0, itemsToPlace.Count);
            GameObject go = PrefabUtility.InstantiatePrefab(itemsToPlace[rand], objectHolder.transform) as GameObject;
            Vector3 spawnPoint = GetPositionOnSpline(startPos, endPos, indexPos * i);
            go.transform.position = spawnPoint;
        }
    }
#endif
    public Vector3 GetPositionOnSpline(Vector3 startPos, Vector3 endPos, float indexPos)
    {
        var p = Vector3.Lerp(startPos, endPos, indexPos);
        p.z = zPosition;
        return p;

    }

    public void ClearObjects()
    {
        var allObjects = objectHolder.GetComponentsInChildren<TreeShadows>();

        if (allObjects.Length > 0)
        {
            for (int i = 0; i < allObjects.Length; i++)
            {

                DestroyImmediate(allObjects[i].gameObject);
            }
        }

    }
}