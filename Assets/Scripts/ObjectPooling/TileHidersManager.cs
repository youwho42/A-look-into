using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHidersManager : MonoBehaviour
{
    public static TileHidersManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private List<GameObject> pooledObjects = new List<GameObject>();


    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private int amountToPool;

    public List<Sprite> tileHiderSprites = new List<Sprite>();

    private void Start()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject go = Instantiate(prefab, transform);
            go.SetActive(false);
            pooledObjects.Add(go);
        }
    }

    public GameObject GetPooledObject(string tileName, Vector3 pos)
    {
        
        Sprite s = null;
        foreach (var item in tileHiderSprites)
        {
            if (item.name == $"{tileName}-Hide")
            {
                s = item;
                break;
            }

        }
        if (s == null)
            return null;

        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (pooledObjects[i].transform.position == pos && pooledObjects[i].activeInHierarchy)
                return null;
        }

        for (int i = 0; i < pooledObjects.Count; i++)
        {
            
            if (!pooledObjects[i].activeInHierarchy)
            {
                pooledObjects[i].GetComponent<SpriteRenderer>().sprite = s;
                pooledObjects[i].transform.position = pos;
                return pooledObjects[i];
            }
        }
        return null;
    }
}
