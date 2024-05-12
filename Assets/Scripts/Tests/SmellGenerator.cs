using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SmellGenerator : MonoBehaviour
{
    
    public int poolAmount;

    public SmellsObject smellObject;
    public ObjectPool<SmellsObject> smellPool;
    //List<SmellsObject> objects = new List<SmellsObject>();
    public DrawZasYDisplacement currentZAsYDisplacement;
    [ColorUsage(true, true)]
    public Color currentEmissionColor;
    public Color currentColor;
    
    public void Start()
    {
        smellPool = new ObjectPool<SmellsObject>
            (
                createFunc: CreateSmell,
                actionOnGet: GetFromPool,
                actionOnRelease: ReleaseToPool,
                defaultCapacity:poolAmount
               
            );
        InvokeRepeating("EmitSmell", 0.0f, 0.4f);
    }

    void EmitSmell()
    {
        SpawnSmell();
    }

    public void StopEmit(SmellsObject smellsObject)
    {
        smellPool.Release(smellsObject);
    }

    public void SpawnSmell()
    {
        
        var smell = smellPool.Get();
        smell.transform.position = currentZAsYDisplacement.transform.position;
        smell.SetSmell(this, currentZAsYDisplacement, currentColor, currentEmissionColor);
    }
    SmellsObject CreateSmell()
    {
        SmellsObject smell = Instantiate(smellObject, transform);
        smell.transform.position = currentZAsYDisplacement.transform.position;
        
        smell.gameObject.SetActive(true);
        smell.SetSmell(this, currentZAsYDisplacement, currentColor, currentEmissionColor);
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
