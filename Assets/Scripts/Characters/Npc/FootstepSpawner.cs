using UnityEngine;
using System.Collections.Generic;


public class FootstepSpawner : MonoBehaviour
{

    ObjectPooler footstepPool;
    public Transform leftFoot;
    public Transform rightFoot;
    public Vector3 offset;
    public int maxFootstepTimer;

    private void Start()
    {
        footstepPool = GetComponent<ObjectPooler>();
        GameEventManager.onTimeTickEvent.AddListener(FadeFootsteps);
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(FadeFootsteps);
    }

    void FadeFootsteps(int tick)
    {
        foreach (var obj in footstepPool.ageList)
        {
            if (obj.activeInHierarchy)
            {
                if (obj.TryGetComponent(out FootstepObject step))
                {
                    if (step.FootstepTimer >= maxFootstepTimer)
                    {
                        footstepPool.ReleasePooledObject(obj);
                        return;
                    }


                    step.TimerTick();

                }
            }
            
            
        }
    }

    void SpawnFootstep(bool isLeft)
    {
        var obj = footstepPool.GetPooledObject();
        var pos = isLeft ? leftFoot.position : rightFoot.position;
        obj.transform.position = pos + offset;
        if (obj.TryGetComponent(out FootstepObject step))
            step.SetNewTimer(maxFootstepTimer);
    }

    public void SpawnLeftFootstep()
    {
        SpawnFootstep(true);
    }
    public void SpawnRightFootstep()
    {
        SpawnFootstep(false);
    }
}
