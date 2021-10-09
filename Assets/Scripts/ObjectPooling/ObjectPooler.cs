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



    private void Start()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject go = Instantiate(prefab/*, this.transform*/);
            go.SetActive(false);
            pooledObjects.Add(go);
        }
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
        return null;
    }
}
