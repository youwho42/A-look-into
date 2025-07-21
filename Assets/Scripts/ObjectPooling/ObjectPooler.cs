using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{

    [HideInInspector]
    public List<GameObject> pooledObjects = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> ageList = new List<GameObject>();

    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private int amountToPool;
    [SerializeField]
    private bool useWorldSpace;
    [SerializeField]
    private int maxPoolSize = 0;

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
        ageList.Add(go);
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
        {
            if (maxPoolSize == 0 || pooledObjects.Count < maxPoolSize)
                go = CreatePooledObject();
            else if (pooledObjects.Count >= maxPoolSize )
            {
                go = ageList[0];
                ageList.RemoveAt(0);
                ageList.Add(go);
            }
                
        }
            
        
        go.SetActive(true);
        return go;
    }

    public void ReleasePooledObject(GameObject pooledObject)
    {
        if(pooledObject != null)
        {
            pooledObject.SetActive(false);
            int ind = ageList.IndexOf(pooledObject);
            ageList.RemoveAt(ind);
            ageList.Add(pooledObject);
        }
    }
}
