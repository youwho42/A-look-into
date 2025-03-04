using Klaxon.SaveSystem;
using System.Collections;
using UnityEngine;

public class FlumpOozeManager : MonoBehaviour
{
    public ObjectPooler oozeObjectPool;
    public DrawZasYDisplacement currentZAsYDisplacementA;
    public FlumpOozeContainer flumpOozeContainer;
    public FlumpOoze finalOoze;
    public Color colorA;
    public Color colorB;
    


    public void StartOoze(Vector3 position, bool isSleeping)
    {
        var go = Instantiate(flumpOozeContainer, position, Quaternion.identity);
        go.GetComponent<SaveableItemEntity>().GenerateId();
        if(!isSleeping)
            StartCoroutine(SpawnOozeCo(position, go.oozeContainer));
        //else

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

        oozeDrop.SetOozeDrop(parent, position, currentZAsYDisplacementA, GetCurrentColor());
    }

    void SpawnFinalOoze(Transform parent)
    {
        var go = Instantiate(finalOoze, transform.position, Quaternion.identity, parent);
        go.SetOoze(parent, GetCurrentColor(), false);
    }

    Color GetCurrentColor()
    {
        return Color.Lerp(colorA, colorB, Random.value);
    }
}
