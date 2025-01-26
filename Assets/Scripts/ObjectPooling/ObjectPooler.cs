using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{

    
    private List<GameObject> pooledObjects = new List<GameObject>();
    

    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private int amountToPool;
    [SerializeField]
    private bool useWorldSpace;


    private void Start()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            CreatePooledObject();
        }
    }

    private GameObject CreatePooledObject()
    {
        GameObject go = Instantiate(prefab/*, transform*/);
        if (!useWorldSpace)
            go.transform.parent = transform;
        go.SetActive(false);
        pooledObjects.Add(go);
        return go;
    }


    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        var go = CreatePooledObject();

        return go;
    }
}
