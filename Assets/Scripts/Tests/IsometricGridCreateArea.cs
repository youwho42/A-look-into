using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IsometricGridCreateArea : MonoBehaviour
{

    public IsometricGridObject gridObject;
    public GameObject itemToPlace;

    

#if (UNITY_EDITOR)
    public void PlaceObject(Vector3 position)
    {
        GameObject go = PrefabUtility.InstantiatePrefab(itemToPlace, transform) as GameObject;
        go.transform.position = position;
    }

    
#endif
}
