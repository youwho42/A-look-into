using BezierSolution;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class SplineObject : MonoBehaviour
{
    public BezierSpline spline;
    public int level;
    public List<GameObject> itemsToPlace = new List<GameObject>();
    public float distanceBetweenObjects;
    public Transform objectHolder;

    [ContextMenu("Set Positions")]
    void SetObjectPositions()
    {

        ClearObjects();
        float len = spline.GetLengthApproximately(0,1);
        
        
        int quant = (int)(len / distanceBetweenObjects);
        float indexPos = 1.0f / quant;
        quant -= spline.loop ? 1 : 0;
        for (int i = 0; i <= quant; i++)
        {
            int rand = Random.Range(0, itemsToPlace.Count);
            var go = PrefabUtility.InstantiatePrefab(itemsToPlace[rand], objectHolder.transform) as GameObject;
            Vector3 spawnPoint = GetPositionOnSpline(indexPos * i);
            go.transform.position = spawnPoint;
        }
    }


    public Vector3 GetPositionOnSpline(float indexPos)
    {
        var p = spline.GetPoint(indexPos);
        p.z = level;
        return p;
        
    }

    void ClearObjects()
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
