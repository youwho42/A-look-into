using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SmellGenerator : MonoBehaviour
{

    public int poolAmount;

    public SmellsObject smellObject;
    public ObjectPool<SmellsObject> smellPool;
    public DrawZasYDisplacement currentZAsYDisplacement;
    public SmellItemData smellData;
    List<SmellsObject> startSmells = new List<SmellsObject>();

    public void Start()
    {

        smellPool = new ObjectPool<SmellsObject>
            (
                createFunc: CreateSmell,
                actionOnGet: GetFromPool,
                actionOnRelease: ReleaseToPool,
                defaultCapacity: poolAmount

            );

    }

    public void StartSmells()
    {
        for (int i = 0; i < 5; i++)
        {
            startSmells.Add(CreateSmell());
        }
        for (int i = 0; i < startSmells.Count; i++)
        {
            for (int j = 0; j < i * 5; j++)
            {
                startSmells[i].UpdateSmell();
            }
        }
        InvokeRepeating("EmitSmell", 0.0f, 0.4f);
    }
    public void StopSmells()
    {
        CancelInvoke();
        startSmells.Clear();
        smellPool.Dispose();
    }

    void EmitSmell()
    {
        SpawnSmell();
    }

    public void StopEmit(SmellsObject smellsObject)
    {
        smellPool.Release(smellsObject);
    }

    void SpawnSmell()
    {

        var smell = smellPool.Get();
        smell.transform.position = currentZAsYDisplacement.transform.position;
        smell.SetSmell(this, currentZAsYDisplacement, smellData);
    }
    SmellsObject CreateSmell()
    {
        SmellsObject smell = Instantiate(smellObject, transform);
        smell.transform.position = currentZAsYDisplacement.transform.position;
        smell.gameObject.SetActive(true);
        smell.SetSmell(this, currentZAsYDisplacement, smellData);
        return smell;
    }

    void GetFromPool(SmellsObject smell)
    {
        smell.gameObject.SetActive(true);
    }

    void ReleaseToPool(SmellsObject smell)
    {
        smell.gameObject.SetActive(false);
    }

}
