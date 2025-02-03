using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquonkTearManager : MonoBehaviour
{
    public int poolAmount;
    public GameObject theSquonk;
    public DrawZasYDisplacement currentZAsYDisplacementA;
    public DrawZasYDisplacement currentZAsYDisplacementB;
    public ObjectPooler tearObjectPool;
    public ObjectPooler tearPuddleObjectPool;
    public SquonkManager manager;
    public void OnEnable()
    {
       if(!manager.isActive)
        {
            theSquonk.SetActive(false);
            return;
        }
        
        StartCoroutine(SpawnTearCo(currentZAsYDisplacementA));
        StartCoroutine(SpawnTearCo(currentZAsYDisplacementB));
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    public void LeaveTrail(Vector3 position)
    {
        SpawnPuddle(position);
    }

    IEnumerator SpawnTearCo(DrawZasYDisplacement disp)
    {
        float timeBetweenFlickers = Random.Range(0.3f, 1.0f);
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenFlickers);
            SpawnTear(disp);
            timeBetweenFlickers = Random.Range(0.3f, 1.0f);

            yield return null;

        }
    }
    
    void SpawnTear(DrawZasYDisplacement displacement)
    {

        var go = tearObjectPool.GetPooledObject();
        var tear = go.GetComponent<SquonkTear>();
        
        tear.transform.localPosition = displacement.transform.localPosition;
        tear.SetTear(this, displacement);
    }

    void SpawnPuddle(Vector3 position)
    {
        var go = tearPuddleObjectPool.GetPooledObject();
        Vector3 offset = new Vector3(Random.Range(-0.02f, 0.02f), Random.Range(-0.02f, 0.02f), 0);
        position += offset;
        go.transform.position = position;
        
    }


}
