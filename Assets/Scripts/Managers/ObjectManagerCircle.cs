using Klaxon.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectManagerCircle : MonoBehaviour
{
    //The object we want to add
    public GameObject[] prefabGO;

    public bool useNeighboringObjects;

    public bool canFlipX;
    //public Tilemap groundMap;
    //Whats the radius of the circle we will add objects inside of?
    public float radius = 1f;
    public float randomMinDistance = 0.05f;
    //How many GOs will we add each time we press a button?
    public int howManyObjects;

    //Should we use poisson
    public bool usePoissonDisc;
    
    public float poissonDiscMinRadius = 0.1f;
    //Should we add or remove objects within the circle
    public enum Actions { AddObjects, RemoveObjects }

    public Actions action;
    //public Vector2 lastRandomPosition;
    List<Vector2> lastRandomPositions = new List<Vector2>();


    public int GetTileZ(Vector3 point)
    {
        
        Vector3Int cellIndex = GridManager.instance.groundMap.WorldToCell(point);
        for (int i = GridManager.instance.groundMap.cellBounds.zMax; i > GridManager.instance.groundMap.cellBounds.zMin-1; i--)
        {
            cellIndex.z = i;
            var tile = GridManager.instance.groundMap.GetTile(cellIndex);
            if (tile != null)
                return i;

        }
        return 1000;

    }

    void SetObjectID(GameObject obj)
    {
        if (obj.TryGetComponent(out SaveableWorldEntity saveableItem))
            saveableItem.GenerateId();
        if (obj.TryGetComponent(out SaveableItemEntity otherSaveableItem))
            otherSaveableItem.GenerateId();
    }
    public void ClearLastPosition()
    {
        lastRandomPositions.Clear();
    }

    bool GetRandomPosition(out Vector2 foundPosition)
    {
        Vector2 randomPos2D = Vector2.zero;
        bool found = false;
        int index = 0;
        while (!found)
        {
            //Get a random position within a circle in 2d space
            index++;
            if (index >= 20)
                break;
            randomPos2D = Random.insideUnitCircle * radius;
            if (lastRandomPositions.Count <= 0)
                found = true;
            else
            {
                foreach (var pos in lastRandomPositions)
                {
                    if (Vector2.Distance(randomPos2D, pos) > randomMinDistance)
                        found = true;
                }

            }
        }
         
        foundPosition = randomPos2D;
        if(found)
            lastRandomPositions.Add(randomPos2D);
        return found;
    }
    //Add a prefab that we instantiated in the editor script
    public void AddPrefab(GameObject newPrefabObj, Vector3 center)
    {
        
        
        //var p = GetRandomPosition();
        //But we are in pseudo3d, so make it so and move it to where the center is
        if(GetRandomPosition(out Vector2 foundPosition))
        {
            Vector3 randomPos = new Vector3(foundPosition.x, foundPosition.y, 0) + center;
            randomPos.z += 1;
            newPrefabObj.transform.position = randomPos;
            newPrefabObj.transform.parent = transform;
            SetObjectID(newPrefabObj);
        }
        else
            DestroyImmediate(newPrefabObj);
            
    }

    //Add a prefab that we instantiated in the editor script using poisson 
    public void AddPrefab(GameObject newPrefabObj, Vector3 center, Vector2 position)
    {

        position.x -= radius;
        position.y -= radius;
        Vector3 newPos = center + (Vector3)position;

        newPos.z += 1;
        newPrefabObj.transform.position = newPos;

        newPrefabObj.transform.parent = transform;
        SetObjectID(newPrefabObj);
    }

    //Remove objects within the circle
    public void RemoveObjects(Vector3 center)
    {
        //Get an array with all children to this transform
        GameObject[] allChildren = GetAllChildren();

        foreach (GameObject child in allChildren)
        {
            //If this child is within the circle
            if (Vector3.SqrMagnitude(child.transform.position - center) < radius * radius)
            {
                DestroyImmediate(child);
            }
        }
    }

    //Remove all objects
    public void RemoveAllObjects()
    {
        //Get an array with all children to this transform
        GameObject[] allChildren = GetAllChildren();

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            DestroyImmediate(child);
        }
    }

    //Get an array with all children to this GO
    private GameObject[] GetAllChildren()
    {
        //This array will hold all children
        GameObject[] allChildren = new GameObject[transform.childCount];

        //Fill the array
        int childCount = 0;
        foreach (Transform child in transform)
        {
            allChildren[childCount] = child.gameObject;
            childCount += 1;
        }

        return allChildren;
    }
}