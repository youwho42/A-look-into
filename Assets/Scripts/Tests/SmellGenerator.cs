using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SmellGenerator : MonoBehaviour
{
    ObjectPooler smellPool;
    
    public DrawZasYDisplacement currentZAsYDisplacement;
    public SmellItemData smellData;

    public void Start()
    {
        smellPool = GetComponent<ObjectPooler>();
    }

    void SpawnSmell()
    {
        var go = smellPool.GetPooledObject();
        var smell = go.GetComponent<SmellsObject>();
        
        smell.transform.position = currentZAsYDisplacement.transform.position;
        smell.SetSmell(currentZAsYDisplacement, smellData);
    }

    public void StartSmells()
    {
        InvokeRepeating("EmitSmell", 0.0f, 0.4f);
    }

    public void StopSmells()
    {
        CancelInvoke();
    }

    void EmitSmell()
    {
        SpawnSmell();
    }

    
}
