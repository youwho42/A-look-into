using Klaxon.GOAD;
using Klaxon.GravitySystem;
using System.Collections.Generic;
using UnityEngine;

public class CokernutManager : MonoBehaviour
{
    public Vector2Int minMaxAppearanceTicks;
    public Vector2Int minMaxBetweenAppearanceTicks;
    public GOAD_Scheduler_CF cokernutScheduler;

    public ParticleSystem particles;
    public FixingSounds fixSound;


    [HideInInspector]
    public CycleTicks nextAppearance;
    [HideInInspector]
    public CycleTicks spawnedTime;

    [HideInInspector]
    public bool nextAppearanceSet;
    RealTimeDayNightCycle dayNightCycle;
    UIScreenManager sleep;

    [HideInInspector]
    public bool isActive;

    public GOAD_ScriptableCondition isHomeCondition;

    private void Start()
    {
        sleep = UIScreenManager.instance;
        dayNightCycle = RealTimeDayNightCycle.instance;
        cokernutScheduler.SetUpCokernutFlump();
        if (!nextAppearanceSet)
            SetNextAppearance();
    }

    

    public void ActivateCokernut(bool active)
    {
        isActive = active;
        cokernutScheduler.gameObject.SetActive(active);
        if(active == true)
            DissolveEffect.instance.StartDissolve(cokernutScheduler.walker.characterRenderer.material, 1.0f, true);
    }

    public void SetNextAppearance()
    {
        nextAppearance = dayNightCycle.GetCycleTime(Random.Range(minMaxBetweenAppearanceTicks.x, minMaxBetweenAppearanceTicks.y));
    }

    public void SetSpawnTime()
    {
        spawnedTime = dayNightCycle.GetCycleTime(Random.Range(minMaxAppearanceTicks.x, minMaxAppearanceTicks.y));
    }

    public bool CheckCanAppear()
    {
        if (!sleep.isSleeping && dayNightCycle.currentTimeRaw >= nextAppearance.tick && dayNightCycle.currentDayRaw >= nextAppearance.day)
            return true;
        return false;
    }

    public bool SpawnTimerExpired()
    {
        if (dayNightCycle.currentTimeRaw >= spawnedTime.tick && dayNightCycle.currentDayRaw >= spawnedTime.day)
            return true;
        return false;
    }


    public bool GetRandomCokernutInteractablePosition(out Vector3 position)
    {
        position = Vector3.zero;

        List<FixableObject> available = new List<FixableObject>(cokernutScheduler.allFixables);
        for (int i = available.Count - 1; i >= 0; i--)
        {
            if (!available[i].fixedObject.activeInHierarchy || !available[i].cokernutFlumpInteractable)
                available.RemoveAt(i);
        }
        if (available.Count == 0)
            return false;
        int r = Random.Range(0, available.Count);
        position = available[r].transform.position;
        return true;
    }


    public Vector3 GetRandomPositionAround(Vector3 basePosition)
    {
        Vector3 pos = Vector3.zero;
        do
        {
            pos = GridManager.instance.GetRandomTileWorldPosition(basePosition, 2.5f, 4f);
        } while (pos.z != basePosition.z);
        
        var hit = Physics2D.OverlapPoint(pos, LayerMask.GetMask("Obstacle", "HouseFloor"), pos.z, pos.z);
        if (!hit)
            return pos;
        for (int d = 1; d < 3; d++)
        {
            for (float i = 0; i < Mathf.PI * 2; i += Mathf.PI * 0.25f)
            {
                Vector2 dir = new Vector2(Mathf.Cos(i), Mathf.Sin(i));
                dir = dir.normalized;
                dir *= d * 0.1f;
                var posI = pos + (Vector3)dir;
                var h = Physics2D.OverlapPoint(posI, LayerMask.GetMask("Obstacle", "HouseFloor"), posI.z, posI.z);
                if (!h && GridManager.instance.GetTileValid(posI))
                    return posI;
                //var dir = Vector2.
            }
        }
        return GetRandomPositionAround(basePosition);
    }
}
