using Klaxon.SaveSystem;
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
    public QI_ItemData itemToBecomeData;

    private void Start()
    {

        GameEventManager.onTimeHourEvent.AddListener(Reproduce);
    }

    private void OnDestroy()
    {
        GameEventManager.onTimeHourEvent.RemoveListener(Reproduce);

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
                Transform item = Instantiate(itemToBecomeData.ItemPrefab, transform.position + GetOffset(), Quaternion.identity, transform.parent);
                item.GetComponent<SaveableItemEntity>().GenerateId();
                canReproduce = false;
                tick = 0;
            }
        }
    }
    public void SetTick(int newTick)
    {
        tick = newTick;
    }
    public int GetTick()
    {
        return tick;
    }
    Vector3 GetOffset()
    {
        return Random.insideUnitCircle * 0.2f;
    }

}
