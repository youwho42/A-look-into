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

    

    CycleTicks nextAppearance;
    
    [HideInInspector]
    public bool nextAppearanceSet;
    RealTimeDayNightCycle dayNightCycle;

    private void OnEnable()
    {
        dayNightCycle = RealTimeDayNightCycle.instance;
        GameEventManager.onTimeTickEvent.AddListener(CheckAppearance);
        cokernutScheduler.SetUpCokernutFlump();
    }

    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(CheckAppearance);
    }

    
    void CheckAppearance(int tick)
    {
        if (dayNightCycle == null)
        {
            dayNightCycle = RealTimeDayNightCycle.instance;
            return;
        }

        if (!cokernutScheduler.gameObject.activeInHierarchy)
        {
            if (!nextAppearanceSet)
            {
                nextAppearance = dayNightCycle.GetCycleTime(Random.Range(minMaxBetweenAppearanceTicks.x, minMaxBetweenAppearanceTicks.y));

                nextAppearanceSet = true;
                return;
            }
            else
            {
                if (dayNightCycle.currentTimeRaw >= nextAppearance.tick && dayNightCycle.currentDayRaw == nextAppearance.day)
                {

                    nextAppearanceSet = false;
                    if (GetRandomPosition(out Vector3 pos))
                    {
                        cokernutScheduler.gameObject.SetActive(true);
                        cokernutScheduler.transform.position = pos;
                        cokernutScheduler.currentTilePosition.position = cokernutScheduler.currentTilePosition.GetCurrentTilePosition(pos);
                        cokernutScheduler.walker.currentLevel = (int)pos.z - 1;
                        DissolveEffect.instance.StartDissolve(cokernutScheduler.walker.characterRenderer.material, 1.0f, true);
                    }


                }
            }
        }
        else
        {
            if (!nextAppearanceSet)
            {
                nextAppearance = dayNightCycle.GetCycleTime(Random.Range(minMaxAppearanceTicks.x, minMaxAppearanceTicks.y));

                nextAppearanceSet = true;
                return;
            }
            else
            {
                if (dayNightCycle.currentTimeRaw >= nextAppearance.tick && dayNightCycle.currentDayRaw == nextAppearance.day)
                {
                    cokernutScheduler.SetBeliefState(cokernutScheduler.fleeCondition.Condition, true);
                }
            }
        }
    }

    bool GetRandomPosition(out Vector3 position)
    {
        position = Vector3.zero;

        List<FixableObject> available = new List<FixableObject>(cokernutScheduler.allFixables);
        for (int i = available.Count - 1; i >= 0; i--)
        {
            if (!available[i].fixedObject.activeInHierarchy)
                available.RemoveAt(i);
        }
        if (available.Count == 0)
            return false;
        int r = Random.Range(0, available.Count);
        position = GetRandomPositionAround(available[r].transform.position);
        return true;
    }


    private Vector3 GetRandomPositionAround(Vector3 basePosition)
    {
        Vector3 pos = Vector3.zero;
        do
        {
            pos = GridManager.instance.GetRandomTileWorldPosition(basePosition, 4f);
        } while (pos.z != basePosition.z);
        
        var hit = Physics2D.OverlapPoint(pos, LayerMask.GetMask("Obstacle"), pos.z, pos.z);
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
                var h = Physics2D.OverlapPoint(posI, LayerMask.GetMask("Obstacle"), posI.z, posI.z);
                if (!h && GridManager.instance.GetTileValid(posI))
                    return posI;
                //var dir = Vector2.
            }
        }
        return GetRandomPositionAround(basePosition);
    }
}
