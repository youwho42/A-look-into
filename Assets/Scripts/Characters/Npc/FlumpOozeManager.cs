using Klaxon.SaveSystem;
using System.Collections;
using UnityEngine;

public class FlumpOozeManager : MonoBehaviour
{
    public ObjectPooler oozeObjectPool;
    public DrawZasYDisplacement currentZAsYDisplacementA;
    public GameObject flumpOozeContainer;

    


    public void StartOoze(Vector3 position)
    {
        var go = Instantiate(flumpOozeContainer, position, Quaternion.identity);
        go.GetComponent<SaveableItemEntity>().GenerateId();
        StartCoroutine(SpawnOozeCo(position, go.transform));
    }

    public void StopOoze()
    {
        StopAllCoroutines();
    }

    IEnumerator SpawnOozeCo(Vector3 position, Transform parent)
    {
        float timeBetweenFlickers = Random.Range(0.001f, 0.05f);
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenFlickers);
            SpawnOoze(position, parent);
            timeBetweenFlickers = Random.Range(0.001f, 0.05f);

            yield return null;

        }
    }

    void SpawnOoze(Vector3 position, Transform parent)
    {

        var go = oozeObjectPool.GetPooledObject();
        var oozeDrop = go.GetComponent<FlumpOozeDrop>();
        //PlaySound();
        //oozeDrop.transform.localPosition = displacement.transform.localPosition;

        oozeDrop.SetOozeDrop(parent, position, currentZAsYDisplacementA);
    }

    

}
