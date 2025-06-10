using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{

    [HideInInspector]
    public List<GameObject> pooledObjects = new List<GameObject>();
    

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
        GameObject go = Instantiate(prefab);
        if (!useWorldSpace)
            go.transform.SetParent(transform);
        go.SetActive(false);
        pooledObjects.Add(go);
        return go;
    }


    public GameObject GetPooledObject()
    {
        GameObject go = null;
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                go = pooledObjects[i];
                break;
            }
        }
        if(go == null)
            go = CreatePooledObject();
        go.SetActive(true);
        return go;
    }
}
