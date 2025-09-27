using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectsPool : MonoBehaviour
{
    
        public Transform poolHolder;
        public List<GameObject> objectsToSpawn = new List<GameObject>();
        int baseQuantity = 20;
        List<GameObject> availableObjects = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < baseQuantity; i++)
        {
            CreateObject();
        }
    }

    void CreateObject()
        {
            var r = Random.Range(0, objectsToSpawn.Count);
            var go = Instantiate(objectsToSpawn[r], poolHolder);
            go.SetActive(false);
            availableObjects.Add(go);
        }

        public void CreateAndSetObject(Vector3 position)
        {
            var r = Random.Range(0, objectsToSpawn.Count);
            var go = Instantiate(objectsToSpawn[r], position, Quaternion.identity, poolHolder);
            availableObjects.Add(go);
        }

        public void SetObjectVisible(Vector3 position)
        {
            for (int i = 0; i < availableObjects.Count; i++)
            {
                if (availableObjects[i].transform.position == position)
                {
                    availableObjects[i].SetActive(true);
                    return;
                }
  
            }

            for (int i = 0; i < availableObjects.Count; i++)
            {
                if (!availableObjects[i].activeSelf)
                {
                    availableObjects[i].transform.position = position;
                    availableObjects[i].SetActive(true);
                    return;
                }
            }
            
            CreateAndSetObject(position);
            
        }
        public void CheckAllObjectsVisible()
        {
            var playerPos = PlayerInformation.instance.player.position;
            for (int i = 0; i < availableObjects.Count; i++)
            {
                if (NumberFunctions.GetDistanceV3(playerPos, availableObjects[i].transform.position) > 25)
                    availableObjects[i].SetActive(false);
            }
        }
    

}
