using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityReproduction : MonoBehaviour
{
    public int ticksToReproduce;
    bool canReproduce;
    int tick;
    public AnimationCurve curve;
    QI_ItemData itemData;

    private void Start()
    {
        itemData = GetComponent<QI_Item>().Data;
        DayNightCycle.instance.FullHourEventCallBack.AddListener(Reproduce);
    }

    public void AllowForReproduction()
    {
        if(ticksToReproduce == 0)
            canReproduce = true;
        else
        {
            tick++;
            if (tick == ticksToReproduce)
                canReproduce = true;
        }
    }
    void Reproduce(int time)
    {
        if (canReproduce && time == 5)
        {
            if(curve.Evaluate(Random.Range(0.0f, 1.0f)) < 0.1f)
            {
                Transform item = Instantiate(itemData.ItemPrefab, transform.position + GetOffset(), Quaternion.identity, transform.parent);
                item.GetComponent<SaveableEntity>().GenerateId();
                canReproduce = false;
                tick = 0;
            }
        }
    }

  
    Vector3 GetOffset()
    {
        return Random.insideUnitCircle * 0.2f;
    }

}
